using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simplex.Problem;
using Simplex.Enums;
using Simplex.Analysis;
using Simplex.Helper;

namespace MsTest
{
    [TestClass]
    public class RevisedUnitTest
    {

        [TestMethod]
        public void SolveRevisedTwoPhasesSimplexModel1_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel3();
            //simplex.PrintMatrix();
            SimplexModel standartModel = simplex.DeepCopy();
            //standartModel.PrintMatrix();
            StandartSimplexModel phasemodel = new StandartSimplexModel(standartModel);
            RevisedSimplexModel revisedModel = new RevisedSimplexModel(phasemodel);

            RevisedSolver tmp_solver = new RevisedSolver();
            Solution tmp_solution = tmp_solver.Solve(revisedModel);
            //            System.Diagnostics.Debug.WriteLine(tmp_matrix.ToString());
            if (tmp_solution.Quality == SolutionQuality.Optimal)
            {
                System.Diagnostics.Debug.WriteLine("*************************");
                System.Diagnostics.Debug.WriteLine("***      Solution");
                foreach (ResultTerm term in tmp_solution.Results)
                {
                    string tmp_sign = string.Empty;
                    System.Diagnostics.Debug.WriteLine("*** Variable | " + term.VarType.ToString() + " | " + term.Vector + " = " + term.Value.ToString("F5") + " | ***");
                }
                System.Diagnostics.Debug.WriteLine("*************************");
            }
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }
    }
}
