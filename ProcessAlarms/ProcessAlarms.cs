/*
 * Created by SharpDevelop.
 * User: salmendros
 * Date: 4/6/2016
 * Time: 12:08 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Net.Sockets;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Xml;
using Util;
using CommandLine;
using CommandLine.Text;

namespace ProcessAlarms
{
	class ProcessAlarms
	{
		private bool debug;
        private Device device;
        private RESTUtil util;
        private String address;
        private TCPReader tcpReader;

        public ProcessAlarms(String address, bool debug)
        {        	
            this.address = address;
            this.debug = debug;
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
            
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                address = options.IPaddress;
                Console.WriteLine("Parsed Arguments:");
                Console.WriteLine("\tAddress:\t" + address);
                
                ProcessAlarms app = new ProcessAlarms(address, false);
        		app.run();
            } else {
            	Console.WriteLine(options.GetUsage());
            	Console.ReadLine();
            }
        }

        public void run()
        {            
		    try {
				ConcurrentQueue<String> queue = new ConcurrentQueue<String>();
				this.util.startStopDevice(device, false, false);
                this.util.setDeviceMode(this.device, "EPCBULK-EAS", true);
                util.startStopDevice(device, true, false);
				tcpReader = new TCPReader(this.util.getAddress(), queue, this.util);
	            Thread tcpReaderThread = new Thread(new ThreadStart(tcpReader.run));
	            tcpReaderThread.Start();
	            
	            while ( true ) {
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
						  		//Console.WriteLine("Dequeued item " + iterator.Current);
						  		string msg = iterator.Current;
						  		if( msg.Contains("<deviceEventMessage>") && msg.Contains("TAG_ALARM") ) {
						  			
						  			XmlDocument xml = new XmlDocument();
						  			xml.LoadXml(msg);
						  			Console.WriteLine(xml.OuterXml);
						  			
						  		}
						  	}
	                    }
					}
				}
	            
			} catch (Exception e) {
				Console.WriteLine("[ERROR] The function 'parseXML' has failed");
                Console.WriteLine(e.StackTrace);
			}
        }
	}
}