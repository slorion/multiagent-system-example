using MathNet.Numerics.LinearAlgebra;

namespace DLC.Scientific.Core.Analysis.LinearAlgebra
{
	public class PlaneData
	{
		public Vector<double> PlaneParams { get; set; }
		public Vector<double> Centroid { get; set; }
		public Vector<double> Residuals { get; set; }

		public PlaneData(int length)
		{
			this.PlaneParams = Vector<double>.Build.Dense(3);
			this.Residuals = Vector<double>.Build.Dense(length);
			this.Centroid = Vector<double>.Build.Dense(3);
		}
	}
}