namespace DLC.Scientific.Core.Geocoding.Gps
{
	/// <summary>
	/// List of possible Fix Type of GPS tracking
	/// </summary>
	public enum FixType
	{
		None = 0,
		Fix = 1,
		Diff = 2,
		RTKfixed = 4,
		RTKfloating = 5,
		WAAS = 9,
		PostProcess = 10,
	}
}