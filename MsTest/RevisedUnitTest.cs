using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simplex.Model;
using Simplex.Enums;
using Simplex.Analysis;
using Simplex.Helper;

namespace MsTest
{
    [TestClass]
    public class RevisedUnitTest
    {
        private const int m_digitRound = 3;
        private const string m_doubleFormat = "F3";
        private const SolverType m_SolverType = SolverType.Revised;

        [TestMethod]
        public void MatrixDeterminant_Test()
        {
            double[,] tmp_array = new double[2, 2] { { 4, 8 }, { 7, -2 } };
            Matrix tmp_matrix = new Matrix(tmp_array);
            double det = tmp_matrix.Det();
            Matrix inv = tmp_matrix.Invert();
            Matrix tpose = Matrix.Transpose(tmp_matrix);
            Assert.IsNull(tmp_matrix, "success");
        }

        [TestMethod]
        public void MatrixUnit_Test()
        {
            double[] tmp_array = new double[3] { 0, 0, 0 };
            Matrix tmp_matrix = new Matrix(tmp_array);
            Matrix inv = tmp_matrix.Invert();
            Matrix tpose = Matrix.Transpose(tmp_matrix);
            Assert.IsNull(tmp_matrix, "success");
        }

        [TestMethod]
        public void SolveRevisedSimplexModel1_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel1();
            Solution tmp_solution = SolveProblem(simplex);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveRevisedSimplexModel3_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel3();
            Solution tmp_solution = SolveProblem(simplex);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveRevisedSimplexModel6_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel6();
            Solution tmp_solution = SolveProblem(simplex);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveRevisedSimplexModel7_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel7();
            Solution tmp_solution = SolveProblem(simplex);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveRevisedSimplexModel8_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel8();
            Solution tmp_solution = SolveProblem(simplex);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveRevisedSimplexModel9_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel9();
            Solution tmp_solution = SolveProblem(simplex);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveRevisedSimplexModel10_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel10();
            Solution tmp_solution = SolveProblem(simplex);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveRevisedSimplexModel11_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel11();
            Solution tmp_solution = SolveProblem(simplex);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveRevisedSimplexModel12_Test()
        {
            //two phase
            SimplexModel simplex = TestHelper.CreateSimplexModel12();
            Solution tmp_solution = SolveProblem(simplex);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        private Solution SolveProblem(SimplexModel model)
        {
            Solution tmp_solution = null;
            SimplexModel standartModel = model.DeepCopy();
            SolutionBuildDirector tmp_Direcor = new SolutionBuildDirector(m_SolverType, standartModel);
            tmp_Direcor.Construct();
            tmp_solution = tmp_Direcor.SolutionBuilder.getResult();
            PrintResult(tmp_solution, standartModel);
            return tmp_solution;
        }

        private void PrintResult(Solution solution, SimplexModel simplexModel)
        {
            if (solution.Quality == SolutionQuality.Optimal)
            {
                System.Diagnostics.Debug.WriteLine("*************************");
                System.Diagnostics.Debug.WriteLine("***         Solution");
                System.Diagnostics.Debug.WriteLine("***         Optimal Value :" + solution.ResultValue.ToString(m_doubleFormat) + "***".PadLeft(15));
                foreach (ResultTerm term in solution.Results)
                {
                    string tmp_sign = string.Empty;
                    System.Diagnostics.Debug.WriteLine("*** Variable | " + term.VarType.ToString().PadRight(10) + " | " + term.Vector + " = " + term.Value.ToString(m_doubleFormat).PadLeft(10) + " | ***");
                }
                System.Diagnostics.Debug.WriteLine("*************************");
            }
        }
    }
}
