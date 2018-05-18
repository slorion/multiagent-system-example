using MathNet.Numerics.LinearAlgebra;

namespace DLC.Scientific.Core.Analysis.LinearAlgebra
{
	public class Line3DData
	{
		public Vector<double> IntersectionPoint { get; set; }
		public Vector<double> LineParams { get; set; }
		public Vector<double> Centroid { get; set; }
		public Vector<double> Residuals { get; set; }
		public double ResidualsNorm { get; set; }

		public Line3DData(int length)
		{
			this.IntersectionPoint = Vector<double>.Build.Dense(3);
			this.LineParams = Vector<double>.Build.Dense(3);
			this.Residuals = Vector<double>.Build.Dense(length);
			this.Centroid = Vector<double>.Build.Dense(3);
			this.ResidualsNorm = 0;
		}
	}
}