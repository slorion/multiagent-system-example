using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	public interface IFileProcessingJournalEntryData
	{
	}

	public abstract class FileProcessingJournalEntryDataBase
		: IFileProcessingJournalEntryData
	{
		public string Name { get; set; }
		public string type { get; protected set; }
	}

	[Serializable]
	public class FileProcessingJournalEntryData<T>
		: FileProcessingJournalEntryDataBase, IXmlSerializable
	{
		public T Value { get; set; }

		public FileProcessingJournalEntryData()
		{
		}

		public FileProcessingJournalEntryData(string name)
		{
			this.type = typeof(T).AssemblyQualifiedName;
			this.Name = name;
		}

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			throw new NotImplementedException();
		}

		public void ReadXml(XmlReader reader)
		{
			Type t = Type.GetType(reader.GetAttribute("type"));
			this.type = t.AssemblyQualifiedName;
			this.Name = reader.GetAttribute("name");

			var ser = XmlSerializerCache.GetOrAdd(t);

			reader.ReadStartElement();
			object obj = ser.Deserialize(reader.ReadSubtree());
			PropertyInfo prop = this.GetType().GetProperty("Value");
			prop.SetValue(this, Convert.ChangeType(obj, t), null);
			if (!reader.IsEmptyElement)
				reader.ReadEndElement();
			else
				reader.Read();
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("type", type);
			writer.WriteAttributeString("name", Name);
			var ser = XmlSerializerCache.GetOrAdd(Type.GetType(this.type));
			ser.Serialize(writer, this.Value);
		}

		public object Clone()
		{
			FileProcessingJournalEntryData<T> temp = new FileProcessingJournalEntryData<T>();
			temp.Name = this.Name;
			temp.Value = this.Value;
			temp.type = this.type;

			return temp;
		}
	}

	public class JournalDataCollection<T>
		: IList<T>
		where T : FileProcessingJournalEntryDataBase
	{
		private IList<T> _list = new List<T>();

		public int IndexOf(T item)
		{
			return _list.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			_list.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_list.RemoveAt(index);
		}

		public T this[int index]
		{
			get
			{
				return _list[index];
			}
			set
			{
				_list[index] = value;
			}
		}

		public void Add(T item)
		{
			var v = _list.Where(x => x.Name == item.Name).Any();
			if (!v)
				_list.Add(item);
			else
			{
				var temp = _list.Where(x => x.Name == item.Name).First();
				_list.Remove(temp);
				_list.Add(item);
			}
		}

		public void Clear()
		{
			_list.Clear();
		}

		public bool Contains(T item)
		{
			return _list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _list.Count; }
		}

		public bool IsReadOnly
		{
			get { return _list.IsReadOnly; }
		}

		public bool Remove(T item)
		{
			return _list.Remove(item);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		public JournalDataCollection<T> Clone()
		{
			JournalDataCollection<T> temp = new JournalDataCollection<T>();
			foreach (var v in this)
			{
				Type t = typeof(FileProcessingJournalEntryData<>).MakeGenericType(Type.GetType(v.type));
				var val = (T) t.GetMethod("Clone").Invoke(v, null);
				T temp1 = (T) val;
				temp.Add(temp1);
			}
			return temp;
		}
	}

	/// <summary>
	/// Entry of an Event Journal containing file information
	/// </summary>
	[Serializable]
	public class FileProcessingJournalEntry
		: FileEntry, IXmlSerializable
	{
		public JournalDataCollection<FileProcessingJournalEntryDataBase> EntryData { get; set; }

		public FileProcessingJournalEntry()
		{
			EntryData = new JournalDataCollection<FileProcessingJournalEntryDataBase>();
		}

		public object this[string Name]
		{
			get
			{
				IEnumerable<FileProcessingJournalEntryDataBase> elements = EntryData.Where(item => item.Name == Name);
				if (elements.Count() == 0)
					return null;
				FileProcessingJournalEntryDataBase baseData = elements.First();

				Type t = typeof(FileProcessingJournalEntryData<>).MakeGenericType(Type.GetType(baseData.type));
				var v = t.GetProperty("Value").GetValue(baseData, null);
				return v;
			}
		}

		public T GetValue<T>(string name)
		{
			T val;
			IEnumerable<FileProcessingJournalEntryDataBase> elements = EntryData.Where(item => item.Name == name);
			if (elements.Count() == 0)
				return default(T);
			FileProcessingJournalEntryDataBase baseData = elements.First();

			Type t = typeof(FileProcessingJournalEntryData<>).MakeGenericType(Type.GetType(baseData.type));
			val = (T) t.GetProperty("Value").GetValue(baseData, null);
			return val;
		}

		//public int ide_rtss { get; set; }
		//public int direction { get; set; }
		//public int voie_id { get; set; }
		//public int chainage_debut { get; set; }
		//public int chainage_fin { get; set; }
		//public int progression_debut { get; set; }
		//public int progression_fin { get; set; }
		//public float latitude_debut { get; set; }
		//public float longitude_debut { get; set; }
		//public float altitude_debut { get; set; }
		//public float latitude_fin { get; set; }
		//public float longitude_fin { get; set; }
		//public float altitude_fin { get; set; }
		//public int vitesse_acquisition { get; set; }
		//public int pas_Calcul { get; set; }
		//public DateTime dateheure_releve { get; set; }
		//public int type_fichier { get; set; }
		//public string module_fichier { get; set; }

		public FileProcessingJournalEntry Clone()
		{
			FileProcessingJournalEntry temp = new FileProcessingJournalEntry();
			temp.AbsolutePath = this.AbsolutePath;
			temp.Checksum = this.Checksum;
			temp.Comment = Comment;
			temp.DateTime = DateTime;
			temp.FileName = FileName;
			temp.RelativePath = RelativePath;
			temp.EntryData = this.EntryData.Clone();
			return temp;
		}

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			throw new NotImplementedException();
		}

		public void ReadXml(System.Xml.XmlReader reader)
		{
			reader.MoveToElement();
			reader.ReadStartElement();
			Type instanceType = base.GetType().BaseType;

			while (reader.IsStartElement())
			{
				if (reader.HasAttributes)
				{
					XmlNodeType nodeType = reader.NodeType;

					Type t = typeof(FileProcessingJournalEntryData<>);
					t = t.MakeGenericType(Type.GetType(reader.GetAttribute("type")));
					string name = reader.GetAttribute("name");

					var ser = XmlSerializerCache.GetOrAdd(t, name);

					object obj = ser.Deserialize(reader.ReadSubtree());
					reader.ReadEndElement();
					this.EntryData.Add((FileProcessingJournalEntryDataBase) obj);
				}
				else
				{
					PropertyInfo Prop = instanceType.GetProperty(reader.Name);
					if (Prop != null)
					{
						var h = reader.ReadElementContentAsObject(Prop.Name, "");
						Prop.SetValue(this, Convert.ChangeType(h, Prop.PropertyType), null);
					}
				}
			}
		}

		public void WriteXml(System.Xml.XmlWriter writer)
		{
			Type instanceType = base.GetType().BaseType;

			foreach (PropertyInfo Prop in instanceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				string value = null;
				object propertyValue = Prop.GetValue(this, null);

				if (propertyValue != null)
					value = Prop.GetValue(this, null).ToString();

				writer.WriteElementString(Prop.Name, value);
			}

			foreach (IFileProcessingJournalEntryData entryData in this.EntryData)
			{
				var val = entryData.GetType().GetProperty("Value").GetValue(entryData, null);
				string name = (string) entryData.GetType().GetProperty("Name").GetValue(entryData, null);
				string type = (string) entryData.GetType().GetProperty("type").GetValue(entryData, null);

				var ser = XmlSerializerCache.GetOrAdd(entryData.GetType(), name);
				ser.Serialize(writer, entryData);
			}
		}
	}
}