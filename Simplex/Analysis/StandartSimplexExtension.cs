using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simplex.Model;
using Simplex.Enums;
using Simplex.Helper;


namespace Simplex.Analysis
{
    public static class StandartSimplexExtension
    {
        internal static void CreateMatrixSet(this StandartSimplexModel model)
        {
            int rowCount = model.Subjects.Count;
            int columnCount = model.ObjectiveFunction.Terms.Count;
            double[] tmp_objectiveMatrix = new double[columnCount];
            //miz w= a1  + a2 + a3 + .. +an
            double[] tmp_phaseObjectiveMatrix = new double[columnCount];
            VariableType[] tmp_types = new VariableType[columnCount];
            double[,] tmp_constarintMatrix = new double[rowCount, columnCount];
            double[,] tmp_RightHandMatrix = new double[rowCount + 1, 2]; // +1 is for objective function, second dimension is for ratio 

            if (model.IsTwoPhase)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    tmp_phaseObjectiveMatrix[j] = model.PhaseObjectiveFunction.Terms[j].Factor;
                }
                tmp_RightHandMatrix[rowCount, 0] = model.PhaseObjectiveFunction.RightHandValue;
            }

            for (int j = 0; j < columnCount; j++)
            {
                tmp_objectiveMatrix[j] = model.ObjectiveFunction.Terms[j].Factor;
                tmp_types[j] = model.ObjectiveFunction.Terms[j].VarType;
            }

            for (int i = 0; i < rowCount; i++)
            {
                //set the basic variable flag as -1
                tmp_RightHandMatrix[i, 1] = -1;
                for (int j = 0; j < columnCount; j++)
                {
                    tmp_constarintMatrix[i, j] = model.Subjects[i].Terms[j].Factor;

                    //set the basic variable flag as j
                    if (model.Subjects[i].Terms[j].isBasic)
                        tmp_RightHandMatrix[i, 1] = j;
                }
                tmp_RightHandMatrix[i, 0] = model.Subjects[i].RightHandValue;
            }

            model.ObjectiveMatrix = new Matrix(tmp_objectiveMatrix);
            model.ArtificialObjectiveMatrix = new Matrix(tmp_phaseObjectiveMatrix);
            model.RightHandMatrix = new Matrix(tmp_RightHandMatrix);
            model.ConstarintMatrix = new Matrix(tmp_constarintMatrix);
            model.VarTypes = tmp_types;

        }
        internal static void CreatePhaseOneObjective(this StandartSimplexModel model)
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
            //Get the list of artificial variables in orginal objective function
            List<Term> tmp_artificialterms = model.ObjectiveFunction.Terms.Where(term => term.VarType == VariableType.Artificial).ToList();
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
                    foreach (Term item in tmp_willaddedterms)
                    {
                        tmp_term = null;
                        tmp_term = model.PhaseObjectiveFunction.Terms.Find(term => term.Vector.Equals(item.Vector));
                        if (tmp_term != null)
                            tmp_term.Factor += item.Factor;
                        else
                            model.PhaseObjectiveFunction.Terms.Add(new Term() { Factor = item.Factor, VarType = item.VarType, Vector = item.Vector });
                    }
                    if (tmp_willaddedterms.Count > 0)
                    {
                        model.PhaseObjectiveFunction.RightHandValue += constraint.RightHandValue;
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

        internal static void TruncatePhaseResult(this StandartSimplexModel model, Solution solution)
        {
            //transfer the phaseoneobjective function factors
            List<int> tmp_RemoveArtficialIndex = new List<int>();
            List<int> tmp_RemoveOtherIndex = new List<int>();
            List<int> tmp_totalRemoveIndex = new List<int>();
            bool tmp_retainArtficialFound = false; //if retain and remove count are eqauls as aritmetich operation

            for (int i = 0; i < model.ArtificialObjectiveMatrix.ColumnCount; i++)
            {
                if (model.VarTypes[i] == VariableType.Artificial)
                {
                    if (model.ArtificialObjectiveMatrix[0, i] < 0)
                        tmp_RemoveArtficialIndex.Add(i);
                    else
                        tmp_retainArtficialFound = true;
                }
                else
                {
                    if (model.ArtificialObjectiveMatrix[0, i] < 0)
                        tmp_RemoveOtherIndex.Add(i);
                }
            }

            //II.Case : If w = 0, and no artificial variables are in the optimal Phase I basis:
            //  i.Drop all columns in the optimal Phase I tableau that correspond to the artificial variables.Drop Phase I row 0.
            //  ii.Combine the original objective function with the constraints from the optimal Phase I tableau(Phase II LP).If original objective function coefficients of BVs are nonzero row operations are done.
            //  iii.Solve Phase II LP using the simplex method.The optimal solution to the Phase II LP is the optimal solution to the original LP.
            if (!tmp_retainArtficialFound)
            {
                tmp_totalRemoveIndex = tmp_RemoveArtficialIndex;
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
            TruncatePhaseColumns(model, tmp_totalRemoveIndex);
        }
        private static void TruncatePhaseColumns(StandartSimplexModel model, List<int> removeList)
        {
            //declare new matrix for replace Phase I result 
            int tmp_rowCount = model.ConstarintMatrix.RowCount;
            int tmp_oldColumnCount = model.ObjectiveMatrix.ColumnCount;
            int tmp_removeCount = removeList.Count;
            int[] tmp_oldbasic = new int[tmp_oldColumnCount];
            int[] tmp_basic = new int[tmp_oldColumnCount - tmp_removeCount];
            double[] tmp_objectiveMatrix = new double[tmp_oldColumnCount - tmp_removeCount];
            VariableType[] tmp_types = new VariableType[tmp_oldColumnCount - tmp_removeCount];
            double[,] tmp_constarintMatrix = new double[tmp_rowCount, tmp_oldColumnCount - tmp_removeCount];

            //transfer the basic flag to teh temprory array
            for (int i = 0; i < tmp_oldColumnCount; i++)
            {
                tmp_oldbasic[i] = -1;
            }
            for (int i = 0; i < tmp_rowCount; i++)
            {
                if (model.RightHandMatrix[i, 1] != -1)
                    tmp_oldbasic[(int)model.RightHandMatrix[i, 1]] = i;
            }
            Dictionary<Term, Subject> tmp_RemovePairList = new Dictionary<Term, Subject>();
            int tmp_newIndex = 0;
            for (int i = 0; i < model.ArtificialObjectiveMatrix.ColumnCount; i++)
            {
                if (removeList.Contains(i))
                {
                    tmp_RemovePairList.Add(model.ObjectiveFunction.Terms[i], model.ObjectiveFunction);
                    for (int j = 0; j < model.ConstarintMatrix.RowCount; j++)
                    {
                        tmp_RemovePairList.Add(model.Subjects[j].Terms[i], model.Subjects[j]);
                    }
                }
                else
                {
                    //narrow the types
                    tmp_types[tmp_newIndex] = model.VarTypes[i];
                    //narrow the basic matrix value
                    tmp_basic[tmp_newIndex] = tmp_oldbasic[i];
                    tmp_objectiveMatrix[tmp_newIndex] = model.ObjectiveMatrix[0, i];
                    for (int j = 0; j < model.ConstarintMatrix.RowCount; j++)
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
            int tmp_ObjectiveLeftValueIndex = model.RightHandMatrix.RowCount - 1;
            tmp_objectiveMatrix.CopyTo(tmp_objectiveMatrixUpdated, 0);
            //VariableType tmp_inclusive = (VariableType.Original | VariableType.Slack | VariableType.Excess);
            double tmp_pivotValue = 0;
            for (int i = 0; i < tmp_objectiveMatrix.Length; i++)
            {
                //is variable inclusion group
                //if (tmp_objectiveMatrix[i] != 0 && tmp_types[i] == (tmp_types[i] & tmp_inclusive))
                if (tmp_objectiveMatrix[i] != 0 && tmp_basic[i] != -1 && tmp_constarintMatrix[tmp_basic[i], i] == 1)
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
            //transfer temprory basic set to righthand
            for (int i = 0; i < tmp_rowCount; i++)
            {
                model.RightHandMatrix[i, 1] = -1;
            }
            for (int i = 0; i < tmp_basic.Length; i++)
            {
                if (tmp_basic[i] != -1)
                    model.RightHandMatrix[tmp_basic[i], 1] = i;
            }
            model.VarTypes = tmp_types;
            model.ObjectiveMatrix = new Matrix(tmp_objectiveMatrixUpdated);
            model.ConstarintMatrix = new Matrix(tmp_constarintMatrix);
        }
    }
}
