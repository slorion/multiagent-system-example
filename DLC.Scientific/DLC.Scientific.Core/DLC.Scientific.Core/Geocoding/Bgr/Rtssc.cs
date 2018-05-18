using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Core.Geocoding.Bgr
{
	[DataContract]
	[Serializable]
	public class Rtssc
		: IRtssc
	{
		private string _numRtssc;

		public Rtssc()
		{
			this.Voie = 1;
		}

		public Rtssc(string route, string troncon, string section, string sousRoute, DirectionBgr direction = DirectionBgr.Unknown, double? chainage = null)
			: this()
		{
			this.Route = route;
			this.Troncon = troncon;
			this.Section = section;
			this.SousRoute = sousRoute;

			this.Direction = direction;
			this.Chainage = chainage;
			this.NumeroRTSS = this.ToString().Replace(" ", "");
		}

		public Rtssc(IRtssc rtssc)
			: this(rtssc, rtssc.Chainage)
		{
		}

		public Rtssc(IRtssc rtssc, double? chainage = null)
			: this()
		{
			if (rtssc == null) throw new ArgumentNullException("rtssc");

			this.Route = rtssc.Route;
			this.Troncon = rtssc.Troncon;
			this.Section = rtssc.Section;
			this.SousRoute = rtssc.SousRoute;

			this.Voie = rtssc.Voie;
			this.Longueur = rtssc.Longueur;

			this.NumeroRTSS = rtssc.NumeroRTSS;

			this.CentreDeServiceID = rtssc.CentreDeServiceID;
			this.CentreDeServiceName = rtssc.CentreDeServiceName;
			this.DirectionGeneraleID = rtssc.DirectionGeneraleID;
			this.DirectionGeneraleName = rtssc.DirectionGeneraleName;
			this.DirectionTerritorialeID = rtssc.DirectionTerritorialeID;
			this.DirectionTerritorialeName = rtssc.DirectionTerritorialeName;

			this.Chainage = chainage;
		}

		public Rtssc(string numeroRTSS, double? chainage = null)
			: this()
		{
			if (string.IsNullOrEmpty(numeroRTSS)) throw new ArgumentNullException("numeroRTSS");

			numeroRTSS = numeroRTSS.Replace("-", string.Empty);
			numeroRTSS = numeroRTSS.Replace(" ", string.Empty);

			if (numeroRTSS.Length != 14)
				throw new InvalidOperationException("numeroRTSS doit contenir 14 caractères exactement.");

			//"0002006030000G"
			this.Route = numeroRTSS.Substring(0, 5); // 00020
			this.Troncon = numeroRTSS.Substring(5, 2); // 06
			this.Section = numeroRTSS.Substring(7, 3); // 030
			this.SousRoute = numeroRTSS.Substring(10, 4); // 000G
			this.NumeroRTSS = numeroRTSS;
			this.Chainage = chainage;
		}

		[DataMember]
		public string Route { get; set; }

		[DataMember]
		public string Troncon { get; set; }

		[DataMember]
		public string Section { get; set; }

		[DataMember]
		public string SousRoute { get; set; }

		public string CodeSousRoute { get { return this.SousRoute == null || this.SousRoute.Length < 4 ? string.Empty : this.SousRoute[0].ToString(); } }
		public string SequenceSousRoute { get { return this.SousRoute == null || this.SousRoute.Length < 4 ? string.Empty : this.SousRoute[1].ToString(); } }
		public string CodeSousCode { get { return this.SousRoute == null || this.SousRoute.Length < 4 ? string.Empty : this.SousRoute[2].ToString(); } }
		public string CodeCoteChaussee { get { return this.SousRoute == null || this.SousRoute.Length < 4 ? string.Empty : this.SousRoute[3].ToString(); } }

		[DataMember]
		public double? Chainage { get; set; }

		[DataMember]
		public int Voie { get; set; }

		[DataMember]
		public DirectionBgr Direction { get; set; }

		[DataMember]
		public long Longueur { get; set; }

		[DataMember]
		public string NumeroRTSS
		{
			get
			{
				if (string.IsNullOrEmpty(_numRtssc))
				{
					_numRtssc = this.Chainage == null ?
						string.Format("{0}{1}{2}{3}", this.Route, this.Troncon, this.Section, this.SousRoute)
						: string.Format("{0}{1}{2}{3}{4}", this.Route, this.Troncon, this.Section, this.SousRoute, this.Chainage.Value.ToString("000000"));
				}
				return _numRtssc;
			}
			set
			{
				_numRtssc = value;
			}
		}

		[DataMember]
		public string NumeroRTSSFormate { get; set; }

		[DataMember]
		public string CentreDeServiceID { get; set; }

		[DataMember]
		public string CentreDeServiceName { get; set; }

		[DataMember]
		public string DirectionGeneraleID { get; set; }

		[DataMember]
		public string DirectionGeneraleName { get; set; }

		[DataMember]
		public string DirectionTerritorialeID { get; set; }

		[DataMember]
		public string DirectionTerritorialeName { get; set; }

		[DataMember]
		public int Ide { get; set; }

		[DataMember]
		public string Statut { get; set; }

		public bool Equals(IRtssc other, bool ignoreChainage)
		{
			if (other == null)
			{
				return false;
			}
			else
			{
				bool equals =
					string.Equals(this.Route, other.Route, StringComparison.OrdinalIgnoreCase)
					&& string.Equals(this.Troncon, other.Troncon, StringComparison.OrdinalIgnoreCase)
					&& string.Equals(this.Section, other.Section, StringComparison.OrdinalIgnoreCase)
					&& string.Equals(this.SousRoute, other.SousRoute, StringComparison.OrdinalIgnoreCase);

				if (equals && !ignoreChainage)
					equals = this.Chainage == other.Chainage;

				return equals;
			}
		}

		public override string ToString()
		{
			if (this.Chainage == null)
				return string.Format("{0} {1} {2} {3}", this.Route, this.Troncon, this.Section, this.SousRoute);
			else
				return string.Format("{0} {1} {2} {3} {4}", this.Route, this.Troncon, this.Section, this.SousRoute, this.Chainage.Value.ToString("000000"));
		}

		#region IEquatable members

		public bool Equals(IRtssc other)
		{
			return Equals(other, false);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as IRtssc);
		}

		public override int GetHashCode()
		{
			int hash = 17;
			hash = hash * 23 + this.Route.GetHashCode();
			hash = hash * 23 + this.Troncon.GetHashCode();
			hash = hash * 23 + this.Section.GetHashCode();
			hash = hash * 23 + this.SousRoute.GetHashCode();
			hash = hash * 23 + this.Chainage.GetHashCode();
			return hash;
		}

		public static bool operator ==(Rtssc x, Rtssc y)
		{
			if (object.ReferenceEquals(x, y))
				return true;
			else if ((object) x == null || (object) y == null)
				return false;
			else
				return x.Equals(y);
		}

		public static bool operator !=(Rtssc x, Rtssc y)
		{
			return !(x == y);
		}

		#endregion

		#region IComparable<Rtssc> members

		public int CompareTo(IRtssc other)
		{
			if (other == null)
				return 1;
			else
			{
				int currentCheck;

				return
					(currentCheck = string.Compare(this.Route, other.Route, StringComparison.OrdinalIgnoreCase)) != 0 ? currentCheck
					: (currentCheck = string.Compare(this.Troncon, other.Troncon, StringComparison.OrdinalIgnoreCase)) != 0 ? currentCheck
					: (currentCheck = string.Compare(this.Section, other.Section, StringComparison.OrdinalIgnoreCase)) != 0 ? currentCheck
					: (currentCheck = string.Compare(this.SousRoute, other.SousRoute, StringComparison.OrdinalIgnoreCase)) != 0 ? currentCheck
					: this.Chainage.GetValueOrDefault().CompareTo(other.Chainage.GetValueOrDefault());
			}
		}

		#endregion
	}
}