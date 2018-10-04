using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Linq;
using Simplex.Enums;
using Simplex.Model;

namespace Simplex.Analysis
{
    public static  class SimplexDecoratorExtension
    {
        public static void ConvertStandardModel(this ISimplexModel model)
        {

            //Steps
            //1.Modify the constraints so that the RHS of each constraint is nonnegative (This requires that each constraint with a negative RHS be multiplied by - 1.Remember that if you multiply an inequality by any negative number, the direction of the inequality is reversed!). After modification, identify each constraint as a ≤, ≥ or = constraint.
            //2.Convert each inequality constraint to standard form(If constraint i is a ≤ constraint, we add a slack variable si; and if constraint i is a ≥ constraint, we subtract an excess variable ei).


            //Two phase standardization
            #region Phase I
            //1) Check and update Right Hand Side- RHS value for positive 
            UpdateNegativeRHSValues(model);

            Dictionary<string, Term> m_VectorList = new Dictionary<string, Term>();
            //1.X) find the native varibles count in model
            foreach (Subject constarint in model.Subjects)
            {

                foreach (Term term in constarint.Terms)
                {
                    if (!m_VectorList.ContainsKey(term.Vector))
                        m_VectorList.Add(term.Vector, term);
                }
            }
            //1.X) if one of term contained only one cosntarint that ise basic varibale, let us find it
            foreach (KeyValuePair<string, Term> item in m_VectorList)
            {
                List<Subject> tmp_list = model.Subjects.Where(subject => subject.Terms.Any(term => term.Vector.Equals(item.Key))).ToList();
                if (tmp_list.Count == 1 && tmp_list[0].Equality == EquailtyType.Equals)
                    item.Value.isBasic = true;
            }

            //2) Add variables for BFS
            //2.1) add slack,excess and artificial Term to constarints
            string m_slackPrefix = "s";
            int m_slackcount = 1;
            string m_excessPrefix = "e";
            int m_excesscount = 1;
            string m_artificialPrefix = "a";
            int m_artificialcount = 1;
            foreach (Subject constarint in model.Subjects)
            {
                //1) add slack Term for not equal constarint
                switch (constarint.Equality)
                {
                    case EquailtyType.LessEquals: // if constarint equation <= then plus Slack variable at the left side
                        constarint.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Slack, Vector = m_slackPrefix + m_slackcount.ToString(), Index = constarint.Terms.Count - 1 });
                        m_slackcount++;
                        break;
                    case EquailtyType.GreaterEquals: // if constarint equation >= then minus excess variable and plus artificial variable at the left side
                        constarint.Terms.Add(new Term() { Factor = -1, VarType = VariableType.Excess, Vector = m_excessPrefix + m_excesscount.ToString(), Index = constarint.Terms.Count - 1 });
                        constarint.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Artificial, Vector = m_artificialPrefix + m_artificialcount.ToString(), Index = constarint.Terms.Count - 1 });
                        m_excesscount++;
                        m_artificialcount++;
                        break;
                    default: // if constarint equation is eqal and subject dos nat contain basic term, then plus artificial variable at the left side
                        if (!constarint.Terms.Any(term => term.isBasic == true))
                        {
                            constarint.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Artificial, Vector = m_artificialPrefix + m_artificialcount.ToString(), Index = constarint.Terms.Count - 1 });
                            m_artificialcount++;
                        }
                        break;
                }
            }

            //4) change signt the factor value of term in objective fonction terms and add positive balance variable ("Z")
            foreach (Term term in model.ObjectiveFunction.Terms)
            {
                term.Factor *= -1;
            }
            model.ObjectiveFunction.RightHandValue = 0;
            //model.ObjectiveFunction.Terms.Insert(0, new Term() { Factor = 1, VarType = VariableType.Balance, Vector = "Z", Index = 0 });


            //3) find the varibles count in model
            //3.1) //Check and collect vector label for constranit
            foreach (Subject constarint in model.Subjects)
            {

                foreach (Term term in constarint.Terms)
                {
                    if (!m_VectorList.ContainsKey(term.Vector))
                        m_VectorList.Add(term.Vector, term);
                }
            }

            //3.X) if one of term that is non original contained only one cosntarint that ise basic varibale, let us find it
            foreach (KeyValuePair<string, Term> item in m_VectorList)
            {
                List<Subject> tmp_list = model.Subjects.Where(subject => subject.Terms.Any(term => term.Vector.Equals(item.Key) && term.VarType != VariableType.Original && term.Factor == 1)).ToList();
                if (tmp_list.Count == 1)
                    item.Value.isBasic = true;
            }

            //3.2) Check and collect vector label for objective funtion
            foreach (Term term in model.ObjectiveFunction.Terms)
            {
                if (!m_VectorList.ContainsKey(term.Vector))
                    m_VectorList.Add(term.Vector, term);
            }

            //3) expand the objective function and all constarints with not exsit variable in Clause 
            //sort the vector list
            foreach (KeyValuePair<string, Term> item in m_VectorList)
            {
                if (!model.ObjectiveFunction.IsVectorContained(item.Key))
                    model.ObjectiveFunction.Terms.Add(new Term() { Vector = item.Key, Factor = 0, VarType = item.Value.VarType, isBasic=item.Value.isBasic });

                foreach (Subject constarint in model.Subjects)
                {
                    if (!constarint.IsVectorContained(item.Key))
                        constarint.Terms.Add(new Term() { Vector = item.Key, Factor = 0, VarType = item.Value.VarType });
                }
            }

            //Sort clause terms
            TermComparer tc = new TermComparer();
            model.ObjectiveFunction.Terms.Sort(tc);

            foreach (Subject constarint in model.Subjects)
            {
                constarint.Terms.Sort(tc);
            }

            //5) check the term count is equeal for objective function and all of constarints

            #endregion
        }

        public static TestMessage CheckBFS(this ISimplexModel model)
        {
            TestMessage retval = new TestMessage() { Exception = null, Message = string.Empty };

            //first, update rhs values for equality direction control
            UpdateNegativeRHSValues(model);

            foreach (Subject constarint in model.Subjects)
            {
                if (constarint.Equality != EquailtyType.LessEquals )
                {
                    retval.Exception = new ArithmeticException();
                    retval.Message += "Constraint has Right-Hand Side Value " + constarint.RightHandValue.ToString() + " is " + constarint.Equality.ToString() + " different from " + EquailtyType.LessEquals.ToString() + "\n";
                }
            }
            if (retval.Exception == null)
                retval.Message = "Success";

            return retval;
        }

        internal static void UpdateNegativeRHSValues(this ISimplexModel model)
        {
            //1) Check and update Right Hand Side- RHS value for positive 
            foreach (Subject constarint in model.Subjects)
            {
                if (constarint.RightHandValue < 0)
                {
                    constarint.RightHandValue *= -1;
                    foreach (Term term in constarint.Terms)
                    {
                        term.Factor *= -1;
                    }

                    if (constarint.Equality == EquailtyType.GreaterEquals)
                        constarint.Equality = EquailtyType.LessEquals;
                    else if (constarint.Equality == EquailtyType.LessEquals)
                        constarint.Equality = EquailtyType.GreaterEquals;
                    //nothing to do for equal, equal is equal
                }
            }
        }
                
        public static void PrintMatrix(this ISimplexModel model)
        {
            string tmp_sign = string.Empty;
            System.Diagnostics.Debug.WriteLine("*********************************");
            System.Diagnostics.Debug.WriteLine("        Simplex Model");
            System.Diagnostics.Debug.WriteLine("Goal :" + model.GoalType.ToString());
            System.Diagnostics.Debug.Write("Objective Function - " + model.GoalType.ToString() + ": ");
            foreach (Term item in model.ObjectiveFunction.Terms)
            {
                tmp_sign = string.Empty;
                if (Math.Sign(item.Factor) > -1)
                    tmp_sign = "+";
                System.Diagnostics.Debug.Write(tmp_sign + item.Factor + "*" + item.Vector + " ");
            }
            System.Diagnostics.Debug.WriteLine("");
            int tmp_counter = 1;
            foreach (Subject constaint in model.Subjects)
            {
                System.Diagnostics.Debug.Write("Constaint#" + tmp_counter + " :");
                tmp_counter++;
                foreach (Term item in constaint.Terms)
                {
                    tmp_sign = string.Empty;
                    switch (Math.Sign(item.Factor))
                    {
                        case 1: tmp_sign = "+"; break;
                        case -1: tmp_sign = string.Empty; break;
                        default: tmp_sign = "+"; break;
                    }
                    System.Diagnostics.Debug.Write(tmp_sign + item.Factor + "*" + item.Vector + " ");
                }
                System.Diagnostics.Debug.Write(constaint.Equality.ToString() + " ");
                System.Diagnostics.Debug.WriteLine(constaint.RightHandValue.ToString());

            }
            System.Diagnostics.Debug.WriteLine("*********************************");
        }

        public static SimplexModel DeepCopy(this SimplexModel basemodel)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, basemodel);
                ms.Position = 0;
                return (SimplexModel)formatter.Deserialize(ms);
            }
        }
    }

    public class TermComparer : IComparer<Term>
    {
        public int Compare(Term x, Term y)
        {
            if (x != null && y != null)
            {
                int typecompare = x.VarType.CompareTo(y.VarType);

                if (typecompare != 0)
                {
                    return typecompare;
                }
                else
                {
                    return x.Vector.CompareTo(y.Vector);
                }
            }
            else if (x != null && y == null)
            {
                return 1;
            }
            else if (x == null && y != null)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}
