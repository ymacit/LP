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
using Simplex.Problem;

namespace Simplex.Analysis
{
    public class RevisedSolver : SolverBase
    {

        public  override Solution Solve(SimplexModel simplexModel)
        {
            Solution tmp_solution = new Solution() { Quality = Enums.SolutionQuality.Infeasible };
            StandartSimplexModel phasemodel = new StandartSimplexModel(simplexModel);
            RevisedSimplexModel revisedModel = new RevisedSimplexModel(phasemodel);

            if (phasemodel.IsTwoPhase)
                tmp_solution = SolveTwoPhase(revisedModel);
            else
                tmp_solution = Solve(revisedModel.VarTypes, VariableType.Original | VariableType.Slack, revisedModel.NonBasisMatrix, revisedModel.BasisObjectiveMatrix, revisedModel.NonBasisMatrix, revisedModel.NonBasisMatrix, revisedModel.BasisRightHandMatrix, false);

            //initial table mut be contain nXn unit matrix that consist of basic varibales ( slack + artificial not original and excess) for feaseble solution
            //for feaseble solution, all of rhs values must be positive or zero and Z must be zero after all iteration 
            return tmp_solution;
        }

        private Solution SolveTwoPhase(RevisedSimplexModel simplexModel)
        {
            Solution tmp_solution = new Solution() { Quality = Enums.SolutionQuality.Infeasible };
            simplexModel.ConvertStandardModel();
            simplexModel.PrintMatrix();
            simplexModel.CreatePhaseOneObjective(false);
            simplexModel.GenerateBasisMatrices();
            //1) Solve the matrix for phase I 
            /*
             * Steps
             * 1. Modify the constraints so that the RHS of each constraint is nonnegative (This requires that each constraint with a negative RHS be multiplied by -1. Remember that if you multiply an inequality by any negative number, the direction of the inequality is reversed!). After modification, identify each constraint as a ≤, ≥ or = constraint.
             * 2. Convert each inequality constraint to standard form (If constraint i is a ≤ constraint, we add a slack variable si; and if constraint i is a ≥ constraint, we subtract an excess variable ei).
             * 3. Add an artificial variable ai to the constraints identified as ≥ or = constraints at the end of Step 1. Also add the sign restriction ai ≥ 0. 
             * 4. In the phase I, ignore the original LP’s objective function, instead solve an LP whose objective function is minimizing w = ai (sum of all the artificial variables). The act of solving the Phase I LP will force the artificial variables to be zero. 5. Since each artificial variable will be in the starting basis, all artificial variables must be eliminated from row 0 before beginning the simplex. Now solve the transformed problem by the simplex.              
             */
            VariableType tmp_inclusive = VariableType.Original | VariableType.Slack | VariableType.Excess;

            tmp_solution = Solve(simplexModel.VarTypes, tmp_inclusive, simplexModel.BasisObjectiveMatrix, simplexModel.PhaseOneBasisObjectiveMatrix, simplexModel.PhaseOneBasisMatrix, simplexModel.PhaseOneNonBasisMatrix, simplexModel.PhaseOneBasisRightHandMatrix,  false);
            //Solving the Phase I LP will result in one of the following three cases:
            //I.Case : If w = 0 
            //TODO test //tmp_solution.RightHandValues[tmp_solution.RightHandValues.GetLength(0) - 1, 0] = 0;
            if (tmp_solution.RightHandValues[tmp_solution.RightHandValues.GetLength(0)-1, 0] == 0)
            {
                //transfer the phaseoneobjective function factors
                simplexModel.TruncatePhaseResult(tmp_solution);
                //II.Case : If w = 0, and no artificial variables are in the optimal Phase I basis:
                //  i.Drop all columns in the optimal Phase I tableau that correspond to the artificial variables.Drop Phase I row 0.
                //  ii.Combine the original objective function with the constraints from the optimal Phase I tableau(Phase II LP).If original objective function coefficients of BVs are nonzero row operations are done.
                //  iii.Solve Phase II LP using the simplex method.The optimal solution to the Phase II LP is the optimal solution to the original LP.
                tmp_solution = Solve(simplexModel.VarTypes, tmp_inclusive, simplexModel.BasisObjectiveMatrix, simplexModel.PhaseOneBasisObjectiveMatrix, simplexModel.PhaseOneNonBasisMatrix, simplexModel.PhaseOneBasisRightHandMatrix, simplexModel.PhaseOneBasisMatrix, false);
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
            PrepareSolutionResult(simplexModel, tmp_solution);
            return tmp_solution;
        }

        private Solution Solve(VariableType[] types, VariableType InclusiveTypeBits, Matrix nonBasisObjective, Matrix basisObjective, Matrix basis, Matrix nonBasis, Matrix rightHandValues, bool artificial)
        {
            Solution tmp_solution = new Solution() { Quality = Enums.SolutionQuality.Infeasible };
            //PrintMatrix(basisObjective, nonBasis, rightHandValues, basis, 0);

            //int tmp_ObjectiveLeftValueIndex = RightHandValues.GetLength(0) - 1;
            //bool tmp_continue = true;
            //int tmp_iteration = 1;
            //double tmp_pivotValue = 0;
            //int tmp_PivotColIndex = -1;
            //int tmp_PivotRowIndex = -1;
            ////Round the matrix values
            //for (int i = 0; i < constarints.GetLength(0); i++)
            //{
            //    objective[i] = Math.Round(objective[i], 5);
            //    for (int j = 0; j < constarints.GetLength(1); j++)
            //    {
            //        constarints[i, j] = Math.Round(constarints[i , j], 5);
            //    }
            //}
            //for (int i = 0; i < RightHandValues.GetLength(0); i++)
            //{
            //    RightHandValues[i,0] = Math.Round(RightHandValues[i,0], 5);
            //}

            //while (tmp_continue)
            //{
            //    //1) Select entering value for original variables in objective function. if maximize, select min value (in negative), if minimize select max value (in positive) for original variables. İf selection is not exist, decide for solution state.
            //    // Maksimizasyonda en negatif değere sahip değişken,
            //    // Minimizasyonda ise en pozitif değere sahip değişken seçilir.
            //    tmp_PivotColIndex = FindEnteringValueIndex(objective, types, InclusiveTypeBits, artificial);
            //    //Check the selected value. If value is zero, nothing to do.
            //    if (tmp_PivotColIndex == -1)
            //    {
            //        tmp_solution.Quality = Enums.SolutionQuality.Optimal;
            //        System.Diagnostics.Debug.WriteLine("Optimal solution is found for problem ", "SolveStandart");
            //        break;                    
            //    }

            //    System.Diagnostics.Debug.WriteLine("Entering Objective Index = " + tmp_PivotColIndex, "SolveStandart");
            //    //2)Calculate the ratio for selected varible all of constarint and set teh RightHandValues[,1]
            //    for (int i = 0; i < constarints.GetLength(0); i++)
            //    {
            //        if(constarints[i, tmp_PivotColIndex]!=0)
            //            RightHandValues[i, 1] = (RightHandValues[i,0] / constarints[i,tmp_PivotColIndex]);
            //    }
            //    //3)Select the  minimum ratio for leaving variable
            //    //Anahtar sütunu bulduktan sonra(X2 Sütunu), anahtar sütunda bulunan her bir hücre içindeki değeri Çözüm sütunundaki değerlere oranlarız ve en küçük negatif olmayan değeri belirleriz. Bu işlem hem maksimizasyon hem de minimizasyon modellerinde aynı şekilde yapılır.
            //    tmp_PivotRowIndex = FindLeavingValueIndex(RightHandValues, 1, false);
            //    if(tmp_PivotRowIndex==-1)
            //    {
            //        tmp_solution.Quality = Enums.SolutionQuality.Unbounded;
            //        System.Diagnostics.Debug.WriteLine("Problem is Unbounded.", "SolveStandart");
            //        break;
            //    }
            //    System.Diagnostics.Debug.WriteLine("Selected Pivot Row = " + tmp_PivotRowIndex, "SolveStandart");

            //    //4)Calculate new Row (Rn') for selected tmp_PivotRowIndex
            //    System.Diagnostics.Debug.WriteLine("**********New Row*********");
            //    tmp_pivotValue = constarints[tmp_PivotRowIndex, tmp_PivotColIndex];
            //    for (int i = 0; i < constarints.GetLength(1); i++)
            //    {
            //        constarints[tmp_PivotRowIndex,i] = Math.Round((constarints[tmp_PivotRowIndex, i] / tmp_pivotValue), 5);
            //        System.Diagnostics.Debug.Write(constarints[tmp_PivotRowIndex,i].ToString("F5") + "  ");
            //    }
            //    //in addition set the left value 
            //    RightHandValues[tmp_PivotRowIndex, 0] = Math.Round(RightHandValues[tmp_PivotRowIndex, 0] / tmp_pivotValue, 5);
            //    RightHandValues[tmp_PivotRowIndex, 1] = 0;
            //    System.Diagnostics.Debug.WriteLine(" = " + RightHandValues[tmp_PivotRowIndex, 0].ToString("F5"));

            //    //5)Calculate new objective Row (Rn') by multiple contraint factor.RO'=RO-xRn'
            //    tmp_pivotValue = objective[tmp_PivotColIndex];
            //    for (int i = 0; i < objective.Length; i++)
            //    {
            //        objective[i] = Math.Round( objective[i] - tmp_pivotValue * constarints[tmp_PivotRowIndex, i],5);
            //    }
            //    //in addition set the left value 
            //    RightHandValues[tmp_ObjectiveLeftValueIndex, 0] = Math.Round(RightHandValues[tmp_ObjectiveLeftValueIndex, 0] - tmp_pivotValue * RightHandValues[tmp_PivotRowIndex,0],5) ;
            //    RightHandValues[tmp_ObjectiveLeftValueIndex, 1] = 0;

            //    //6)Calculate new row for other cosntraint rows (Rj'=Rj-xRn')
            //    for (int i = 0; i < constarints.GetLength(0); i++)
            //    {
            //        if (i == tmp_PivotRowIndex)
            //            continue;

            //        tmp_pivotValue = constarints[i, tmp_PivotColIndex];
            //        for (int j = 0; j < constarints.GetLength(1) ; j++)
            //        {
            //            constarints[i, j] = Math.Round(constarints[i, j]  - tmp_pivotValue * constarints[tmp_PivotRowIndex, j], 5);
            //        }
            //        //in addition set the left value 
            //        RightHandValues[i, 0] = Math.Round(RightHandValues[i, 0] - tmp_pivotValue * RightHandValues[tmp_PivotRowIndex, 0], 5);
            //        RightHandValues[i, 1] = 0;
            //    }

            //   // PrintMatrix(objective, constarints, RightHandValues, tmp_iteration);
            //    tmp_iteration++;
            //    //break;
            //}
            //tmp_solution.ObjectiveMatrix = objective;
            //tmp_solution.RightHandValues = RightHandValues;
            return tmp_solution;

        }
    }
}
