//***************************
//Sınıf Adı : Solver 
//Dosya Adı : Solver.cs 
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
using Simplex.Helper;
using Simplex.Model;

namespace Simplex.Analysis
{
    public class RevisedSolutionBuilder : SolverBase, ISolutionBuilder
    {

        private SimplexModel m_BaseModel = null;
        private StandartSimplexModel m_StandartModel = null;
        private RevisedSimplexModel m_RevisedModel = null;
        private IColumnSelector m_ColumnSelector = null;
        private Matrix WorkingRightHandValues = null;

        private Solution SolveTwoPhase()
        {
            Solution tmp_solution = new Solution() { Quality = Enums.SolutionQuality.Infeasible };
            m_RevisedModel.PrintMatrix();

            //1) Solve the matrix for phase I 
            /*
             * Steps
             * 1. Modify the constraints so that the RHS of each constraint is nonnegative (This requires that each constraint with a negative RHS be multiplied by -1. Remember that if you multiply an inequality by any negative number, the direction of the inequality is reversed!). After modification, identify each constraint as a ≤, ≥ or = constraint.
             * 2. Convert each inequality constraint to standard form (If constraint i is a ≤ constraint, we add a slack variable si; and if constraint i is a ≥ constraint, we subtract an excess variable ei).
             * 3. Add an artificial variable ai to the constraints identified as ≥ or = constraints at the end of Step 1. Also add the sign restriction ai ≥ 0. 
             * 4. In the phase I, ignore the original LP’s objective function, instead solve an LP whose objective function is minimizing w = ai (sum of all the artificial variables). The act of solving the Phase I LP will force the artificial variables to be zero. 5. Since each artificial variable will be in the starting basis, all artificial variables must be eliminated from row 0 before beginning the simplex. Now solve the transformed problem by the simplex.              
             */
            m_ColumnSelector = ColumnSelectorFactory.GetSelector(ObjectiveType.Minumum);
            tmp_solution = Solve(m_RevisedModel.PhaseNonBasisObjectiveMatrix, m_RevisedModel.PhaseBasisObjectiveMatrix, m_RevisedModel.BasisMatrix, m_RevisedModel.BasisInverseMatrix, m_RevisedModel.NonBasisMatrix, m_RevisedModel.BasisRightHandMatrix, m_RevisedModel.BasicVariables, m_RevisedModel.ObjectiveCost);
            //Solving the Phase I LP will result in one of the following three cases:
            //I.Case : If w = 0 
            //TODO test //tmp_solution.RightHandValues[tmp_solution.RightHandValues.GetLength(0) - 1, 0] = 0;

            if (tmp_solution.ResultValue + m_RevisedModel.ObjectiveCost== 0)
            {
                m_ColumnSelector = ColumnSelectorFactory.GetSelector(m_RevisedModel.GoalType);
                //transfer the phaseoneobjective function factors
                RevisedSimplexModel tmp_phaseModel = m_RevisedModel;
                tmp_phaseModel.TruncatePhaseResult(tmp_solution);
                //II.Case : If w = 0, and no artificial variables are in the optimal Phase I basis:
                //  i.Drop all columns in the optimal Phase I tableau that correspond to the artificial variables.Drop Phase I row 0.
                //  ii.Combine the original objective function with the constraints from the optimal Phase I tableau(Phase II LP).If original objective function coefficients of BVs are nonzero row operations are done.
                //  iii.Solve Phase II LP using the simplex method.The optimal solution to the Phase II LP is the optimal solution to the original LP.
                    //tmp_phaseModel.BasisObjectiveMatrix = new Matrix(1, tmp_solution.BasicVariables.Count);
                    //tmp_phaseModel.BasisRightHandMatrix = new Matrix(tmp_solution.BasicVariables.Count,1);
                    //for (int i = 0; i < tmp_solution.BasicVariables.Count; i++)
                    //{
                    //    tmp_phaseModel.BasisObjectiveMatrix[0, i] =tmp_phaseModel.ObjectiveFunction.Terms[tmp_solution.BasicVariables[i]].Factor;
                    //    tmp_phaseModel.BasisRightHandMatrix[i,0] = tmp_phaseModel.Subjects[i].RightHandValue;
                    //}

                tmp_solution = Solve(tmp_phaseModel.BasisNonObjectiveMatrix, tmp_phaseModel.BasisObjectiveMatrix, tmp_phaseModel.BasisMatrix, tmp_phaseModel.BasisInverseMatrix, tmp_phaseModel.NonBasisMatrix, tmp_phaseModel.BasisRightHandMatrix, tmp_phaseModel.BasicVariables, tmp_phaseModel.ObjectiveCost);
                //tmp_solution = Solve(tmp_phaseModel.PhaseNonOneBasisObjectiveMatrix, tmp_phaseModel.BasisObjectiveMatrix, tmp_phaseModel.PhaseOneBasisMatrix, tmp_phaseModel.BasisMatrix, tmp_phaseModel.PhaseOneNonBasisMatrix, tmp_phaseModel.BasisRightHandMatrix, tmp_solution.BasicVariables, tmp_phaseModel.ObjectiveCost);
                System.Diagnostics.Debug.WriteLine("Solution " + tmp_solution.Quality.ToString());
                //if ( )
                //III.Case : If w = 0, and at least one artificial variable is in the optimal Phase I basis:
                //  i.Drop all columns in the optimal Phase I tableau that correspond to the nonbasic artificial variables and any variable from the original problem that has a negative coefficient in row 0 of the optimal Phase I tableau. Drop Phase I row 0.
                //  ii.Combine the original objective function with the constraints from the optimal Phase I tableau(Phase II LP).If original objective function coefficients of BVs are nonzero row operations are done.
                //  iii.Solve Phase II LP using the simplex method.The optimal solution to the Phase II LP is the optimal solution to the original LP.
                //if ( )
            }
            //II.Case  : If w > 0 then the original LP has no feasible solution(stop here).
            else
            {
                tmp_solution.Quality = SolutionQuality.Infeasible;
            }
            //assign the actual value to the result terms
            return tmp_solution;
        }

        private Solution Solve( Matrix nonBasisObjective, Matrix basisObjective, Matrix basis, Matrix inverseBasis, Matrix nonBasis, Matrix RightHandValues, List<int> basicVariables,  double objectiveCost)
        {
            Solution tmp_solution = new Solution() { Quality = Enums.SolutionQuality.Infeasible };
            //PrintBasisMatrix(basisObjective, nonBasis, RightHandValues, basis, 0);


            bool tmp_continue = true;
            int tmp_iteration = 1;
            double tmp_pivotValue = 0;
            int tmp_PivotColIndex = -1;
            int tmp_PivotRowIndex = -1;

            Matrix tmp_EnteringColumnMatrix = null;
            Matrix tmp_ColumnVectorMatrix = null;
            Matrix tmp_ShadowPriceMatrix = null;
            Matrix tmp_ObjectiveCostMatrix = null;
            Matrix tmp_RowVectorMatrix = null;
            Matrix tmp_nonBasis = nonBasis.Duplicate();

            List<int> tmp_BasicRows = new List<int>();

            double tmp_MinLeavingValue = double.MaxValue;
            double tmp_MinCalculateValue = 0;
            WorkingRightHandValues = inverseBasis * RightHandValues; //ac


            #region Step-0
            //Note the columns from which the current B-1 will be read. Initially, B-1 = I.
            //initial shadow price
            tmp_ShadowPriceMatrix = basisObjective * inverseBasis;
            tmp_ObjectiveCostMatrix = tmp_ShadowPriceMatrix  * RightHandValues;
            tmp_solution.ResultValue = tmp_ObjectiveCostMatrix[0, 0];
            PrintMatrix(basisObjective, inverseBasis, WorkingRightHandValues, basicVariables, tmp_solution.ResultValue, 0);

            //if given objective cost and calculated objective cost are equal, solution is optimal 
            if (tmp_solution.ResultValue == objectiveCost)
                tmp_continue = false;

            #endregion


            while (tmp_continue)
            {

                #region Step-1
                //For the current tableau, compute 𝐰= cBVB-1. (w is called as simplex multipliers or shadow prices (dual prices))
                //BV = {s1, s2 , s3 }, B-1 = I, 𝐜B=[0,0,0], 𝐰=𝐜B𝐁−1; 𝐰=[0, 0, 0]𝐈=[0, 0, 0] 
                //# shadow price is in end of the step 4, for the cycling opetation tmp_ShadowPriceMatrix = tmp_basisObjective * inverseBasis;
                #endregion

                #region Step-2
                /*
                 *Price out all nonbasic variables (𝑧𝑗−𝑐𝑗=cBVB−1a𝑗−𝑐𝑗=wa𝑗−𝑐𝑗) in the current tableau.
                 *- If each nonbasic variable prices out to be nonnegative, then the current basis is optimal.
                 *- If the current basis is not optimal, then enter into the basis the nonbasic variable with the most negative coefficient in row 0.Call this variable xk.
                 * 
                 * 1) Select entering value for original variables in objective function. if maximize, select min value (in negative), if minimize select max value (in positive) for original variables. İf selection is not exist, decide for solution state.
                 * Maksimizasyonda en negatif değere sahip değişken,
                 * Minimizasyonda ise en pozitif değere sahip değişken seçilir.
                */

                for (int i = 0; i < tmp_ShadowPriceMatrix.ColumnCount; i++)
                {
                    tmp_ShadowPriceMatrix[0,i]= Math.Round(tmp_ShadowPriceMatrix[0, i], m_digitRound);
                }

                tmp_RowVectorMatrix = nonBasisObjective - tmp_ShadowPriceMatrix * nonBasis;
                tmp_PivotColIndex = m_ColumnSelector.GetSelectedIndex(tmp_RowVectorMatrix, 0, basicVariables);

                //Check the selected value. If value is zero, nothing to do.
                System.Diagnostics.Debug.WriteLine("Entering Objective Index = " + tmp_PivotColIndex, "SolveStandart");
                if (tmp_PivotColIndex == -1)
                {
                    tmp_solution.Quality = Enums.SolutionQuality.Optimal;
                    System.Diagnostics.Debug.WriteLine("Optimal solution is found for this problem ", "SolveStandart");
                    break;
                }
                #endregion

                #region Step-3
                /*
                 *To determine the row in which xk enters the basis,
                 *- compute xk’s column in the current tableau (𝐲𝑗=𝐁−1𝐚𝑗 )
                 *- compute the right-hand side of the current tableau (𝐛̅=𝐁−1𝐛)
                 *- Use the ratio test to determine the row in which xk should enter the basis.
                 *- We now know the set of basic variables (BV) for the new tableau.
                */
                tmp_EnteringColumnMatrix = nonBasis.GetCol(tmp_PivotColIndex);
                tmp_ColumnVectorMatrix = inverseBasis * tmp_EnteringColumnMatrix;
                //Get olumn vector for selected pivotcolumn
                tmp_PivotRowIndex = -1;
                tmp_MinLeavingValue = double.MaxValue;

                //Select the  minimum ratio for leaving variable. Ratio =   tmp_WorkingRightHandValue /  tmp_ColumnVectorMatrix                
                System.Diagnostics.Debug.Write("aj column: ");
                for (int i = 0; i < tmp_ColumnVectorMatrix.RowCount; i++)
                {
                    if (tmp_BasicRows.Contains(i))
                        continue;

                    if (tmp_ColumnVectorMatrix[i, 0] != 0)
                    {
                        tmp_MinCalculateValue = Math.Round(WorkingRightHandValues[i, 0] / tmp_ColumnVectorMatrix[i, 0], m_digitRound);
                        System.Diagnostics.Debug.Write(tmp_MinCalculateValue.ToString(i.ToString() + " : "  + tmp_MinCalculateValue.ToString()) + "\t");
                        if (tmp_MinCalculateValue >= 0 && tmp_MinCalculateValue < tmp_MinLeavingValue)
                        {
                            tmp_MinLeavingValue = tmp_MinCalculateValue;
                            tmp_PivotRowIndex = i;
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine("");
                if (tmp_PivotRowIndex == -1)
                {
                    /*
                     *  Unbounded solution
                     *  Theorem 5
                     *  Consider a linear model in maximization standard form. Let B be a basis formed by columns of A, and let xB = B−1b be its corresponding basic feasible solution, and z = cT BxB its objective value.
                     *  If there exists a nonbasic vector ak in matrix A such that zk − ck < 0, and for such vector ak all coordinates yik are less than or equal to zero, i = 1, . . . ,m, then
                     *  the solution to the model is UNBOUNDED.
                    */

                    /*                     
                     * Alernative solutions
                     * Theorem 6
                     * Consider a linear model in maximization standard form. Let B be a basis formed by columns of A, and let xB = B−1b be its corresponding basic feasible solution, and z = cTBxB its objective value.
                     * If zj−cj ≥ 0 holds for every vector aj of matrix A, then the solution xB is optimal. Moreover, if there exists a nonbasic vector ak such that zk − ck = 0, and at least one coordinate yik > 0, i = 1, . . . ,m, then there exist multiple optimal solutions. 
                    */

                    tmp_solution.Quality = Enums.SolutionQuality.Unbounded;
                    System.Diagnostics.Debug.WriteLine("Problem is Unbounded.", "SolveStandart");
                    break;
                }

                //set the variable tracking values
                basicVariables[tmp_PivotRowIndex] = tmp_PivotColIndex;
                tmp_BasicRows.Add(tmp_PivotRowIndex);
                basicVariables[tmp_PivotRowIndex] = tmp_PivotColIndex;
                //RightHandValues[tmp_PivotRowIndex, 1] = tmp_PivotColIndex;
                //tmp_basicVariableTracking[tmp_PivotRowIndex, 0] = tmp_PivotColIndex;
                System.Diagnostics.Debug.WriteLine("Pivot Row = " + tmp_PivotRowIndex, "SolveStandart");
                #endregion

                #region Step-4
                /*
                 *Use the column for xk in the current tableau to determine the EROs needed to enter xk into the basis. 
                 *Perform these EROs on the current B-1. This will yield the new B-1. Return to step 1.
                 * Formulas used in Revised Simplex method
                 * 𝐲𝒋=𝐁−𝟏𝐚𝒋 : Column for xj in BV tableau
                 * 𝐰=𝐜𝐁𝐁−𝟏 : Simplex multipliers – shadow prices (dual price)
                 * 𝑧𝑗−𝑐𝑗 = 𝐜𝐁𝐁−𝟏𝐚𝒋−𝒄𝒋 = 𝐰𝐚𝒋−𝒄𝒋 : Coefficient of xj in row 0
                 * 𝐛̅=𝐁−𝟏𝐛 :Right-hand side of constraints in BV tableau – values of basic variables
                 * 𝑍=𝐜𝐁𝐁−𝟏𝐛=𝐜𝐁𝐛̅=𝐰𝐛 : Right-hand side of BV row 0 – objective function value
                */
                    //4)Calculate new Row (Rn') for selected tmp_PivotRowIndex
                    System.Diagnostics.Debug.WriteLine("**********New Row*********");
                tmp_pivotValue = tmp_ColumnVectorMatrix[tmp_PivotRowIndex, 0];
                //tmp_pivotValue = nonBasis[tmp_PivotRowIndex, tmp_PivotColIndex];
                for (int i = 0; i < inverseBasis.ColumnCount; i++)
                {
                    inverseBasis[tmp_PivotRowIndex, i] = Math.Round((inverseBasis[tmp_PivotRowIndex, i] / tmp_pivotValue), m_digitRound);
                    //System.Diagnostics.Debug.Write(inverseBasis[tmp_PivotRowIndex, i].ToString(m_doubleFormat) + "  ");
                }
                //5)Calculate new row for other cosntraint rows (Rj'=Rj-xRn')
                for (int i = 0; i < inverseBasis.RowCount; i++)
                {
                    if (i == tmp_PivotRowIndex)
                        continue;

                    //tmp_pivotValue = nonBasis[i, tmp_PivotColIndex];
                    tmp_pivotValue = tmp_ColumnVectorMatrix[i, 0];
                    for (int j = 0; j < inverseBasis.ColumnCount; j++)
                    {
                        inverseBasis[i, j] = Math.Round(inverseBasis[i, j] - tmp_pivotValue * inverseBasis[tmp_PivotRowIndex, j], m_digitRound);
                    }
                    //in addition set the left value 
                    //tmp_WorkingRightHandValues[i, 0] = Math.Round(tmp_WorkingRightHandValues[i, 0] - tmp_pivotValue * tmp_WorkingRightHandValues[tmp_PivotRowIndex, 0], m_digitRound);
                }

                //başla
                //5)Calculate new row for other cosntraint rows (Rj'=Rj-xRn')
                for (int i = 0; i < tmp_nonBasis.RowCount; i++)
                {

                    //tmp_pivotValue = nonBasis[i, tmp_PivotColIndex];
                    tmp_pivotValue = tmp_ColumnVectorMatrix[i, 0];
                    for (int j = 0; j < tmp_nonBasis.ColumnCount; j++)
                    {
                        tmp_nonBasis[i, j] = Math.Round(tmp_nonBasis[i, j] - tmp_pivotValue * tmp_nonBasis[tmp_PivotRowIndex, j], m_digitRound);
                    }
                    //in addition set the left value 
                    //tmp_WorkingRightHandValues[i, 0] = Math.Round(tmp_WorkingRightHandValues[i, 0] - tmp_pivotValue * tmp_WorkingRightHandValues[tmp_PivotRowIndex, 0], m_digitRound);
                }
                //bitir


                //6)Calculate new objective Row (Rn') by multiple contraint factor.RO'=RO-xRn'
                //basisObjective[0, tmp_PivotRowIndex] = (-1) * nonBasisObjective[0, tmp_PivotColIndex];
                basisObjective[0, tmp_PivotRowIndex] = nonBasisObjective[0, tmp_PivotColIndex];
                
                //in addition set the left value 
                WorkingRightHandValues = inverseBasis * RightHandValues; //ac
                #endregion

                tmp_ShadowPriceMatrix = basisObjective * inverseBasis;
                tmp_ObjectiveCostMatrix = tmp_ShadowPriceMatrix * RightHandValues;
                tmp_solution.ResultValue = tmp_ObjectiveCostMatrix[0, 0];
                PrintMatrix(tmp_ShadowPriceMatrix, inverseBasis, WorkingRightHandValues, basicVariables, tmp_solution.ResultValue, tmp_iteration); 
                tmp_iteration++;
                //break;
            }
            //Z=w*b  
            //m_RevisedModel.PhaseOneBasisObjectiveMatrix = tmp_ShadowPriceMatrix;
            tmp_solution.BasicVariables = basicVariables;
            return tmp_solution;
        }

        void ISolutionBuilder.setStandartModel(SimplexModel model)
        {
            m_BaseModel = model;
            m_StandartModel = new StandartSimplexModel(model);
            m_RevisedModel = new RevisedSimplexModel(m_StandartModel);
            m_RevisedModel.ConvertStandardModel();
        }

        void ISolutionBuilder.setPhase()
        {
            if (m_RevisedModel.IsTwoPhase)
                m_RevisedModel.CreatePhaseOneObjective();
        }

        void ISolutionBuilder.setMatrices()
        {
            //m_RevisedModel.CreateMatrixSet();
            m_RevisedModel.GenerateBasisMatrices();
        }

        Solution ISolutionBuilder.getResult()
        {
            Solution tmp_solution = new Solution() { Quality = Enums.SolutionQuality.Infeasible };

            if (m_RevisedModel.IsTwoPhase)
            {
                tmp_solution = SolveTwoPhase();
            }
            else
            {
                m_ColumnSelector = ColumnSelectorFactory.GetSelector(m_RevisedModel.GoalType);
                tmp_solution = Solve(m_RevisedModel.BasisNonObjectiveMatrix, m_RevisedModel.BasisObjectiveMatrix, m_RevisedModel.BasisMatrix, m_RevisedModel.BasisMatrix.Invert(), m_RevisedModel.NonBasisMatrix, m_RevisedModel.BasisRightHandMatrix, m_RevisedModel.BasicVariables, m_RevisedModel.ObjectiveCost);
            }
            //for feaseble solution, all of rhs values must be positive or zero and Z must be zero after all iteration 
            PrepareSolutionResult(m_RevisedModel.NonBasisMatrix, WorkingRightHandValues, m_RevisedModel.ObjectiveFunction.Terms, tmp_solution);

            return tmp_solution;
        }

        internal static void PrintCalculatedMatrix(Matrix matrix, string message)
        {
            string tmp_sign = string.Empty;
            System.Diagnostics.Debug.WriteLine("*********************************");
            System.Diagnostics.Debug.WriteLine("        " + message);

            for (int i = 0; i < matrix.RowCount; i++)
            {
                System.Diagnostics.Debug.Write("R" + i.ToString() + "   :");
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    System.Diagnostics.Debug.Write(matrix[i,j].ToString("F3") + "\t");
                }
                System.Diagnostics.Debug.WriteLine("");
            }
        }
        internal static void PrintBasisMatrix(RevisedSimplexModel model)
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
    }
}
