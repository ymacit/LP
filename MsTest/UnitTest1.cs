using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simplex.Problem;
using Simplex.Enums;
using Simplex.Analysis;
using Simplex.Helper;

namespace MsTest
{
    [TestClass]
    public class UnitTest1
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
        public void StandartSimplexModel_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel2();
            //simplex.PrintMatrix();
            SimplexModel standartModel = simplex.DeepCopy();
            standartModel.ConvertStandardModel();
            //standartModel.PrintMatrix();
            StandartSimplexModel phasemodel = new StandartSimplexModel(standartModel);
            phasemodel.CreatePhaseOneObjective(true);
            phasemodel.PhaseOnePrintMatrix();
            Assert.IsNull(standartModel, "success");
        }

        [TestMethod]
        public void SolveRegularTwoPhasesSimplexModel1_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel3();
            //simplex.PrintMatrix();
            SimplexModel standartModel = simplex.DeepCopy();
            //standartModel.PrintMatrix();
            StandartSimplexModel phasemodel = new StandartSimplexModel(standartModel);
            RegularSolver tmp_solver = new RegularSolver();
            Solution tmp_solution = tmp_solver.Solve(phasemodel);
            //            System.Diagnostics.Debug.WriteLine(tmp_matrix.ToString());
            if (tmp_solution.Quality == SolutionQuality.Optimal)
            {
                System.Diagnostics.Debug.WriteLine("*************************");
                System.Diagnostics.Debug.WriteLine("***      Solution");
                foreach (ResultTerm term in tmp_solution.Results)
                {
                    string tmp_sign = string.Empty;
                    System.Diagnostics.Debug.WriteLine("*** Variable | " + term.VarType.ToString() + " | " + term.Vector +  " = " + term.Value.ToString("F5") + " | ***" );
                }
                System.Diagnostics.Debug.WriteLine("*************************");
            }
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        [TestMethod]
        public void SolveRegularSimplexModel1_Test()
        {
            SimplexModel simplex = TestHelper.CreateSimplexModel2();
            //simplex.PrintMatrix();
            SimplexModel standartModel = simplex.DeepCopy();
            //standartModel.PrintMatrix();
            StandartSimplexModel phasemodel = new StandartSimplexModel(standartModel);
            RegularSolver tmp_solver = new RegularSolver();
            Solution tmp_solution = tmp_solver.Solve(phasemodel);
            //            System.Diagnostics.Debug.WriteLine(tmp_matrix.ToString());
            if (tmp_solution.Quality == SolutionQuality.Optimal)
            {
                System.Diagnostics.Debug.WriteLine("*************************");
                System.Diagnostics.Debug.WriteLine("***      Solution");
                System.Diagnostics.Debug.WriteLine("***      Optimal Value :" + phasemodel.RightHandMatrix[phasemodel.Subjects.Count,0].ToString("F5") + " | ***");
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
