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
            double[,] tmp_RightHandMatrix = new double[rowCount,1]; // +1 is for objective function, second dimension is for ratio 

            List<int> tmp_basicVariables = new List<int>();
            int tmp_colIndex = -1;

            //set the basic variable flag as -1
            for (int i = 0; i < rowCount; i++)
            {
                tmp_basicVariables.Add(-1);
            }

            for (int j = 0; j < columnCount; j++)
            {
                tmp_objectiveMatrix[j] = model.ObjectiveFunction.Terms[j].Factor;
                tmp_types[j] = model.ObjectiveFunction.Terms[j].VarType;
            }

            for (int i = 0; i < rowCount; i++)
            {
                tmp_colIndex = 0;
                for (int j = 0; j < columnCount; j++)
                {
                    //we add absolutely column element to the constarint matrix.
                    tmp_constarintMatrix[i, j] = model.Subjects[i].Terms[j].Factor;
                    //if term is basic, let us calculate the constarint matrix
                    if (model.ObjectiveFunction.Terms[j].Basic)// && model.Subjects[i].Terms[j].Factor==1)
                    {
                        //basic variable flag
                        if (tmp_constarintMatrix[i, j] == 1)// && tmp_rowIndex == tmp_colIndex)
                        {
                            tmp_basicVariables[i] = j;
                        }
                        tmp_colIndex++;
                    }
                }
                tmp_RightHandMatrix[i, 0] = model.Subjects[i].RightHandValue;
            }

            if (model.IsTwoPhase)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    tmp_phaseObjectiveMatrix[j] = (-1) * model.PhaseObjectiveFunction.Terms[j].Factor;
                }
                model.ObjectiveCost = (-1) * model.PhaseObjectiveFunction.RightHandValue;
            }

            model.ObjectiveMatrix = new Matrix(tmp_objectiveMatrix);
            model.ArtificialObjectiveMatrix = new Matrix(tmp_phaseObjectiveMatrix);
            model.RightHandMatrix = new Matrix(tmp_RightHandMatrix);
            model.ConstarintMatrix = new Matrix(tmp_constarintMatrix);
            model.VarTypes = tmp_types;
            model.BasicVariables = tmp_basicVariables;

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
            TruncatePhaseColumns(model, tmp_totalRemoveIndex, model.BasicVariables);
        }
        private static void TruncatePhaseColumns(StandartSimplexModel model, List<int> removeList, List<int> basicVariables)
        {
            //declare new matrix for replace Phase I result 
            int tmp_rowCount = model.ConstarintMatrix.RowCount;
            int tmp_oldColumnCount = model.ObjectiveMatrix.ColumnCount;
            int tmp_removeCount = removeList.Count;
            //int[] tmp_basic = new int[tmp_oldColumnCount - tmp_removeCount];
            double[] tmp_objectiveMatrix = new double[tmp_oldColumnCount - tmp_removeCount];
            VariableType[] tmp_types = new VariableType[tmp_oldColumnCount - tmp_removeCount];
            double[,] tmp_constarintMatrix = new double[tmp_rowCount, tmp_oldColumnCount - tmp_removeCount];

            //transfer the basic flag to teh temprory array
            for (int i = removeList.Count-1; i > -1; i--)
            {
                for (int j = 0; j < basicVariables.Count; j++)
                {
                    if (basicVariables[j] > removeList[i])
                        basicVariables[j] = basicVariables[j] - 1;
                }
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
            tmp_objectiveMatrix.CopyTo(tmp_objectiveMatrixUpdated, 0);
            //VariableType tmp_inclusive = (VariableType.Original | VariableType.Slack | VariableType.Excess);
            double tmp_pivotValue = 0;
            for (int i = 0; i < tmp_objectiveMatrix.Length; i++)
            {
                //is variable inclusion group
                //if (tmp_objectiveMatrix[i] != 0 && tmp_types[i] == (tmp_types[i] & tmp_inclusive))
                if (tmp_objectiveMatrix[i] != 0 && basicVariables.Contains(i) && tmp_constarintMatrix[basicVariables.IndexOf(i), i] == 1)
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
                        model.ObjectiveCost = Math.Round(model.ObjectiveCost - tmp_pivotValue * model.RightHandMatrix[j, 0], 5);                        
                    }
                }
            }
            model.VarTypes = tmp_types;
            model.ObjectiveMatrix = new Matrix(tmp_objectiveMatrixUpdated);
            model.ConstarintMatrix = new Matrix(tmp_constarintMatrix);
            model.BasicVariables = basicVariables;
            model.ObjectiveCost = 0;
        }
    }
}
