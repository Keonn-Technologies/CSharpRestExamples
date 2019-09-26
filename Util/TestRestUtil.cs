using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;

namespace Util
{
    class TestRestUtil
    {
        private enum KEONN_FAMILY
        {
            AdvanReader, AdvanPay
        }
        public bool debug;
        public String call;
        public RESTUtil util;
        private HashSet<Device> devices;
        private string address;

        public TestRestUtil(String address, bool debug, String call)
        {
            this.debug = debug;
            this.address = address;
            util = new RESTUtil(address, debug);
            this.call = call;
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

            [Option('c', "call", Required = false, DefaultValue = "none",
              HelpText = "The REST call to execute.")]
            public string Call
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
            String call = null;
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                address = options.IPaddress;
                debug = options.Debug;
                call = options.Call;
            }

            if(call.Equals("none"))
            	call = null;
            TestRestUtil app = new TestRestUtil(address, false, call);
            app.run();
        }

        public void run()
        {
            /**
		     * This application is meant to test each function of the RESTUtil java class
		     */

            /**
			 * PARSE DEVICES
			 */
            try
            {
                devices = util.parseDevices(false);
                Console.WriteLine("[INFO] The function 'parseDevices' was successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("[INFO] The function 'parseDevices' was a failure");
            }

            foreach (Device d in devices)
            {
                if(KEONN_FAMILY.AdvanReader.ToString().Equals(d.family)
                    || KEONN_FAMILY.AdvanPay.ToString().Equals(d.family))
                {
                    /**
				     * CONNECT
				     */
				    if(this.call != null){
					    if(this.call.Equals("connect")){
						    this.connect(d);
					    }
				    }else{
					    this.connect(d);
				    }

                    /**
				     * PRINT DEVICE
				     */
                    if (this.call != null)
                    {
                        if (this.call.Equals("printDevice"))
                        {
                            this.printDevice(d);
                        }
                    }
                    else
                    {
                        this.printDevice(d);
                    }

                    /**
                     * GET READ MODE
                     */
                    if (this.call != null)
                    {
                        if (this.call.Equals("getReadMode"))
                        {
                            this.getReadMode(d);
                        }
                    }
                    else
                    {
                        this.getReadMode(d);
                    }

                    /**
                     * START & STOP DEVICE
                     */
                    if (this.call != null)
                    {
                        if (this.call.Equals("startStopDevice"))
                        {
                            this.startStopDevice(d);
                        }
                    }
                    else
                    {
                        this.startStopDevice(d);
                    }

                    /**
                     * PARSE DEVICE
                     */
                    if (this.call != null)
                    {
                        if (this.call.Equals("parseDevice"))
                        {
                            this.parseDevice();
                        }
                    }
                    else
                    {
                        this.parseDevice();
                    }

                    /**
                     * GET INVENTORY
                     */
                    if (this.call != null)
                    {
                        if (this.call.Equals("getInventory"))
                        {
                            this.getInventory(d);
                        }
                    }
                    else
                    {
                        this.getInventory(d);
                    }

                    /**
                     * GET LOCATION
                     */
                    if (this.call != null)
                    {
                        if (this.call.Equals("getLocation"))
                        {
                            this.getLocation(d);
                        }
                    }
                    else
                    {
                        this.getLocation(d);
                    }

                    /**
                     * SET DEVICE MODE
                     */
                    if (this.call != null)
                    {
                        if (this.call.Equals("setDeviceMode"))
                        {
                            this.setDeviceMode(d);
                        }
                    }
                    else
                    {
                        this.setDeviceMode(d);
                    }

                    /**
                     * SET READ MODE
                     */
                    if (this.call != null)
                    {
                        if (this.call.Equals("setReadMode"))
                        {
                            this.setReadMode(d);
                        }
                    }
                    else
                    {
                        this.setReadMode(d);
                    }

                    /**
                     * SET POWER
                     */
                    if (this.call != null)
                    {
                        if (this.call.Equals("setPower"))
                        {
                            this.setPower(d);
                        }
                    }
                    else
                    {
                        this.setPower(d);
                    }

                    /**
                     * SET SENSITIVITY
                     */
                    if (this.call != null)
                    {
                        if (this.call.Equals("setSensitivity"))
                        {
                            this.setSensitivity(d);
                        }
                    }
                    else
                    {
                        this.setSensitivity(d);
                    }

                    /**
                     * CONFIGURE ANTENNA
                     */
                    if (this.call != null)
                    {
                        if (this.call.Equals("configureAntenna"))
                        {
                            this.configureAntenna(d);
                        }
                    }
                    else
                    {
                        this.configureAntenna(d);
                    }

                    /**
                     * SET ANTENNA CONFIGURATION
                     */
                    if (this.call != null)
                    {
                        if (this.call.Equals("setAntennaConfiguration"))
                        {
                            this.setAntennaConfiguration(d);
                        }
                    }
                    else
                    {
                        this.setAntennaConfiguration(d);
                    }

                    /**
                     * PROCESS TCP DATA
                     */
                    if (this.call != null)
                    {
                        if (this.call.Equals("processTCPData"))
                        {
                            this.processTCPData(d);
                        }
                    }
                    else
                    {
                        this.processTCPData(d);
                    }

                    /**
                     * PARSE ANTENNAS
                     */
                    if (this.call != null)
                    {
                        if (this.call.Equals("parseAntennas"))
                        {
                            this.parseAntennas(d);
                        }
                    }
                    else
                    {
                        this.parseAntennas(d);
                    }

                    /**
                     * SET SEQUENTIAL READ TIME
                     */
                    if (this.call != null)
                    {
                        if (this.call.Equals("setSequentialReadTime"))
                        {
                            this.setSequentialReadTime(d);
                        }
                    }
                    else
                    {
                        this.setSequentialReadTime(d);
                    }

                    /**
                     * COMMISSION TAG OP
                     */
                    if (this.call != null)
                    {
                        if (this.call.Equals("commissionTagOp"))
                        {
                            this.commissionTagOp(d);
                        }
                    }
                    else
                    {
                        this.commissionTagOp(d);
                    }

                    /**
                     * WRITE DATA OP
                     */
                    if (this.call != null)
                    {
                        if (this.call.Equals("writeDataOp"))
                        {
                            this.writeDataOp(d);
                        }
                    }
                    else
                    {
                        this.writeDataOp(d);
                    }

                    /**
                     * READ DATA OP
                     */
                    if (this.call != null)
                    {
                        if (this.call.Equals("readDataOp"))
                        {
                            this.readDataOp(d);
                        }
                    }
                    else
                    {
                        this.readDataOp(d);
                    }

                    /**
                     * LOCK OP
                     */
                    if (this.call != null)
                    {
                        if (this.call.Equals("lockOp"))
                        {
                            this.lockOp(d);
                        }
                    }
                    else
                    {
                        this.lockOp(d);
                    }

                    /**
                     * NXP EAS CHECK
                     */
                    if (this.call != null)
                    {
                        if (this.call.Equals("NXP_EASCheck"))
                        {
                            this.NXP_EASCheck(d);
                        }
                    }
                    else
                    {
                        this.NXP_EASCheck(d);
                    }
				
					/**
					 * SET GPO
					 */
					if(this.call != null){
						if(this.call.Equals("setGPO")){
							this.setGPO(d);
						}
					}else{
						this.setGPO(d);
					}
				
					/**
					 * GET GPIO ALL
					 */
					if(this.call != null){
						if(this.call.Equals("getGPIOAll")){
							this.getGPIOAll(d);
						}
					}else{
						this.getGPIOAll(d);
					}
					
					/**
					 * SPEAKER
					 */
					if(this.call != null){
						if(this.call.Equals("speaker")){
							this.speaker(d);
						}
					}else{
						this.speaker(d);
					}
					
					/**
					 * BUZZER
					 */
					if(this.call != null){
						if(this.call.Equals("buzzer")){
							this.buzzer(d);
						}
					}else{
						this.buzzer(d);
					}
					
					/**
					 * SET EPCGEN2 FILTER
					 */
					if(this.call != null){
						if(this.call.Equals("setEPCGen2Filter")){
							this.setEPCGen2Filter(d);
						}
					}else{
						this.setEPCGen2Filter(d);
					}
	
					/**
					 * SET ACTUATOR
					 */
					if(this.call != null){
						if(this.call.Equals("setActuator")){
							this.setActuator(d);
						}
					}else{
						this.setActuator(d);
					}
	
					/**
					 * ERASE ACTUATORS
					 */
					if(this.call != null){
						if(this.call.Equals("eraseActuators")){
							this.eraseActuators(d);
						}
					}else{
						this.eraseActuators(d);
					}
	
					/**
					 * SET SQL PARAMETERS
					 */
					if(this.call != null){
						if(this.call.Equals("setSQLParameters")){
							this.setSQLParameters(d);
						}
					}else{
						this.setSQLParameters(d);
					}
					
					/**
					 * PROCESS GPI DATA
					 */
					if(this.call != null){
						if(this.call.Equals("processGPIdata")){
							this.processGPIdata(d);
						}
					}else{
						this.processGPIdata(d);
					}
					
					/**
					 * PARSE XML
					 */
					if(this.call != null){
						if(this.call.Equals("parseXML")){
							this.parseXML(d);
						}
					}else{
						this.parseXML(d);
					}
					
					/**
					 * PROCESS ALARM MESSAGES
					 */
					if(this.call != null){
						if(this.call.Equals("processAlarmMessages")){
							this.processAlarmMessages(d);
						}
					}else{
						this.processAlarmMessages(d);
					}
					Console.WriteLine("Press a key to finish...");
					Console.ReadLine();
                }
            }

        }
        
        public void connect(Device d){
		    try 
            {
			    this.util.connect(d, false);
			    Console.WriteLine("[INFO] The function 'connect' was successful");
		    } 
            catch (Exception ex) 
            {
			    Console.WriteLine("[ERROR] The function 'connect' has failed");
                Console.WriteLine(ex.StackTrace);
		    }
	    }

        public void printDevice(Device d){
		    try 
            {
			    this.util.printDevice(d, 1, false);
			    Console.WriteLine("[INFO] The function 'printDevice' was successful");
            }
            catch (Exception ex)
            {
			    Console.WriteLine("[ERROR] The function 'printDevice' has failed");
			    Console.WriteLine(ex.StackTrace);
		    }
	    }
	
	    public void getReadMode(Device d){
		    try 
            {
			    String readMode = this.util.getReadMode(d, false);
			    Console.WriteLine("[INFO] The read mode is " + readMode);
			    Console.WriteLine("[INFO] The function 'getReadMode' was successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] The function 'getReadMode' has failed");
                Console.WriteLine(ex.StackTrace);
		    }
	    }
	
	    public void startStopDevice(Device d){
		    try 
            {
			    this.util.startStopDevice(d, true, false);
			    this.util.startStopDevice(d, false, false);
			    Console.WriteLine("[INFO] The function 'startStopDevice' was successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] The function 'startStopDevice' has failed");
                Console.WriteLine(ex.StackTrace);
		    }
	    }
	
	    public void parseDevice(){
		    try 
            {
			    Device device = this.util.parseDevice(false);
			    Console.WriteLine("[INFO] Device id: " + device.id + " Device family: " + device.family);
			    Console.WriteLine("[INFO] The function 'parseDevice' was successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] The function 'parseDevice' has failed");
                Console.WriteLine(ex.StackTrace);
		    }
	    }
	
	    public void getInventory(Device d){
		    try {
			    this.util.setDeviceMode(d, "Sequential", false);
			    this.util.startStopDevice(d, true, false);
			    HashSet<TagData> tagDatas = this.util.getInventory(d, false);
			    this.util.startStopDevice(d, false, false);
			    /**
			     * PRINT INVENTORY
			     *  - getLocation
			     *  - printInventory
			     */
			    this.util.printInventory(d, tagDatas);
			    Console.WriteLine("[INFO] The function 'getInventory' was successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] The function 'getInventory' has failed");
                Console.WriteLine(ex.StackTrace);
		    }
	    }
	
	    public void getLocation(Device d){
		    try {
			    this.util.setDeviceMode(d, "Sequential", false);
			    this.util.startStopDevice(d, true, false);
			    HashSet<TagData> tagDatas = this.util.getLocation(d, false);
			    this.util.startStopDevice(d, false, false);
			
			    /**
			     * PRINT INVENTORY
			     *  - getLocation
			     *  - printInventory
			     */
			    this.util.printInventory(d, tagDatas);
			    Console.WriteLine("[INFO] The function 'getLocation' was successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] The function 'getLocation' has failed");
                Console.WriteLine(ex.StackTrace);
		    }
	    }
	
	    public void setDeviceMode(Device d){
		    try 
            {
			    this.util.setDeviceMode(d, "Sequential", false);
			    Console.WriteLine("[INFO] The function 'setDeviceMode' was successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] The function 'setDeviceMode' has failed");
                Console.WriteLine(ex.StackTrace);
		    }
	    }
	
	    public void setReadMode(Device d){
		    try 
            {
			    this.util.setDeviceMode(d, "Alarm mode", false);
			    this.util.setReadMode(d, "EPC_EAS_DISABLE", false);
			    Console.WriteLine("[INFO] The function 'setReadMode' was successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] The function 'setReadMode' has failed");
                Console.WriteLine(ex.StackTrace);
		    }
	    }
	
	    public void setPower(Device d){
		    try 
            {
			    this.util.setPower(d, 20, true);
			    Console.WriteLine("[INFO] The function 'setPower' was successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] The function 'setPower' has failed");
                Console.WriteLine(ex.StackTrace);
		    }
	    }
	
	    public void setSensitivity(Device d){
		    try 
            {
			    this.util.setSensitivity(d, -78, false);
			    Console.WriteLine("[INFO] The function 'setSensitivity' was successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] The function 'setSensitivity' has failed");
                Console.WriteLine(ex.StackTrace);
		    }
	    }
	
	    public void configureAntenna(Device d){
		    try
            {
			    this.util.configureAntenna(d, 1, 0, 0, -1, "antenna1", 1, 0, 0, false);
			    this.util.configureAntenna(d, 1, 0, 0, -1, "antenna1", 1, 0, 0, 25, -60, false);
			    Console.WriteLine("[INFO] The function 'configureAntenna' was successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] The function 'configureAntenna' has failed");
                Console.WriteLine(ex.StackTrace);
		    }
	    }
	
        public void setAntennaConfiguration(Device d){
	        try 
            {
		        List<Antenna> lantennas = new List<Antenna>();
		        lantennas.Add(new Antenna(1, 0, 0, -1, "antenna1.0", 5, 3, -1));
		        this.util.setAntennaConfiguration(d, lantennas, false);
		        Console.WriteLine("[INFO] The function 'setAntennaConfiguration' was successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] The function 'setAntennaConfiguration' has failed");
                Console.WriteLine(ex.StackTrace);
	        }
        }
        
        public void processTCPData(Device d)
        {
            try
            {
                this.util.setDeviceMode(d, "Autonomous", false);
                util.startStopDevice(d, true, false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
		    ConcurrentQueue<String> queue = new ConcurrentQueue<String>();
		    List<String> tagDataList = new List<String>(); 
		    Console.WriteLine("Device["+d.id+"] Reading the 3177 port... ");
		    TCPReader tcpReader = new TCPReader(this.address, queue, this.util);
            Thread tcpReaderThread = new Thread(new ThreadStart(tcpReader.run));
            tcpReaderThread.Start();
		    Console.WriteLine("Done.");
		
		    Stopwatch stopwatch2 = new Stopwatch();
            stopwatch2.Start();

            long time = 10000;
            /*
             * This loop will wait until the queue of events has something, 
             * it will deque the elements, parse them, and print only the 
             * gpi events
             */
            while (stopwatch2.ElapsedMilliseconds < time)
            {
                while (queue.IsEmpty)
                {
                    try
                    {
                        Thread.Sleep(20);
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine("Thread sleeping failure: " + exc.Source);
                    }
                }

                if (!queue.IsEmpty)
                {
                    string tag;
                    while (queue.TryDequeue(out tag))
                    {
                        string dTag = tag;
                        this.util.processTCPdata(dTag, tagDataList);
					
					    foreach (String tagData in tagDataList)
						    Console.WriteLine("epc: " + tagData);
					
					    tagDataList.Clear();
                    }
                }
            }
            tcpReader.Shutdown();
		
		    try 
            {
			    util.startStopDevice(d,false, false);
		    } 
            catch (Exception e) 
            {
			    Console.WriteLine(e.StackTrace);
		    }
		    tcpReader.Shutdown();
	    }
	
	    public void parseAntennas(Device d){
		    try 
            {
			    List<Antenna> antennas = this.util.parseAntennas(d, false);
			    this.util.printAntennas(antennas);
			    Console.WriteLine("[INFO] The function 'parseAntennas' was successful");
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] The function 'parseAntennas' has failed");
                Console.WriteLine(e.StackTrace);
		    }
	    }
	
	    public void setSequentialReadTime(Device d){
		    try 
            {
			    this.util.setDeviceMode(d, "Sequential", false);
			    if(d.advanNetVersion.Equals("2.1.x"))
				    this.util.setSequentialReadTime(d, 300, RESTUtil.AdvanNetVersion.a21.ToString(), false);
			    else if(d.advanNetVersion.Equals("2.3.x"))
				    this.util.setSequentialReadTime(d, 300, RESTUtil.AdvanNetVersion.a23.ToString(), false);
			
			    Console.WriteLine("[INFO] The function 'setSequentialReadTime' was successful");
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] The function 'setSequentialReadTime' has failed");
                Console.WriteLine(e.StackTrace);
		    }
        }

        public void commissionTagOp(Device d){
		    try 
            {
			    String oldEPC = "6666fbbbdc2b0862b81191c1";
			    String newEPC = "6666fbbbdc2b0862b81191ce";
			    String accessPwd = "";
			    String newAccessPwd = "";
			    String newKillPwd = "";
			    int antenna = 1;
			    Console.WriteLine("[INFO] A tag without password and with the EPC: " + oldEPC);
			    Console.WriteLine("[INFO] Press enter when the tag is near the antenna");
			    Console.Read();
                this.util.CommissionTagOp(d, oldEPC, newEPC, 
					    accessPwd, newAccessPwd, newKillPwd, antenna, false);
			    Console.WriteLine("[INFO] The function 'commissionTagOp' was successful");
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] The function 'commissionTagOp' has failed");
                Console.WriteLine(e.StackTrace);
		    }
	    }

        public void writeDataOp(Device d){
		    try 
            {
			    String data = "6666fbbbdc2b0862b81191c1";
			    String bank = "EPC";
			    int offset = 2;
			    String mask = "6666fbbbdc2b0862b81191ce";
			    String filterBank = "EPC";
			    String password = "";
			    int antenna = 1;
			    Console.WriteLine("[INFO] A tag without password and with the EPC: " + mask);
			    Console.WriteLine("[INFO] Press enter when the tag is near the antenna");
			    Console.Read();
			    this.util.WriteDataOp(d, data, bank, offset, mask, filterBank, password, antenna, false);
			    Console.WriteLine("[INFO] The function 'writeDataOp' was successful");
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] The function 'writeDataOp' has failed");
                Console.WriteLine(e.StackTrace);
		    }
	    }

        public void readDataOp(Device d){
		    try {
			    String bank = "EPC";
			    int offset = 2;
			    int length = 2;
			    String filterBank = "EPC";
			    String filterMask = "6666fbbbdc2b0862b81191c1";
			    String password = "";
			    int antenna = 1;
			    Console.WriteLine("[INFO] A tag without password and with the EPC: "
							    + filterMask);
			    Console.WriteLine("[INFO] Press enter when the tag is near the antenna");
			    Console.Read();
			    String data = this.util.readDataOp(d, bank, offset, length, filterBank,
					    filterMask, password, antenna, false);
			    Console.WriteLine("[INFO] The data read is: " + data);
			    Console.WriteLine("[INFO] The function 'readDataOp' was successful");
		    }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] The function 'readDataOp' has failed");
                Console.WriteLine(e.StackTrace);
		    }
	    }
        
        public void lockOp(Device d){
		    try 
            {
			    String actualPwd = "0x12345678";
			    String[] lockActions = {"EPC_LOCK"};
			    String filterBank = "EPC";
			    String filterMask = "300730353031303430300022";
			    int antenna = 1;
			    Console.WriteLine("[INFO] A tag with password and with the EPC: " + filterMask);
			    Console.WriteLine("[INFO] Press enter when the tag is near the antenna");
                Console.Read();
			    this.util.lockOp(d, actualPwd, lockActions, filterBank, filterMask, antenna, false);
			    Console.WriteLine("[INFO] The function 'lockOp' was successful");
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] The function 'lockOp' has failed");
                Console.WriteLine(e.StackTrace);
		    }
	    }

        public void NXP_EASCheck(Device d){
		    try 
            {
			    String mask = "";
			    int antenna = 1;
			    Console.WriteLine("[INFO] A tag with password and with the EPC: " + mask);
			    Console.WriteLine("[INFO] Press enter when the tag is near the antenna");
			    Console.Read();
			    String nxp_state = this.util.NXP_EASCheck(d, mask, antenna, false);
			    Console.WriteLine("[INFO] The NXP EAS state is " + nxp_state);
			    Console.WriteLine("[INFO] The function 'NXP_EASCheck' was successful");
            }
            catch (Exception e)
            {
			    Console.WriteLine("[ERROR] The function 'NXP_EASCheck' has failed");
                Console.WriteLine(e.StackTrace);
		    }
	    }
	
		public void setGPO(Device d){
			try {
				int gpo = 1;
				bool state = true;
				this.util.setGPO(d, gpo, state, false);
				Console.WriteLine("[INFO] The function 'setGPO' was successful");
			} catch (Exception e) {
				Console.WriteLine("[ERROR] The function 'setGPO' has failed");
                Console.WriteLine(e.StackTrace);
			}
		}
	
		public void getGPIOAll(Device d){
			try {
				String gpioAll = this.util.getGPIOAll(d, false);
				Console.WriteLine("[INFO] GPIO: " + gpioAll);
				Console.WriteLine("[INFO] The function 'getGPIOAll' was successful");
			} catch (Exception e) {
				Console.WriteLine("[ERROR] The function 'getGPIOAll' has failed");
                Console.WriteLine(e.StackTrace);
			}
		}
        
        public void speaker(Device d){
			try {
				int frequency = 3000;
				int volume = 5;
				int timeOff = 0;
				int timeOn = 200;
				int totalDuration = 200;
				this.util.speaker(d, frequency, volume, timeOff, timeOn, totalDuration, false);
				Console.WriteLine("[INFO] The function 'speaker' was successful");
			} catch (Exception e) {
				Console.WriteLine("[ERROR] The function 'speaker' has failed");
                Console.WriteLine(e.StackTrace);
			}
		}
		
		public void buzzer(Device d){
			try {
				int timeOff = 0;
				int timeOn = 200;
				int totalDuration = 200;
				this.util.buzzer(d, totalDuration, timeOn, timeOff, false);
				Console.WriteLine("[INFO] The function 'buzzer' was successful");
			} catch (Exception e) {
				Console.WriteLine("[ERROR] The function 'buzzer' has failed");
                Console.WriteLine(e.StackTrace);
			}
		}
		
		public void setEPCGen2Filter(Device d){
			try {
				String className = RESTUtil.ReadModesClassName.READMODE_EPC_EAS_ALARM
						.ToString();
				int maskOffset = 0;
				int maskLength = 0;
				String filterMask = "";
				bool swFilterOnly = false;
				if (d.advanNetVersion.Equals("2.1.x"))
					this.util.setEPCGen2Filter(d, className, maskOffset,
							maskLength, filterMask, swFilterOnly,
							RESTUtil.AdvanNetVersion.a21.ToString(), false);
				else if (d.advanNetVersion.Equals("2.3.x"))
					this.util.setEPCGen2Filter(d, className, maskOffset,
							maskLength, filterMask, swFilterOnly,
							RESTUtil.AdvanNetVersion.a23.ToString(), false);
	
				Console.WriteLine("[INFO] The function 'setEPCGen2Filter' was successful");
			} catch (Exception e) {
				Console.WriteLine("[ERROR] The function 'setEPCGen2Filter' has failed");
                Console.WriteLine(e.StackTrace);
			}
		}
		
		public void setActuator(Device d){
			try {
				this.util.setActuator(d, RESTUtil.EventTypes.TAG_ALARM.ToString(),
						new Buzzer(200, 200, 0), false);
				Console.WriteLine("[INFO] The function 'setActuator' was successful");
			} catch (Exception e) {
				Console.WriteLine("[ERROR] The function 'setActuator' has failed");
                Console.WriteLine(e.StackTrace);
			}
		}
		
		public void eraseActuators(Device d){
			try {
				this.util.eraseActuators(d, false);
				Console.WriteLine("[INFO] The function 'eraseActuators' was successful");
			} catch (Exception e) {
				Console.WriteLine("[ERROR] The function 'eraseActuators' has failed");
                Console.WriteLine(e.StackTrace);
			}
		}
		
		public void setSQLParameters(Device d){
			try {
				String driverClass = "com.mysql.jdbc.Driver";
				String connString = "jdbc:mysql://localhost/db_name";
				String username = "user";
				String password = "pass";
				String queryString = "select paid from sale_info where (epc=${epc} and paid=1)";
				int queryCacheTime = 1500;
				this.util.setDeviceMode(d, "Alarm mode", false);
				this.util.setReadMode(d, "SQL_EAS_ALARM", false);
				this.util.setSQLParameters(d, driverClass, connString,
						username, password, queryString, queryCacheTime, false);
				Console.WriteLine("[INFO] The function 'setSQLParameters' was successful");
			} catch (Exception e) {
				Console.WriteLine("[ERROR] The function 'setSQLParameters' has failed");
                Console.WriteLine(e.StackTrace);
			} finally {
				this.processAlarmMessages(d);
			}
		}
		
		public void processGPIdata(Device d){
			try {
				ConcurrentQueue<String> queue = new ConcurrentQueue<String>();
				Console.WriteLine("Device["+d.id+"] Reading the 3177 port... ");
				TCPReader tcpReader = new TCPReader(this.util.getAddress(), queue, this.util);
	            Thread tcpReaderThread = new Thread(new ThreadStart(tcpReader.run));
	            tcpReaderThread.Start();
				GPIevent readerGPIevent = new GPIevent();
				Console.WriteLine("Done.");
		
			    Stopwatch stopwatch2 = new Stopwatch();
	            stopwatch2.Start();
				
				long inventoryTime = 1000; //Milliseconds
	            /*
	             * This loop will wait until the queue of events has something, 
	             * it will deque the elements, parse them, and print only the 
	             * gpi events
	             */
	            while (stopwatch2.ElapsedMilliseconds < inventoryTime) {
					
            		while (queue.IsEmpty)
	                {
	                    try
	                    {
	                        Thread.Sleep(20);
	                    }
	                    catch (Exception exc)
	                    {
	                        Console.WriteLine("Thread sleeping failure: " + exc.Source);
	                    }
	                }
					
					if(!queue.IsEmpty){
						string tag;
	                    while (queue.TryDequeue(out tag))
	                    {
	                        string dTag = tag;
	                        if(!dTag.Contains("<type>GPI</type>"))
	                        	continue;
	
	                        readerGPIevent = this.util.processGPIdata(dTag);
	
	                        if (readerGPIevent == null)
	                            continue;
	                        Console.WriteLine(readerGPIevent.ToString());
	                    }
					}
				}
				
				this.util.startStopDevice(d,false, false);
				tcpReader.Shutdown();
				Console.WriteLine("[INFO] The function 'processGPIdata' was successful");
			} catch (Exception e) {
				Console.WriteLine("[ERROR] The function 'processGPIdata' has failed");
                Console.WriteLine(e.StackTrace);
			}
		}
		
		public void parseXML(Device d){
        	try {
				ConcurrentQueue<String> queue = new ConcurrentQueue<String>();
				this.util.startStopDevice(d, false, false);
				this.util.setDeviceMode(d, "Autonomous", false);
				this.util.startStopDevice(d, true, false);
				TCPReader tcpReader = new TCPReader(this.util.getAddress(), queue, this.util);
	            Thread tcpReaderThread = new Thread(new ThreadStart(tcpReader.run));
	            tcpReaderThread.Start();
		
			    Stopwatch stopwatch2 = new Stopwatch();
	            stopwatch2.Start();
				
				long inventoryTime = 1000; //Milliseconds
	            /*
	             * This loop will wait until the queue of events has something, 
	             * it will deque the elements, parse them, and print only the 
	             * gpi events
	             */
	            while (stopwatch2.ElapsedMilliseconds < inventoryTime) {
            		while (queue.IsEmpty)
	                {
	                    try
	                    {
	                        Thread.Sleep(20);
	                    }
	                    catch (Exception exc)
	                    {
	                        Console.WriteLine("Thread sleeping failure: " + exc.Source);
	                    }
	                }
					
					if(!queue.IsEmpty){
						string tag;
	                    while (queue.TryDequeue(out tag))
	                    {
	                        string dTag = tag;
	                        Queue<string> parsedQueue = new Queue<string>();
	                        this.util.parseXML(dTag, parsedQueue);
	                        var iterator = parsedQueue.GetEnumerator();
						  	while (iterator.MoveNext())
						  	{
						  		Console.WriteLine("Dequeued item " + iterator.Current);
						  	}
	                    }
					}
				}
				
				this.util.startStopDevice(d,false, false);
				tcpReader.Shutdown();
				Console.WriteLine("[INFO] The function 'parseXML' was successful");
			} catch (Exception e) {
				Console.WriteLine("[ERROR] The function 'parseXML' has failed");
                Console.WriteLine(e.StackTrace);
			}
		}
		
		public void processAlarmMessages(Device d){
			
			try {
				ConcurrentQueue<String> queue = new ConcurrentQueue<String>();
				this.util.setDeviceMode(d, "Autonomous", false);
				this.util.startStopDevice(d, true, false);
				TCPReader tcpReader = new TCPReader(this.util.getAddress(), queue, this.util);
	            Thread tcpReaderThread = new Thread(new ThreadStart(tcpReader.run));
	            tcpReaderThread.Start();
				String epcToAlarm = "6666fbbbdc2b0862b81191c1";
		
			    Stopwatch stopwatch2 = new Stopwatch();
	            stopwatch2.Start();
				
				long inventoryTime = 1000; //Milliseconds
	            /*
	             * This loop will wait until the queue of events has something, 
	             * it will deque the elements, parse them, and print only the 
	             * gpi events
	             */
	            while (stopwatch2.ElapsedMilliseconds < inventoryTime) {
					
            		while (queue.IsEmpty)
	                {
	                    try
	                    {
	                        Thread.Sleep(20);
	                    }
	                    catch (Exception exc)
	                    {
	                        Console.WriteLine("Thread sleeping failure: " + exc.Source);
	                    }
	                }
					
					if(!queue.IsEmpty){
						string tag;
	                    while (queue.TryDequeue(out tag))
	                    {
	                        string dTag = tag;
	                        this.util.processAlarmMessages(d, dTag, epcToAlarm);
	                    }
					}
				}
				
				this.util.startStopDevice(d,false, false);
				tcpReader.Shutdown();
				Console.WriteLine("[INFO] The function 'processAlarmMessages' was successful");
			} catch (Exception e) {
				Console.WriteLine("[ERROR] The function 'processAlarmMessages' has failed");
                Console.WriteLine(e.StackTrace);
			}
		}
    }
}
