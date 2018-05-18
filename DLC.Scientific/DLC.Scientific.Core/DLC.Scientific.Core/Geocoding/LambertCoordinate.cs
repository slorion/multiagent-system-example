namespace DLC.Scientific.Core.Geocoding
{
	public class LambertCoordinate
	{
		public double X { get; set; }
		public double Y { get; set; }

		public LambertCoordinate()
		{
		}

		public LambertCoordinate(double x, double y)
			: this()
		{
			this.X = x;
			this.Y = y;
		}

		public LambertCoordinate(double[] data)
			: this()
		{
			this.X = data[0];
			this.Y = data[1];
		}
	}
}