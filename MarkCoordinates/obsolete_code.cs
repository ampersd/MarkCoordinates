using System;

namespace MarkCoordinates
{
    using Emgu.CV;
    using Emgu.CV.CvEnum;

    class ObsoleteCode
    {
        ////Creating a test matrix
        ////Matrix<double> A = new Matrix<double>(2, 2);
        ////Matrix<double> B = new Matrix<double>(1, 2);
        ////A[0, 0] = 5;
        ////A[0, 1] = 2;
        ////A[1, 0] = 2;
        ////A[1, 1] = 1;
        ////B[0, 0] = 7;
        ////B[0, 1] = 9;

        ////Matrix<Double> X = this.SolveEq(A, B);
        ////var Y = new Matrix<Double>(1,2);


        //переписать потом через использование матричных методов
/*
        /// <summary>
        /// A*X = B
        /// X = A^(-1) * B
        /// </summary>
        /// <param name="A">matrix of coefficients</param>
        /// <param name="B">vector of free members</param>
        /// <returns>Matrix with two cols - x1 and x2</returns>
        private static Matrix<Single> SolveEq(Matrix<Single> A, Matrix<Single> B)
        {
            var X = new Matrix<Single>(1, 2);
            var InvA = new Matrix<Single>(2, 2);
            CvInvoke.cvInvert(A, InvA, SOLVE_METHOD.CV_LU);
            X[0, 0] = InvA[0, 0] * B[0, 0] + InvA[0, 1] * B[0, 1];
            X[0, 1] = InvA[1, 0] * B[0, 0] + InvA[1, 1] * B[0, 1];
            return X;
        }
*/

    }
}
