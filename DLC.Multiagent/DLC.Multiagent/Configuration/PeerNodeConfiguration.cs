namespace DLC.Multiagent.Configuration
{
	public class PeerNodeConfiguration
	{
		public PeerNodeConfiguration()
		{
			this.Enabled = true;
		}

		public bool Enabled { get; set; }
		public string Host { get; set; }
		public int Port { get; set; }
		public int RxPort { get; set; }
		public string Description { get; set; }
	}
}