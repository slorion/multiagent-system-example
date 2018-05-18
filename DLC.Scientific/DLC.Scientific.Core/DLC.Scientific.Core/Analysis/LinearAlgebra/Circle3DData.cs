using MathNet.Numerics.LinearAlgebra;

namespace DLC.Scientific.Core.Analysis.LinearAlgebra
{
	public class Circle3DData
	{
		public double Theta { get; set; }
		public double Z { get; set; }
		public Vector<double> PlaneCentroid { get; set; }
		public Vector<double> CircleCentroid { get; set; }
		public double CircleRadius { get; set; }
		public Vector<double> RotationCentroid { get; set; }
		public Matrix<double> RotationMatrix { get; set; }
		public bool IsAscending { get; set; }

		public Circle3DData()
		{
			this.Theta = 0;
			this.Z = 0;
			this.PlaneCentroid = Vector<double>.Build.Dense(3);
			this.CircleCentroid = Vector<double>.Build.Dense(2);
			this.RotationCentroid = Vector<double>.Build.Dense(3);
			this.RotationMatrix = Matrix<double>.Build.Dense(3, 3);
			this.CircleRadius = 0;
			this.IsAscending = false;
		}
	}
}