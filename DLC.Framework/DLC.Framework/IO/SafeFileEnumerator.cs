using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;

namespace DLC.Framework.IO
{
	public static class SafeFileEnumerator
	{
		public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
		{
			if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
			if (string.IsNullOrEmpty(searchPattern)) throw new ArgumentNullException("searchPattern");

			Func<string, IEnumerable<string>> getChildren = child => SafeFileSystemEnumerate(() => Directory.EnumerateDirectories(child, searchPattern, SearchOption.TopDirectoryOnly));

			if (searchOption == SearchOption.TopDirectoryOnly)
				return getChildren(path);
			else
				return LevelOrder(path, getChildren);
		}

		public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
		{
			if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
			if (string.IsNullOrEmpty(searchPattern)) throw new ArgumentNullException("searchPattern");

			foreach (var dir in EnumerateDirectories(path, "*.*", searchOption))
				foreach (var file in SafeFileSystemEnumerate(() => Directory.EnumerateFiles(dir, searchPattern)))
					yield return file;
		}

		private static IEnumerable<T> LevelOrder<T>(T root, Func<T, IEnumerable<T>> getChildren)
			where T : class
		{
			if (root == null) throw new ArgumentNullException("root");
			if (getChildren == null) throw new ArgumentNullException("getChildren");

			var queue = new Queue<T>();
			queue.Enqueue(root);

			while (queue.Count > 0)
			{
				T current = queue.Dequeue();
				yield return current;

				foreach (var child in getChildren(current))
					queue.Enqueue(child);
			}
		}

		private static readonly IEnumerable<string> _empty = Enumerable.Empty<string>();
		private static IEnumerable<string> SafeFileSystemEnumerate(Func<IEnumerable<string>> enumerate)
		{
			if (enumerate == null) throw new ArgumentNullException("enumerate");

			try { return enumerate(); }
			catch (SecurityException) { return _empty; }
			catch (UnauthorizedAccessException) { return _empty; }
			catch (IOException) { return _empty; }
		}
	}
}