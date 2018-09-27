//***************************
//Sınıf Adı : SolverBase 
//Dosya Adı : SolverBase.cs 
//Tanım : Simplex model için çözümleme olanakları sağar
/*
 * 
 * Dr. Y. İlker Topcu (www.ilkertopcu.info) & Dr. Özgür Kabak (kabak@itu.edu.tr) 
 *4.3 THE SIMPLEX ALGORITHM
 *Note that in the examples considered at the graphical solution, the unique optimal solution to the LP occurred at a vertex (corner) of the feasible region. In fact it is true that for any LP the optimal solution occurs at a vertex of the feasible region. This fact is the key to the simplex algorithm for solving LP's.
 *Essentially the simplex algorithm starts at one vertex of the feasible region and moves (at each iteration) to another (adjacent) vertex, improving (or leaving unchanged) the objective function as it does so, until it reaches the vertex corresponding to the optimal LP solution.
 *The simplex algorithm for solving linear programs (LP's) was developed by Dantzig in the late 1940's and since then a number of different versions of the algorithm have been developed. One of these later versions, called the revised simplex algorithm (sometimes known as the "product form of the inverse" simplex algorithm) forms the basis of most modern computer packages for solving LP's.
 *Steps
 *1. Convert the LP to standard form
 *2. Obtain a basic feasible solution (bfs) from the standard form
 *3. Determine whether the current bfs is optimal. If it is optimal, stop.
 *4. If the current bfs is not optimal, determine which nonbasic variable should become a basic variable and which basic variable should become a nonbasic variable to find a new bfs with a better objective function value
 *5. Go back to Step 3.
 */
//****************************


/*
 * https://mat.gsia.cmu.edu/classes/QUANT/NOTES/chap7.pdf
 *[Rule 1]
 *If al l variables have a nonnegative coecient in Row 0, the current basic solution is optimal.
 *Otherwise, pick a variable xj with a negative coecient in Row 0.
 * 
 *[Rule 2]
 * For each Row i, i  1, where there is a strictly positive "entering variable coecient",
 * compute the ratio of the Right Hand Side to the "entering variable coecient". Choose the pivot  row as being the one with MINIMUM ratio.
*/

using System;
using System.Collections.Generic;
using System.Text;
using Simplex.Enums;
using Simplex.Model;

namespace Simplex.Analysis
{
    public abstract class SolverBase
    {
        protected const double m_epsilon = 0.0001;
        protected const int m_digitRound = 3;
        protected const string m_doubleFormat = "F3";

        //public abstract Solution Solve(VariableType[] types, VariableType InclusiveTypeBits, double[] objective, double[,] constarints, double[,] RightHandValues, bool MaxEntering);
        internal void PrepareSolutionResult(StandartSimplexModel simplexModel, Solution solution)
        {
            //assign the actual value to the result terms
            if (solution.Quality == SolutionQuality.Optimal || solution.Quality == SolutionQuality.Alternative)
            {
                int tmp_ColIndex = -1;
                for (int i = 0; i < simplexModel.ConstarintMatrix.GetLength(0); i++)
                {
                    tmp_ColIndex = (int)simplexModel.RightHandMatrix[i, 1];
                    if (tmp_ColIndex != -1 && simplexModel.RightHandMatrix[i, 0] != 0 && simplexModel.ConstarintMatrix[i, tmp_ColIndex] == 1)
                        solution.Results.Add(new ResultTerm() { VarType = simplexModel.ObjectiveFunction.Terms[tmp_ColIndex].VarType, Vector = simplexModel.ObjectiveFunction.Terms[tmp_ColIndex].Vector, Value = simplexModel.RightHandMatrix[i, 0] });
                }

                solution.ResultValue = simplexModel.RightHandMatrix[simplexModel.ConstarintMatrix.GetLength(0), 0];
            }
        }

        protected void PrintMatrix(double[] objective, double[,] constarints, double[,] RightHandValues, int iteration)
        {
            string tmp_sign = string.Empty;
            System.Diagnostics.Debug.WriteLine("*********************************");
            System.Diagnostics.Debug.WriteLine("   İteration " + iteration.ToString());
            for (int i = 0; i < objective.Length; i++)
            {
                tmp_sign = string.Empty;
                if (Math.Sign(objective[i]) >= 0)
                    tmp_sign = "+";
                System.Diagnostics.Debug.Write(tmp_sign + objective[i].ToString("F3") + " ");
            }
            System.Diagnostics.Debug.WriteLine(" = " + RightHandValues[RightHandValues.GetLength(0) - 1, 0].ToString());
            System.Diagnostics.Debug.WriteLine("     *******Constarints*****    ");
            for (int i = 0; i < constarints.GetLength(0); i++)
            {
                for (int j = 0; j < constarints.GetLength(1); j++)
                {
                    tmp_sign = string.Empty;
                    if (Math.Sign(constarints[i, j]) >= 0)
                        tmp_sign = "+";
                    System.Diagnostics.Debug.Write(tmp_sign + constarints[i, j].ToString("F3") + " ");
                }
                System.Diagnostics.Debug.Write(" = " + RightHandValues[i, 0].ToString("F4"));
                System.Diagnostics.Debug.WriteLine("  | " + RightHandValues[i, 1].ToString());
            }
            System.Diagnostics.Debug.WriteLine("*********************************");
        }

        protected int FindEnteringValueIndex(double[] matrix, VariableType[] types, VariableType InclusiveType, bool MaxEntering)
        {
            int tmp_index = -1;
            double tmp_value = 0;
            for (int i = 0; i < matrix.Length; i++)
            {
                if (MaxEntering && matrix[i] > tmp_value && (types[i] == (types[i] & InclusiveType)))
                {
                    tmp_value = matrix[i];
                    tmp_index = i;
                }
                else if (!MaxEntering && matrix[i] < tmp_value && (types[i] == (types[i] & InclusiveType)))
                {
                    tmp_value = matrix[i];
                    tmp_index = i;
                }
            }
            System.Diagnostics.Debug.WriteLine("Selected value :" + tmp_value.ToString(), "FindEnteringValueIndex");
            return tmp_index;
        }

        protected int FindLeavingValueIndex(double[,] matrix, int column)
        {
            int tmp_index = -1;
            double tmp_value = double.MaxValue;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (matrix[i, column] > 0 && matrix[i, column] < tmp_value)
                {
                    tmp_value = matrix[i, column];
                    tmp_index = i;
                }
            }
            return tmp_index;
        }
    }
}
