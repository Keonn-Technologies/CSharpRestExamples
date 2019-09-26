using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.IO;
using System.Xml;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Diagnostics;

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
 * @date 15 Oct 2015
 * @copyright 2015 Keonn Technologies S.L. {@link http://www.keonn.com}
 *
 */


namespace Util
{
    public class RESTUtil
    {
        private String address;
        private Boolean debug;
        private HexUtil utilities;

        /**
	     * The different versions of AdvanNet can make the Rest Calls different,
	     * in this case we are working only with 2, AdvanNet-2.1.x (a21), and
	     * AdvanNet-2.3.x (a23)
	     */
        public enum AdvanNetVersion
        {
            a21,
            a23
        }
        public enum EventTypes
        {
            TAG_ALARM
        }
        public enum ReadModesClassName
        {
            READMODE_EPC_EAS_ALARM,
            READMODE_SQL_EAS_ALARM
        }

        public RESTUtil(String address, Boolean debug)
        {
            this.address = address;
            this.debug = debug;
            this.utilities = new HexUtil();
        }

        public String getAddress()
        {
            return this.address;
        }

        public void setAddress(String address)
        {
            this.address = address;
        }

        /// <summary>
        /// It will print the device information: id, parameters, antenna configuration...
        /// </summary>
        /// <param name="d">the device to print information of</param>
        /// <param name="no">Number of the device</param>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Xml.XmlException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="SystemException"></exception>
        public void printDevice(Device d, int no, bool displayResponse)
        {
            /**
             * Print device ID
             */
            Console.WriteLine("Device #" + no + " [type: " + d.family + " id: "
                    + d.id + "]");

            /**
             * Print device RF parameters
             */
            HashSet<string> paramNames = parseParameterNames(d, displayResponse);
            foreach (string paramName in paramNames)
            {
                string param = getParameter(d, paramName, false);
                Console.WriteLine("  Param[" + paramName + "]: " + param);
            }

            /**
             * Print device antennas
             */
            List<Antenna> antennas = parseAntennas(d, false);
            foreach (Antenna antenna in antennas)
            {
                Console.WriteLine("  Antenna[" + antenna.readerPort + ","
                        + antenna.mux1 + "," + antenna.mux2 + "]");
            }

            /**
             * Print read modes
             */
            HashSet<String> readModes = parseReadModes(d, false);
            foreach (String readMode in readModes)
            {
                Console.WriteLine("  ReadMode[" + readMode + "]");
            }
        }

        /// <summary>
        /// Parse the parameters of the reader
        /// </summary>
        /// <param name="d">The device to retrieve the parameters</param>
        /// <returns>A set of parameters</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Xml.XmlException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="SystemException"></exception>
        public HashSet<String> parseParameterNames(Device d, bool displayResponse)
        {
            /**
             * Build the parameters URL The URL depends on the id of the device
             */
            String parmetersURL = "http://" + address + ":3161" + "/devices/" + d.id + "/reader/params";

            /**
             * Access the URL to retrieve parameters names
             */
            String xmlFile = getFileFromURL(parmetersURL);
            
            

            if (xmlFile.Contains("ERROR") || xmlFile.Contains("error"))
                throw new SystemException("[ERROR] Problem retrieving the parameters");
            /**
             * Print device file
             */
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }
            
            /**
             * Parse parameters file Parameters file is an xml file (a sample can be
             * found at the end of this file)
             * 
             * The parsing of an xml file can be done using several different
             * approaches, we have use the XML Path Language (XPath) but any
             * approach is ok.
             * 
             * The goal is to retrieve the list of parameter names
             * 
             */
            
            HashSet<string> parameters = new HashSet<string>();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlFile);
            XmlNodeList paramsNodes = xmlDocument.GetElementsByTagName("params");
            XmlNodeList nodes = null;
            foreach (XmlNode paramNode in paramsNodes)
            {
                nodes = paramNode.ChildNodes;
                continue;
            }
            if (nodes == null)
                throw new SystemException("[ERROR] Something is wrong with the XML format");

            foreach (XmlNode node in nodes)
            {
                parameters.Add(node.Name);
            }

            return parameters;
        }

        /// <summary>
        /// Function to get the value of a reader parameter, to know which parameter a reader has, do a printDevice function
        /// </summary>
        /// <param name="d">the device to get the parameter value</param>
        /// <param name="paramName">String name of the parameter</param>
        /// <returns>String with the value of the parameter</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Xml.XmlException"></exception>
        /// <exception cref="SystemException"></exception>
        /// <exception cref="System.IO.PathTooLongException"></exception>
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public String getParameter(Device d, String paramName, bool displayResponse)
        {
            /**
             * Build the parameter URL The URL depends on the id of the device
             */
            String parmeterURL = "http://" + address + ":" + 3161 + "/devices/" + d.id
                    + "/reader/parameter/" + paramName;

            /**
             * Access the URL to retrieve parameter value
             */
            String xmlFile = getFileFromURL(parmeterURL);

            /**
             * Print parameter file
             */
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }

            /**
             * Parse parameter file Parameter file is an xml file (a sample can be
             * found at the end of this file)
             * 
             * The parsing of an xml file can be done using several different
             * approaches, we have use the XML Path Language (XPath) but any
             * approach is ok.
             * 
             * The goal is to retrieve the value of a parameter
             * 
             */
            XmlDocument inventory = new XmlDocument();
            inventory.LoadXml(xmlFile);
            XmlNodeList elements = inventory.SelectNodes("//response/data/result");
            if (elements.Count == 0)
                return "";
            if (elements.Count > 1)
                throw new SystemException("[WRONG_RESPONSE] " + paramName);

            foreach (XmlElement element in elements)
            {
                String parameter = element.InnerText;
                return parameter;
            }

            throw new SystemException("[WRONG_RESPONSE] " + paramName);
        }

        /// <summary>
        /// Checks if a device is alive
        /// </summary>
        /// <returns>State of the device</returns>
        /// <exception cref="FormatException"></exception>
        public bool checkDeviceAlive()
        {
            if (!CheckIPValid(address))
                throw new FormatException("IP address " + address + " is not valid");
            if (!CheckAdvanNetRunning(address))
                return false;
            return true;
        }

        /// <summary>
        /// Checks if the IP is valid
        /// </summary>
        /// <param name="strIP">IP address</param>
        /// <returns>If the IP is ok it will return true, false if not</returns>
        private bool CheckIPValid(string strIP)
        {
            IPAddress result = null;
            return
                !String.IsNullOrEmpty(strIP) &&
                IPAddress.TryParse(strIP, out result);
        }

        /// <summary>
        /// Checks if AdvanNet is running in the device
        /// </summary>
        /// <param name="address">IP address</param>
        /// <returns>If the device has AdvanNet running it will return true, false if not</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.Net.NetworkInformation.PingException"></exception>
        /// <exception cref="System.ObjectDisposedException"></exception>
        /// <exception cref="System.Net.Sockets.SocketException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        private bool CheckAdvanNetRunning(string address)
        {
            
            Ping pingSender = new Ping();
            PingReply reply = pingSender.Send(address, 120);
            if (reply.Status != IPStatus.Success)
                return false;
            
            Int32 port = 3177;
            TcpClient client = new TcpClient(address, port);
            NetworkStream stream = client.GetStream();
            StreamReader streamReader = new StreamReader(stream);

            string line;
            Stopwatch stopwatch2 = new Stopwatch();
            stopwatch2.Start();
            long time = 5000;
            while (stopwatch2.ElapsedMilliseconds < time)
            {
                
                line = streamReader.ReadLine();
                if (line.Contains("ADVANNET"))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Parse the read modes of the reader
        /// </summary>
        /// <param name="d">The device to retrieve the read modes</param>
        /// <returns>A set of read modes</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Xml.XmlException"></exception>
        /// <exception cref="SystemException"></exception>
        /// <exception cref="System.IO.PathTooLongException"></exception>
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public HashSet<String> parseReadModes(Device d, bool displayResponse)
        {
            /**
             * Build the parameters URL
             * The URL depends on the id of the device
             */
            String parametersURL = "http://" + this.address + ":3161/devices/" + d.id
                + "/readModes";

            /**
             * Acess the URL to retrieve parameters names
             */
            String xmlFile = this.getFileFromURL(parametersURL);

            if (xmlFile.Contains("ERROR") || xmlFile.Contains("error"))
                throw new SystemException("[ERROR] Problem retrieving the read modes");

            /**
             * Print device file
             */
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }

            /**
             * Parse parameters file.
             * 
             * The parsing of an xml file can be done using several different aproaches,
             * we have used the xml path language but any approach is ok.
             * 
             * The goal is to retrieve the list of parameter names.
             * 
             */

            var paramshash = new HashSet<string>();

            XmlDocument inventory = new XmlDocument();
            inventory.LoadXml(xmlFile);

            XmlNodeList elements = inventory.SelectNodes("//response/data/entries/entry/defaultReadMode/text()");
            foreach (XmlElement element in elements)
            {
                String id = element.Value;
                paramshash.Add(id);
            }

            return paramshash;
        }

        /// <summary>
        /// Check the AdvanNet status
        /// </summary>
        /// <param name="d">The device running AdvanNet</param>
        /// <returns>The state of the device</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Xml.XmlException"></exception>
        /// <exception cref="System.Xml.XPath.XPathException"></exception>
        /// <exception cref="SystemException"></exception>
        public String isRunning(Device d, bool displayResponse)
        {
            /**
             * Build the parameters URL
             * The URL depends on the id of the device
             */
            String parametersURL = "http://" + this.address + ":3161/status";

            /**
             * Acess the URL to retrieve parameters names
             */
            String xmlFile = this.getFileFromURL(parametersURL);
            
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }
            
            XmlDocument inventory = new XmlDocument();
            inventory.LoadXml(xmlFile);
            
            XmlNodeList elements = inventory.SelectNodes("//response/data/devices/device");
            foreach (XmlElement element in elements)
            {
                String dev = "<device>" + element.InnerXml + "</device>";
                XmlDocument devXML = new XmlDocument();
                inventory.LoadXml(dev);
                XmlNodeList devElements = inventory.SelectNodes("//device/status");
                foreach (XmlElement devElement in devElements)
                {
                    return devElement.InnerText;
                }
            }

            throw new SystemException("[ERROR] AdvanNet is not running");
        }

        /// <summary>
        /// Parse the devices of an AdvanNet instance
        /// </summary>
        /// <returns>A set with all the devices</returns>
        /// <exception cref="System.InvalidCastException"></exception>
        /// <exception cref="System.Xml.XPath.XPathException"></exception>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Xml.XmlException"></exception>
        public HashSet<Device> parseDevices(bool displayResponse)
        {
            /**
             * Build the parameters URL
             * The URL depends on the id of the device
             */
            String parametersURL = "http://" + this.address + ":3161/devices/";

            /**
             * Acess the URL to retrieve parameters names
             */
            String xmlFile = this.getFileFromURL(parametersURL);

            /**
             * Print device file
             */
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }

            XmlDocument inventory = new XmlDocument();
            inventory.LoadXml(xmlFile);

            XmlNodeList elements = inventory.SelectNodes("//response/msg-version");
            String advanNetVersion = "";
            foreach (XmlElement element in elements)
            {
                advanNetVersion = element.InnerText;
            }

            /**
             * Parse parameters file.
             * 
             * The parsing of an xml file can be done using several different aproaches,
             * we have used the xml path language but any approach is ok.
             * 
             * The goal is to retrieve the list of parameter names.
             * 
             */
            var devices = new HashSet<Device>();

            // TODO Test if this works
            XmlNodeList elements2 = inventory.SelectNodes("//response/data/devices/device/*[self::id or self::family]");
            String id = "";
            String family = "";
            Device d;
            for (int i = 0; i < elements2.Count; i += 2)
            {
                id = elements2.Item(i).InnerText;
                family = elements2.Item(i + 1).InnerText;
                d = new Device(id, family, advanNetVersion);
                devices.Add(d);

            }

            return devices;
        }

        /// <summary>
        /// Parse the unique device in an AdvanNet instance
        /// </summary>
        /// <returns>A device object defining the reader</returns>
        /// <exception cref="IOException"></exception>
        /// <exception cref="System.InvalidCastException"></exception>
        /// <exception cref="System.Xml.XPath.XPathException"></exception>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Xml.XmlException"></exception>
        public Device parseDevice(bool displayResponse) {
		    HashSet<Device> devices = parseDevices(displayResponse);
		    if (devices.Count == 1) {
                foreach (Device device in devices)
                {
                    return device;
                }
		    }
		    throw new IOException("Wrong device number: " + devices.Count);
	    }

        /// <summary>
        /// Starting a device means to connect to it, and whenever configured start
        /// </summary>
        /// <param name="d">The device to be started</param>
        /// <param name="start">Whether to start or stop the device</param>
        /// <exception cref="SystemException">Error in the response</exception>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public void startStopDevice(Device d, bool start, bool displayResponse)
        {
            /**
             * Build the parameters URL
             * The URL depends on the id of the device
             */
            String startURL = "http://" + this.address + ":3161/devices/" + d.id + "/"
                + (start ? "start" : "stop");

            /**
             * Acess the URL to retrieve parameters names
             */
            String xmlFile = this.getFileFromURL(startURL);

            /**
             * Print device file
             */
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }

            if (xmlFile.Contains("ERROR"))
                throw new SystemException("[ERROR] Failed to start/stop the device " + d.id);
        }

        /// <summary>
        /// Function to connect to the device
        /// </summary>
        /// <param name="d">The device to be connected</param>
        /// <exception cref="SystemException">Error in the response</exception>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public void connect(Device d, bool displayResponse)
        {
            /**
             * Build the parameters URL
             * The URL depends on the id of the device
             */
            String URL = "http://" + this.address + ":3161/devices/" + d.id + "/connect";

            /**
             * Acess the URL to retrieve parameters names
             */
            String xmlFile = this.getFileFromURL(URL);

            /**
             * Print device file
             */
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }

            if (xmlFile.Contains("ERROR"))
                throw new SystemException("[ERROR] Failed to connect to the device " + d.id);
        }

        /// <summary>
        /// Function to get the inventory information
        /// </summary>
        /// <param name="d">The device to get the inventory information</param>
        /// <returns>A HashSet of TagDatas with all the inventory information</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Xml.XmlException"></exception>
        /// <exception cref="SystemException"></exception>
        /// <exception cref="System.IO.PathTooLongException"></exception>
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public HashSet<TagData> getInventory(Device d, bool displayResponse)
        {

            /**
             * Build the inventory URL The URL depends on the id of the device
             */
            String inventoryURL = "http://" + address + ":3161/devices/" + d.id
                    + "/inventory";

            /**
             * Access the URL to retrieve inventory data
             */
            String xmlFile = getFileFromURL(inventoryURL);

            /**
             * Print raw inventory file
             */
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }

            /**
             * Parse inventory data Inventory data is an xml file (a sample can be
             * found at the end of this file)
             * 
             * The parsing of an xml file can be done using several different
             * approaches, we have use the XML Path Language (XPath) but any
             * approach is ok.
             * 
             * The goal is to retrieve the list of found tags
             * 
             */
            HashSet<TagData> tags = new HashSet<TagData>();

            XmlDocument inventory = new XmlDocument();
            inventory.LoadXml(xmlFile);

            // TODO Test if this works
            XmlNodeList elements = inventory.SelectNodes("//inventory/data/inventory/items/item/*[self::epc or self::ts]");

            // Iteration over the results
            for (int i = 0; i < elements.Count; i += 2)
            {
                String hexEpc = elements.Item(i).InnerText;
                String ts = elements.Item(i + 1).InnerText;

                tags.Add(new TagData(hexEpc, ts));

            }

            return tags;
        }

        /// <summary>
        /// Function to get the inventory information with location
        /// </summary>
        /// <param name="d">The device to get the inventory and location information</param>
        /// <returns>A Set of TagDatas with all the inventory and location information</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Xml.XmlException"></exception>
        /// <exception cref="SystemException"></exception>
        /// <exception cref="System.IO.PathTooLongException"></exception>
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        /// <exception cref="System.Xml.XPath.XPathException"></exception>
        public HashSet<TagData> getLocation(Device d, bool displayResponse)
        {
            /**
             * Build the inventory URL The URL depends on the id of the device
             */
            String inventoryURL = "http://" + address + ":3161/devices/" + d.id
                    + "/location";

            /**
             * Access the URL to retrieve inventory data
             */
            String xmlFile = getFileFromURL(inventoryURL);

            /**
             * Print raw inventory file
             */
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }

            /**
             * Parse inventory data Inventory data is an xml file (a sample can be
             * found at the end of this file)
             * 
             * The parsing of an xml file can be done using several different
             * approaches, we have use the XML Path Language (XPath) but any
             * approach is ok.
             * 
             * The goal is to retrieve the list of found tags
             * 
             */
            HashSet<TagData> tags = new HashSet<TagData>();
            XmlDocument inventory = new XmlDocument();
            inventory.LoadXml(xmlFile);

            // TODO Test if this works
            XmlNodeList elements = inventory.SelectNodes("//inventory/data/inventory/items/item/locationData");

            for (int i = 0; i < elements.Count; i++)
            {
                XmlNode ni = elements.Item(i);
                XmlNode parent = ni.ParentNode;
                XmlNodeList pl = parent.ChildNodes;
                String ts = null;
                String epc = null;
                for (int p = 0; p < pl.Count; p++)
                {
                    XmlNode pp = pl.Item(p);
                    if (pp.Name.Equals("ts"))
                    {
                        ts = pp.InnerText;
                        break;
                    }
                }
                XmlNodeList nli = ni.ChildNodes;
                for (int j = 0; j < nli.Count; j++)
                {
                    XmlNode nj = nli.Item(j);
                    if (nj.Name.Equals("epc"))
                    {
                        epc = nj.InnerText;
                    }
                    else if (nj.Name.Equals("location"))
                    {
                        XmlNodeList nlj = nj.ChildNodes;
                        for (int k = 0; k < nlj.Count; k++)
                        {
                            XmlNode nk = nlj.Item(k);
                            if (nk.Name.Equals("loc"))
                            {
                                if (epc != null && ts != null)
                                {
                                    TagData tagData = new TagData(epc, ts);
                                    tagData.addLocation(nk.InnerText);
                                    tags.Add(tagData);
                                }
                            }
                        }
                    }

                }
            }

            return tags;
        }

        /// <summary>
        /// Creates a String list with all the EPCs in data
        /// </summary>
        /// <param name="data">An XML file containing AdvanNet information from the TCP port</param>
        /// <param name="tagDataList">A String list to add the EPCs</param>
        /// <exception cref="System.NullReferenceException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.Text.RegularExpressions.RegexMatchTimeoutException"></exception>
        public void processTCPdata(String data, List<String> tagDataList)
        {
            Regex regex = new Regex("<epc>(.*?)</epc>");
            Match match = regex.Match(data);
            while (match.Success)
            {
                tagDataList.Add(match.Groups[1].Value);
                match = match.NextMatch();
            }
        }

        /// <summary>
        /// Creates a GPIevent object.
        /// </summary>
        /// <param name="sgpiEvent">A string containing an event object.</param>
        /// <returns>Returns a GPIevent object</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.Text.RegularExpressions.RegexMatchTimeoutException"></exception>
        private GPIevent createGpiEvent(string sgpiEvent)
        {
            GPIevent gpiEvent = new GPIevent();
            
            Regex regex = new Regex("<deviceId>(.*?)</deviceId>");
            Match match = regex.Match(sgpiEvent);
            while (match.Success)
            {
                gpiEvent.setDeviceId(match.Groups[1].Value);
                break;
            }

            regex = new Regex("<line>(.*?)</line>");
            match = regex.Match(sgpiEvent);            
            while (match.Success)
            {
                gpiEvent.setLine(match.Groups[1].Value);
                break;
            }

            regex = new Regex("<lowToHigh>(.*?)</lowToHigh>");
            match = regex.Match(sgpiEvent);
            while (match.Success)
            {
                gpiEvent.setLowToHigh(match.Groups[1].Value);
                break;
            }

            return gpiEvent;
        }

        /// <summary>
        /// Function to parse and process the GPI event data
        /// </summary>
        /// <param name="data">An XML String containing a GPI event</param>
        /// <returns>It returns a GPIevent object</returns>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Text.RegularExpressions.RegexMatchTimeoutException"></exception>
        public GPIevent processGPIdata(String data)
        {
            GPIevent readerGPIevent = null;
            data = data.Replace(" ", "");
            data = data.Replace("\n", "");
            data = data.Replace("\r", "");
            data = data.Replace("\t", "");

            Regex regex = new Regex("<event>(.*?)</event>");
            Match match = regex.Match(data);

            string aux;
            List<string> events = new List<string>();
            while (match.Success)
            {
                aux = match.Groups[1].Value;
                if (aux.Contains("<type>GPI</type>"))
                    events.Add(aux);
                match = match.NextMatch();
            }
            
            foreach (string e in events)
            {
                readerGPIevent = createGpiEvent(e);
            }
            return readerGPIevent;
        }

        /// <summary>
        /// Parse the XML that AdvanNet produces in the TCP port 3177.
        /// </summary>
        /// <param name="data">The XML data from the TCP port</param>
        /// <param name="queue">A queue to add the parse XML file</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.Text.RegularExpressions.RegexMatchTimeoutException"></exception>
        public void parseXML(String data, Queue<String> queue)
        {
            String data2 = data.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
            
            Regex regex = new Regex("<inventory>(.*?)</inventory>");
            Match match = regex.Match(data2);
            String f = "";
            while (match.Success)
            {
                f += "<inventory>";
                f += match.Groups[1].Value;
                f += "</inventory></data></inventory>";
                if (f.Contains("<type>"))
                    queue.Enqueue(f);
                match = match.NextMatch();
            }

            regex = new Regex("<deviceEventMessage>(.*?)</deviceEventMessage>");
            match = regex.Match(data2);
            while (match.Success)
            {
                f += "<deviceEventMessage>";
                f += match.Groups[1].Value;
                f += "</deviceEventMessage>";
                queue.Enqueue(f);
                match = match.NextMatch();
            }

            regex = new Regex("<eventMessage>(.*?)</eventMessage>");
            match = regex.Match(data2);
            while (match.Success)
            {
                f += "<eventMessage>";
                f += match.Groups[1].Value;
                f += "</eventMessage>";
                queue.Enqueue(f);
                match = match.NextMatch();
            }

        }

        /// <summary>
        /// Function to trigger an buzzer action when an alarm event
        /// </summary>
        /// <param name="d">The device to trigger the alarm/action</param>
        /// <param name="advanNetInformation">An XML containing the information to parse</param>
        /// <param name="epcToAlarm">The EPC hex to trigger the alarm</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.Text.RegularExpressions.RegexMatchTimeoutException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public void processAlarmMessages(Device d, String advanNetInformation, String epcToAlarm)
        {
            Regex regex = new Regex("<epc>(.+?)</epc>");
            Match match = regex.Match(advanNetInformation);
		    String epc = "";
            while (match.Success)
            {
                epc = match.Groups[1].Value;
			    if(epc.Equals(epcToAlarm)){
				    Console.WriteLine("[INFO] Alarmed Tag: " + epc);
				    this.buzzer(d, 200, 200, 0, false);
			    }
            }
        }

        /// <summary>
        /// Turn the speaker on
        /// </summary>
        /// <param name="d">The device that has the speaker</param>
        /// <param name="frequency">The frequency in Hz</param>
        /// <param name="volume">The volume from 1 to 10</param>
        /// <param name="timeOff">The time off in milliseconds</param>
        /// <param name="timeOn">The time on in milliseconds</param>
        /// <param name="totalDuration">The total time in milliseconds</param>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="SystemException"></exception>
        public void speaker(Device d, int frequency, int volume, int timeOff, int timeOn, int totalDuration, bool displayResponse)
        {
            /**
            * Build the parameters URL
            * The URL depends on the id of the device
            */
            String parametersURL = "http://" + this.address + ":3161/devices/" + d.id
                + "/speaker/" + frequency + "/" + volume + "/" + timeOff + "/" + timeOn + "/" + totalDuration;

            /**
             * Acess the URL to retrieve parameters names
             */
            String xmlFile = this.getFileFromURL(parametersURL);

            /**
             * Print device file
             */
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }

            if (xmlFile.Contains("ERROR") || xmlFile.Contains("error"))
                throw new SystemException("[ERROR] The speaker could not be turned on/off");
        }

        /// <summary>
        /// Turn the buzzer on
        /// </summary>
        /// <param name="d">The device that has the buzzer</param>
        /// <param name="totalDuration">The total time in milliseconds</param>
        /// <param name="timeOn">The time on in milliseconds</param>
        /// <param name="timeOff">The time off in milliseconds</param>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="SystemException"></exception>
        public void buzzer(Device d, int totalDuration, int timeOn, int timeOff, bool displayResponse)
        {
            /**
             * Build the parameters URL
             * The URL depends on the id of the device
             */
            String parametersURL = "http://" + this.address + ":3161/devices/" + d.id
                + "/buzz/" + timeOn + "/" + timeOff + "/" + totalDuration;

            /**
             * Acess the URL to retrieve parameters names
             */
            String xmlFile = this.getFileFromURL(parametersURL);

            /**
             * Print device file
             */
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }

            if (xmlFile.Contains("ERROR") || xmlFile.Contains("error"))
                throw new SystemException("[ERROR] The buzzer could not be turned on/off");
        }

        /// <summary>
        /// Function to set the EPCGen2 filter
        /// </summary>
        /// <param name="d">The device to change the EPCGen2 filter</param>
        /// <param name="className">A String ReadModesClassName</param>
        /// <param name="maskOffset">The EPCGen2 mask offset</param>
        /// <param name="maskLength">The EPCGen2 mask length</param>
        /// <param name="filterMask">The EPCGen2 mask</param>
        /// <param name="swFilterOnly">To use or not to use only the software filter only</param>
        /// <param name="advanNetVersion">The AdvanNet version of the device</param>
        /// <exception cref="SystemException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.Xml.XmlException"></exception>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.Text.EncoderFallbackException"></exception>
        /// <exception cref="System.OverflowException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
	    public void setEPCGen2Filter(Device d, String className, int maskOffset,
			    int maskLength, String filterMask, bool swFilterOnly,
			    String advanNetVersion, bool displayResponse) 
        {
		    if (AdvanNetVersion.a21.ToString().Equals(advanNetVersion)) 
            {
			    this.setEPCGen2Filter21(d, className, maskOffset, maskLength,
					    filterMask, swFilterOnly, displayResponse);
		    } 
            else if (AdvanNetVersion.a23.ToString().Equals(advanNetVersion)) 
            {
			    this.setEPCGen2Filter23(d, className, maskOffset, maskLength,
					    filterMask, swFilterOnly, displayResponse);
            }
            else
            {
                throw new SystemException("[ERROR] The AdvanNet version is not supported");
            }
	    }

        /// <summary>
        /// Function to set the EPCGen2 filter
        /// </summary>
        /// <param name="d">The device to change the EPCGen2 filter</param>
        /// <param name="className">A String ReadModesClassName</param>
        /// <param name="maskOffset">The EPCGen2 mask offset</param>
        /// <param name="maskLength">The EPCGen2 mask length</param>
        /// <param name="filterMask">The EPCGen2 mask</param>
        /// <param name="swFilterOnly">To use or not to use only the software filter only</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.Xml.XmlException"></exception>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.Text.EncoderFallbackException"></exception>
        /// <exception cref="System.OverflowException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="SystemException"></exception>
	    public void setEPCGen2Filter21(Device d, String className, int maskOffset,
			    int maskLength, String filterMask, bool swFilterOnly, bool displayResponse) 
        {
		    XmlDocument doc = new XmlDocument();
            
		    XmlElement xobject = doc.CreateElement("object");
		    doc.AppendChild(xobject);

		    XmlElement objectClass = doc.CreateElement("class");
		    objectClass.AppendChild(doc.CreateTextNode("SPEC"));
		    xobject.AppendChild(objectClass);

		    XmlElement objectClassName = doc.CreateElement("classname");
		    objectClassName.AppendChild(doc.CreateTextNode(className));
		    xobject.AppendChild(objectClassName);

		    XmlElement oparams = doc.CreateElement("params");
		    xobject.AppendChild(oparams);

		    oparams.AppendChild(createParam(doc, "SPEC_PARAM", "select", "INTEGER",
				    "filterMaskOffset", "" + maskOffset));

		    oparams.AppendChild(createParam(doc, "SPEC_PARAM", "select", "INTEGER",
				    "filterMaskBitLength", "" + maskLength));

		    oparams.AppendChild(createParam(doc, "SPEC_PARAM", "text", "STRING",
				    "filterMaskHex", filterMask));

		    oparams.AppendChild(createParam(doc, "SPEC_PARAM", "select", "BOOLEAN",
				    "swFilterOnly", Convert.ToString(swFilterOnly)));

		    String xml = doc.InnerXml;
            
		    String url = "http://" + this.address + ":3161/devices/" + d.id
				    + "/readModes[0]";

		    String xmlFile = this.getFileFromURL(url, xml);
		    
		    if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }
		    
		    if (xmlFile.Contains("ERROR"))
			    throw new SystemException("[ERROR] The setEPCGen2Filter function failed");
	    }

        /// <summary>
        /// Creates an XML parameter
        /// </summary>
        /// <param name="doc">A document to create the parameter</param>
        /// <param name="paramClass">A parameter class</param>
        /// <param name="type">A type of the parameter</param>
        /// <param name="typedef">A type definition of the parameter</param>
        /// <param name="name">The name of the parameter</param>
        /// <param name="value">The value of the parameter</param>
        /// <returns>An XML element containing a parameter</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        private XmlElement createParam(XmlDocument doc, String paramClass, String type,
                String typedef, String name, String value)
        {
            
            XmlElement param = doc.CreateElement("param");

            XmlElement param1Class = doc.CreateElement("class");
            param1Class.AppendChild(doc.CreateTextNode(paramClass));
            param.AppendChild(param1Class);

            XmlElement param1Type = doc.CreateElement("type");
            param1Type.AppendChild(doc.CreateTextNode(type));
            param.AppendChild(param1Type);

            XmlElement param1TypeDef = doc.CreateElement("typedef");
            param1TypeDef.AppendChild(doc.CreateTextNode(typedef));
            param.AppendChild(param1TypeDef);

            XmlElement param1Name = doc.CreateElement("name");
            param1Name.AppendChild(doc.CreateTextNode(name));
            param.AppendChild(param1Name);

            XmlElement param1Value = doc.CreateElement("value");
            param1Value.AppendChild(doc.CreateTextNode(value));
            param.AppendChild(param1Value);

            return param;
        }
	
	    /// <summary>
        /// Function to set the EPCGen2 filter
	    /// </summary>
        /// <param name="d">The device to change the EPCGen2 filter</param>
        /// <param name="className">A String ReadModesClassName</param>
        /// <param name="maskOffset">The EPCGen2 mask offset</param>
        /// <param name="maskLength">The EPCGen2 mask length</param>
        /// <param name="filterMask">The EPCGen2 mask</param>
        /// <param name="swFilterOnly">To use or not to use only the software filter only</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.Xml.XmlException"></exception>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.Text.EncoderFallbackException"></exception>
        /// <exception cref="System.OverflowException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="SystemException"></exception>
	    public void setEPCGen2Filter23(Device d, String className, int maskOffset,
			    int maskLength, String filterMask, bool swFilterOnly, bool displayResponse) 
        {
		    /**
		     * Build the URL The URL depends on the id of the device
		     */
		    String URL = "http://" + address + ":3161/devices/" + d.id
				    + "/readModes[0]";

		    String paramURL = "http://" + address + ":3161/devices/" + d.id
				    + "/readModes[0]/paramSpec";

		    Dictionary<string, string> rmParams = this.getReadModeParamSpec(paramURL, false);

		    XmlDocument doc = new XmlDocument();

		    XmlElement request = doc.CreateElement("request");
		    doc.AppendChild(request);

		    XmlElement rname = doc.CreateElement("name");
		    rname.AppendChild(doc.CreateTextNode("SEQUENTIAL"));
		    request.AppendChild(rname);

		    XmlElement rxxx1 = doc.CreateElement("xxx");
		    rxxx1.AppendChild(doc.CreateTextNode("undefined"));
		    request.AppendChild(rxxx1);

		    XmlElement rxxx2 = doc.CreateElement("xxx");
		    rxxx2.AppendChild(doc.CreateTextNode("undefined"));
		    request.AppendChild(rxxx2);

		    XmlElement rxxx3 = doc.CreateElement("xxx");
		    rxxx3.AppendChild(doc.CreateTextNode("undefined"));
		    request.AppendChild(rxxx3);

            foreach(KeyValuePair<string, string> entry in rmParams)
            {
			    XmlElement element = doc.CreateElement(entry.Key);
                if (!entry.Value.Equals(""))
                {
                    if (entry.Key.Equals("filterMaskOffset"))
                    {
					    element.AppendChild(doc.CreateTextNode(Convert.ToString(maskOffset)));
                    }
                    else if (entry.Key.Equals("filterMaskBitLength"))
                    {
					    element.AppendChild(doc.CreateTextNode(Convert.ToString(maskLength)));
                    }
                    else if (entry.Key.Equals("filterMaskHex"))
                    {
					    element.AppendChild(doc.CreateTextNode(filterMask));
                    }
                    else if (entry.Key.Equals("swFilterOnly"))
                    {
					    element.AppendChild(doc.CreateTextNode(Convert.ToString(swFilterOnly)));
				    } else {
					    element.AppendChild(doc.CreateTextNode(entry.Value));
				    }
			    }
			    request.AppendChild(element);
		    }

		    String test = doc.InnerXml;
		    String xmlFile = this.getFileFromURL(URL, test);
		    
		    if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }
		    
		    if (xmlFile.Contains("ERROR"))
			    throw new SystemException("[ERROR] Sequential read was not set successfully");
            
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="devicesURL"></param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public String getFileFromURL(String devicesURL)
        {
            WebRequest wrGETURL = WebRequest.Create(devicesURL);
            Stream objStream = wrGETURL.GetResponse().GetResponseStream();
            StreamReader objReader = new StreamReader(objStream);

            string sLine = "";
            string final = "";

            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                final += sLine;
            }

            return final;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.Text.EncoderFallbackException"></exception>
        /// <exception cref="System.OverflowException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        private string getFileFromURL(string url, string postData)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);

            req.ContentType = "text/xml";
            req.Method = "PUT";

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(postData);
            req.ContentLength = bytes.Length;

            using (Stream os = req.GetRequestStream())
            {
                os.Write(bytes, 0, bytes.Length);
            }

            using (System.Net.WebResponse resp = req.GetResponse())
            {
                if (resp == null)
                    return null;

                using (System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream()))
                {
                    return sr.ReadToEnd().Trim();
                }
            }
        }

        /// <summary>
        /// Function that prints the EPC and the location (if there is a location)
        /// </summary>
        /// <param name="d">The device that has read the tags</param>
        /// <param name="data">data Set of TagData objects with the information to print</param>
        public void printInventory(Device d, HashSet<TagData> data)
        {
            if (data.Count > 0)
            {
                String s;

                s = "Device[" + d.id + "] " + data.Count + " tags detected at device.";

                foreach (TagData td in data)
                {
                    s += System.Environment.NewLine;
                    s += "    Tag [hex epc: " + td.hexEpc + "]";

                    if (td.loc != null)
                    {
                        s += System.Environment.NewLine;
                        s += "    Location: " + td.loc;
                    }
                }

                Console.WriteLine(s);
            }
        }

        /// <summary>
        /// Function to retrieve the read mode of the Device
        /// </summary>
        /// <param name="d">The device to retrieve the read mode from</param>
        /// <returns>A String with the read mode</returns>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.InvalidCastException"></exception>
        /// <exception cref="System.Xml.XPath.XPathException"></exception>
        public String getReadMode(Device d, bool displayResponse)
        {
            /**
             * Build the URL
             * The URL depends on the id of the device
             */
            String URL = "http://" + this.address + ":3161/devices/" + d.id + "/activeReadMode";

            /**
             * Access the URL to retrieve the content
             */
            String xmlFile = this.getFileFromURL(URL);

            /**
             * Print file
             */
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }

            /**
             * Parse parameters file.
             * 
             * The parsing of an xml file can be done using several different aproaches,
             * we have used the xml path language but any approach is ok.
             * 
             * The goal is to retrieve the list of parameter names.
             * 
             */
            String readmode = "";
            XmlDocument inventory = new XmlDocument();
            inventory.LoadXml(xmlFile);
            XmlNodeList elements = inventory.SelectNodes("//response/data/result");

            foreach (XmlElement element in elements)
            {
                readmode = element.InnerText;

            }

            return readmode;
        }

        /// <summary>
        /// Sets the device mode for the device
        /// </summary>
        /// <param name="d">The device to change the device mode</param>
        /// <param name="readMode">String with the device mode</param>
        /// <exception cref="SystemException">Error in the response</exception>
        public void setDeviceMode(Device d, String readMode, bool displayResponse)
        {
            /**
             * Build the URL
             * The URL depends on the id of the device
             */
            String URL = "http://" + this.address + ":3161/devices/" + d.id + "/activeDeviceMode";

            /**
             * Access the URL to retrieve the content
             */
            String xmlFile = this.getFileFromURL(URL, readMode);

            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }
            
            if (xmlFile.Contains("ERROR"))
                throw new SystemException("[ERROR] Not able to set the Device Mode");
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="session"></param>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.Text.EncoderFallbackException"></exception>
        /// <exception cref="System.OverflowException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="SystemException"></exception>
        public void setGEN2_SESSION(Device device, String session, bool displayResponse) {
            /**
             * Build the URL
             * The URL depends on the id of the device
             */
            String URL = "http://" + this.address + ":3161/devices/" + device.id + "/reader/parameter/GEN2_SESSION";
            
            String xmlFile = this.getFileFromURL(URL, session);
            
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }
            
            if(xmlFile.Contains("ERROR"))
            	throw new SystemException("[ERROR] Not able to set the Session");
        	
        }

        /// <summary>
        /// Sets the read mode for the device
        /// </summary>
        /// <param name="d">The device to change the read mode</param>
        /// <param name="readMode">A String containing the read mode to set</param>
        /// <exception cref="SystemException">Error in the response</exception>
        public void setReadMode(Device d, String readMode, bool displayResponse)
        {
            /**
             * Build the URL
             * The URL depends on the id of the device
             */
            String URL = "http://" + this.address + ":3161/devices/" + d.id + "/activeReadMode";

            /**
             * Access the URL to retrieve the content
             */
            String xmlFile = this.getFileFromURL(URL, readMode);
            
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }

            if (xmlFile.Contains("ERROR"))
                throw new SystemException("[ERROR] Not able to set the Read Mode");
        }

        /// <summary>
        /// Function to set the power of the device
        /// </summary>
        /// <param name="d">The device to change the power</param>
        /// <param name="power">An integer with the power to set</param>
        /// <exception cref="SystemException">Error in the response</exception>
        /// <exception cref="FormatException">Error with the power</exception>
        public void setPower(Device d, int power, bool displayResponse)
        {
            /**
             * Build the URL
             * The URL depends on the id of the device
             */
            String URL = "http://" + this.address + ":3161/devices/" + d.id + "/reader/parameter/RF_READ_POWER";

            /**
             * Access the URL to retrieve the content
             */
            String xmlFile = this.getFileFromURL(URL, power.ToString());
            
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }
            
            if (xmlFile.Contains("ERROR"))
                throw new SystemException("[ERROR] Not able to set the Read Mode");
        }

        /// <summary>
        /// Function to set the sensitivity to the device
        /// </summary>
        /// <param name="d">The device to change the sensitivity</param>
        /// <param name="sensitivity">An integer with the sensitivity to set</param>
        /// <exception cref="SystemException">Error in the response</exception>
        /// <exception cref="FormatException">Error with the sensitivity</exception>
        public void setSensitivity(Device d, int sensitivity, bool displayResponse)
        {
            /**
             * Build the URL
             * The URL depends on the id of the device
             */
            String URL = "http://" + this.address + ":3161/devices/" + d.id + "/reader/parameter/RF_SENSITIVITY";

            /**
             * Access the URL to retrieve the content
             */
            String xmlFile = this.getFileFromURL(URL, sensitivity.ToString());
            
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }
            
            if (xmlFile.Contains("ERROR"))
                throw new SystemException("[ERROR] Not able to set the Read Mode");
        }

        /// <summary>
        /// Function that allows to configure only one antenna, leaving the ones configured.
        /// </summary>
        /// <param name="d">The device to configure the antenna</param>
        /// <param name="antennaNumber">Antenna port number</param>
        /// <param name="mux1">First multiplexor number</param>
        /// <param name="mux2">Second multiplexor number</param>
        /// <param name="orientation">An integer with the orientation of the antenna (e.g. -1, +1)</param>
        /// <param name="location">A string for user to define the location of the antenna</param>
        /// <param name="x">Coordinate X</param>
        /// <param name="y">Coordinate Y</param>
        /// <param name="z">Coordinate Z</param>
        /// <exception cref="SystemException">Error setting the antenna</exception>
        public void configureAntenna(Device d, int antennaNumber, int mux1,
            int mux2, int orientation, String location, int x, int y, int z, bool displayResponse)
        {
            configureAntenna(d, antennaNumber, mux1, mux2, orientation,
                location, x, y, z, 0, 0, displayResponse);
        }

        /// <summary>
        /// Function that allows to configure only one antenna, leaving the ones configured.
        /// </summary>
        /// <param name="d">The device to configure the antenna</param>
        /// <param name="antennaNumber">Antenna port number</param>
        /// <param name="mux1">First multiplexor number</param>
        /// <param name="mux2">Second multiplexor number</param>
        /// <param name="orientation">An integer with the orientation of the antenna (e.g. -1, +1)</param>
        /// <param name="location">A string for user to define the location of the antenna</param>
        /// <param name="x">Coordinate X</param>
        /// <param name="y">Coordinate Y</param>
        /// <param name="z">Coordinate Z</param>
        /// <param name="power">The power for the antenna</param>
        /// <param name="sensitivity">The sensitivity for the antenna</param>
        /// <exception cref="SystemException">Error setting the antenna</exception>
        public void configureAntenna(Device d, int antennaNumber, int mux1,
            int mux2, int orientation, String location, int x, int y, int z,
            int power, int sensitivity, bool displayResponse)
        {
            /**
             * Build the URL
             * The URL depends on the id of the device
             */
            String URL = "http://" + this.address + ":3161/devices/" + d.id + "/antennas";

            /**
             * 1) Do a GET to receive the current configuration of the antennas.
             * 2) Modify the antenna that it is indicated by the variable antennaNumber.
             */

            Antenna a = new Antenna(antennaNumber, mux1, mux2, orientation, location, x, y, z);
            if (power != 0)
                a.setPower(power);
            if (sensitivity != 0)
                a.setSensitivity(sensitivity);

            List<Antenna> lantennas = this.parseAntennas(d, false);
            bool configuredAntenna = false;

            XmlDocument doc = new XmlDocument();

            XmlElement request = doc.CreateElement(string.Empty, "request", string.Empty);
            doc.AppendChild(request);

            XmlElement entries = doc.CreateElement(string.Empty, "entries", string.Empty);
            request.AppendChild(entries);

            foreach (Antenna antenna in lantennas)
            {
                XmlDocument ant = null;
                if (antenna.readerPort == antennaNumber)
                {
                    ant = this.getAntennaXML(a, d);
                    configuredAntenna = true;
                }
                else
                {
                    ant = this.getAntennaXML(antenna, d);
                }

                entries.AppendChild(
                    entries.OwnerDocument.ImportNode(
                        ant.DocumentElement, true));
            }

            if (!configuredAntenna)
            {
                XmlDocument ant = this.getAntennaXML(a, d);
                entries.AppendChild(
                    entries.OwnerDocument.ImportNode(
                        ant.DocumentElement, true));
            }

            String antennaConfiguration = doc.InnerXml;
            String xmlFile = this.getFileFromURL(URL, antennaConfiguration);
            
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }

            if (xmlFile.Contains("ERROR"))
                throw new SystemException("[ERROR] Problem setting the antenna configuration");
        }

        /// <summary>
        /// Configure a set of antennas deleting the old ones
        /// </summary>
        /// <param name="d">The device to configure the antennas</param>
        /// <param name="lantennas">A list of Antenna objects to configure</param>
        /// <exception cref="SystemException">Error setting the antennas</exception>
        public void setAntennaConfiguration(Device d, List<Antenna> lantennas, bool displayResponse)
        {
            /**
             * Build the URL
             * The URL depends on the id of the device
             */
            String URL = "http://" + this.address + ":3161/devices/" + d.id + "/antennas";

            /**
             * 1) Do a GET to receive the current configuration of the antennas.
             * 
             */

            XmlDocument doc = new XmlDocument();

            XmlElement request = doc.CreateElement(string.Empty, "request", string.Empty);
            doc.AppendChild(request);

            XmlElement entries = doc.CreateElement(string.Empty, "entries", string.Empty);
            request.AppendChild(entries);

            foreach (Antenna antenna in lantennas)
            {
                XmlDocument ant = this.getAntennaXML(antenna, d);

                entries.AppendChild(
                    entries.OwnerDocument.ImportNode(
                        ant.DocumentElement, true));
            }

            string antennaConfiguration = doc.InnerXml;

            String xmlFile = this.getFileFromURL(URL, antennaConfiguration);
            
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }

            if (xmlFile.Contains("ERROR"))
                throw new SystemException("[ERROR] Problem setting the antenna configuration");
        }

        /// <summary>
        /// From an Antenna object it will create an XML document
        /// </summary>
        /// <param name="a">Antenna object to use</param>
        /// <param name="d">The device for the antenna</param>
        /// <returns>A document containing the XML representation of the Antenna</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public XmlDocument getAntennaXML(Antenna a, Device d)
        {
            XmlDocument doc = new XmlDocument();

            XmlElement entry = doc.CreateElement(string.Empty, "entry", string.Empty);
            doc.AppendChild(entry);

            XmlElement antennaClass = doc.CreateElement(string.Empty, "class", string.Empty);
            string text = "ANTENNA_DEFINITION";
            XmlText antennaClassText = doc.CreateTextNode(text);
            antennaClass.AppendChild(antennaClassText);
            entry.AppendChild(antennaClass);

            XmlElement cid = doc.CreateElement(string.Empty, "cid", string.Empty);
            text = d.id;
            XmlText cidText = doc.CreateTextNode(text);
            cid.AppendChild(cidText);
            entry.AppendChild(cid);

            XmlElement port = doc.CreateElement(string.Empty, "port", string.Empty);
            text = a.readerPort.ToString();
            XmlText portText = doc.CreateTextNode(text);
            port.AppendChild(portText);
            entry.AppendChild(port);

            XmlElement mux1 = doc.CreateElement(string.Empty, "mux1", string.Empty);
            text = a.mux1.ToString();
            XmlText mux1Text = doc.CreateTextNode(text);
            mux1.AppendChild(mux1Text);
            entry.AppendChild(mux1);

            XmlElement mux2 = doc.CreateElement(string.Empty, "mux2", string.Empty);
            text = a.mux2.ToString();
            XmlText mux2Text = doc.CreateTextNode(text);
            mux2.AppendChild(mux2Text);
            entry.AppendChild(mux2);

            XmlElement orientation = doc.CreateElement(string.Empty, "orientation", string.Empty);
            text = a.orientation.ToString();
            XmlText orientationText = doc.CreateTextNode(text);
            orientation.AppendChild(orientationText);
            entry.AppendChild(orientation);

            XmlElement location = doc.CreateElement(string.Empty, "location", string.Empty);
            entry.AppendChild(location);

            XmlElement locationClass = doc.CreateElement(string.Empty, "class", string.Empty);
            text = "LOCATION";
            XmlText locationClassText = doc.CreateTextNode(text);
            locationClass.AppendChild(locationClassText);
            location.AppendChild(locationClass);

            XmlElement locID = doc.CreateElement(string.Empty, "locID", string.Empty);
            if (a.location != "")
            {
                text = a.location;
                XmlText locIDText = doc.CreateTextNode(text);
                locID.AppendChild(locIDText);
            }
            location.AppendChild(locID);

            XmlElement x = doc.CreateElement(string.Empty, "x", string.Empty);
            text = a.x.ToString();
            XmlText xText = doc.CreateTextNode(text);
            x.AppendChild(xText);
            location.AppendChild(x);

            XmlElement y = doc.CreateElement(string.Empty, "y", string.Empty);
            text = a.y.ToString();
            XmlText yText = doc.CreateTextNode(text);
            y.AppendChild(yText);
            location.AppendChild(y);

            XmlElement z = doc.CreateElement(string.Empty, "z", string.Empty);
            text = a.z.ToString();
            XmlText zText = doc.CreateTextNode(text);
            z.AppendChild(zText);
            location.AppendChild(z);

            XmlElement conf = doc.CreateElement(string.Empty, "conf", string.Empty);
            entry.AppendChild(conf);

            XmlElement confClass = doc.CreateElement(string.Empty, "class", string.Empty);
            text = "ANTENNA_CONF";
            XmlText confClassText = doc.CreateTextNode(text);
            confClass.AppendChild(confClassText);
            conf.AppendChild(confClass);

            XmlElement power = doc.CreateElement(string.Empty, "power", string.Empty);
            text = a.power.ToString();
            XmlText powerText = doc.CreateTextNode(text);
            power.AppendChild(powerText);
            conf.AppendChild(power);

            XmlElement sensitivity = doc.CreateElement(string.Empty, "sensitivity", string.Empty);
            text = a.sensitivity.ToString();
            XmlText sensitivityText = doc.CreateTextNode(text);
            sensitivity.AppendChild(sensitivityText);
            conf.AppendChild(sensitivity);

            XmlElement readTime = doc.CreateElement(string.Empty, "readTime", string.Empty);
            conf.AppendChild(readTime);

            //string xml = doc.InnerXml;

            return doc;

        }

        /// <summary>
        /// Function to retrieve the antenna configuration of a device
        /// </summary>
        /// <param name="d">The device to retrieve the antenna configuration</param>
        /// <returns>A list of Antenna objects containing the antenna configuration of the device</returns>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Xml.XmlException"></exception>
        /// <exception cref="System.Xml.XPath.XPathException"></exception>
        /// <exception cref="System.OverflowException"></exception>
        public List<Antenna> parseAntennas(Device d, bool displayResponse)
        {
            /**
             * Build the URL
             * The URL depends on the id of the device
             */
            String URL = "http://" + this.address + ":3161/devices/" + d.id + "/antennas";

            /**
             * Access the URL to retrieve the content
             */
            String xmlFile = this.getFileFromURL(URL);

            /**
             * Print file
             */
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }

            var antennas = new List<Antenna>();

            String data = "";

            XmlDocument inventory = new XmlDocument();
            inventory.LoadXml(xmlFile);
            XmlNodeList elements = inventory.SelectNodes("//response/data/entries/entry/def");

            foreach (XmlElement element in elements)
            {
                data = element.InnerText;
                String[] dataSplit = data.Split(',');
                Antenna antenna = null;
                if (dataSplit.Length >= 9)
                {

                    XmlNodeList elements2 = inventory.SelectNodes("//response/data/entries/entry/conf/power");
                    String power = "";
                    foreach (XmlElement element2 in elements2)
                    {
                        power = element2.InnerText;
                    }

                    elements2 = inventory.SelectNodes("//response/data/entries/entry/conf/sensitivity");
                    String sensitivity = "";
                    foreach (XmlElement element2 in elements2)
                    {
                        sensitivity = element2.InnerText;
                    }

                    antenna = new Antenna(dataSplit[1], dataSplit[2], dataSplit[3]
                            , dataSplit[4], dataSplit[5], dataSplit[6], dataSplit[7], dataSplit[8]);

                    if (!power.Equals(""))
                        antenna.setPower(power);

                    if (!sensitivity.Equals(""))
                        antenna.setSensitivity(sensitivity);

                    antennas.Add(antenna);
                }
                else
                {
                    throw new SystemException("[ERROR] Bad antenna configuration");
                }
            }


            return antennas;
        }

        /// <summary>
        /// Function to print a set of antennas
        /// </summary>
        /// <param name="antennas">A list of Antenna objects</param>
        public void printAntennas(List<Antenna> antennas)
        {
            foreach (Antenna antenna in antennas)
            {
                Console.WriteLine(antenna.ToString());
            }
        }

        public void sequentialInventory(Device d, bool displayResponse)
        {
            /**
             * Build the URL
             * The URL depends on the id of the device
             */
            String URL = "http://" + this.address + ":3161/devices/" + d.id + "/inventory";

            /**
             * Access the URL to retrieve the content
             */
            String xmlFile = this.getFileFromURL(URL);

            /**
             * Print file
             */
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }

            /**
             * Parse parameters file.
             * 
             * The parsing of an xml file can be done using several different aproaches,
             * we have used the xml path language but any approach is ok.
             * 
             * The goal is to retrieve the list of parameter names.
             * 
             */
            Regex regex = new Regex("<epc>(.*?)</epc>");
            Match match = regex.Match(xmlFile);

            try
            {
                while (match.Success)
                {
                    Console.WriteLine("[EPC] " + match.Groups[1].Value);
                    match = match.NextMatch();
                }

            }
            catch (System.NullReferenceException nre)
            {
                Console.WriteLine("NullReferenceException: " + nre.Source);
            }
        }

        public List<string> getSequentialInventory(Device d, bool location, bool displayResponse)
        {
            /**
             * Build the URL
             * The URL depends on the id of the device
             */
            String URL = String.Empty;
            if (location)
                URL = "http://" + this.address + ":3161/devices/" + d.id + "/location";
            else
                URL = "http://" + this.address + ":3161/devices/" + d.id + "/inventory";

            /**
             * Access the URL to retrieve the content
             */
            String xmlFile = this.getFileFromURL(URL);

            /**
             * Print file
             */
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }

            /**
             * Parse parameters file.
             * 
             * The parsing of an xml file can be done using several different aproaches,
             * we have used the xml path language but any approach is ok.
             * 
             * The goal is to retrieve the list of parameter names.
             * 
             */
            Regex regex = new Regex("<epc>(.*?)</epc>");
            Match match = regex.Match(xmlFile);

            List<string> lepcs = new List<string>();
            try
            {
                while (match.Success)
                {
                    //Console.WriteLine("[EPC] " + match.Groups[1].Value);
                    lepcs.Add(match.Groups[1].Value);
                    match = match.NextMatch();
                }

            }
            catch (System.NullReferenceException nre)
            {
                Console.WriteLine("NullReferenceException: " + nre.Source);
            }

            return lepcs;
        }

        /// <summary>
        /// Function that changes the read time in the Sequential mode
        /// </summary>
        /// <param name="d">The device to be change</param>
        /// <param name="sequentialReadTime">The new read time</param>
        /// <param name="advanNetVersion">The AdvanNet version of the device, there is an ENUM in the RestUtil describing the types </param>
        /// <exception cref="SystemException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.InvalidCastException"></exception>
        /// <exception cref="System.Xml.XPath.XPathException"></exception>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.Text.EncoderFallbackException"></exception>
        /// <exception cref="System.OverflowException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public void setSequentialReadTime(Device d, long sequentialReadTime, String advanNetVersion, bool displayResponse)
        {

            if (!this.getReadMode(d, false).Equals("SEQUENTIAL"))
                throw new SystemException("[ERROR] Read mode is not Sequential");

            if (AdvanNetVersion.a21.ToString().Equals(advanNetVersion))
            {
                this.setSequentialReadTimeAN21(d, sequentialReadTime, displayResponse);
            }
            else if (AdvanNetVersion.a23.ToString().Equals(advanNetVersion))
            {
                this.setSequentialReadTimeAN23(d, sequentialReadTime, displayResponse);
            }
            else
            {
                throw new SystemException("[ERROR] Wrong AdvanNet Version");
            }
        }

        /// <summary>
        /// Function to change the read time on AdvanNet version 2.1.x
        /// </summary>
        /// <param name="d">The device to be change</param>
        /// <param name="sequentialReadTime">The new read time</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.Xml.XmlException"></exception>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.Text.EncoderFallbackException"></exception>
        /// <exception cref="System.OverflowException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        private void setSequentialReadTimeAN21(Device d, long sequentialReadTime, bool displayResponse)
        {
            /**
		     * Build the URL The URL depends on the id of the device
		     */
            String URL = "http://" + address + ":3161/devices/" + d.id + "/readModes[0]";

            XmlDocument doc = new XmlDocument();

            XmlElement xml_object = doc.CreateElement(string.Empty, "object", string.Empty);
            doc.AppendChild(xml_object);

            XmlElement oclass = doc.CreateElement(string.Empty, "class", string.Empty);
            XmlText oClassText = doc.CreateTextNode("SPEC");
            oclass.AppendChild(oClassText);
            xml_object.AppendChild(oclass);

            XmlElement oclassname = doc.CreateElement(string.Empty, "classname", string.Empty);
            XmlText oclassnamesText = doc.CreateTextNode("READMODE_SEQUENTIAL");
            oclassname.AppendChild(oclassnamesText);
            xml_object.AppendChild(oclassname);

            XmlElement oparams = doc.CreateElement(string.Empty, "params", string.Empty);
            xml_object.AppendChild(oparams);

            XmlElement oparam = doc.CreateElement(string.Empty, "param", string.Empty);
            oparams.AppendChild(oparam);

            XmlElement pclass = doc.CreateElement(string.Empty, "class", string.Empty);
            XmlText pclassText = doc.CreateTextNode("SPEC_PARAM");
            pclass.AppendChild(pclassText);
            oparam.AppendChild(pclass);

            XmlElement ptype = doc.CreateElement(string.Empty, "type", string.Empty);
            XmlText ptypeText = doc.CreateTextNode("select");
            ptype.AppendChild(ptypeText);
            oparam.AppendChild(ptype);

            XmlElement ptypedef = doc.CreateElement(string.Empty, "typedef", string.Empty);
            XmlText ptypedefText = doc.CreateTextNode("INTEGER");
            ptypedef.AppendChild(ptypedefText);
            oparam.AppendChild(ptypedef);

            XmlElement pname = doc.CreateElement(string.Empty, "name", string.Empty);
            XmlText pnameText = doc.CreateTextNode("readTime");
            pname.AppendChild(pnameText);
            oparam.AppendChild(pname);

            XmlElement pvalue = doc.CreateElement(string.Empty, "value", string.Empty);
            XmlText pvalueText = doc.CreateTextNode(sequentialReadTime.ToString());
            pvalue.AppendChild(pvalueText);
            oparam.AppendChild(pvalue);

            string antennaConfiguration = doc.InnerXml;

            String xmlFile = this.getFileFromURL(URL, antennaConfiguration);
            
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }

            if (xmlFile.Contains("ERROR"))
                throw new SystemException("[ERROR] Sequential read was not set successfully");
        }

        /// <summary>
        /// Function to change the read time on AdvanNet version 2.3.x
        /// </summary>
        /// <param name="d">The device to be change</param>
        /// <param name="sequentialReadTime">The new read time</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.Xml.XmlException"></exception>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.Text.EncoderFallbackException"></exception>
        /// <exception cref="System.OverflowException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Xml.XPath.XPathException"></exception>
        /// <exception cref="SystemException"></exception>
        private void setSequentialReadTimeAN23(Device d, long sequentialReadTime, bool displayResponse)
        {
            /**
             * Build the URL The URL depends on the id of the device
             */
            String URL = "http://" + address + ":3161/devices/" + d.id
                    + "/readModes[0]";

            String paramURL = "http://" + address + ":3161/devices/" + d.id
                    + "/readModes[0]/paramSpec";

            Dictionary<String, String> hmparams = this.getReadModeParamSpec(paramURL, false);

            XmlDocument doc = new XmlDocument();

            XmlElement request = doc.CreateElement(string.Empty, "request", string.Empty);
            doc.AppendChild(request);

            XmlElement rname = doc.CreateElement(string.Empty, "name", string.Empty);
            XmlText rnameText = doc.CreateTextNode("SEQUENTIAL");
            rname.AppendChild(rnameText);
            request.AppendChild(rname);

            XmlElement rxxx1 = doc.CreateElement(string.Empty, "xxx", string.Empty);
            XmlText rxxx1Text = doc.CreateTextNode("undefined");
            rxxx1.AppendChild(rxxx1Text);
            request.AppendChild(rxxx1);

            XmlElement rxxx2 = doc.CreateElement(string.Empty, "xxx", string.Empty);
            XmlText rxxx2Text = doc.CreateTextNode("undefined");
            rxxx2.AppendChild(rxxx2Text);
            request.AppendChild(rxxx2);

            XmlElement rxxx3 = doc.CreateElement(string.Empty, "xxx", string.Empty);
            XmlText rxxx3Text = doc.CreateTextNode("undefined");
            rxxx3.AppendChild(rxxx3Text);
            request.AppendChild(rxxx3);

            foreach (KeyValuePair<string, string> entry in hmparams)
            {
                XmlElement element = doc.CreateElement(string.Empty, entry.Key, string.Empty);
                if (!entry.Value.Equals(""))
                {
                    if (entry.Key.Equals("readTime"))
                    {
                        element.AppendChild(doc.CreateTextNode(sequentialReadTime.ToString()));
                    }
                    else
                    {
                        element.AppendChild(doc.CreateTextNode(entry.Value));
                    }
                }
                request.AppendChild(element);
            }

            String xml = doc.InnerXml;
            String xmlFile = this.getFileFromURL(URL, xml);
            
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }
            
            if (xmlFile.Contains("ERROR"))
                throw new SystemException("[ERROR] Sequential read was not set successfully");
        }

        /// <summary>
        /// Function that reads the parameters of the read mode
        /// </summary>
        /// <param name="paramURL">The URL to retrieve the parameters of that read mode, 
        ///     only tested with readModes[0]/paramSpec</param>
        /// <returns>A Dictionary with all the elements and values of readModes[0]/paramSpec</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Xml.XmlException"></exception>
        /// <exception cref="System.Xml.XPath.XPathException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// TODO Test it with all the other read modes
        private Dictionary<String, String> getReadModeParamSpec(String paramURL, bool displayResponse)
        {
            String paramsXML = this.getFileFromURL(paramURL);
            
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(paramsXML);
                Console.WriteLine("==============================");
            }
            
            Dictionary<String, String> hmparams = new Dictionary<String, String>();

            XmlDocument inventory = new XmlDocument();
            inventory.LoadXml(paramsXML);
            XmlNodeList elements = inventory.SelectNodes("//response/data/params/param");

            foreach (XmlNode ni in elements)
            {
                XmlNodeList nli = ni.ChildNodes;
                String name = null;
                foreach (XmlNode nj in nli)
                {
                    if (nj.InnerXml.Equals("name"))
                    {
                        name = nj.InnerText;
                    }
                    else if (nj.InnerXml.Equals("default"))
                    {
                        if (nj.FirstChild == null && name != null)
                        {
                            if (name.Equals("xxx"))
                                continue;
                            hmparams.Add(name, "");
                        }
                        else if (nj.FirstChild != null && name != null)
                        {
                            hmparams.Add(name, nj.FirstChild.InnerText);
                        }
                    }
                }
            }

            return hmparams;
        }

        /// <summary>
        /// Commission operation that will change the EPC, access password, and kill password.
        /// </summary>
        /// <param name="d">The device that will modify the Tag</param>
        /// <param name="oldEPC">The EPC (old) to change</param>
        /// <param name="newEPC">The new EPC</param>
        /// <param name="accessPwd">The old EPC password, if the TAG does not have a password, it empty</param>
        /// <param name="newAccessPwd">The new access password, if the password is not going to change, set it empty</param>
        /// <param name="newKillPwd">The new kill password, if the password is not going to change, set it empty</param>
		/// <param name="antenna">The antenna that will do the operation</param>
		/// <param name="displayResponse">When enabled, it will display the response to the operation</param>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.Text.EncoderFallbackException"></exception>
        /// <exception cref="System.OverflowException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="SystemException"></exception>
        public void CommissionTagOp(Device d, String oldEPC, String newEPC, 
            String accessPwd, String newAccessPwd, String newKillPwd, int antenna, bool displayResponse)
        {

            if (oldEPC.Equals("") || newEPC.Equals(""))
			    throw new SystemException("[ERROR] Old EPC or new EPC are empty");

		    String url = "http://" + this.address + ":3161/devices/" + d.id
				    + "/execOp";

            XmlDocument doc = new XmlDocument();

            XmlElement request = doc.CreateElement(string.Empty, "request", string.Empty);
            doc.AppendChild(request);

            XmlElement op = doc.CreateElement(string.Empty, "op", string.Empty);
            request.AppendChild(op);
            
            XmlElement opClass = doc.CreateElement(string.Empty, "class", string.Empty);
            opClass.AppendChild(doc.CreateTextNode("com.keonn.spec.reader.op.CommissionTagOp"));
            op.AppendChild(opClass);
            
            XmlElement e_newEPC = doc.CreateElement(string.Empty, "epc", string.Empty);
            e_newEPC.AppendChild(doc.CreateTextNode(newEPC));
            op.AppendChild(e_newEPC);

		    XmlElement e_accessPwd = doc.CreateElement(string.Empty, "accessPwd", string.Empty);
		    if (!accessPwd.Equals(""))
			    e_accessPwd.AppendChild(doc.CreateTextNode(accessPwd));
		    op.AppendChild(e_accessPwd);

		    XmlElement e_newAccessPwd = doc.CreateElement("newAccessPwd");
		    if (!newAccessPwd.Equals(""))
			    e_newAccessPwd.AppendChild(doc.CreateTextNode(newAccessPwd));
		    op.AppendChild(e_newAccessPwd);

		    XmlElement e_newKillPwd = doc.CreateElement("newKillPwd");
		    if (!newKillPwd.Equals(""))
			    e_newKillPwd.AppendChild(doc.CreateTextNode(newKillPwd));
		    op.AppendChild(e_newKillPwd);

		    XmlElement rparams = doc.CreateElement("params");
		    request.AppendChild(rparams);

		    XmlElement param1 = doc.CreateElement("param");
		    rparams.AppendChild(param1);

		    XmlElement id_param1 = doc.CreateElement("id");
		    id_param1.AppendChild(doc.CreateTextNode("GEN2_FILTER"));
		    param1.AppendChild(id_param1);

		    XmlElement obj_param1 = doc.CreateElement("obj");
		    param1.AppendChild(obj_param1);

		    XmlElement obj_param1_Class = doc.CreateElement("class");
		    obj_param1_Class.AppendChild(doc
				    .CreateTextNode("com.keonn.spec.filter.SelectTagFilter"));
		    obj_param1.AppendChild(obj_param1_Class);

		    XmlElement obj_param1_bank = doc.CreateElement("bank");
		    obj_param1_bank.AppendChild(doc.CreateTextNode("EPC"));
		    obj_param1.AppendChild(obj_param1_bank);

		    XmlElement obj_param1_bitPointer = doc.CreateElement("bitPointer");
		    obj_param1_bitPointer.AppendChild(doc.CreateTextNode("32"));
		    obj_param1.AppendChild(obj_param1_bitPointer);

		    XmlElement obj_param1_bitLength = doc.CreateElement("bitLength");
		    obj_param1_bitLength.AppendChild(doc.CreateTextNode(""
				    + (oldEPC.Length * 4)));
		    obj_param1.AppendChild(obj_param1_bitLength);

		    XmlElement obj_param1_mask = doc.CreateElement("mask");
		    obj_param1_mask.AppendChild(doc.CreateTextNode(oldEPC));
		    obj_param1.AppendChild(obj_param1_mask);

		    XmlElement param2 = doc.CreateElement("param");
		    rparams.AppendChild(param2);

		    XmlElement id_param2 = doc.CreateElement("id");
		    id_param2.AppendChild(doc.CreateTextNode("TAG_OP_ANTENNA"));
		    param2.AppendChild(id_param2);

		    XmlElement obj_param2 = doc.CreateElement("obj");
		    obj_param2.AppendChild(doc.CreateTextNode(Convert.ToString(antenna)));
		    param2.AppendChild(obj_param2);

		    String xmlFile = this.getFileFromURL(url, doc.InnerXml);
		    
		    if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }
		    
		    if (xmlFile.Contains("ERROR"))
			    throw new SystemException("[ERROR] The tag operation 'commissioning' failed");
        }

		/// <summary>
		/// Kill Tag operation.
		/// </summary>
		/// <param name="d">The device that will modify the Tag</param>
		/// <param name="killPwd">Kill password of the EPC to modify</param>
		/// <param name="epc">EPC of the tag to kill</param>
		/// <param name="antenna">The antenna that will do the operation</param>
		/// <param name="displayResponse">When enabled, it will display the response to the operation</param>
		/// <exception cref="System.NotSupportedException"></exception>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="System.Security.SecurityException"></exception>
		/// <exception cref="System.UriFormatException"></exception>
		/// <exception cref="System.Text.EncoderFallbackException"></exception>
		/// <exception cref="System.OverflowException"></exception>
		/// <exception cref="System.NotImplementedException"></exception>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="System.OutOfMemoryException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="System.InvalidOperationException"></exception>
		/// <exception cref="SystemException"></exception>
		public void killTagOp(Device d, String killPwd, String epc, int antenna, bool displayResponse){

			String url = "http://" + this.address + ":3161/devices/" + d.id
				+ "/execOp";

			XmlDocument doc = new XmlDocument();

			XmlElement request = doc.CreateElement(string.Empty, "request", string.Empty);
			doc.AppendChild(request);

			XmlElement op = doc.CreateElement(string.Empty, "op", string.Empty);
			request.AppendChild(op);

			XmlElement opClass = doc.CreateElement(string.Empty, "class", string.Empty);
			opClass.AppendChild(doc.CreateTextNode("com.keonn.spec.reader.op.KillTagOp"));
			op.AppendChild(opClass);

			XmlElement e_killPwd = doc.CreateElement(string.Empty, "killPwd", string.Empty);
			e_killPwd.AppendChild(doc.CreateTextNode(killPwd));
			op.AppendChild(e_killPwd);

			XmlElement rparams = doc.CreateElement("params");
			request.AppendChild(rparams);

			XmlElement param1 = doc.CreateElement("param");
			rparams.AppendChild(param1);

			XmlElement id_param1 = doc.CreateElement("id");
			id_param1.AppendChild(doc.CreateTextNode("GEN2_FILTER"));
			param1.AppendChild(id_param1);

			XmlElement obj_param1 = doc.CreateElement("obj");
			param1.AppendChild(obj_param1);

			XmlElement obj_param1_Class = doc.CreateElement("class");
			obj_param1_Class.AppendChild(doc
				.CreateTextNode("com.keonn.spec.filter.SelectTagFilter"));
			obj_param1.AppendChild(obj_param1_Class);

			XmlElement obj_param1_bank = doc.CreateElement("bank");
			obj_param1_bank.AppendChild(doc.CreateTextNode("EPC"));
			obj_param1.AppendChild(obj_param1_bank);

			XmlElement obj_param1_bitPointer = doc.CreateElement("bitPointer");
			obj_param1_bitPointer.AppendChild(doc.CreateTextNode("32"));
			obj_param1.AppendChild(obj_param1_bitPointer);

			XmlElement obj_param1_bitLength = doc.CreateElement("bitLength");
			obj_param1_bitLength.AppendChild(doc.CreateTextNode(""
				+ (epc.Length * 4)));
			obj_param1.AppendChild(obj_param1_bitLength);

			XmlElement obj_param1_mask = doc.CreateElement("mask");
			obj_param1_mask.AppendChild(doc.CreateTextNode(epc));
			obj_param1.AppendChild(obj_param1_mask);

			XmlElement param2 = doc.CreateElement("param");
			rparams.AppendChild(param2);

			XmlElement id_param2 = doc.CreateElement("id");
			id_param2.AppendChild(doc.CreateTextNode("TAG_OP_ANTENNA"));
			param2.AppendChild(id_param2);

			XmlElement obj_param2 = doc.CreateElement("obj");
			obj_param2.AppendChild(doc.CreateTextNode(Convert.ToString(antenna)));
			param2.AppendChild(obj_param2);

			String xmlFile = this.getFileFromURL(url, doc.InnerXml);

			if (displayResponse)
			{
				Console.WriteLine("Raw parameters data:");
				Console.WriteLine("==============================");
				Console.WriteLine(xmlFile);
				Console.WriteLine("==============================");
			}

			if (xmlFile.Contains("ERROR"))
				throw new SystemException("[ERROR] The tag operation 'killTag' failed");

		}

        /// <summary>
        /// Function to set an actuator
        /// </summary>
        /// <param name="d">The device to set the actuator</param>
        /// <param name="eventType">The event type, which changes from read modes (e.g. TAG_READ, TAG_ALARM)</param>
        /// <param name="action">An Action object defining the type of action</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.Xml.XmlException"></exception>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.Text.EncoderFallbackException"></exception>
        /// <exception cref="System.OverflowException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="SystemException"></exception>
	    public void setActuator(Device d, String eventType, Action action, bool displayResponse) 
        {
		    XmlDocument doc = new XmlDocument();
            
		    XmlElement entries = doc.CreateElement("entries");
		    doc.AppendChild(entries);
            
		    XmlElement entry = doc.CreateElement("entry");
		    entries.AppendChild(entry);

		    XmlElement eevent = doc.CreateElement("event");
		    entry.AppendChild(eevent);

		    XmlElement typeclass = doc.CreateElement("typeclass");
		    typeclass.AppendChild(doc
				    .CreateTextNode("com.keonn.spec.event.DeviceEvent$EventType"));
		    eevent.AppendChild(typeclass);

		    XmlElement type = doc.CreateElement("type");
		    type.AppendChild(doc.CreateTextNode(eventType));
		    eevent.AppendChild(type);

		    entry.AppendChild(action.toXMLElement(doc));

            String xml = doc.InnerXml;
            
		    String url ="http://" + this.address + ":3161/devices/" + d.id
				    + "/actuatorConf";

		    String xmlFile = this.getFileFromURL(url, xml);
		    
		    if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }
		    
		    if (xmlFile.Contains("ERROR"))
			    throw new SystemException("[ERROR] The actuator was not set successfully");
	    }

        /// <summary>
        /// Erase the actuators
        /// </summary>
        /// <param name="d">The device to erase the actuators</param>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.Text.EncoderFallbackException"></exception>
        /// <exception cref="System.OverflowException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.Xml.XmlException"></exception>
        /// <exception cref="SystemException"></exception>
	    public void eraseActuators(Device d, bool displayResponse) 
        {
            XmlDocument doc = new XmlDocument();

            XmlElement entries = doc.CreateElement("entries");
		    doc.AppendChild(entries);
            
		    String xml = doc.InnerXml;

		    String url = "http://" + this.address + ":3161/devices/" + d.id
				    + "/actuatorConf";

		    String xmlFile = this.getFileFromURL(url, xml);
		    
		    if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }
		    
		    if (xmlFile.Contains("ERROR"))
			    throw new SystemException("[ERROR] The actuators were not erased");
	    }

        /**
	     * Function to set the SQL parameters in the SQL_EAS_ALARM mode
	     * 
	     * @param d
	     *            The device to set the read mode parameters
	     * @param driverClass
	     *            Driver class (e.g. com.mysql.jdbc.Driver)
	     * @param connString
	     *            The connection string (e.g. jdbc:mysql://localhost/db_name)
	     * @param username
	     *            The user name
	     * @param password
	     *            The password
	     * @param queryString
	     *            The query string which will set the alarm (e.g. select paid
	     *            from sale_info where (epc=${epc} and paid=1))
	     * @param queryCacheTime
	     *            The cache time (e.g. 1500)
	     * @return
	     * @throws ParserConfigurationException
	     * @throws IOException
	     * @throws RuntimeException
	     */
	    public void setSQLParameters(Device d,
			    String driverClass, String connString, String username,
			    String password, String queryString, int queryCacheTime, bool displayResponse) 
        {

		    XmlDocument doc = new XmlDocument();

		    XmlElement xobject = doc.CreateElement("object");
		    doc.AppendChild(xobject);

		    XmlElement objectClass = doc.CreateElement("class");
		    objectClass.AppendChild(doc.CreateTextNode("SPEC"));
		    xobject.AppendChild(objectClass);

		    XmlElement objectClassName = doc.CreateElement("classname");
		    objectClassName.AppendChild(doc.CreateTextNode(RESTUtil.ReadModesClassName.READMODE_SQL_EAS_ALARM.ToString()));
		    xobject.AppendChild(objectClassName);

		    XmlElement oparams = doc.CreateElement("params");
		    xobject.AppendChild(oparams);

		    oparams.AppendChild(createParam(doc, "SPEC_PARAM", "text", "STRING",
				    "driverClass", driverClass));

		    oparams.AppendChild(createParam(doc, "SPEC_PARAM", "text", "STRING",
				    "connString", connString));

		    oparams.AppendChild(createParam(doc, "SPEC_PARAM", "text", "STRING",
				    "user", username));

		    oparams.AppendChild(createParam(doc, "SPEC_PARAM", "text", "STRING",
				    "password", password));

		    oparams.AppendChild(createParam(doc, "SPEC_PARAM", "text", "STRING",
				    "queryString", queryString));

		    oparams.AppendChild(createParam(doc, "SPEC_PARAM", "INTEGER", "STRING",
				    "queryCacheTime", "" + queryCacheTime));

		    String xml = doc.InnerXml;

		    String url = "http://" + this.address + ":3161/devices/" + d.id
				    + "/readModes[6]";

		    String xmlFile = this.getFileFromURL(url, xml);
		    
		    if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }
		    
		    if (xmlFile.Contains("ERROR"))
			    throw new SystemException("[ERROR] The SQL parameters were not setted");
	    }

        /// <summary>
        /// Write data allows to write all the banks of a Tag.
        /// </summary>
        /// <param name="d">The device that will modify the Tag</param>
        /// <param name="data">The data to write in the Tag</param>
        /// <param name="bank">The bank to write the data: USER, EPC, TID</param>
        /// <param name="offset">The byte to start writing</param>
        /// <param name="mask">The mask to filter the Tag to write</param>
        /// <param name="filterBank">The bank to filter</param>
        /// <param name="password">The password of the Tag</param>
        /// <param name="antenna">The antenna to execute the operation</param>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.Text.EncoderFallbackException"></exception>
        /// <exception cref="System.OverflowException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void WriteDataOp(Device d, string data, string bank, int offset,
            string mask, string filterBank, string password, int antenna, bool displayResponse)
        {

		    if (data.Equals(""))
			    throw new SystemException("[ERROR] Data is empty");

		    String url = "http://" + this.address + ":3161/devices/" + d.id
				    + "/execOp";
            
		    XmlDocument doc = getDataOp(data, bank, offset, mask, filterBank,
				    password, "com.keonn.spec.reader.op.WriteDataOp", antenna);

            if (doc == null)
                throw new SystemException("[ERROR] The arguments were not correct");

		    String xmlFile = this.getFileFromURL(url, doc.InnerXml);
		    
		    if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }
		    
		    if (xmlFile.Contains("ERROR"))
			    throw new SystemException("[ERROR] The tag operation 'write' failed");
        }

        /// <summary>
        /// Private function to generate the XML document that the functions writeDataOp and readDataOp use
        /// </summary>
        /// <param name="data">For the function writeDataOp, data is an hex string to write, for the function readDataOp is the length to read</param>
        /// <param name="bank">The bank to read/write the data: USER, EPC, TID</param>
        /// <param name="offset">The byte to start reading/writing</param>
        /// <param name="mask">The mask to filter</param>
        /// <param name="filterBank">The bank to filter</param>
        /// <param name="password">The password of the Tag</param>
        /// <param name="classOp">The class operation is different for each function, writeDataOp: com.keonn.spec.reader.op.WriteDataOp and readDataOp: com.keonn.spec.reader.op.ReadDataOp</param>
        /// <param name="antenna">The antenna to execute the operation</param>
        /// <returns>A XML document defining the content for the two functions: writeDataOp and readDataOp.</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        private XmlDocument getDataOp(String data, String bank, int offset,
			String mask, String filterBank, String password, String classOp,
			int antenna)
        {
		
		    if (data.Equals("") || bank.Equals("") || mask.Equals("")
				    || filterBank.Equals("") || classOp.Equals(""))
			    return null;
		
            XmlDocument doc = new XmlDocument();
            
            XmlElement request = doc.CreateElement(string.Empty, "request", string.Empty);
            doc.AppendChild(request);

		    XmlElement op = doc.CreateElement("op");
		    request.AppendChild(op);

		    XmlElement opClass = doc.CreateElement("class");
		    opClass.AppendChild(doc.CreateTextNode(classOp));
		    op.AppendChild(opClass);

		    XmlElement opBank = doc.CreateElement("bank");
		    opBank.AppendChild(doc.CreateTextNode(bank));
		    op.AppendChild(opBank);

		    XmlElement opOffset = doc.CreateElement("offset");
		    opOffset.AppendChild(doc.CreateTextNode("" + offset));
		    op.AppendChild(opOffset);

		    if (classOp.Equals("com.keonn.spec.reader.op.WriteDataOp")) {
			    XmlElement opData = doc.CreateElement("data");
			    opData.AppendChild(doc.CreateTextNode(data));
			    op.AppendChild(opData);
		    } else if (classOp.Equals("com.keonn.spec.reader.op.ReadDataOp")) {
			    XmlElement opLength = doc.CreateElement("length");
			    opLength.AppendChild(doc.CreateTextNode(data));
			    op.AppendChild(opLength);
		    }else{
			    return null;
		    }

		    XmlElement rparams = doc.CreateElement("params");
		    request.AppendChild(rparams);

		    XmlElement param1 = doc.CreateElement("param");
		    rparams.AppendChild(param1);

		    XmlElement id_param1 = doc.CreateElement("id");
		    id_param1.AppendChild(doc.CreateTextNode("GEN2_FILTER"));
		    param1.AppendChild(id_param1);

		    XmlElement obj_param1 = doc.CreateElement("obj");
		    param1.AppendChild(obj_param1);

		    XmlElement obj_param1_Class = doc.CreateElement("class");
		    obj_param1_Class.AppendChild(doc
				    .CreateTextNode("com.keonn.spec.filter.SelectTagFilter"));
		    obj_param1.AppendChild(obj_param1_Class);

		    XmlElement obj_param1_bank = doc.CreateElement("bank");
		    obj_param1_bank.AppendChild(doc.CreateTextNode(filterBank));
		    obj_param1.AppendChild(obj_param1_bank);

		    XmlElement obj_param1_bitPointer = doc.CreateElement("bitPointer");
		    obj_param1_bitPointer.AppendChild(doc.CreateTextNode("32"));
		    obj_param1.AppendChild(obj_param1_bitPointer);

		    XmlElement obj_param1_bitLength = doc.CreateElement("bitLength");
		    obj_param1_bitLength.AppendChild(doc.CreateTextNode(""
				    + (mask.Length * 4)));
		    obj_param1.AppendChild(obj_param1_bitLength);

		    XmlElement obj_param1_mask = doc.CreateElement("mask");
		    obj_param1_mask.AppendChild(doc.CreateTextNode(mask));
		    obj_param1.AppendChild(obj_param1_mask);

		    XmlElement param2 = doc.CreateElement("param");
		    rparams.AppendChild(param2);

		    XmlElement id_param2 = doc.CreateElement("id");
		    id_param2.AppendChild(doc.CreateTextNode("TAG_OP_ANTENNA"));
		    param2.AppendChild(id_param2);

		    XmlElement obj_param2 = doc.CreateElement("obj");
		    obj_param2.AppendChild(doc.CreateTextNode("" + antenna));
		    param2.AppendChild(obj_param2);

		    if (!password.Equals("")) {

			    XmlElement param3 = doc.CreateElement("param");
			    rparams.AppendChild(param3);

			    XmlElement id_param3 = doc.CreateElement("id");
			    id_param2.AppendChild(doc.CreateTextNode("GEN2_ACCESS_PASSWORD"));
			    param3.AppendChild(id_param3);

			    XmlElement obj_param3 = doc.CreateElement("obj");
			    obj_param2.AppendChild(doc.CreateTextNode(password));
			    param3.AppendChild(obj_param3);
		    }

		    return doc;
	    }

        /// <summary>
        /// Returns the content of a part of a Tag memory
        /// </summary>
        /// <param name="d">The device that will do the operation</param>
        /// <param name="bank">The bank to read the data: USER, EPC, TID</param>
        /// <param name="offset">The byte to start reading</param>
        /// <param name="length">The number of bytes to read</param>
        /// <param name="filterBank">The bank to filter</param>
        /// <param name="filterMask">The mask to filter</param>
        /// <param name="password">The password of the Tag</param>
        /// <param name="antenna">The antenna to execute the operation</param>
        /// <returns>Returns the content of a part of a Tag memory</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.Text.EncoderFallbackException"></exception>
        /// <exception cref="System.OverflowException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="SystemException"></exception>
        /// <exception cref="System.Text.RegularExpressions.RegexMatchTimeoutException"></exception>
	    public String readDataOp(Device d, String bank, int offset, int length,
			    String filterBank, String filterMask, String password,
			    int antenna, bool displayResponse)
        {

		    String url = "http://" + this.address + ":3161/devices/" + d.id
				    + "/execOp";

		    XmlDocument doc = getDataOp(("" + length), bank, offset, filterMask,
				    filterBank, password, "com.keonn.spec.reader.op.ReadDataOp",
				    antenna);

		    if (doc == null)
			    throw new SystemException("[ERROR] The arguments were not correct");

		    String xmlFile = this.getFileFromURL(url, doc.InnerXml);
		    
		    if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }
		    
		    if (xmlFile.Contains("ERROR"))
			    throw new SystemException("[ERROR] The tag operation 'read' failed");
            
            Regex regex = new Regex("<result>(.+?)</result>");
            Match match = regex.Match(xmlFile);
            while (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
	    }

        /// <summary>
        /// This function will lock a bank of a Tag, but it needs to have a password saved
        /// </summary>
        /// <param name="d">The device that will do the operation</param>
        /// <param name="actualPwd">The password of the Tag</param>
        /// <param name="lockActions">A array of strings with the lockActions, go to the rest reference document to have more information: 
        /// - ACCESS_LOCK
        /// - ACCESS_UNLOCK
        /// - ACCESS_PERMALOCK
        /// - ACCESS_PERMAUNLOCK
        /// - KILL_LOCK
        /// - KILL_UNLOCK
        /// - KILL_PERMALOCK
        /// - KILL_PERMAUNLOCK
        /// - EPC_LOCK
        /// - EPC_UNLOCK
        /// - EPC_PERMALOCK
        /// - EPC_PERMAUNLOCK
        /// - TID_LOCK
        /// - TID_UNLOCK
        /// - TID_PERMALOCK
        /// - TID_PERMAUNLOCK
        /// - USER_LOCK
        /// - USER_UNLOCK
        /// - USER_PERMALOCK
        /// - USER_PERMAUNLOCK</param>
        /// <param name="filterBank">The bank to filter</param>
        /// <param name="filterMask">The mask to filter</param>
        /// <param name="antenna">The antenna to execute the operation</param>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.Text.EncoderFallbackException"></exception>
        /// <exception cref="System.OverflowException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.OverflowException"></exception>
        /// <exception cref="SystemException"></exception>
        public void lockOp(Device d, String actualPwd, String[] lockActions,
			String filterBank, String filterMask, int antenna, bool displayResponse)
        {

		    String url = "http://" + this.address + ":3161/devices/" + d.id
				    + "/execOp";

		    XmlDocument doc = new XmlDocument();
            
		    XmlElement request = doc.CreateElement("request");
		    doc.AppendChild(request);

		    XmlElement op = doc.CreateElement("op");
		    request.AppendChild(op);

		    XmlElement opClass = doc.CreateElement("class");
		    opClass.AppendChild(doc
				    .CreateTextNode("com.keonn.spec.reader.op.LockOp"));
		    op.AppendChild(opClass);

		    XmlElement opAccessPwd = doc.CreateElement("accessPwd");
		    opAccessPwd.AppendChild(doc.CreateTextNode(actualPwd));
		    op.AppendChild(opAccessPwd);

		    XmlElement opMask = doc.CreateElement("mask");
		    opMask.AppendChild(doc.CreateTextNode("0"));
		    op.AppendChild(opMask);

		    XmlElement opAction = doc.CreateElement("action");
		    opAction.AppendChild(doc.CreateTextNode("0"));
		    op.AppendChild(opAction);

		    XmlElement opLocks = doc.CreateElement("locks");
		    String aux = "";
		    for (int i = 0; i < lockActions.Length; i++) {
			    if (i < lockActions.Length - 1)
				    aux += lockActions[i] + ",";
			    else
				    aux += lockActions[i];
		    }
		    opLocks.AppendChild(doc.CreateTextNode(aux));
		    op.AppendChild(opLocks);

		    XmlElement rparams = doc.CreateElement("params");
		    request.AppendChild(rparams);

		    XmlElement param1 = doc.CreateElement("param");
		    rparams.AppendChild(param1);

		    XmlElement id_param1 = doc.CreateElement("id");
		    id_param1.AppendChild(doc.CreateTextNode("GEN2_FILTER"));
		    param1.AppendChild(id_param1);

		    XmlElement obj_param1 = doc.CreateElement("obj");
		    param1.AppendChild(obj_param1);

		    XmlElement obj_param1_Class = doc.CreateElement("class");
		    obj_param1_Class.AppendChild(doc
				    .CreateTextNode("com.keonn.spec.filter.SelectTagFilter"));
		    obj_param1.AppendChild(obj_param1_Class);

		    XmlElement obj_param1_bank = doc.CreateElement("bank");
		    obj_param1_bank.AppendChild(doc.CreateTextNode(filterBank));
		    obj_param1.AppendChild(obj_param1_bank);

		    XmlElement obj_param1_bitPointer = doc.CreateElement("bitPointer");
		    obj_param1_bitPointer.AppendChild(doc.CreateTextNode("32"));
		    obj_param1.AppendChild(obj_param1_bitPointer);

		    XmlElement obj_param1_bitLength = doc.CreateElement("bitLength");
		    obj_param1_bitLength.AppendChild(doc.CreateTextNode(""
				    + (filterMask.Length * 4)));
		    obj_param1.AppendChild(obj_param1_bitLength);

		    XmlElement obj_param1_mask = doc.CreateElement("mask");
		    obj_param1_mask.AppendChild(doc.CreateTextNode(filterMask));
		    obj_param1.AppendChild(obj_param1_mask);

		    XmlElement param2 = doc.CreateElement("param");
		    rparams.AppendChild(param2);

		    XmlElement id_param2 = doc.CreateElement("id");
		    id_param2.AppendChild(doc.CreateTextNode("TAG_OP_ANTENNA"));
		    param2.AppendChild(id_param2);

		    XmlElement obj_param2 = doc.CreateElement("obj");
		    obj_param2.AppendChild(doc.CreateTextNode("" + antenna));
		    param2.AppendChild(obj_param2);

		    String xmlFile = this.getFileFromURL(url, doc.InnerXml);
		    
		    if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }
		    
		    if (xmlFile.Contains("ERROR"))
			    throw new SystemException("[ERROR] The tag operation 'lock' failed");

	    }

        /// <summary>
        /// Function to check the value of the NXP chip of a Tag
        /// </summary>
        /// <param name="d">The device to do the operation</param>
        /// <param name="mask">The mask to filter the Tag</param>
        /// <param name="antenna">The antenna that will do the operation</param>
        /// <returns>It will return enabled, disabled, or error</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.Text.EncoderFallbackException"></exception>
        /// <exception cref="System.OverflowException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.Xml.XmlException"></exception>
        /// <exception cref="SystemException"></exception>
        /// <exception cref="System.FormatException"></exception>
        public string NXP_EASCheck(Device d, string mask, int antenna, bool displayResponse)
        {
            XmlDocument doc = new XmlDocument();
            
            //(2) string.Empty makes cleaner code
            XmlElement request = doc.CreateElement(string.Empty, "request", string.Empty);
            doc.AppendChild(request);

            XmlElement op = doc.CreateElement(string.Empty, "op", string.Empty);
            request.AppendChild(op);

            XmlElement aclass = doc.CreateElement(string.Empty, "class", string.Empty);
            XmlText aclassText = doc.CreateTextNode("com.keonn.spec.reader.op.NXP_EASAlarm");
            aclass.AppendChild(aclassText);
            op.AppendChild(aclass);

            XmlElement aparams = doc.CreateElement(string.Empty, "params", string.Empty);
            request.AppendChild(aparams);

            XmlElement aparam1 = doc.CreateElement(string.Empty, "param", string.Empty);
            aparams.AppendChild(aparam1);

            XmlElement id = doc.CreateElement(string.Empty, "id", string.Empty);
            XmlText idText = doc.CreateTextNode("GEN2_FILTER");
            id.AppendChild(idText);
            aparam1.AppendChild(id);

            XmlElement obj = doc.CreateElement(string.Empty, "obj", string.Empty);
            aparam1.AppendChild(obj);

            XmlElement oclass = doc.CreateElement(string.Empty, "class", string.Empty);
            XmlText oclassText = doc.CreateTextNode("com.keonn.spec.filter.SelectTagFilter");
            oclass.AppendChild(oclassText);
            obj.AppendChild(oclass);

            XmlElement obank = doc.CreateElement(string.Empty, "bank", string.Empty);
            XmlText obankText = doc.CreateTextNode("EPC");
            obank.AppendChild(obankText);
            obj.AppendChild(obank);

            XmlElement obitPointer = doc.CreateElement(string.Empty, "bitPointer", string.Empty);
            XmlText obitPointerText = doc.CreateTextNode("32");
            obitPointer.AppendChild(obitPointerText);
            obj.AppendChild(obitPointer);

            XmlElement obitLength = doc.CreateElement(string.Empty, "bitLength", string.Empty);
            string a = "" + mask.Length * 4;
            XmlText obitLengthText = doc.CreateTextNode(a);
            obitLength.AppendChild(obitLengthText);
            obj.AppendChild(obitLength);

            XmlElement omask = doc.CreateElement(string.Empty, "mask", string.Empty);
            XmlText omaskText = doc.CreateTextNode(mask);
            omask.AppendChild(omaskText);
            obj.AppendChild(omask);

            XmlElement aparam2 = doc.CreateElement(string.Empty, "param", string.Empty);
            aparams.AppendChild(aparam2);

            XmlElement a2id = doc.CreateElement(string.Empty, "id", string.Empty);
            XmlText a2idText = doc.CreateTextNode("TAG_OP_ANTENNA");
            a2id.AppendChild(a2idText);
            aparam2.AppendChild(a2id);

            XmlElement a2obj = doc.CreateElement(string.Empty, "obj", string.Empty);
            XmlText a2objText = doc.CreateTextNode(Convert.ToString(antenna));
            a2obj.AppendChild(a2objText);
            aparam2.AppendChild(a2obj);

            XmlElement aparam3 = doc.CreateElement(string.Empty, "param", string.Empty);
            aparams.AppendChild(aparam3);

            XmlElement a3id = doc.CreateElement(string.Empty, "id", string.Empty);
            XmlText a3idText = doc.CreateTextNode("TAG_OP_TIMEOUT");
            a3id.AppendChild(a3idText);
            aparam3.AppendChild(a3id);

            XmlElement a3obj = doc.CreateElement(string.Empty, "obj", string.Empty);
            XmlText a3objText = doc.CreateTextNode("300");
            a3obj.AppendChild(a3objText);
            aparam3.AppendChild(a3obj);

            string xml = doc.InnerXml;
            
            String URL = "http://" + this.address + ":3161/devices/" + d.id + "/execOp";
            
            String xmlFile = this.getFileFromURL(URL, xml);
            
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }
            
            if (xmlFile.Contains("ERROR"))
            {
                throw new SystemException("[ERROR] The tag operation 'nxp eas check' failed");
            }
            else if (xmlFile.Contains("<result>"))
            {
            	Console.WriteLine("[NXP_EASCheck]\n" + this.PrintXML(xml));
            	return "enable";
            }
            else
            {
            	Console.WriteLine("[NXP_EASCheck]\n" + this.PrintXML(xml));
                return "disabled";
            }
        }

        /// <summary>
        /// Function to change the value of the NXP bit
        /// </summary>
        /// <param name="d">The device to do the operation</param>
        /// <param name="mask">The mask of the tag to do the operation on</param>
        /// <param name="accessPwd">The password of the tag, if it doesn't have a password, set it empty</param>
        /// <param name="value">True o false value of the NXP bit</param>
        /// <param name="antenna">The antenna that will do the operation</param>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.Text.EncoderFallbackException"></exception>
        /// <exception cref="System.OverflowException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.Xml.XmlException"></exception>
        /// <exception cref="SystemException"></exception>
        /// <exception cref="System.FormatException"></exception>
        public void NXP_EASChange(Device d, string mask, string accessPwd, bool value, int antenna, bool displayResponse)
        {
            XmlDocument doc = new XmlDocument();

            XmlElement request = doc.CreateElement(string.Empty, "request", string.Empty);
            doc.AppendChild(request);

            XmlElement op = doc.CreateElement(string.Empty, "op", string.Empty);
            request.AppendChild(op);

            XmlElement aclass = doc.CreateElement(string.Empty, "class", string.Empty);
            XmlText aclassText = doc.CreateTextNode("com.keonn.spec.reader.op.NXP_EASChange");
            aclass.AppendChild(aclassText);
            op.AppendChild(aclass);

            XmlElement aaccessPwd = doc.CreateElement(string.Empty, "accessPwd", string.Empty);
            XmlText aaccessPwdText = doc.CreateTextNode(accessPwd);
            aaccessPwd.AppendChild(aaccessPwdText);
            op.AppendChild(aaccessPwd);

            XmlElement aenable = doc.CreateElement(string.Empty, "enable", string.Empty);
            string sval = "";
            if (value)
                sval = "true";
            else
                sval = "false";
            XmlText aenableText = doc.CreateTextNode(sval);
            aenable.AppendChild(aenableText);
            op.AppendChild(aenable);

            XmlElement aparams = doc.CreateElement(string.Empty, "params", string.Empty);
            request.AppendChild(aparams);

            XmlElement aparam1 = doc.CreateElement(string.Empty, "param", string.Empty);
            aparams.AppendChild(aparam1);

            XmlElement id = doc.CreateElement(string.Empty, "id", string.Empty);
            XmlText idText = doc.CreateTextNode("GEN2_FILTER");
            id.AppendChild(idText);
            aparam1.AppendChild(id);

            XmlElement obj = doc.CreateElement(string.Empty, "obj", string.Empty);
            aparam1.AppendChild(obj);

            XmlElement oclass = doc.CreateElement(string.Empty, "class", string.Empty);
            XmlText oclassText = doc.CreateTextNode("com.keonn.spec.filter.SelectTagFilter");
            oclass.AppendChild(oclassText);
            obj.AppendChild(oclass);

            XmlElement obank = doc.CreateElement(string.Empty, "bank", string.Empty);
            XmlText obankText = doc.CreateTextNode("EPC");
            obank.AppendChild(obankText);
            obj.AppendChild(obank);

            XmlElement obitPointer = doc.CreateElement(string.Empty, "bitPointer", string.Empty);
            XmlText obitPointerText = doc.CreateTextNode("32");
            obitPointer.AppendChild(obitPointerText);
            obj.AppendChild(obitPointer);

            XmlElement obitLength = doc.CreateElement(string.Empty, "bitLength", string.Empty);
            string a = "" + mask.Length * 4;
            XmlText obitLengthText = doc.CreateTextNode(a);
            obitLength.AppendChild(obitLengthText);
            obj.AppendChild(obitLength);

            XmlElement omask = doc.CreateElement(string.Empty, "mask", string.Empty);
            XmlText omaskText = doc.CreateTextNode(mask);
            omask.AppendChild(omaskText);
            obj.AppendChild(omask);

            XmlElement aparam2 = doc.CreateElement(string.Empty, "param", string.Empty);
            aparams.AppendChild(aparam2);

            XmlElement a2id = doc.CreateElement(string.Empty, "id", string.Empty);
            XmlText a2idText = doc.CreateTextNode("TAG_OP_ANTENNA");
            a2id.AppendChild(a2idText);
            aparam2.AppendChild(a2id);

            XmlElement a2obj = doc.CreateElement(string.Empty, "obj", string.Empty);
            XmlText a2objText = doc.CreateTextNode(Convert.ToString(antenna));
            a2obj.AppendChild(a2objText);
            aparam2.AppendChild(a2obj);

            XmlElement aparam3 = doc.CreateElement(string.Empty, "param", string.Empty);
            aparams.AppendChild(aparam3);

            XmlElement a3id = doc.CreateElement(string.Empty, "id", string.Empty);
            XmlText a3idText = doc.CreateTextNode("TAG_OP_TIMEOUT");
            a3id.AppendChild(a3idText);
            aparam3.AppendChild(a3id);

            XmlElement a3obj = doc.CreateElement(string.Empty, "obj", string.Empty);
            XmlText a3objText = doc.CreateTextNode("300");
            a3obj.AppendChild(a3objText);
            aparam3.AppendChild(a3obj);

            string xml = doc.InnerXml;

            String URL = "http://" + this.address + ":3161/devices/" + d.id + "/execOp";

            String xmlFile = this.getFileFromURL(URL, xml);
            
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }
            
            if (xmlFile.Contains("ERROR"))
                throw new SystemException("[ERROR] The tag operation 'nxp eas change' failed");
            
            Console.WriteLine("[NXP_EASChange]\n" + this.PrintXML(xml));
        }

        /// <summary>
        /// Setting a GPO to high (true) or low (false)
        /// </summary>
        /// <param name="d">The device to change the state of a GPO</param>
        /// <param name="gpo">An integer to determine the GPO</param>
        /// <param name="state">A boolean defining the state of the GPO</param>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="SystemException"></exception>
        public void setGPO(Device d, int gpo, bool state, bool displayResponse)
        {
            /**
             * Build the URL
             * The URL depends on the id of the device
             */

            String URL = "http://" + this.address + ":3161/devices/" + d.id
                + "/setGPO/" + gpo + "/" + state;

            String xmlFile = this.getFileFromURL(URL);

            /**
             * Print device file
             */
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }

            if (xmlFile.Contains("ERROR") || xmlFile.Contains("error"))
                throw new SystemException("[ERROR] The GPO could not be changed");
        }

        /// <summary>
        /// Retrieving the state of all the GPIO
        /// </summary>
        /// <param name="d">The device to retrieve the GPO data</param>
        /// <returns>Returns the XML file with all the GPIO information</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public String getGPIOAll(Device d, bool displayResponse)
        {

            String URL = "http://" + this.address + ":3161/devices/" + d.id 
                + "/gpioAll";

            String xmlFile = this.getFileFromURL(URL);

            /**
             * Print device file
             */
            if (displayResponse)
            {
                Console.WriteLine("Raw parameters data:");
                Console.WriteLine("==============================");
                Console.WriteLine(xmlFile);
                Console.WriteLine("==============================");
            }

            if (xmlFile.Contains("ERROR") || xmlFile.Contains("error"))
                throw new SystemException("[ERROR] The GPIO data could not be retrieved");
            return xmlFile;
        }

        public String PrintXML(String XML)
        {
            String Result = "";

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(XML);

                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                String FormattedXML = sReader.ReadToEnd();

                Result = FormattedXML;
            }
            catch (XmlException)
            {
            }

            mStream.Close();
            writer.Close();

            return Result;
        }

		public bool changeDate(Device d, String newDate, bool displayResponse) {
			String URL = "http://" + this.address + ":3161/devices/" + d.id + "/MCU/parameter/MCU_DATETIME";

			String xmlFile = this.getFileFromURL(URL, newDate);

			if (displayResponse)
			{
				Console.WriteLine("Raw parameters data:");
				Console.WriteLine("==============================");
				Console.WriteLine(xmlFile);
				Console.WriteLine("==============================");
			}

			if(xmlFile.Contains("<status>OK</status")) {
				return true;
			}
			else {
				return false;
			}
		}

		public bool changeTimeZone(Device d, String newTimeZone, bool displayResponse) {
			String URL = "http://" + this.address + ":3161/devices/" + d.id + "/MCU/parameter/MCU_TIMEZONE";

			String xmlFile = this.getFileFromURL(URL, newTimeZone);

			if (displayResponse)
			{
				Console.WriteLine("Raw parameters data:");
				Console.WriteLine("==============================");
				Console.WriteLine(xmlFile);
				Console.WriteLine("==============================");
			}

			if(xmlFile.Contains("<status>OK</status")) {
				return true;
			}
			else {
				return false;
			}
		}

		public List<String> getTimeZones(Device d, bool displayResponse) {
			String URL = "http://" + this.address + ":3161/devices/" + d.id + "/MCU/parameter/MCU_TIMEZONE_LIST";

			String xmlFile = this.getFileFromURL(URL);

			if (displayResponse)
			{
				Console.WriteLine("Raw parameters data:");
				Console.WriteLine("==============================");
				Console.WriteLine(xmlFile);
				Console.WriteLine("==============================");
			}

			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xmlFile);

			List<String> tzs = new List<String> ();

			XmlNodeList elements = xmlDocument.SelectNodes("//response/data/result/text()");
			foreach (XmlNode element in elements) {
				string timezones = element.InnerText;
				string[] atz = timezones.Split (',');
				for (int i = 0; i < atz.Length; i++) {
					tzs.Add (atz[i]);
				}
			}

			return tzs;
		}
    }
}