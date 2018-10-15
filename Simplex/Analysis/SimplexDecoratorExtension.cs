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
                        constarint.AddTerm(1, VariableType.Slack, m_slackPrefix + m_slackcount.ToString());
                        m_slackcount++;
                        break;
                    case EquailtyType.GreaterEquals: // if constarint equation >= then minus excess variable and plus artificial variable at the left side
                        constarint.AddTerm( -1, VariableType.Excess, m_excessPrefix + m_excesscount.ToString());
                        constarint.AddTerm( 1, VariableType.Artificial, m_artificialPrefix + m_artificialcount.ToString());
                        m_excesscount++;
                        m_artificialcount++;
                        break;
                    default: // if constarint equation is eqal and subject dos nat contain basic term, then plus artificial variable at the left side
                        if (!constarint.Terms.Any(term => term.Basic == true))
                        {
                            constarint.AddTerm( 1, VariableType.Artificial, m_artificialPrefix + m_artificialcount.ToString());
                            m_artificialcount++;
                        }
                        break;
                }
            }


            //3) find the varibles in model and distinct them
            Dictionary<string, TermCore> m_VectorList = new Dictionary<string, TermCore>();
            //3.1) Check and collect vector label for objective funtion
            foreach (Term term in model.ObjectiveFunction.Terms)
            {
                if (!m_VectorList.ContainsKey(term.Vector))
                    m_VectorList.Add(term.Vector, term.Core);
            }
            //3.2) //Check and collect vector label for constranit
            foreach (Subject constarint in model.Subjects)
            {

                foreach (Term term in constarint.Terms)
                {
                    if (!m_VectorList.ContainsKey(term.Vector))
                        m_VectorList.Add(term.Vector, term.Core);
                }
            }

            //3) expand the objective function and all constarints with not exsit variable in Clause 
            //sort the vector list
            foreach (KeyValuePair<string, TermCore> item in m_VectorList)
            {
                if (!model.ObjectiveFunction.IsVectorContained(item.Key))
                    model.ObjectiveFunction.Terms.Add(new Term() { Factor = 0, Core=item.Value});

                foreach (Subject constarint in model.Subjects)
                {
                    if (!constarint.IsVectorContained(item.Key))
                        constarint.Terms.Add(new Term() {Factor = 0, Core = item.Value });
                }
            }

            //Sort clause terms
            TermComparer tc = new TermComparer();
            model.ObjectiveFunction.Terms.Sort(tc);

            foreach (Subject constarint in model.Subjects)
            {
                constarint.Terms.Sort(tc);
            }

            //4) change signt the factor value of term in objective fonction terms and add positive balance variable ("Z")
            foreach (Term term in model.ObjectiveFunction.Terms)
            {
                term.Factor *= -1;
            }
            model.ObjectiveFunction.RightHandValue = 0;

            //find and flag basic variable
            // if one of term that is non original contained only one cosntarint that ise basic varibale, let us find it
            int tmp_zeroCounter = 0;
            int tmp_OneCounter = 0;
            int tmp_rowcount = model.Subjects.Count;
            for (int i = 0; i < model.ObjectiveFunction.Terms.Count; i++)
            {
                tmp_zeroCounter = 0;
                tmp_OneCounter = 0;
                foreach (Subject constarint in model.Subjects)
                {
                    if (constarint.Terms[i].Factor == 0)
                        tmp_zeroCounter++;
                    else if (constarint.Terms[i].Factor == 1)
                        tmp_OneCounter++;
                }
                if (tmp_OneCounter == 1 && (tmp_OneCounter + tmp_zeroCounter == tmp_rowcount))
                    model.ObjectiveFunction.Terms[i].Basic = true;
                else
                    model.ObjectiveFunction.Terms[i].Basic = false;
            }

            ////reflect basic flag from objective function to the cosntarint 
            //for (int i = 0; i < model.ObjectiveFunction.Terms.Count; i++)
            //{
            //    foreach (Subject constarint in model.Subjects)
            //    {
            //        constarint.Terms[i].Basic= model.ObjectiveFunction.Terms[i].Basic;
            //    }
            //}

            //5) check the term count is equeal for objective function and all of constarints

            #endregion

            #region UnitMatrix

            //List<Term> tmp_basicVariableslist = model.ObjectiveFunction.Terms.Where(item => item.isBasic).ToList();
            //Dictionary<Term, int> tmp_UnitMatrixDictionary = new Dictionary<Term, int>();
            //int tmp_index = -1;
            //foreach (Term item in tmp_basicVariableslist)
            //{
            //    tmp_index = -1;
            //    foreach (Subject constarint in model.Subjects)
            //    {
            //        tmp_index = constarint.Terms.FindIndex(consitem => consitem.Vector == item.Vector && consitem.Factor == 1);
            //        if (tmp_index > -1)
            //        {
            //            tmp_UnitMatrixDictionary.Add(item, tmp_index);
            //            break;
            //        }
            //    }
            //}

            #endregion
        }

        internal static void CreatePhaseOneObjective(this SimplexModelDecorator model)
        {
            if (!model.IsTwoPhase)
                return;

            //Steps
            //#.Modify the constraints so that the RHS of each constraint is nonnegative (This requires that each constraint with a negative RHS be multiplied by - 1.Remember that if you multiply an inequality by any negative number, the direction of the inequality is reversed!). After modification, identify each constraint as a ≤, ≥ or = constraint.
            //#.Convert each inequality constraint to standard form(If constraint i is a ≤ constraint, we add a slack variable si; and if constraint i is a ≥ constraint, we subtract an excess variable ei).

            //Steps
            //1.Add an artificial variable ai to the constraints identified as ≥ or = constraints at the end of Step 1.Also add the sign restriction ai ≥ 0.
            //2.In the phase I, ignore the original LP’s objective function, instead solve an LP whose objective function is minimizing w = ai(sum of all the artificial variables).The act of solving the Phase I LP will force the artificial variables to be zero.
            //3.Since each artificial variable will be in the starting basis, all artificial variables must be eliminated from row 0 before beginning the simplex. Now solve the transformed problem by the simplex.

            //1) add all of artificial variables in orginal objective function to the phaseonefunction
            //copy the objective function
            foreach (Term  item in model.ObjectiveFunction.Terms)
            {
                if(item.VarType== VariableType.Artificial)
                    model.PhaseObjectiveFunction.Terms.Add(new Term() { Factor = 1, Core = item.Core });
                else
                    model.PhaseObjectiveFunction.Terms.Add(new Term() { Factor = 0, Core = item.Core});
            }

            ////2) change signt the factor value of term in new objective fonction terms and add positive balance variable ("w")
            //foreach (Term term in model.PhaseObjectiveFunction.Terms)
            //{
            //    term.Factor *= -1;
            //}
            //3) add balance type variable ("w") to the new objective function
            //model.PhaseObjectiveFunction.Terms.Insert(0, new Term() { Factor = 1, VarType = VariableType.Balance, Vector = "w", Index = 0 });

            //Let us define new objective function as negative (-w)
            model.PhaseObjectiveFunction.RightHandValue = 0;

            //4) find all artificial variable that has factor value is equal to 0 in cosntarint terms and put the artificial variable value in the new objective function;
            //   all artificial variables must be eliminated from row 0 before we can solve Phase I
            foreach (Subject constraint in model.Subjects)
            {
                if (constraint.Terms.Any(term => term.VarType == VariableType.Artificial && term.Factor == 1))
                {
                    for (int i = 0; i < constraint.Terms.Count; i++)
                    {
                        model.PhaseObjectiveFunction.Terms[i].Factor -= constraint.Terms[i].Factor;
                    }
                    model.PhaseObjectiveFunction.RightHandValue -= constraint.RightHandValue;
                }

                //if (constraint.Terms.Any(term => term.Basic && term.Factor == 1))
                //{
                //    //Find 
                //    List<Term> tmp_willaddedterms = constraint.Terms.Where(term => term.Factor != 0).ToList();
                //    foreach (Term item in tmp_willaddedterms)
                //    {
                //        //tmp_term = null;
                //        tmp_term = model.PhaseObjectiveFunction.Terms.Find(term => term.Vector.Equals(item.Vector));
                //        //if (tmp_term != null)
                //        tmp_term.Factor -= item.Factor;
                //        //else
                //        //    model.PhaseObjectiveFunction.Terms.Add(new Term() { Factor = item.Factor, VarType = item.VarType, Vector = item.Vector });
                //    }
                //    if (tmp_willaddedterms.Count > 0)
                //    {
                //        model.PhaseObjectiveFunction.RightHandValue -= constraint.RightHandValue;
                //    }
                //}
            }

            //6) sort the terms of new objective 
            TermComparer tc = new TermComparer();
            model.PhaseObjectiveFunction.Terms.Sort(tc);
        }
        internal static void PhaseOnePrintMatrix(this StandartSimplexModel model)
        {
            model.PrintMatrix();

            string tmp_sign = string.Empty;
            System.Diagnostics.Debug.WriteLine("Goal :" + model.GoalType.ToString());
            System.Diagnostics.Debug.Write("New Objective Function - " + model.GoalType.ToString() + ": ");
            foreach (Term item in model.PhaseObjectiveFunction.Terms)
            {
                tmp_sign = string.Empty;
                if (Math.Sign(item.Factor) > -1)
                    tmp_sign = "+";
                System.Diagnostics.Debug.Write(tmp_sign + item.Factor + "*" + item.Vector + " ");
            }
            System.Diagnostics.Debug.Write(" = ");
            System.Diagnostics.Debug.WriteLine(model.PhaseObjectiveFunction.RightHandValue);
            System.Diagnostics.Debug.WriteLine("*********************************");
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

        public static object DeepCopy( Type basemodel)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, basemodel);
                ms.Position = 0;
                return formatter.Deserialize(ms);
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
