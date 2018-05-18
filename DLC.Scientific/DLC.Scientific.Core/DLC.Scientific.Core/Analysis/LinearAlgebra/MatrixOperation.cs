using MathNet.Numerics.LinearAlgebra;
using System;

namespace DLC.Scientific.Core.Analysis.LinearAlgebra
{
	public static class MatrixOperation
	{
		public static Matrix<double> RotationZ_3D(Vector<double> planeParam)
		{
			if (planeParam == null) throw new ArgumentNullException("planeParam");

			double x1 = planeParam[1];
			double y1 = planeParam[2];

			// form first Givens rotation
			Matrix<double> R = GivenRotation(ref x1, ref y1);
			double z = x1 * planeParam[1] + y1 * planeParam[2];

			var V = Matrix<double>.Build.DenseOfRowArrays(new double[][] {
				new double[] { 1, 0, 0 },
				new double[] { 0, y1, -x1 },
				new double[] { 0, x1, y1 }
			});

			double x2 = planeParam[0];
			double y2 = z;
			// form second Givens rotation
			R = GivenRotation(ref x2, ref y2);

			// check positivity
			if (x2 * planeParam[0] + y2 * z < 0)
			{
				x2 = -x2;
				y2 = -y2;
			}

			var W = Matrix<double>.Build.DenseOfRowArrays(new double[][] {
				new double[] { y2, 0, -x2 },
				new double[] { 0, 1, 0 },
				new double[] { x2, 0, y2 }
			});

			return W * V;
		}

		public static Matrix<double> GivenRotation(ref double x, ref double y)
		{
			double z = 0;

			if (y == 0)
			{
				x = 1;
				y = 0;
			}
			else if (Math.Abs(y) >= Math.Abs(x))
			{
				z = x / y;
				y = 1 / Math.Sqrt(1 + Math.Pow(z, 2));
				x = z * y;
			}
			else
			{
				z = y / x;
				x = 1 / Math.Sqrt(1 + Math.Pow(z, 2));
				y = z * x;
			}

			return Matrix<double>.Build.DenseOfRowArrays(new double[][] {
				new double[] { x, y },
				new double[] { -y, x }
			});
		}

		public static Vector<double> CrossProduct(Vector<double> vectorA, Vector<double> vectorB)
		{
			if (vectorA == null) throw new ArgumentNullException("vectorA");
			if (vectorB == null) throw new ArgumentNullException("vectorB");

			double vectorAZ = (vectorA.Count == 3) ? vectorA[2] : 0;
			double vectorBZ = (vectorB.Count == 3) ? vectorB[2] : 0;

			return Vector<double>.Build.Dense(new[] {
				vectorA[1] * vectorBZ - vectorAZ * vectorB[1],
				vectorAZ * vectorB[0] - vectorA[0] * vectorBZ,
				vectorA[0] * vectorB[1] - vectorA[1] * vectorB[0]
			});
		}

		public static Vector<double> Diagonal(Matrix<double> A)
		{
			if (A == null) throw new ArgumentNullException("A");

			int size = Math.Min(A.RowCount, A.ColumnCount);
			var diagonal = Vector<double>.Build.Dense(size);

			for (int i = 0; i < size; i++)
				diagonal[i] = A[i, i];

			return diagonal;
		}

		public static Matrix<double> AddToSubMatrix(Matrix<double> A, Matrix<double> B, int rowIndex, int columnIndex)
		{
			if (A == null) throw new ArgumentNullException("A");
			if (B == null) throw new ArgumentNullException("B");

			if (rowIndex < 0 || rowIndex > A.RowCount
				|| columnIndex < 0 || columnIndex > A.ColumnCount)
			{
				return A;
			}

			int x = 0;
			int y = 0;
			for (int i = 0; i < B.RowCount * B.ColumnCount; i++)
			{
				if (x > 0 && x % B.RowCount == 0)
				{
					x = 0;
					y++;
				}

				A[x + rowIndex, y + columnIndex] = A[x + rowIndex, y + columnIndex] + B[x, y];
				x++;
			}

			return A;
		}

		public static Matrix<double> SubMatrix(Matrix<double> A, int rowStartIndex, int rowStopIndex, int columnStartIndex, int columnStopIndex)
		{
			if (A == null) throw new ArgumentNullException("A");

			if (rowStartIndex < 0 || rowStartIndex > A.RowCount
				|| rowStopIndex < 0 || rowStopIndex > A.RowCount || rowStopIndex < rowStartIndex
				|| columnStartIndex < 0 || columnStartIndex > A.ColumnCount
				|| columnStopIndex < 0 || columnStopIndex > A.ColumnCount || columnStopIndex < columnStartIndex)
			{
				return A;
			}

			var resultMatrix = Matrix<double>.Build.Dense(rowStopIndex - rowStartIndex, columnStopIndex - columnStartIndex);

			for (int i = rowStartIndex; i < rowStopIndex; i++)
			{
				for (int j = columnStartIndex; j < columnStopIndex; j++)
					resultMatrix[i - rowStartIndex, j - columnStartIndex] = A[i, j];
			}

			return resultMatrix;
		}

		public static Matrix<double> Divide(Matrix<double> A, double divisor)
		{
			if (A == null) throw new ArgumentNullException("A");

			var resultMatrix = Matrix<double>.Build.Dense(A.RowCount, A.ColumnCount);

			int x = 0;
			int y = 0;
			for (int i = 0; i < A.RowCount * A.ColumnCount; i++)
			{
				if (x > 0 && x % A.RowCount == 0)
				{
					x = 0;
					y++;
				}

				resultMatrix[x, y] = A[x, y] / divisor;
				x++;
			}

			return resultMatrix;
		}

		public static Matrix<double> VandermondeMatrix(Vector<double> data, int order)
		{
			if (data == null) throw new ArgumentNullException("data");

			// Construct Vandermonde matrix
			var vData = new double[data.Count, order + 1];
			for (int i = order; i >= 0; i--)
			{
				if (i == order)
				{
					for (int j = 0; j < data.Count; j++)
						vData[j, i] = 1;
				}
				else
				{
					for (int j = 0; j < data.Count; j++)
						vData[j, i] = data[j] * vData[j, i + 1];
				}
			}

			return Matrix<double>.Build.DenseOfArray(vData);
		}
	}
}