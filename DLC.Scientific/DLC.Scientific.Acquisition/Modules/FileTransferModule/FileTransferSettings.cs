using System.Collections.Generic;

namespace DLC.Scientific.Acquisition.Modules.FileTransferModule
{
	public class FileTransferSettings
	{
		public List<string> DestinationFolders { get; set; }
		public string SourceFolder { get; set; }
		public string Filter { get; set; }
	}
}