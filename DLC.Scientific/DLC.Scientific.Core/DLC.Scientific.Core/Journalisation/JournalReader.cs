using DLC.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace DLC.Scientific.Core.Journalisation
{
	/// <summary>
	/// Generic reader for reading a EventJournal of any type from a stream
	/// </summary>
	/// <remarks>
	/// The reader can only read the generic properties as contained in an instance of sealed class EventJournal
	/// </remarks>
	public sealed class JournalReader
		: JournalReader<Journal>
	{
	}

	/// <summary>
	/// Read an EventJournal from a stream
	/// </summary>
	/// <typeparam name="TEventJournal"></typeparam>
	public class JournalReader<TEventJournal>
		where TEventJournal : class, IJournal, new()
	{
		/// <summary>
		/// Méthode qui retourne un journal d'événement à partir d'un stream xml.
		/// Si le stream est erronné, elle le répare avant de renvoyer un journal.
		/// Si le stream est non réparable, elle renvoit null.
		/// </summary>
		/// <param name="stream">Le stream xml du journal</param>
		/// <returns>Renvoit le journal si le stream est correct ou réparable, null autrement</returns>
		public static TEventJournal FromStream(Stream stream)
		{
			TEventJournal journal = null;

			using (MemoryStream tempStream = new MemoryStream())
			{
				stream.CopyTo(tempStream);

				if (EnsureFooter(tempStream))
				{
					try
					{
						journal = StreamToJournal(tempStream);
					}
					catch
					{
						journal = null;
					}
				}
			}

			return journal;
		}

		/// <summary>
		/// Tente d'extraire le journal à partir d'un stream XML. Aucune correction n'est
		/// appliquée dans le cas où le stream d'entrée est corrompu ou incomplet.
		/// </summary>
		/// <param name="stream">Le stream XML du journal à ouvrir.</param>
		/// <returns>
		/// Un journal de type <paramref name="TEventJournal"/>, ou <c>null</c> s'il a été
		/// impossible d'extraire le journal.
		/// </returns>
		public static TEventJournal FromStreamUncorrected(Stream stream)
		{
			TEventJournal journal = null;

			using (MemoryStream tempStream = new MemoryStream())
			{
				stream.CopyTo(tempStream);
				journal = StreamToJournal(tempStream);
			}

			return journal;
		}

		public static string GetJournalType(string fileName)
		{
			string type = "";

			using (FileStream fs = new FileStream(fileName, FileMode.Open))
			{
				XmlReader reader = XmlReader.Create(fs);
				reader.MoveToContent();

				if (reader.Name == "EventJournal")
				{
					type = reader.GetAttribute("Type");
				}
			}

			return type;
		}

		private static TEventJournal StreamToJournal(Stream stream)
		{
			TEventJournal journal = null;

			stream.Seek(0, SeekOrigin.Begin);

			// Cette manière va loader le journal de façon sécurisé, si un tag n'est pas trouvé, rien ne va planter
			journal = new TEventJournal();

			//Convert the type names into types
			Type HeaderType = ((IJournalInternal) journal).JournalHeaderType;
			Type EntryType = ((IJournalInternal) journal).JournalEntryType;
			Type FooterType = ((IJournalInternal) journal).JournalFooterType;

			XDocument xDoc = XDocument.Load(stream);
			XElement rootElement = xDoc.Descendants("EventJournal").FirstOrDefault();

			MethodInfo convertXNodeMethodInfo = typeof(JournalReader<TEventJournal>)
					.GetMethod("ConvertXNode", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, null, new Type[] { typeof(XElement), typeof(string) }, null);

			// ******** READ HEADER **********
			IEnumerable<XElement> elementList = rootElement.Descendants("Header");

			if (elementList.Count() == 1)
			{
				//Créer dynamiquement le type générique de "ConvertXNode" pour
				//l'appeler avec le type du Header lu dans le Xml
				MethodInfo headerConvertXNodeMethodInfo = convertXNodeMethodInfo.MakeGenericMethod(HeaderType);

				object value = headerConvertXNodeMethodInfo.Invoke(null, new object[] { elementList.FirstOrDefault(), "Header" });

				PropertyInfo eventJournalHeaderPropertyInfo = journal.GetType().GetProperty("JournalHeader");
				eventJournalHeaderPropertyInfo.SetValue(journal, value, null);
			}

			// ******** READ CONTENT **********
			elementList = rootElement.Descendants("Entries");

			if (elementList.Count() == 1)
			{
				//Créer dynamiquement le type générique de "ConvertXNode" pour
				//l'appeler avec le type du Content lu dans le Xml
				MethodInfo entryConvertXNodeMethodInfo = convertXNodeMethodInfo.MakeGenericMethod(EntryType);

				elementList = elementList.Descendants("Entry");

				//Utiliser la réflection pour appeler la méthode Add du journal car avec les
				//types génériques qu'on ne connait pas d'avance, ça simplifie la conversion de types
				MethodInfo journalAddMethodInfo = journal.GetType().GetMethod("Add", new Type[] { EntryType });

				foreach (XElement element in elementList)
				{
					object value = entryConvertXNodeMethodInfo.Invoke(null, new object[] { element, "Entry" });

					journalAddMethodInfo.Invoke(journal, new object[] { value });
				}
			}

			// ******** READ FOOTER **********
			elementList = rootElement.Descendants("Footer");

			if (elementList.Count() == 1)
			{
				MethodInfo footerConvertXNodeMethodInfo = convertXNodeMethodInfo.MakeGenericMethod(FooterType);

				object value = footerConvertXNodeMethodInfo.Invoke(null, new object[] { "Footer", elementList.FirstOrDefault() });

				PropertyInfo eventJournalFooterPropertyInfo = journal.GetType().GetProperty("JournalFooter");
				eventJournalFooterPropertyInfo.SetValue(journal, value, null);
			}

			return journal;
		}

		private static TNode ConvertXNode<TNode>(XElement node)
		{
			return ConvertXNode<TNode>(node, null);
		}

		private static TNode ConvertXNode<TNode>(XElement node, string nodeName)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				using (XmlWriter writer = XmlWriter.Create(ms))
				{
					node.WriteTo(writer);
					writer.Flush();
					ms.Position = 0;

					var serializer = XmlSerializerCache.GetOrAdd(typeof(TNode), nodeName);

					return (TNode) serializer.Deserialize(ms);
				}
			}
		}

		/// <summary>
		/// Repair the stream by adding the missing footer.
		/// If the in stream is ok, it is just copied into the out stream
		/// </summary>
		/// <returns>Returns true if the stream is repaired, false otherwise</returns>
		private static bool EnsureFooter(Stream stream)
		{
			stream.Seek(0, SeekOrigin.Begin);

			IJournalInternal tmp = (IJournalInternal) new TEventJournal();

			//Type headerType = tmp.EventJournalHeaderType;
			//Type contentType = tmp.EventJournalEntryType;
			Type footerType = tmp.JournalFooterType;

			XDocument xDoc;
			try
			{
				xDoc = XDocument.Load(stream);
			}
			catch (XmlException)
			{
				//Forcer une réparation brute
				stream.Seek(0, SeekOrigin.End);
				XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
				writer.WriteRaw("</Entries>");

				JournalFooter newFooter = (JournalFooter) Activator.CreateInstance(footerType);
				newFooter.CloseDateTime = DateTimePrecise.Now;
				newFooter.Repaired = true;

				// String properties are null, but must be string.Empty
				foreach (PropertyInfo prop in footerType.GetProperties().Where(x => x.PropertyType == typeof(string)))
				{
					prop.SetValue(newFooter, string.Empty);
				}

				JournalRTRecorder.ToXml(writer, newFooter, "Footer");

				writer.WriteRaw("</EventJournal>");
				writer.Flush();

				try
				{
					stream.Seek(0, SeekOrigin.Begin);
					xDoc = XDocument.Load(stream);
				}
				catch (XmlException)
				{
					return false;
				}
			}

			return true;
		}
	}
}