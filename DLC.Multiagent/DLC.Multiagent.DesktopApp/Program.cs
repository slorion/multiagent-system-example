using DLC.Framework.UI;
using DLC.Multiagent.DesktopApp.UI;
using Mono.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLC.Multiagent.DesktopApp
{
	static class Program
	{
		private const string ServiceName = "DLC.Multiagent.Service";

		enum Action
		{
			Run,
			Service,
			Install,
			Uninstall,
			Start,
			Stop
		}

		[STAThread]
		static void Main(string[] args)
		{
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException, true);
			Application.ThreadException += (s, e) => ErrorHandler.LogAndShowError(e.Exception);
			AppDomain.CurrentDomain.UnhandledException += (s, e) => ErrorHandler.LogAndShowError((Exception) e.ExceptionObject);
			TaskScheduler.UnobservedTaskException += (s, e) => { e.SetObserved(); ErrorHandler.LogAndShowError(e.Exception); };

			Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			Action action = Action.Run;
			string config = null;
			string username = null;
			string password = null;

			var options = new OptionSet() {
				"Usage: DLC.Multiagent.DesktopApp.exe -<action>",
				"Actions disponibles:",
				"run [-config=<configuration file path>] : (action par défaut) exécute le Multiagent avec le fichier de configuration si fourni ou sinon .\\DLC.Multiagent.DesktopApp.conf",
				"install [-config=<configuration file path>] [-user=<service username>] [-password=<password>] : installe le service Windows avec les paramètres fournis",
				"uninstall : désinstalle le service Windows",
				"start : démarre le service Windows",
				"stop : arrête le service Windows",
				{ string.Join("|", Enum.GetNames(typeof(Action)).Concat(Enum.GetNames(typeof(Action)).Select(name => name.ToLowerInvariant()))), arg => action = (Action) Enum.Parse(typeof(Action), arg, true)},
				{ "config=", v => config = v },
				{ "user=", v => username = v },
				{ "password=", v => password = v }
			};

			options.Parse(args);

			if (!string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
				throw new InvalidOperationException("Si un nom d'utilisateur est fourni, un mot de passe doit également être fourni.");

			switch (action)
			{
				case Action.Run:
					TelerikHelper.InitializeLocalizationProviders();
					Application.EnableVisualStyles();
					Application.Run(new MultiagentUI(config));
					break;

				case Action.Service:
					ServiceBase.Run(new AgentBrokerService(ServiceName, config));
					break;

				case Action.Install:
					string codeBase = Assembly.GetExecutingAssembly().CodeBase;
					UriBuilder uri = new UriBuilder(codeBase);
					string binPath = Path.GetFullPath(Uri.UnescapeDataString(uri.Path));

					RunSC("create", string.Format(@"DisplayName= ""{0}"" start= auto binpath= ""{1} -service -config={2}"" {3}", ServiceName, binPath, config, string.IsNullOrEmpty(username) ? null : string.Format(@"obj= ""{0}"" password= ""{1}""", username, password)));
					RunSC("failure", "reset= 30 actions= restart/5000");
					RunSC("start");
					break;

				case Action.Uninstall:
					RunSC("stop", checkExitCode: false);
					RunSC("delete");
					break;

				case Action.Start:
					RunSC("start");
					break;

				case Action.Stop:
					RunSC("stop");
					break;
			}
		}

		private static void RunSC(string command, string args = null, bool checkExitCode = true)
		{
			if (string.IsNullOrEmpty(command)) throw new ArgumentNullException("command");

			// le verbe "runas" fonctionne seulement si UseShellExecute = true, 
			// mais la redirection de la console ne fonctionne que si UseShellExecute = false,
			// dans le cas présent, l'obtention du message d'erreur a été privilégiée
			// voir https://stackoverflow.com/questions/18660014/redirect-standard-output-and-prompt-for-uac-with-processstartinfo
			var process = new Process {
				StartInfo = new ProcessStartInfo {
					//Verb = "runas", 
					FileName = "sc.exe",
					Arguments = string.Format("{0} {1} {2}", command, ServiceName, args),
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardError = true,
					RedirectStandardOutput = true
				}};

			string output = null;
			process.ErrorDataReceived += (s, e) => output += e.Data;
			process.OutputDataReceived += (s, e) => output += e.Data;

			process.Start();
			process.BeginErrorReadLine();
			process.BeginOutputReadLine();
			
			process.WaitForExit();
			if (checkExitCode && process.ExitCode != 0)
				throw new InvalidOperationException(output);
		}
	}
}