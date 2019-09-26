using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Net.Sockets;
using System.Diagnostics;
using System.Collections.Concurrent;
using Util;
using CommandLine;
using CommandLine.Text;

/**
 * Copyright (c) 2015 Keonn technologies S.L.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY KEONN TECHNOLOGIES S.L.
 * ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
 * TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
 * PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL KEONN TECHNOLOGIES S.L.
 * BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 *
 * 
 * @author salmendros
 * @date 29 Jan 2015
 * @copyright 2015 Keonn Technologies S.L. {@link http://www.keonn.com}
 *
 */

namespace GPIEvents
{
    public class GPIEvents
    {
        private bool debug;
        private Device device;
        private RESTUtil util;
        private String address;

        public GPIEvents(String address, bool debug)
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
            
            [Option('t', "Read time", Required = false, DefaultValue = 1000,
              HelpText = "Read time.")]
            public long time
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
            long time = 1000;
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                address = options.IPaddress;
                time = options.time;
                debug = options.Debug;
                Console.WriteLine("Parsed Arguments:");
                Console.WriteLine("\tAddress:\t" + address);
                Console.WriteLine("\tRead Time:\t" + time);
                Console.WriteLine("\tDebug:\t" + debug);
                
                GPIEvents app = new GPIEvents(address, false);
        		app.run(time);
            } else {
            	Console.WriteLine(options.GetUsage());
            	Console.ReadLine();
            }
        }

        public void run(long time)
        {
        	try
            {
                this.util.setDeviceMode(this.device, "Autonomous", false);
                util.startStopDevice(device, true, false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            
		    ConcurrentQueue<String> queue = new ConcurrentQueue<String>();
		    List<String> tagDataList = new List<String>(); 
		    Console.WriteLine("Device["+device.id+"] Reading the 3177 port... ");
		    TCPReader tcpReader = new TCPReader(this.address, queue, this.util);
            Thread tcpReaderThread = new Thread(new ThreadStart(tcpReader.run));
            tcpReaderThread.Start();
		    Console.WriteLine("Done.");
		
		    Stopwatch stopwatch2 = new Stopwatch();
            stopwatch2.Start();
            
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
                        GPIevent gpiEvent = this.util.processGPIdata(dTag);
                        
                        if(gpiEvent != null)
                        	Console.WriteLine(gpiEvent.ToString());
                    }
                }
            }
            tcpReader.Shutdown();
		
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
