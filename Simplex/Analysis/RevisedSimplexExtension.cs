using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using Simplex.Model;
using Simplex.Enums;
using Simplex.Helper;

namespace Simplex.Analysis
{
    public static class RevisedSimplexExtension
    {
        internal static void GenerateBasisMatrices(this RevisedSimplexModel model)
        {
            //Allways primal matrices
            GeneratePrimalMatrices(model);

            if (model.IsTwoPhase)
                //plus artficial variables                 
                GeneratePhaseMatrices(model);

            //set the inverse matrix
            model.BasisInverseMatrix = model.BasisMatrix.Invert();
        }


        private static void GeneratePrimalMatrices(RevisedSimplexModel model)
        {
            /*
             *1-amaç fonksiyonda sadece temel değişkenler kalacak şekilde daralt
            */
            int tmp_colIndex = 0;
            int rowCount = model.Subjects.Count;
            int columnCount = model.ObjectiveFunction.Terms.Count;
            int tmp_basicCount = model.ObjectiveFunction.Terms.Where(term => term.Basic == true).Count<Term>();

            Matrix tmp_BasisMatrix = new Matrix(tmp_basicCount, tmp_basicCount);
            Matrix tmp_NonBasisMatrix = new Matrix(model.Subjects.Count, model.ObjectiveFunction.Terms.Count);
            Matrix tmp_BasisRightHandMatrix = new Matrix(model.Subjects.Count, 1);
            Matrix tmp_BasisObjectiveMatrix = new Matrix(1, tmp_basicCount);
            Matrix tmp_BasisNonObjectiveMatrix = new Matrix(1, model.ObjectiveFunction.Terms.Count);
            VariableType[] tmp_variableTypes = new VariableType[tmp_basicCount];
            List<int>tmp_basicVariables = new List<int>();

            for (int i = 0; i < tmp_basicCount; i++)
            {
                tmp_basicVariables.Add(-1);
            }

            tmp_colIndex = 0;
            for (int j = 0; j < model.ObjectiveFunction.Terms.Count; j++)
            {
                //tmp_types[j] = model.ObjectiveFunction.Terms[j].VarType;
                tmp_BasisNonObjectiveMatrix[0, j] =  model.ObjectiveFunction.Terms[j].Factor;
                if (model.ObjectiveFunction.Terms[j].Basic)
                {
                    tmp_BasisObjectiveMatrix[0, tmp_colIndex] = 0;
                    tmp_colIndex++;
                }
            }

            tmp_colIndex = 0;

            for (int i = 0; i < rowCount; i++)
            {
                tmp_colIndex = 0;
                for (int j = 0; j < columnCount; j++)
                {
                    //we add absolutely column element to the nonbasis matrix.
                    tmp_NonBasisMatrix[i, j] = model.Subjects[i].Terms[j].Factor;
                    //if term is basic, let us calculate the basic matrix
                    if (model.ObjectiveFunction.Terms[j].Basic)// && model.Subjects[i].Terms[j].Factor==1)
                    {
                        tmp_BasisMatrix[i, tmp_colIndex] = tmp_NonBasisMatrix[i, j];
                        //basic variable flag
                        if (tmp_BasisMatrix[i, tmp_colIndex] == 1)// && tmp_rowIndex == tmp_colIndex)
                        {
                            tmp_basicVariables[i] = j;
                            tmp_variableTypes[i] = model.ObjectiveFunction.Terms[j].VarType;
                        }
                        tmp_colIndex++;
                    }
                }
                tmp_BasisRightHandMatrix[i, 0] = model.Subjects[i].RightHandValue;
                model.ObjectiveCost -= tmp_BasisRightHandMatrix[i, 0];
            }

            model.BasisMatrix = tmp_BasisMatrix;
            model.NonBasisMatrix = tmp_NonBasisMatrix;
            model.BasisObjectiveMatrix = tmp_BasisObjectiveMatrix;
            model.BasisRightHandMatrix = tmp_BasisRightHandMatrix;
            model.BasisNonObjectiveMatrix = tmp_BasisNonObjectiveMatrix;
            model.BasicVariables = tmp_basicVariables;
            model.VarTypes = tmp_variableTypes;
        }

        private static void GeneratePhaseMatrices(RevisedSimplexModel model)
        {
            /*
             *  -eğer iki aşamalı ise amaç fonksiyonu sadece yapay değişkenler cinsinden yazılır  ve sadece onlar temel değişkendir.
             */
            int tmp_colIndex = 0;
            int columnCount = model.PhaseObjectiveFunction.Terms.Count;
            int tmp_basicCount = model.ObjectiveFunction.Terms.Where(term => term.Basic == true).Count<Term>();

            Matrix tmp_PhaseBasisObjectiveMatrix = null;
            Matrix tmp_PhaseNonBasisObjectiveMatrix = null;

            tmp_colIndex = 0;

            tmp_PhaseBasisObjectiveMatrix = new Matrix(1, tmp_basicCount);
            tmp_PhaseNonBasisObjectiveMatrix = new Matrix(1, columnCount);

            for (int j = 0; j < columnCount; j++)
            {
                tmp_PhaseNonBasisObjectiveMatrix[0, j] = (-1) * model.PhaseObjectiveFunction.Terms[j].Factor;
                //if (model.PhaseObjectiveFunction.Terms[j].VarType == VariableType.Artificial)
                if (model.ObjectiveFunction.Terms[j].Basic)
                {
                    tmp_PhaseBasisObjectiveMatrix[0, tmp_colIndex] = (-1) * model.PhaseObjectiveFunction.Terms[j].Factor;
                    tmp_colIndex++;
                }
            }

            model.ObjectiveCost =  model.PhaseObjectiveFunction.RightHandValue;

            model.PhaseBasisObjectiveMatrix = tmp_PhaseBasisObjectiveMatrix;
            model.PhaseNonBasisObjectiveMatrix = tmp_PhaseNonBasisObjectiveMatrix;
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

        internal static void TruncatePhaseResult(this RevisedSimplexModel model, Solution solution)
        {
            
            //transfer the phaseoneobjective function factors
            List<int> tmp_RemoveArtficialIndex = new List<int>();
            List<int> tmp_RemoveOtherIndex = new List<int>();
            List<int> tmp_totalRemoveIndex = new List<int>();
            bool tmp_retainArtficialFound = false; //if retain and remove count are eqauls as aritmetich operation

            for (int i = 0; i < model.PhaseObjectiveFunction.Terms.Count; i++)
            {
                if (solution.BasicVariables.Contains(i))
                    continue;
                else if (model.PhaseObjectiveFunction.Terms[i].VarType == VariableType.Artificial)
                    tmp_RemoveArtficialIndex.Add(i);
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

            TruncatePhaseColumns(model, tmp_totalRemoveIndex, solution.BasicVariables);
            model.BasicVariables = solution.BasicVariables;
        }

        private static void TruncatePhaseColumns(RevisedSimplexModel model, List<int> removeVariables, List<int> basicVariables)
        {
            //VariableType tmp_inclusive = (VariableType.Original | VariableType.Slack | VariableType.Excess);

            //remove the listed column
            //set the native objective function
            for (int i = 0; i < basicVariables.Count; i++)
            {
                model.BasisObjectiveMatrix[0, i] =  model.ObjectiveFunction.Terms[basicVariables[i]].Factor;
            }

            int tmp_RemoveIndex = -1;
            for (int i = removeVariables.Count - 1; i > -1; i--)
            {
                tmp_RemoveIndex = removeVariables[i];
                model.ObjectiveFunction.Terms.RemoveAt(tmp_RemoveIndex);
                foreach (Subject item in model.Subjects)
                {
                    item.Terms.RemoveAt(tmp_RemoveIndex);
                }
                //roll back the variable index
                for (int j = 0; j < model.BasicVariables.Count; j++)
                {
                    if (model.BasicVariables[j] >= tmp_RemoveIndex)
                        model.BasicVariables[j] = model.BasicVariables[j] - 1;
                }
            }

            for (int i = 0; i < model.ObjectiveFunction.Terms.Count; i++)
            {
                if (basicVariables.Contains(i))
                    model.ObjectiveFunction.Terms[i].Basic = true;
                else
                    model.ObjectiveFunction.Terms[i].Basic = false;
            }

            int tmp_columnCount = model.ObjectiveFunction.Terms.Count;
            int tmp_rowCount = model.Subjects.Count;
            int tmp_basicCount = model.ObjectiveFunction.Terms.Where(term => term.Basic == true).Count<Term>();

            Matrix tmp_NonObjective = new Matrix(1, tmp_columnCount);
            Matrix tmp_NonBasis = new Matrix(tmp_rowCount, tmp_columnCount);
            int tmp_colIndex = 0;
            for (int i = 0; i < model.NonBasisMatrix.ColumnCount; i++)
            {
                if (!removeVariables.Contains(i))
                {
                    tmp_NonObjective[0, tmp_colIndex] = model.BasisNonObjectiveMatrix[0, i];
                    for (int j = 0; j < model.NonBasisMatrix.RowCount; j++)
                    {
                        tmp_NonBasis[j, tmp_colIndex] = model.NonBasisMatrix[j, i];
                    }
                    tmp_colIndex++;
                }
            }
            model.NonBasisMatrix = tmp_NonBasis;
            model.BasisNonObjectiveMatrix = tmp_NonObjective;
        }

        internal static RevisedSimplexModel DeepCopy(this RevisedSimplexModel basemodel)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, basemodel);
                ms.Position = 0;
                return (RevisedSimplexModel)formatter.Deserialize(ms);
            }
        }

    }
}
