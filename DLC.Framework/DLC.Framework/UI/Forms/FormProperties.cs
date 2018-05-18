using Microsoft.Win32;
using System.Windows.Forms;

namespace DLC.Framework.UI.Forms
{
	internal class FormProperties
	{
		private const string RootKey = "Software\\TransportsQuebec\\DLC\\Windows\\Forms";

		public FormProperties(string name)
		{
			this.Name = name;

			RegistryKey key = Registry.CurrentUser.OpenSubKey(RootKey + Name);
			if (key != null)
			{
				try
				{
					this.Top = (int) key.GetValue("top");
					this.Left = (int) key.GetValue("left");
					this.Width = (int) key.GetValue("width");
					this.Height = (int) key.GetValue("height");
					this.State = (string) key.GetValue("state");
				}
				catch { }
			}
			else
			{
				this.Top = -1;
				this.Left = -1;
				this.Width = -1;
				this.Height = -1;
				this.State = FormWindowState.Normal.ToString();
			}
		}

		public string Name { get; set; }
		public int Top { get; set; }
		public int Left { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public string State { get; set; }

		public void Save()
		{
			RegistryKey key = Registry.CurrentUser.OpenSubKey(RootKey + this.Name, true);
			if (key == null)
			{
				key = Registry.CurrentUser.CreateSubKey(RootKey + this.Name);
			}
			if (key != null)
			{
				try
				{
					key.SetValue("top", this.Top);
					key.SetValue("left", this.Left);
					key.SetValue("width", this.Width);
					key.SetValue("height", this.Height);
					key.SetValue("state", this.State);
				}
				catch
				{ }
			}
		}

		public void SaveState()
		{
			RegistryKey key = Registry.CurrentUser.OpenSubKey(RootKey + this.Name, true);
			if (key == null)
			{
				key = Registry.CurrentUser.CreateSubKey(RootKey + this.Name);
			}
			if (key != null)
			{
				try
				{
					key.SetValue("state", this.State);
				}
				catch
				{ }
			}
		}

		public override string ToString()
		{
			return string.Join(";", this.Name, this.Top, this.Left, this.Width, this.Height, this.State);
		}
	}
}