using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simplex.Model;
using Simplex.Enums;
using Simplex.Analysis;

namespace MsTest
{
    [TestClass]
    public class SimplexUnitTest
    {
        private const int m_digitRound = 3;
        private const string m_doubleFormat = "F3";
        private const SolverType m_SolverType = SolverType.Revised; 
      //private const SolverType m_SolverType = SolverType.Regular;


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
        public void SolveSimplexModel1_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel1();
            Solution tmp_solution = SolveProblem(simplex);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveSimplexModel2_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel2();
            Solution tmp_solution = SolveProblem(simplex);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveSimplexModel3_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel3();
            Solution tmp_solution = SolveProblem(simplex);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveSimplexModel4_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel5();
            Solution tmp_solution = SolveProblem(simplex);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }
        [TestMethod]
        public void SolveSimplexModel5_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel5();
            Solution tmp_solution = SolveProblem(simplex);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveSimplexModel6_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel6();
            Solution tmp_solution = SolveProblem(simplex);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveSimplexModel7_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel7();
            Solution tmp_solution = SolveProblem(simplex);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveSimplexModel8_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel8();
            Solution tmp_solution = SolveProblem(simplex);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveSimplexModel9_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel9();
            Solution tmp_solution = SolveProblem(simplex);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveSimplexModel10_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel10();
            Solution tmp_solution = SolveProblem(simplex);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveSimplexModel11_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel11();
            Solution tmp_solution = SolveProblem(simplex);
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveSimplexModel12_Test()
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
