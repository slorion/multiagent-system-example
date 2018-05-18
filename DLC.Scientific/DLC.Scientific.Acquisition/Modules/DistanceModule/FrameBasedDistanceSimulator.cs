using DLC.Framework.Reactive;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace DLC.Scientific.Acquisition.Modules.DistanceModule
{
	public class FrameBasedDistanceSimulator
		: DistanceSimulator
	{

		private readonly SubjectSlim<DistanceData> _distanceDataSource = new SubjectSlim<DistanceData>();
		private DiagnosticEventData _curDiagnosticEventData;
		private bool _diagnosticActive = false;
		private string _seqDir = string.Empty;
		private SortedDictionary<ulong, long> _dicFrameIdDistance = null;
		private IDisposable _currentObserver;

		protected override void ValidateConfigurationCore()
		{
			base.ValidateConfigurationCore();

			if (this.SimulatorFrequencyInMetersPerSecond < 1)
				throw new InvalidOperationException("SimulatorFrequencyInMetersPerSecond doit être supérieur à 0.");
		}

		public override void SignalateDiagnosticEvent(DiagnosticEventData data)
		{
			_curDiagnosticEventData = data;
			if (data.Actived != _diagnosticActive)
			{
				if (_currentObserver != null)
				{
					_currentObserver.Dispose();
				}
				if (!data.Actived)
				{
					WithoutDiagnosticTimerObserver();
				}
				_diagnosticActive = data.Actived;
			}

			if (_seqDir != data.SequencePath)
			{
				_seqDir = data.SequencePath;
				LoadPhotoDistance();
			}
		
			if (_curDiagnosticEventData.CurrentState == DiagnosticState.Restart)
			{
				_absoluteDistance = 0;
				_distanceDataSource.OnNext(new DistanceData { AbsoluteDistance = 0, ReferenceEncoderNumber = 1, AbsoluteLeftPulseCount = 0 * PulsePerMeter, AbsoluteRightPulseCount = 0 * PulsePerMeter });
			}
			else if (_curDiagnosticEventData.CurrentState == DiagnosticState.Next)
			{
				long last = _absoluteDistance;
				long distance = GetDistance(1);

				if (last != 0 && distance != 0 && last / 100 != distance / 100 && distance != (distance / 100) * 100)
				{
					_distanceDataSource.OnNext(new DistanceData { AbsoluteDistance = (distance / 100) * 100, ReferenceEncoderNumber = 1, AbsoluteLeftPulseCount = distance * PulsePerMeter, AbsoluteRightPulseCount = distance * PulsePerMeter });
				}

				_distanceDataSource.OnNext(new DistanceData { AbsoluteDistance = distance, ReferenceEncoderNumber = 1, AbsoluteLeftPulseCount = distance * PulsePerMeter, AbsoluteRightPulseCount = distance * PulsePerMeter });
			}
			else if (_curDiagnosticEventData.CurrentState == DiagnosticState.Back)
			{
				long distance = GetDistance(-1);

				_distanceDataSource.OnNext(new DistanceData { AbsoluteDistance = distance, ReferenceEncoderNumber = 1, AbsoluteLeftPulseCount = distance * PulsePerMeter, AbsoluteRightPulseCount = distance * PulsePerMeter });
			}	

		}

		private long GetDistance(int increment)
		{
			long last = _absoluteDistance;
			long distance = _absoluteDistance + increment;
			if (_dicFrameIdDistance != null)
			{
				distance = GetDistanceFromDic();
			}
			else if (_curDiagnosticEventData.CameraType == DiagnosticCameraType.Panasonic)
			{
				distance = last + 5;
				
			}
			_absoluteDistance = (int) distance;
			return distance;
		}

		private long GetDistanceFromDic()
		{
			if (_curDiagnosticEventData.LastFrameTreated == ulong.MaxValue || _dicFrameIdDistance == null || _dicFrameIdDistance.Count() == 0)
			{
				return 0;
			}

			ulong lastKey = _dicFrameIdDistance.Keys.FirstOrDefault(num => num > _curDiagnosticEventData.LastFrameTreated);
			if (lastKey == 0)
			{
				lastKey = _dicFrameIdDistance.Keys.Last();
			}
			return _dicFrameIdDistance[lastKey];

		}

		private void LoadPhotoDistance()
		{
			if (string.IsNullOrWhiteSpace(_seqDir))
			{
				return;
			}
			string fvxFile = Directory.EnumerateFiles(_seqDir, "*AcquisitionManagerAgent.fvx").FirstOrDefault();
			if (fvxFile == null)
			{
				_dicFrameIdDistance = null;
				return;
			}

			if (!Directory.Exists(Path.Combine(_seqDir, "Photo")))
			{
				return;
			}
			string ejxFile = Directory.EnumerateFiles(Path.Combine(_seqDir, "Photo"), "*.PhotoAgent.ejx").FirstOrDefault();
			if (ejxFile == null)
			{
				return;
			}
			_dicFrameIdDistance = new SortedDictionary<ulong, long>();
			using (var reader = XmlReader.Create(new StreamReader((ejxFile))))
			{
				ulong? frameId = null;
				long? distance = null;
				while (reader.Read())
				{

					if (reader.Name == "FrameId")
					{
						frameId = Convert.ToUInt64(reader.ReadInnerXml());
					}
					else if (reader.Name == "Progress")
					{
						distance = Convert.ToInt64(reader.ReadInnerXml());
					}
					if (frameId.HasValue && distance.HasValue)
					{
						_dicFrameIdDistance.Add(frameId.Value, distance.Value);
						frameId = null;
						distance = null;
					}

				}
			}

		}

		private void WithoutDiagnosticTimerObserver()
		{
			_currentObserver = Observable.Timer(DateTimeOffset.Now, TimeSpan.FromMilliseconds(1000 / this.SimulatorFrequencyInMetersPerSecond))
							   .Subscribe((x) =>
							   {
								   _distanceDataSource.OnNext(new DistanceData { AbsoluteDistance = _absoluteDistance++, ReferenceEncoderNumber = 1, AbsoluteLeftPulseCount = _absoluteDistance * PulsePerMeter, AbsoluteRightPulseCount = _absoluteDistance * PulsePerMeter });
							   }
						   );
			RegisterObserver(_currentObserver);
		}

		protected override Task<IObservable<DistanceData>> InitializeCore()
		{
			return Task.Run(
				() =>
				{
					WithoutDiagnosticTimerObserver();

					return _distanceDataSource.Publish().RefCount();
				});
		}

		protected override void DisposeCore(bool disposing)
		{
			if (_currentObserver != null)
			{
				_currentObserver.Dispose();
				_currentObserver = null;
			}
			base.DisposeCore(disposing);

		}
	}
}