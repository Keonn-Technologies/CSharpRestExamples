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


namespace TestWriteEPC
{
	public class TestWriteEPC {
	    private bool debug;
	    private Device device;
	    private RESTUtil util;
	    private HexUtil utilities;
	    private String address;
	
	    public TestWriteEPC(String address, bool debug)
	    {
	        this.address = address;
	        this.debug = debug;
	        this.util = new RESTUtil(address, debug);
	        this.utilities = new HexUtil();
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
	
	        [Option('d', "debug", Required = false, DefaultValue = false,
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
	
	    static void Main(string[] args)
	    {
	    	
	    	var options = new Options();
	        String address = null;
	        bool debug = false;
	        if (CommandLine.Parser.Default.ParseArguments(args, options))
	        {
	            address = options.IPaddress;
	            debug = options.Debug;
	            Console.WriteLine("Parsed Arguments:");
	            Console.WriteLine("\tAddress:\t" + address);
	            Console.WriteLine("\tDebug:\t" + debug);
	            
	            TestWriteEPC app = new TestWriteEPC(address, false);
	    		app.run();
	        } else {
	        	Console.WriteLine(options.GetUsage());
	        	Console.ReadLine();
	        }
	    }
	
	    public void run()
	    {
	    	this.readAndwriteEPC();
	    	this.testWriteEPC();
	    	this.testWriteEPC_CommissionTagOp();
			this.testKillTag();
	    }
        
        public void readAndwriteEPC()
        {            
            //Connect to the device
            this.util.connect(this.device, false);

            String readMode = this.util.getReadMode(this.device, false);
            if (readMode != "AUTONOMOUS")
            {
                this.util.setDeviceMode(this.device, "Autonomous", false);
            }

            while (true)
            {
                //Make an inventory and collect all the epcs
                this.util.startStopDevice(this.device, true, false);
                Thread.Sleep(2000);
                List<string> lepcs = util.getSequentialInventory(this.device, true, false);
                utilities.printList(lepcs);
                this.util.startStopDevice(this.device, false, false);

                //It will change the first bit of every epc
                for (int i = 0; i < lepcs.Count(); i++)
                {
                    string binEPC =  this.utilities.HexStringToBinary(lepcs[i]);
                    string newbinEPC = string.Empty;
                    if (binEPC[0] == '0')
                        newbinEPC = '1' + binEPC.Substring(1);
                    else if (binEPC[0] == '1')
                        newbinEPC = '0' + binEPC.Substring(1);

                    string newEPC = this.utilities.BinaryStringToHex(newbinEPC);

                    Stopwatch stopwatch2 = new Stopwatch();
                    stopwatch2.Start();
                    this.util.CommissionTagOp(this.device, lepcs[i], newEPC, "", "", "", 1, true);

                    Console.WriteLine("Writing an EPC (" + lepcs[i] + ") last: " + stopwatch2.ElapsedMilliseconds.ToString() + " [ms]");
                }

                Thread.Sleep(10000);
            }
            
        }

        public void testWriteEPC_CommissionTagOp()
        {

            this.util.connect(this.device, false);

            String oldEPC = "e2006b0600000000000000f8";
            String newEPC = "e2006b0600000000000000f7";
            String auxEPC;
            String accessPwd = "";
			String newAccessPwd = "";
			String newKillPwd = "";
			int antenna = 1;

            for (int i = 0; i < 20; i++)
            {
                Stopwatch stopwatch2 = new Stopwatch();
                stopwatch2.Start();
				this.util.CommissionTagOp(this.device, oldEPC, newEPC, accessPwd, newAccessPwd, newKillPwd, antenna, false);

                Console.WriteLine("Writing an EPC lasted: " + stopwatch2.ElapsedMilliseconds.ToString() + " [ms]");
                Thread.Sleep(200);
                auxEPC = oldEPC;
                oldEPC = newEPC;
                newEPC = auxEPC;
            }
        }

		public void testKillTag(){

			this.util.connect(this.device, false);

			String epc = "e2006b0600000000000000f8";
			String killPwd = "12345678";
			int antenna = 1;
			try {
				this.util.killTagOp(this.device, killPwd, epc, antenna, true);
			} catch (System.SystemException ex) {
				Console.WriteLine ("Exception: " + ex.StackTrace.ToString ());
			}
		}

        public void testWriteEPC()
        {
            this.util.connect(this.device, false);

            String oldEPC = "30073035303130343030e208";
            String newEPC = "30073035303130343030e209";
            string auxEPC;

            for (int i = 0; i < 20; i++)
            {
                Stopwatch stopwatch2 = new Stopwatch();
                stopwatch2.Start();
                this.util.WriteDataOp(this.device, newEPC, "EPC", 2, oldEPC, "EPC", "", 1, false);

                Console.WriteLine("Writing an EPC last: " + stopwatch2.ElapsedMilliseconds.ToString() + " [ms]");
                Thread.Sleep(500);
                auxEPC = oldEPC;
                oldEPC = newEPC;
                newEPC = auxEPC;
            }
        }
    }
}
