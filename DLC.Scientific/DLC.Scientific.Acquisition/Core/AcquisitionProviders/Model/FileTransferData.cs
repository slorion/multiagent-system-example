using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model
{
	[DataContract]
	[Serializable]
	public class FileTransferData
		: ProviderData
	{
		public FileTransferData()
		{
		}

		public FileTransferData(FileTransferData data)
			: this()
		{
			if (data == null) throw new ArgumentNullException("data");

			this.MachineName = data.MachineName;
			this.MonitoredFolderPath = data.MonitoredFolderPath;
			this.DestinationFolderPath = data.DestinationFolderPath;
			this.FileName = data.FileName;
			this.CopiedBytes = data.CopiedBytes;
			this.TotalBytes = data.TotalBytes;
			this.Exception = data.Exception;
		}

		[DataMember]
		public string MachineName { get; set; }

		[DataMember]
		public string MonitoredFolderPath { get; set; }

		[DataMember]
		public string DestinationFolderPath { get; set; }

		[DataMember]
		public string FileName { get; set; }

		[DataMember]
		public long CopiedBytes { get; set; }

		[DataMember]
		public long TotalBytes { get; set; }

		[DataMember]
		public Exception Exception { get; set; }

		public override string ToString()
		{
			return string.Format("{0};{1}", base.ToString(), this.FileName);
		}
	}
}