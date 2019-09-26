using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using Util;
using CommandLine;
using CommandLine.Text;

namespace SystemConfig
{
	class SystemConfiguration
	{
		private String address;
		private bool debug;
		private Device device;
		private RESTUtil util;

		private String newDate;
		private String newTimeZone;

		public SystemConfiguration(String address, bool debug, String newdate, String newtimezone) {
			this.address = address;
			this.debug = debug;
			this.newDate = newdate;
			this.newTimeZone = newtimezone;
		}

		public void init() {

			this.util = new RESTUtil(address, debug);
			this.device = this.util.parseDevice(false);
		}

		class Options
		{

			[Option('a', "address", Required = true,
				HelpText = "IP address of the device.")]
			public string IPaddress
			{
				get;
				set;
			}

			[Option('d', "newDate", Required = true,
				HelpText = "New Date in format MM/DD/YYYY.")]
			public string NewDate
			{
				get;
				set;
			}

			[Option('t', "newTime", Required = true,
				HelpText = "New Time in format HH:MM:SS.")]
			public string NewTime
			{
				get;
				set;
			}

			[Option('z', "newTimeZone", Required = true,
				HelpText = "A timezone from the list of possible timezones.")]
			public string NewTimeZone
			{
				get;
				set;
			}

			[Option('x', "debug", Required = false, DefaultValue = false,
				HelpText = "Prints the debug messages to standard output.")]
			public bool Debug
			{
				get;
				set;
			}

			[ParserState]
			public IParserState LastParserState
			{
				get;
				set;
			}

			[HelpOption]
			public string GetUsage()
			{
				return HelpText.AutoBuild(this,
					(HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
			}
		}

		public static void Main (string[] args)
		{
			var options = new Options();
			String address = null;
			bool debug = false;
			String newdate = null;
			String newtimezone = null;

			if (CommandLine.Parser.Default.ParseArguments(args, options))
			{
				address = options.IPaddress;
				debug = options.Debug;
				newdate = options.NewDate + " " + options.NewTime;
				newtimezone = options.NewTimeZone;

				Console.WriteLine("Parsed Arguments:");
				Console.WriteLine("\tAddress:\t" + address);
				Console.WriteLine("\tDebug:\t" + debug);
				Console.WriteLine("\tNew Date:\t" + newdate);
				Console.WriteLine("\tNew TimeZone:\t" + newtimezone);

				SystemConfiguration app = new SystemConfiguration(address, false, newdate, newtimezone);
				app.run();
			} else {
				Console.WriteLine(options.GetUsage());
				Console.ReadLine();
			}
		}

		public void run() {

			this.init ();

			if (this.changeDate ()) {
				Console.WriteLine ("Date Time changed to {0}", this.newDate);
				if (this.changeTimeZone ()) {
					Console.WriteLine ("TimeZone changed to {0}", this.newTimeZone);
				} else {
					Console.WriteLine ("TimeZone not changed, stopping the process!");
				}
			} else {
				Console.WriteLine ("Date not changed, stopping the process!");
			}
			this.shutdown ();
		}

		private bool changeDate() {
			return this.util.changeDate (this.device, this.newDate, false);
		}

		private bool changeTimeZone() {
			List<String> tzs = this.util.getTimeZones(this.device, false);
			if (tzs.Contains (this.newTimeZone)) {
				return this.util.changeTimeZone (this.device, this.newTimeZone, false);
			} else {
				Console.WriteLine ("The Timezone {0} does not exist", this.newTimeZone);
				return false;
			}
		}

		private void shutdown() {
			try 
			{
				util.startStopDevice(device,false, false);
			} 
			catch (Exception e) 
			{
				Console.WriteLine(e.StackTrace);
			}
		}
	}
}
