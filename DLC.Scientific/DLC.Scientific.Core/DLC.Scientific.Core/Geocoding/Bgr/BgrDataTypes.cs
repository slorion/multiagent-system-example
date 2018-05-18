using System;

namespace DLC.Scientific.Core.Geocoding.Bgr
{
	[Flags]
	public enum BgrDataTypes
	{
		Unknown = 0x0000,
		Routes = 0x0001,
		Bretelles = 0x0002,
		CarrefoursGiratoires = 0x0004,
		All = 0xFFFF
	}
}