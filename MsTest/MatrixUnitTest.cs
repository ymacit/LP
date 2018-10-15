using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simplex.Model;
using Simplex.Enums;
using Simplex.Analysis;
using Simplex.Helper;

namespace MsTest
{
    [TestClass]
    public class MatrixUnitTest
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
//            Matrix inv = tmp_matrix.Invert();
//            Matrix tpose = Matrix.Transpose(tmp_matrix);
            Assert.IsNull(tmp_matrix, "success");
        }

        [TestMethod]
        public void MatrixInverse_Test()
        {
            double[,] tmp_array = new double[2, 2] { { 4, 1 }, { 7, 0 } };
            Matrix tmp_matrix = new Matrix(tmp_array);
            double det = tmp_matrix.Det();
            Matrix inv = tmp_matrix.Invert();            
            Assert.IsNull(tmp_matrix, "success");
        }

        [TestMethod]
        public void MatrixInverse2_Test()
        {
            double[,] tmp_array = new double[3, 3] { { 3, 1,2 }, { 1, 1,1 }, { 4, 3, 4 } };
            Matrix tmp_matrix = new Matrix(tmp_array);
            double det = tmp_matrix.Det();
            Matrix inv = tmp_matrix.Invert();
            Assert.IsNull(tmp_matrix, "success");
        }

        [TestMethod]
        public void MatrixInverse3_Test()
        {
            double[,] tmp_array = new double[3, 3] { { 1, 2, -1 }, { 0, 4, -1 }, { -1, -5, 2 } };
            double[,] tmp_arrayObj = new double[1, 3] { { 1, 1, 1 } };
            double[,] tmp_arrayRhs = new double[3, 1] { { 225 }, { 117 }, { 420 }  };

            Matrix tmp_matrix = new Matrix(tmp_array);
            Matrix tmp_matrixObj = new Matrix(tmp_arrayObj);
            Matrix tmp_matrixRhs = new Matrix(tmp_arrayRhs);
            Matrix tmp_matrixWz =  (-1)*tmp_matrixObj * tmp_matrix;
            Matrix tmp_Z = tmp_matrixWz * tmp_matrixRhs;
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

    }
}
