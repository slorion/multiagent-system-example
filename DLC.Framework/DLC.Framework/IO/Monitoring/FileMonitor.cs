using DLC.Framework.Reactive;
using System;
using System.IO;
using System.Reactive.Subjects;

namespace DLC.Framework.IO.Monitoring
{
	public class FileMonitor
		: IDisposable
	{
		private readonly FileSystemWatcher _fsw;
		private readonly ISubject<FileSystemEventArgs, FileSystemEventArgs> _fileChangedSubject;

		public IObservable<FileSystemEventArgs> FileChangedDataSource { get { return _fileChangedSubject; } }

		public string Path { get { return _fsw.Path; } set { _fsw.Path = value; } }
		public bool IncludeSubdirectories { get { return _fsw.IncludeSubdirectories; } set { _fsw.IncludeSubdirectories = value; } }
		public NotifyFilters NotifyFilter { get { return _fsw.NotifyFilter; } set { _fsw.NotifyFilter = value; } }
		//public int InternalBufferSize { get { return _fsw.InternalBufferSize; } set { _fsw.InternalBufferSize = value; } }
		public string Filter { get { return _fsw.Filter; } set { _fsw.Filter = value; } }

		public FileMonitor(string path, string filter = null, WatcherChangeTypes watchedChangeTypes = WatcherChangeTypes.All)
		{
			if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");

			_fileChangedSubject = Subject.Synchronize(new SubjectSlim<FileSystemEventArgs>());

			_fsw = new FileSystemWatcher(path, filter);

			// by default, try to reduce to a minimum the information returned by the FileSystemWatcher
			// to reduce the occurence of an InternalBufferOverflowException when dealing with many files
			_fsw.NotifyFilter = NotifyFilters.FileName;

			// Changed event requires NotifyFilters.LastWrite
			if (watchedChangeTypes.HasFlag(WatcherChangeTypes.Changed))
				_fsw.NotifyFilter |= NotifyFilters.LastWrite;

			// increase internal buffer size of the FSW to avoid an InternalBufferOverflowException
			// (the size must be a multiple of 4096 for a better performance on Intel platform)
			// see http://stackoverflow.com/questions/13916595/is-it-really-that-expensive-to-increase-filesystemwatcher-internalbuffersize
			_fsw.InternalBufferSize = 16 * 4096; // 64KB

			if (watchedChangeTypes.HasFlag(WatcherChangeTypes.Changed)) _fsw.Changed += (s, e) => _fileChangedSubject.OnNext(e);
			if (watchedChangeTypes.HasFlag(WatcherChangeTypes.Created)) _fsw.Created += (s, e) => _fileChangedSubject.OnNext(e);
			if (watchedChangeTypes.HasFlag(WatcherChangeTypes.Deleted)) _fsw.Deleted += (s, e) => _fileChangedSubject.OnNext(e);
			if (watchedChangeTypes.HasFlag(WatcherChangeTypes.Renamed)) _fsw.Renamed += (s, e) => _fileChangedSubject.OnNext(e);

			_fsw.Error += (s, e) => _fileChangedSubject.OnError(e.GetException());
		}

		public void StartWatch()
		{
			_fsw.EnableRaisingEvents = true;
		}

		public void StopWatch()
		{
			_fsw.EnableRaisingEvents = false;
		}

		#region IDisposable members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			var fsw = _fsw;
			if (fsw != null)
				fsw.Dispose();
		}

		~FileMonitor()
		{
			Dispose(false);
		}

		#endregion
	}
}