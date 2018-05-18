using MathNet.Numerics.LinearAlgebra;

namespace DLC.Scientific.Core.Analysis.LinearAlgebra
{
	public class LineData
	{
		public Vector<double> IntersectionPoint { get; set; }
		public Vector<double> LineParams { get; set; }
		public Vector<double> Centroid { get; set; }
		public Vector<double> Residuals { get; set; }
		public double ResidualsNorm { get; set; }

		public LineData(int length)
		{
			this.IntersectionPoint = Vector<double>.Build.Dense(2);
			this.LineParams = Vector<double>.Build.Dense(2);
			this.Residuals = Vector<double>.Build.Dense(length);
			this.Centroid = Vector<double>.Build.Dense(2);
			this.ResidualsNorm = 0;
		}
	}
}