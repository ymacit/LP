//***************************
//Sınıf Adı : SimplexExtension 
//Dosya Adı : SimplexExtension.cs 
//Tanım : amaç ve ksıtlardan oluşan simplex model standartaştrıma yetenekleri sağlar
//****************************


using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Simplex.Problem;
using Simplex.Enums;
using Simplex.Helper;

namespace Simplex.Analysis
{
    public static class SimplexExtension
    {
        public static void ConvertStandardModel(this SimplexModel model)
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
            Dictionary<string, Term> m_VectorList = new Dictionary<string, Term>();
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
                    default: // if constarint equation = then plus artificial variable at the left side
                        constarint.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Artificial, Vector = m_artificialPrefix + m_artificialcount.ToString(), Index = constarint.Terms.Count - 1 });
                        m_artificialcount++;
                        break;
                }
            }

            //4) change signt the factor value of term in objective fonction terms and add positive balance variable ("Z")
            foreach (Term term in model.ObjectiveFunction.Terms)
            {
                term.Factor *= -1;
            }
            model.ObjectiveFunction.RightHandValue =0;
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

            ////3.X) if one of term contains of only one cosntarint that ise basic varibale, let us find it
            //foreach (KeyValuePair<string, Term> item in m_VectorList)
            //{
            //    List<Subject> tmp_list = model.Subjects.Where(subject => subject.Terms.Any(term => term.Vector.Equals(item.Key))).ToList();
            //    if (tmp_list.Count == 1 && tmp_list[0].Equality== EquailtyType.Equals)
            //        item.Value.isBasic = true;
            //    //(term => term.VarType== VariableType.Artificial).ToList();
            //}

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
                    model.ObjectiveFunction.Terms.Add(new Term() { Vector = item.Key, Factor = 0, VarType = item.Value.VarType });

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

        public static void PrintMatrix(this SimplexModel model)
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
        public static void PhaseOnePrintMatrix(this StandartSimplexModel model)
        {
            PrintMatrix((SimplexModel)model);

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
        public static void UpdateNegativeRHSValues(this SimplexModel model)
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

        public static TestMessage CheckBFS(this SimplexModel model)
        {
            TestMessage retval = new TestMessage() { Exception = null, Message = string.Empty };

            //first, update rhs values for equality direction control
            UpdateNegativeRHSValues(model);

            foreach (Subject constarint in model.Subjects)
            {
                if (constarint.Equality != EquailtyType.LessEquals)
                {
                    retval.Exception = new ArithmeticException();
                    retval.Message += "Constraint has Right-Hand Side Value " + constarint.RightHandValue.ToString() + " is " + constarint.Equality.ToString() + " different from " + EquailtyType.LessEquals.ToString() + "\n";
                }
            }
            if (retval.Exception == null)
                retval.Message = "Success";

            return retval;
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

        public static void CreatePhaseOneObjective(this StandartSimplexModel model, bool regularSimplex)
        {
            //Steps
            //#.Modify the constraints so that the RHS of each constraint is nonnegative (This requires that each constraint with a negative RHS be multiplied by - 1.Remember that if you multiply an inequality by any negative number, the direction of the inequality is reversed!). After modification, identify each constraint as a ≤, ≥ or = constraint.
            //#.Convert each inequality constraint to standard form(If constraint i is a ≤ constraint, we add a slack variable si; and if constraint i is a ≥ constraint, we subtract an excess variable ei).

            //Steps
            //1.Add an artificial variable ai to the constraints identified as ≥ or = constraints at the end of Step 1.Also add the sign restriction ai ≥ 0.
            //2.In the phase I, ignore the original LP’s objective function, instead solve an LP whose objective function is minimizing w = ai(sum of all the artificial variables).The act of solving the Phase I LP will force the artificial variables to be zero.
            //3.Since each artificial variable will be in the starting basis, all artificial variables must be eliminated from row 0 before beginning the simplex. Now solve the transformed problem by the simplex.


            //1) add all of artificial variables in orginal objective function to the phaseonefunction
            //Get the list of artificial variables in orginal objective function
            List<Term> tmp_artificialterms = model.ObjectiveFunction.Terms.Where(term => term.VarType== VariableType.Artificial).ToList();
            foreach (Term item in tmp_artificialterms)
            {
                model.PhaseObjectiveFunction.Terms.Add(new Term() { Factor = 1, VarType = item.VarType, Vector = item.Vector });
            }

            ////2) change signt the factor value of term in new objective fonction terms and add positive balance variable ("w")
            foreach (Term term in model.PhaseObjectiveFunction.Terms)
            {
                term.Factor *= -1;
            }
            //3) add balance type variable ("w") to the new objective function
            //model.PhaseObjectiveFunction.Terms.Insert(0, new Term() { Factor = 1, VarType = VariableType.Balance, Vector = "w", Index = 0 });

            //Let us define new objective function as negative (-w)
            model.PhaseObjectiveFunction.RightHandValue = 0;

            //4) find all artificial variable that has factor value is equal to 0 in cosntarint terms and put the artificial variable value in the new objective function;
            //   all artificial variables must be eliminated from row 0 before we can solve Phase I
            Term tmp_term = null;
            foreach (Subject constraint in model.Subjects)
            {
                if (constraint.Terms.Any(term => term.VarType == VariableType.Artificial && term.Factor == 1))
                {
                    //Find 
                    List<Term> tmp_willaddedterms = constraint.Terms.Where(term => term.Factor != 0).ToList();
                    if (regularSimplex)
                    {
                        foreach (Term item in tmp_willaddedterms)
                        {
                            tmp_term = null;
                            tmp_term = model.PhaseObjectiveFunction.Terms.Find(term => term.Vector.Equals(item.Vector));
                            if (tmp_term != null)
                                tmp_term.Factor += item.Factor;
                            else
                                model.PhaseObjectiveFunction.Terms.Add(new Term() { Factor = item.Factor, VarType = item.VarType, Vector = item.Vector });
                        }
                    }
                    if (tmp_willaddedterms.Count > 0)
                    {
                        if(regularSimplex)
                            model.PhaseObjectiveFunction.RightHandValue += constraint.RightHandValue;
                        else
                            model.PhaseObjectiveFunction.RightHandValue -= constraint.RightHandValue;
                    }
                }
            }

            //5)Add other variable that has zero factor value from objective function
            foreach (Term item in model.ObjectiveFunction.Terms)
            {
                if (item.VarType == VariableType.Balance)
                    continue;
                if (!model.PhaseObjectiveFunction.IsVectorContained(item.Vector))
                    model.PhaseObjectiveFunction.Terms.Add(new Term() { Vector = item.Vector, Factor = 0, VarType = item.VarType });
            }

            //6) sort the terms of new objective 
            TermComparer tc = new TermComparer();
            model.PhaseObjectiveFunction.Terms.Sort(tc);
        }


        //public static Matrix GetFullMatrix(this StandartSimplexModel model)
        //{

        //    int rowCount = model.Subjects.Count + 1;
        //    int columnCount = model.PhaseObjectiveFunction.Terms.Count;
        //    double[,] matrixArray = new double[rowCount, columnCount];

        //    for (int j = 0; j < columnCount; j++)
        //    {
        //        matrixArray[0,j]=model.PhaseObjectiveFunction.Terms[j].Factor;
        //    }
        //    for (int i = 1; i < rowCount; i++)
        //    {
        //        for (int j = 0; j < columnCount; j++)
        //        {
        //            matrixArray[i, j] = model.Subjects[i - 1].Terms[j].Factor;
        //        }
        //    }

        //    return new Matrix(matrixArray);
        //}

        public static void TruncatePhaseResult(this StandartSimplexModel model, Solution solution)
        {
            //transfer the phaseoneobjective function factors
            List<int> tmp_RemoveArtficialIndex = new List<int>();
            List<int> tmp_RemoveOtherIndex = new List<int>();
            List<int> tmp_totalRemoveIndex = new List<int>();
            bool tmp_retainArtficialFound = false; //if retain and remove count are eqauls as aritmetich operation

            for (int i = 0; i < model.PhaseOneObjectiveMatrix.Length; i++)
            {
                if (model.VarTypes[i] == VariableType.Artificial)
                {
                    if (model.PhaseOneObjectiveMatrix[i] < 0)
                        tmp_RemoveArtficialIndex.Add(i);
                    else
                        tmp_retainArtficialFound = true;
                }
                else
                {
                    if (model.PhaseOneObjectiveMatrix[i] < 0)
                        tmp_RemoveOtherIndex.Add(i);
                }
            }

            //II.Case : If w = 0, and no artificial variables are in the optimal Phase I basis:
            //  i.Drop all columns in the optimal Phase I tableau that correspond to the artificial variables.Drop Phase I row 0.
            //  ii.Combine the original objective function with the constraints from the optimal Phase I tableau(Phase II LP).If original objective function coefficients of BVs are nonzero row operations are done.
            //  iii.Solve Phase II LP using the simplex method.The optimal solution to the Phase II LP is the optimal solution to the original LP.
            if (!tmp_retainArtficialFound)
            {
                tmp_totalRemoveIndex=tmp_RemoveArtficialIndex;
            }
            //III.Case : If w = 0, and at least one artificial variable is in the optimal Phase I basis:
            //  i.Drop all columns in the optimal Phase I tableau that correspond to the nonbasic artificial variables and any variable from the original problem that has a negative coefficient in row 0 of the optimal Phase I tableau. Drop Phase I row 0.
            //  ii.Combine the original objective function with the constraints from the optimal Phase I tableau(Phase II LP).If original objective function coefficients of BVs are nonzero row operations are done.
            //  iii.Solve Phase II LP using the simplex method.The optimal solution to the Phase II LP is the optimal solution to the original LP.
            else
            {
                //merge and sort removed artificial and removed other variables 
                tmp_totalRemoveIndex.AddRange(tmp_RemoveArtficialIndex);
                tmp_totalRemoveIndex.AddRange(tmp_RemoveOtherIndex);
            }

            tmp_totalRemoveIndex.Sort();
            TruncatePhaseColumns (model, tmp_totalRemoveIndex);

        }
        //public static void TruncateAllArtificialVariables(this StandartSimplexModel model, List<int> removeList)
        //{
        //    //declare new matrix for replace Phase I result 
        //    int tmp_removeCount = removeList.Count;
        //    double[] tmp_objectiveMatrix = new double[model.ObjectiveMatrix.Length - tmp_removeCount];
        //    VariableType[] tmp_types = new VariableType[model.VarTypes.Length- tmp_removeCount];
        //    double[,] tmp_constarintMatrix = new double[model.ConstarintMatrix.GetLength(0), model.ConstarintMatrix.GetLength(1)- tmp_removeCount];

        //    int tmp_newIndex = 0;
        //    for (int i = 0; i < model.PhaseOneObjectiveMatrix.Length; i++)
        //    {
        //        if(model.VarTypes[i] != VariableType.Artificial)
        //        {
        //            tmp_types[tmp_newIndex] = model.VarTypes[i];
        //            tmp_objectiveMatrix[tmp_newIndex] = model.ObjectiveMatrix[i];
        //            for (int j=0; j< model.ConstarintMatrix.GetLength(0); j++)
        //            {
        //                tmp_constarintMatrix[j, tmp_newIndex] = Math.Round( model.ConstarintMatrix[j,i],5);
        //            }
        //            tmp_newIndex++;
        //        }
        //    }

        //    //Update the objective function original variable with cosntraint value;
        //    double[] tmp_objectiveMatrixUpdated = new double[tmp_objectiveMatrix.Length];
        //    int tmp_ObjectiveLeftValueIndex = model.RightHandMatrix.GetLength(0)-1;
        //    tmp_objectiveMatrix.CopyTo(tmp_objectiveMatrixUpdated, 0);
        //    VariableType tmp_inclusive = (VariableType.Original | VariableType.Slack | VariableType.Excess);
        //    double tmp_pivotValue = 0;
        //    for (int i = 0; i < tmp_objectiveMatrix.Length; i++)
        //    {
        //        //is variable inclusion group
        //        if (tmp_objectiveMatrix[i]!=0 && tmp_types[i] == (tmp_types[i] & tmp_inclusive) )
        //        {
        //            tmp_pivotValue = tmp_objectiveMatrix[i];
        //            //Find the related cosntraint row
        //            for (int j = 0; j < tmp_constarintMatrix.GetLength(0); j++)
        //            {
        //                //pivot cell must be addresset by unit matrix 
        //                if (tmp_constarintMatrix[j, i] != 1)
        //                    continue;

        //                //5)Calculate new objective Row (Rn') by multiple contraint factor.RO'=RO-xRn'
        //                for (int k = 0; k < tmp_objectiveMatrix.Length; k++)
        //                {
        //                    tmp_objectiveMatrixUpdated[k] = Math.Round(tmp_objectiveMatrixUpdated[k] - tmp_pivotValue * tmp_constarintMatrix[j, k],5);
        //                }
        //                //in addition set the left value 
        //                model.RightHandMatrix[tmp_ObjectiveLeftValueIndex, 0] = Math.Round(model.RightHandMatrix[tmp_ObjectiveLeftValueIndex, 0] - tmp_pivotValue * model.RightHandMatrix[j, 0],5);
        //                model.RightHandMatrix[tmp_ObjectiveLeftValueIndex, 1] = 0;
        //            }
        //        }
        //    }

        //    model.VarTypes = tmp_types;
        //    model.ObjectiveMatrix = tmp_objectiveMatrixUpdated;
        //    model.ConstarintMatrix = tmp_constarintMatrix;

        //}

        private static void TruncatePhaseColumns(StandartSimplexModel model, List<int> removeList)
        {
            //declare new matrix for replace Phase I result 
            int tmp_removeCount = removeList.Count;
            int[] tmp_basic = new int[model.Basics.Length - tmp_removeCount];
            double[] tmp_objectiveMatrix = new double[model.ObjectiveMatrix.Length - tmp_removeCount];
            VariableType[] tmp_types = new VariableType[model.VarTypes.Length - tmp_removeCount];
            double[,] tmp_constarintMatrix = new double[model.ConstarintMatrix.GetLength(0), model.ConstarintMatrix.GetLength(1) - tmp_removeCount];

            Dictionary<Term, Subject> tmp_RemovePairList = new Dictionary<Term, Subject>();

            int tmp_newIndex = 0;
            for (int i = 0; i < model.PhaseOneObjectiveMatrix.Length; i++)
            {
                if (removeList.Contains(i))
                {
                    tmp_RemovePairList.Add(model.ObjectiveFunction.Terms[i], model.ObjectiveFunction);
                    for (int j = 0; j < model.ConstarintMatrix.GetLength(0); j++)
                    {
                        tmp_RemovePairList.Add(model.Subjects[j].Terms[i], model.Subjects[j]);
                    }
                }
                else
                {
                    //narrow the types
                    tmp_types[tmp_newIndex] = model.VarTypes[i];
                    //narrow the basic matrix value
                    tmp_basic[tmp_newIndex] = model.Basics[i];
                    tmp_objectiveMatrix[tmp_newIndex] = model.ObjectiveMatrix[i];
                    for (int j = 0; j < model.ConstarintMatrix.GetLength(0); j++)
                    {
                        tmp_constarintMatrix[j, tmp_newIndex] = Math.Round(model.ConstarintMatrix[j, i], 5);
                    }
                    tmp_newIndex++;
                }
            }

            foreach (KeyValuePair<Term, Subject> item in tmp_RemovePairList)
            {
                item.Value.Terms.Remove(item.Key);
            }

            //Update the objective function original variable with cosntraint value;
            double[] tmp_objectiveMatrixUpdated = new double[tmp_objectiveMatrix.Length];
            int tmp_ObjectiveLeftValueIndex = model.RightHandMatrix.GetLength(0) - 1;
            tmp_objectiveMatrix.CopyTo(tmp_objectiveMatrixUpdated, 0);
            //VariableType tmp_inclusive = (VariableType.Original | VariableType.Slack | VariableType.Excess);
            double tmp_pivotValue = 0;
            for (int i = 0; i < tmp_objectiveMatrix.Length; i++)
            {
                //is variable inclusion group
                //if (tmp_objectiveMatrix[i] != 0 && tmp_types[i] == (tmp_types[i] & tmp_inclusive))
                if (tmp_objectiveMatrix[i] != 0 && tmp_basic[i]!= -1 && tmp_constarintMatrix[tmp_basic[i], i]==1)
                {
                    tmp_pivotValue = tmp_objectiveMatrix[i];
                    //Find the related cosntraint row
                    for (int j = 0; j < tmp_constarintMatrix.GetLength(0); j++)
                    {
                        //pivot cell must be addresset by unit matrix 
                        if (tmp_constarintMatrix[j, i] != 1)
                            continue;

                        //5)Calculate new objective Row (Rn') by multiple contraint factor.RO'=RO-xRn'
                        for (int k = 0; k < tmp_objectiveMatrix.Length; k++)
                        {
                            tmp_objectiveMatrixUpdated[k] = Math.Round(tmp_objectiveMatrixUpdated[k] - tmp_pivotValue * tmp_constarintMatrix[j, k], 5);
                        }
                        //in addition set the left value 
                        model.RightHandMatrix[tmp_ObjectiveLeftValueIndex, 0] = Math.Round(model.RightHandMatrix[tmp_ObjectiveLeftValueIndex, 0] - tmp_pivotValue * model.RightHandMatrix[j, 0], 5);
                        model.RightHandMatrix[tmp_ObjectiveLeftValueIndex, 1] = 0;
                    }
                }
            }
            model.Basics = tmp_basic;
            model.VarTypes = tmp_types;
            model.ObjectiveMatrix = tmp_objectiveMatrixUpdated;
            model.ConstarintMatrix = tmp_constarintMatrix;
        }

        public static void CreateMatrixSet(this StandartSimplexModel model)
        {
            int rowCount = model.Subjects.Count;
            int columnCount = model.ObjectiveFunction.Terms.Count;
            double[] tmp_objectiveMatrix = new double[columnCount];
            //miz w= a1  + a2 + a3 + .. +an
            double[] tmp_phaseObjectiveMatrix = new double[columnCount];
            VariableType[] tmp_types = new VariableType[columnCount];
            double[,] tmp_constarintMatrix = new double[rowCount, columnCount];
            double[,] tmp_RightHandMatrix = new double[rowCount+1, 2]; // +1 is for objective function, second dimension is for ratio 
            int[] tmp_basics = new int[columnCount];

            if (model.IsTwoPhase)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    tmp_phaseObjectiveMatrix[j] = model.PhaseObjectiveFunction.Terms[j].Factor;
                    tmp_types[j] = model.ObjectiveFunction.Terms[j].VarType;
                }
                tmp_RightHandMatrix[rowCount, 0] = model.PhaseObjectiveFunction.RightHandValue;
            }

            for (int j = 0; j < columnCount; j++)
            {
                tmp_objectiveMatrix[j] = model.ObjectiveFunction.Terms[j].Factor;
            }

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    tmp_constarintMatrix[i, j] = model.Subjects[i].Terms[j].Factor;
                }
                tmp_RightHandMatrix[i, 0] = model.Subjects[i].RightHandValue;
            }

            for (int i = 0; i < tmp_basics.Length; i++)
            {
                for (int j = 0; j < rowCount; j++)
                {
                    if (tmp_types[i] != VariableType.Original && tmp_constarintMatrix[j, i] == 1)
                        tmp_basics[i] = j;
                    else
                        tmp_basics[i] = -1;
                }
            }

            model.Basics = tmp_basics;
            model.ObjectiveMatrix = tmp_objectiveMatrix;
            model.PhaseOneObjectiveMatrix = tmp_phaseObjectiveMatrix;
            model.RightHandMatrix = tmp_RightHandMatrix;
            model.ConstarintMatrix = tmp_constarintMatrix;
            model.VarTypes = tmp_types;

            //model.ObjectiveMatrix = new Matrix(tmp_objectiveMatrix);
            //model.PhaseOneObjectiveMatrix = new Matrix(tmp_phaseObjectiveMatrix);
            //model.RightHandMatrix = new Matrix(tmp_RightHandMatrix);
            //model.ConstarintMatrix = new Matrix(tmp_constarintMatrix);

        }

        public static void GenerateBasisMatrices(this RevisedSimplexModel model)
        {

            /*
             *1-amaç fonksiyonda sadece temel değişkenler kalacak şekilde daralt
             *  -eğer iki aşamalı ise amaç fonksiyonu sadece yapay değişkenler cinsinden yazılır  ve sadece onlar temel değişkendir.
             *2- 
             * 
             * 
             * 
             * 
             * 
             */
            int rowCount = model.Subjects.Count;
            int columnCount = model.ObjectiveFunction.Terms.Count;
            int tmp_artificalCount = model.PhaseObjectiveFunction.Terms.Where(term => term.VarType == VariableType.Artificial).Count<Term>();

            VariableType[] tmp_types = new VariableType[columnCount];
            Matrix tmp_PhaseOneBasisMatrix = null;
            Matrix tmp_PhaseOneBasisRightHandMatrix = null;
            Matrix tmp_PhaseOneBasisObjectiveMatrix = null;
            Matrix tmp_PhaseOneNonBasisMatrix = null;

            Matrix tmp_BasisMatrix = new Matrix(tmp_artificalCount, 1); ;
            Matrix tmp_NonBasisMatrix = new Matrix(model.ObjectiveFunction.Terms.Count, model.Subjects.Count); ;
            Matrix tmp_BasisRightHandMatrix = new Matrix(model.Subjects.Count,2);
            Matrix tmp_BasisObjectiveMatrix = new Matrix(model.ObjectiveFunction.Terms.Count- tmp_artificalCount, 1);

            for (int j = 0; j < model.ObjectiveFunction.Terms.Count; j++)
            {
                if(model.ObjectiveFunction.Terms[j].VarType!= VariableType.Artificial)
                    tmp_BasisObjectiveMatrix[j, 0] = model.ObjectiveFunction.Terms[j].Factor;
            }

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    tmp_NonBasisMatrix[i, j] = model.Subjects[i].Terms[j].Factor;
                }
                tmp_BasisRightHandMatrix[i, 0] = model.Subjects[i].RightHandValue;
            }


            if (model.IsTwoPhase)
            {
                int tmp_rowIndex = 0;
                int tmp_colIndex = 0;

                tmp_PhaseOneBasisObjectiveMatrix = new Matrix(tmp_artificalCount, 1);
                tmp_PhaseOneBasisRightHandMatrix = new Matrix(tmp_artificalCount+1, 1);
                tmp_PhaseOneBasisMatrix = new Matrix(tmp_artificalCount, tmp_artificalCount);
                tmp_PhaseOneNonBasisMatrix = new Matrix(tmp_artificalCount, model.ObjectiveFunction.Terms.Count);


                for (int j = 0; j < model.PhaseObjectiveFunction.Terms.Count; j++)
                {
                    if (model.PhaseObjectiveFunction.Terms[j].VarType == VariableType.Artificial)
                    {
                        tmp_PhaseOneBasisObjectiveMatrix[tmp_rowIndex, 0] = model.PhaseObjectiveFunction.Terms[j].Factor;
                        tmp_rowIndex++;
                    }
                    //tmp_types[j] = model.ObjectiveFunction.Terms[j].VarType;
                }
                tmp_PhaseOneBasisRightHandMatrix[tmp_rowIndex, 0] = model.PhaseObjectiveFunction.RightHandValue;

                List<Term> tmp_SubjectArtificalTerms = null;

                //reset temrory row index
                tmp_rowIndex = 0;
                for (int i = 0; i < rowCount; i++)
                {
                    //reset temrory column index
                    tmp_colIndex = 0;
                    //get list of artificial term from current(i) subject.
                    tmp_SubjectArtificalTerms = model.Subjects[i].Terms.Where(term => term.VarType == VariableType.Artificial).ToList();
                    //we assume this subject row does not contain member of unit matrix artificial term
                    if (tmp_SubjectArtificalTerms.Any(term => term.VarType == VariableType.Artificial && term.Factor == 1))
                    {
                        for (int j = 0; j < model.Subjects[0].Terms.Count; j++)
                        {
                            //we add absolutely column element to the nonbasis matrix.
                            tmp_PhaseOneNonBasisMatrix[tmp_rowIndex, j] = model.Subjects[i].Terms[j].Factor;
                            //we add selectively column element to the basis matrix .
                            if (model.Subjects[i].Terms[j].VarType == VariableType.Artificial)
                            {
                                tmp_PhaseOneBasisMatrix[tmp_rowIndex, tmp_colIndex] = tmp_PhaseOneNonBasisMatrix[tmp_rowIndex, j];
                                tmp_colIndex++;
                            }
                        }
                        tmp_PhaseOneBasisRightHandMatrix[tmp_rowIndex, 0] = model.Subjects[i].RightHandValue;
                        tmp_rowIndex++;
                    }
                }
                
            }


            model.PhaseOneBasisMatrix = tmp_PhaseOneBasisMatrix;
            model.PhaseOneBasisObjectiveMatrix = tmp_PhaseOneBasisObjectiveMatrix;
            model.PhaseOneNonBasisMatrix = tmp_PhaseOneNonBasisMatrix;
            model.PhaseOneBasisRightHandMatrix = tmp_PhaseOneBasisRightHandMatrix;

            model.BasisMatrix = tmp_BasisMatrix;
            model.BasisObjectiveMatrix = tmp_BasisObjectiveMatrix;
            model.BasisRightHandMatrix = tmp_BasisRightHandMatrix;

            model.VarTypes = tmp_types;
        }
    }

    public struct TestMessage
    {
        public Exception Exception { get; set; }
        public string Message { get; set; }
    }

    public class TermComparer : IComparer<Term>
    {
        public int Compare(Term x, Term y)
        {
            if (x != null && y != null)
            {
                int typecompare = x.VarType.CompareTo(y.VarType);

                if (typecompare!=0)
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
