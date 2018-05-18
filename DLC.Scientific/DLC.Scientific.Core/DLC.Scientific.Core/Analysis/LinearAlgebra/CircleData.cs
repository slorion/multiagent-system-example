using MathNet.Numerics.LinearAlgebra;

namespace DLC.Scientific.Core.Analysis.LinearAlgebra
{
	public class CircleData
	{
		public Vector<double> CircleCentroid { get; set; }
		public double CircleRadius { get; set; }
		public double Inclination { get; set; }

		public CircleData()
		{
			this.CircleCentroid = Vector<double>.Build.Dense(2);
			this.CircleRadius = 0;
			this.Inclination = 0;
		}
	}
}