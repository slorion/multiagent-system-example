using DLC.Framework.Reactive;
using DLC.Multiagent;
using DLC.Scientific.Acquisition.Agents.BgrDirectionalAgent.Configuration;
using DLC.Scientific.Acquisition.Agents.BgrDirectionalAgent.UI;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.Agents.Model;
using DLC.Scientific.Acquisition.Core.Configuration;
using DLC.Scientific.Core.Geocoding;
using DLC.Scientific.Core.Geocoding.Bgr;
using DLC.Scientific.Core.Journalisation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Agents.BgrDirectionalAgent
{
	public class BgrDirectionalAgent
		: AcquisitionableAgent<BgrProvider, BgrData, BgrAgentConfiguration, AcquisitionModuleConfiguration>, IBgrDirectionalAgent, IVisibleAgent
	{
		private readonly SubjectSlim<LocalisationData> _localisationSubject = new SubjectSlim<LocalisationData>();

		public string ItineraryLogRelativeFilePath { get; private set; }

		public int AutoCorrectDelta { get; private set; }
		public int ManualSearchRadiusInMeters { get; private set; }
		public int AutoSearchRadiusInMeters { get; private set; }
		public int AutoSearchIntervalInMs { get; private set; }

		protected override BgrProvider CreateAndConfigureProvider()
		{
			var provider = (BgrProvider) this.Configuration.Module.Provider;
			provider.LocalisationDataSource = _localisationSubject;

			return provider;
		}

		protected override void ConfigureAgent()
		{
			base.ConfigureAgent();

			this.AutoCorrectDelta = this.Configuration.Agent.AutoCorrectDelta;
			this.ManualSearchRadiusInMeters = this.Configuration.Agent.ManualSearchRadiusInMeters;
			this.AutoSearchRadiusInMeters = this.Configuration.Agent.AutoSearchRadiusInMeters;
			this.AutoSearchIntervalInMs = this.Configuration.Agent.AutoSearchIntervalInMs;

			this.AutoShowUI = this.Configuration.Agent.AutoShowUI;
			this.MainUITypeName = typeof(BgrUI).AssemblyQualifiedName;
			this.MainUIAgentTypeName = typeof(IBgrDirectionalAgent).AssemblyQualifiedName;
			this.AgentUniversalName = "ItineraryAgent";
		}

		protected override IEventJournal CreateEventJournal(InitializeRecordParameter parameters)
		{
			return null;
		}

		protected override void SetupAgentOperationalCommunicationsCore()
		{
			base.SetupAgentOperationalCommunicationsCore();

			TrackDependencyOperationalState<ILocalisationAgent>(false);
		}

		protected override async Task<AcquisitionActionResult> StartCore(StartAcquisitionParameter parameters, AcquisitionActionResult result)
		{
			result = await base.StartCore(parameters, result).ConfigureAwait(false);
			if (!result.IsSuccessful)
				return result;

			this.RegisterObserver(AgentBroker.Instance.ObserveAny<ILocalisationAgent, LocalisationData>("DataSource").Subscribe(data => _localisationSubject.OnNext(data)));

			return result;
		}

		protected override Task<AcquisitionActionResult> InitializeRecordCore(InitializeRecordParameter parameters, AcquisitionActionResult result)
		{
			this.ItineraryLogRelativeFilePath = this.SequenceId + ".iti";

			try { File.Delete(Path.Combine(this.JournalAbsoluteSavePath, this.ItineraryLogRelativeFilePath)); }
			catch { }

			return base.InitializeRecordCore(parameters, result);
		}

		public BgrDataTypes AllowedBgrDataTypes
		{
			get { return this.Provider.AllowedBgrDataTypes; }
			set
			{
				this.Provider.AllowedBgrDataTypes = value;
				AcquisitionConfigurationFactory.Instance.Update(this.ConfigurationFilePath, "Module.AllowedBgrDataTypes", value.ToString());
			}
		}

		public IRtssc GetNextRtsscSameDirection(GeoCoordinate coord)
		{
			return this.Provider.GetNextRtsscSameDirection(coord);
		}

		public GeoCoordinate GeoCodage(IRtssc rtssc)
		{
			return this.Provider.GeoCodage(rtssc);
		}

		public double GetSectionLength(IRtssc rtssc)
		{
			return this.Provider.GetSectionLength(rtssc);
		}

		public IEnumerable<IRtssc> GetRtssFromRoute(string route)
		{
			return this.Provider.GetRtssFromRoute(route);
		}

		public IEnumerable<string> SelectRoutes(GeoCoordinate coord, double searchRadiusInMeters, int? maxRouteNumber)
		{
			return this.Provider.SelectRoutes(coord, searchRadiusInMeters, maxRouteNumber);
		}
	}
}