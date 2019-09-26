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


namespace ADRDSynch
{
    public class ADRDSynch
    {
        public bool debug;
        private Device device;
        private RESTUtil util;
        private String address;
        
        public ADRDSynch(String address, bool debug)
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
            
            [Option('t', "inventory time", Required = false, DefaultValue = 1000,
              HelpText = "Inventory Time.")]
            public long inventoryTime
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
            long inventoryTime = 1000;
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                address = options.IPaddress;
                inventoryTime = options.inventoryTime;
                debug = options.Debug;
                Console.WriteLine("Parsed Arguments:");
                Console.WriteLine("\tAddress:\t" + address);
                Console.WriteLine("\tInventory Time:\t" + inventoryTime);
                Console.WriteLine("\tDebug:\t" + debug);
            
	        	ADRDSynch app = new ADRDSynch(address, false);
	        	app.run(inventoryTime);
            } else {
            	Console.WriteLine(options.GetUsage());
            	Console.ReadLine();
            }
        }

        public void run(long inventoryTime)
        {
            this.util.startStopDevice(device, true, false);
            Thread.Sleep(200);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.ElapsedMilliseconds < inventoryTime)
            {
                //With the sequential mode, it only needs to ask for an inventory
                this.util.sequentialInventory(device, false);
                this.util.getSequentialInventory(device, true, false);
            }

            this.util.startStopDevice(device, false, false);
            
        }
    }

}
