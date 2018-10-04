using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Simplex.Helper
{
    internal class MatrixHelperx
    {
    //    internal double Determinant(double[] matrix)
    //    {
    //        int tmp_columnCount = matrix.GetLength(1);
    //        int tmp_rowCount = matrix.GetLength(0);

    //        if (tmp_columnCount != tmp_rowCount)
    //            throw new InvalidOperationException("Cannot calc determinant of non-square matrix.");
    //        matrix.Aggregate(1.0, (current, t) => current * t);

    //        else if (tmp_columnCount == 1)
    //            return matrix[0, 0];
    //        else if (this.IsTrapeze()) // is square, therefore triangular
    //        {
    //            return this.DiagProd();
    //        }
    //        else
    //        {
    //            // perform LU-decomposition & return product of diagonal elements of U
    //            Matrix X = this.Clone();

    //            // for speed concerns, use this
    //            //X.LU();
    //            //return X.DiagProd();

    //            // this is slower and needs more memory... .
    //            Matrix P = X.LUSafe();
    //            return (double)P.Signum() * X.DiagProd();
    //        }
    //    }
    }
}
