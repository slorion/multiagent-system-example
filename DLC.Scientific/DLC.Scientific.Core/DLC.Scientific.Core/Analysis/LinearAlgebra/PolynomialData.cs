using MathNet.Numerics.LinearAlgebra;
using System;

namespace DLC.Scientific.Core.Analysis.LinearAlgebra
{
	public class PolynomialData
	{
		public Vector<double> PolynomialParams { get; set; }
		public Matrix<double> VandermondeMatrix { get; set; }
		public double FreedomDegrees { get; set; }
		public double ResidualNorm { get; set; }
		public int PolynomialOrder { get; set; }

		public PolynomialData()
		{
		}

		public PolynomialData(int order)
			: this()
		{
			if (order != 3 && order != 5)
				throw new NotImplementedException("Only 3rd and 5th order are implemented");

			this.PolynomialOrder = order;
		}
	}
}