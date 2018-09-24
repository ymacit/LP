using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simplex.Problem;
using Simplex.Enums;
using Simplex.Analysis;
using Simplex.Helper;

namespace MsTest
{
    [TestClass]
    public class RegularUnitTest1
    {
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
        public void SimplexModel_Copy_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel3();
            SimplexModel simplex2 = simplex.DeepCopy();
            simplex2.GoalType = ObjectiveType.Maximum;
            simplex2.Subjects[0].Terms[0].Vector = "X1 Copy";
            var result = simplex.Subjects[0].Terms[0].Vector == simplex2.Subjects[0].Terms[0].Vector;
            Assert.IsFalse(result, "copy is not verified");          
        }

        [TestMethod]
        public void SimplexModel_BFS_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel1();
            TestMessage testMessage = simplex.CheckBFS();
            Assert.IsNull(testMessage.Exception, testMessage.Message);
        }

        [TestMethod]
        public void StandartSimplexModel2_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel2();
            SimplexModel standartModel = simplex.DeepCopy();
            StandartSimplexModel phasemodel = new StandartSimplexModel(standartModel);
            RegularSolver tmp_solver = new RegularSolver();
            Solution tmp_solution = tmp_solver.Solve(phasemodel);
            PrintResult(tmp_solution, phasemodel);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveRegularTwoPhasesSimplexModel3_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel3();
            SimplexModel standartModel = simplex.DeepCopy();
            StandartSimplexModel phasemodel = new StandartSimplexModel(standartModel);
            RegularSolver tmp_solver = new RegularSolver();
            Solution tmp_solution = tmp_solver.Solve(phasemodel);
            PrintResult(tmp_solution, phasemodel);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveRegularSimplexModel1_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel1();
            SimplexModel standartModel = simplex.DeepCopy();
            StandartSimplexModel phasemodel = new StandartSimplexModel(standartModel);
            RegularSolver tmp_solver = new RegularSolver();
            Solution tmp_solution = tmp_solver.Solve(phasemodel);
            PrintResult(tmp_solution, phasemodel);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveRegularSimplexModel4_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel4();
            SimplexModel standartModel = simplex.DeepCopy();
            StandartSimplexModel phasemodel = new StandartSimplexModel(standartModel);
            RegularSolver tmp_solver = new RegularSolver();
            Solution tmp_solution = tmp_solver.Solve(phasemodel);
            PrintResult(tmp_solution, phasemodel);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveRegularSimplexModel5_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel5();
            SimplexModel standartModel = simplex.DeepCopy();
            StandartSimplexModel phasemodel = new StandartSimplexModel(standartModel);
            RegularSolver tmp_solver = new RegularSolver();
            Solution tmp_solution = tmp_solver.Solve(phasemodel);
            PrintResult(tmp_solution, phasemodel);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        private void PrintResult(Solution solution, StandartSimplexModel simplexModel)
        {
            if (solution.Quality == SolutionQuality.Optimal)
            {
                System.Diagnostics.Debug.WriteLine("*************************");
                System.Diagnostics.Debug.WriteLine("***      Solution");
                System.Diagnostics.Debug.WriteLine("***      Optimal Value :" + simplexModel.RightHandMatrix[simplexModel.Subjects.Count, 0].ToString("F5") + " | ***");
                foreach (ResultTerm term in solution.Results)
                {
                    string tmp_sign = string.Empty;
                    System.Diagnostics.Debug.WriteLine("*** Variable | " + term.VarType.ToString() + " | " + term.Vector + " = " + term.Value.ToString("F5") + " | ***");
                }
                System.Diagnostics.Debug.WriteLine("*************************");
            }
        }
    }
}
