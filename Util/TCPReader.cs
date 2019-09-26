/*
 * Created by SharpDevelop.
 * User: salmendros
 * Date: 12/14/2015
 */
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Threading;

namespace Util
{
	public class TCPReader
	{
		public static int CHAR_TAB = 9;
		public static int CHAR_NEW_LINE = 10;
		public static int CHAR_CR = 13;
		public static int BUFFER_LENGTH = 512;
		
		bool shutdown = false;
		private bool isInError;
		private bool firstRun=true;
		private RESTUtil restUtil;
		private Device device;
		
		private Socket socket;		
		string addressIP;
		ConcurrentQueue<String> queue;
		
		public TCPReader(string addressIP, ConcurrentQueue<String> queue, RESTUtil restUtil)
		{
			this.addressIP = addressIP;
			this.queue = queue;
			this.restUtil = restUtil;
			this.device = this.restUtil.parseDevice(false);
		}
		
		public void Shutdown() {
			this.shutdown = true;
		}
		
		/// <summary>
		/// 
		/// </summary>
        /// <exception cref="System.Net.Sockets.SocketException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.FormatException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="System.ObjectDisposedException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
		public void openSocket() {
        	if(this.socket!=null) {
        		this.socket.Shutdown(SocketShutdown.Both);
				this.socket.Close();
        	}
			this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			IPAddress ipAddress = IPAddress.Parse(this.addressIP);
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, 3177);
			this.socket.Connect(remoteEP);
		}
        
		public void startDevice() {	
			/**
			 * Stop the device, change the Device Mode, and 
			 * change the session
			 */
			this.restUtil.startStopDevice(device, false, false);
			//this.restUtil.setDeviceMode(device, "Autonomous", false);
			//this.restUtil.setGEN2_SESSION(device, "S0", false);
			this.restUtil.startStopDevice(device, true, false);
		}
        
		/// <summary>
		/// 
		/// </summary>
        /// <exception cref="System.Net.Sockets.SocketException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.FormatException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="System.ObjectDisposedException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
		public void run(){
			
			while(!this.shutdown){
				try {
					if(isInError || firstRun){
							
						openSocket();
						
						if(isInError){
							this.startDevice();
						}
						
						isInError=false;
						firstRun=false;
					}
						
					// read header
					String line = this.readLine(this.socket, true, true);
					if(line==null){
						isInError=true;
					}
						
					int errorCounter=0;
					bool serveRequest=false;
					while(true && !shutdown){
						if(line == null){
							isInError = true;
							break;
						}

						if(line.StartsWith("ADVANNET")){
							String version = getLastPart(line, "/");
							serveRequest = true;
							break;
						}
						
						if(errorCounter++ > 100) {
							Console.WriteLine("[" + this.addressIP 
							                  + "] Receiving garbage data[" 
							                  + line + "]. Resetting connection");
							isInError=true;
							break;
						}
						
						line = readLine(this.socket, true, true);
						if(line == null){
							isInError = true;
							break;
						}
					}
						
					if(isInError || !serveRequest){
						continue;
					}
					
					line = readLine(this.socket, true, true);
					if(line==null){
						isInError = true;
						continue;
					}
					
					int length = 0;
					try
					{
						String ss = getLastPart(line, ":");
						length = isVoid(ss)?0:Convert.ToInt32(ss);
					}
					catch(System.FormatException fe)
					{
						Console.WriteLine("Invalid header length: "+line);
						break;
					}
					catch(System.OverflowException ofe)
					{
						Console.WriteLine("Overflow exception " + ofe.StackTrace);
						break;
					}
					
					line = readLine(this.socket, true, true);
					if(line==null){
						isInError=true;
						continue;
					}

					// Read Content-type
					line = readLine(this.socket, true, true);
					if(line==null){
						continue;
					}
					
					if(length>0){

						byte[] buf = new byte[length];
						int bufferLength = length > BUFFER_LENGTH? BUFFER_LENGTH:length;
						int l=-1;
						int offset=0;
						
						while(true && !shutdown){
							l = socket.Receive(buf, offset, bufferLength, SocketFlags.None);
							offset+=l;
							
							if(offset == length) 
								break;
							
							if(length - offset > BUFFER_LENGTH){
								bufferLength = BUFFER_LENGTH;
							} else {
								bufferLength = length - offset;
							}
						}
						String rspns = System.Text.Encoding.UTF8.GetString(buf);
						
						queue.Enqueue(rspns);
						
						if(!serveRequest)
							continue;
					}

				} catch(System.Net.Sockets.SocketException se) {
        			
        			if(se.StackTrace.Contains("socket closed") && shutdown){
						Console.WriteLine("[" + this.addressIP + "] connection closed");
						
					} else {
        			
        				Console.WriteLine("Socket exception: " + se.StackTrace);
						
						isInError = true;
						try {
							int l = 2000;
							Console.WriteLine("[" + this.addressIP + "] Waiting "+ l +" ms. to reconnect.");
							
							if(shutdown)
								break;

							Thread.Sleep(l);
						} catch (System.ArgumentOutOfRangeException e1) {
							Console.WriteLine("[" + this.addressIP
							                  + "] InterruptedException caught. " 
							                  + "Terminating thread.");
							break;
						}
					}
        			
        		}
			}
			
			Console.WriteLine("End of TCPReader.");
			if(this.socket != null){
				try {
        			if(this.socket!=null) {
						this.socket.Shutdown(SocketShutdown.Both);
						this.socket.Close();
					}
				} catch (System.Net.Sockets.SocketException se) {
					Console.WriteLine("Socket exception: " + se.StackTrace);
				} catch (System.ObjectDisposedException ode) {
					Console.WriteLine("Object Disposed Exception: " + ode.StackTrace);
				}
			}
		}
		
		public string readLine(Socket socket, bool onlyReadable, bool trimLine){
			
			int bufferSize = 128;
			byte[] buffer = new byte[bufferSize];
			int index = 0;
			int lastChar = -1;
			byte[] btmp = new byte[1];
			
			while(true)
			{
				int c = socket.Receive(btmp, 1, SocketFlags.None);
				int b = Convert.ToInt32(btmp[0]);
				if (onlyReadable && !isReadable(b))
					return null;

				if (b == '\r') {
					
					int ci = socket.Receive(btmp, 1, SocketFlags.None);
					int ii = Convert.ToInt32(btmp[0]);
					if(trimLine)
						return System.Text.Encoding.UTF8.GetString(buffer, 0, index).Trim();
					else
						return System.Text.Encoding.UTF8.GetString(buffer, 0, index);
					
				} else if (b == '\n') {
					if(trimLine)
						return System.Text.Encoding.UTF8.GetString(buffer, 0, index).Trim();
					else
						return System.Text.Encoding.UTF8.GetString(buffer, 0, index);
				}
	
				lastChar = b;
	
				if (index == bufferSize) {
					byte[] tmp = new byte[bufferSize * 2];
					Buffer.BlockCopy(buffer, 0, tmp, 0, index);
					buffer = tmp;
					bufferSize *= 2;
				}
	
				buffer[index++] = (byte) b;
			}
		}
		
		private bool isReadable(int c) {
			return (c >= 32 && c <= 126) || c == CHAR_TAB || c == CHAR_NEW_LINE
					|| c == CHAR_CR;
		}
	
		public string getLastPart(string s, string separator) {
			if (s != null && separator != null) {
				int index = s.LastIndexOf(separator);
				if (index != -1)
					return s.Substring(index + separator.Length);
				else
					return s;
			}
	
			return null;
		}
	
		public Boolean isVoid(string str)
		{
			if (str == null || str.Length == 0)
				return true;

			return str.Trim().Length == 0;
		}
		
		static byte[] GetBytes(string str)
		{
		    byte[] bytes = new byte[str.Length * sizeof(char)];
		    System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
		    return bytes;
		}

	}
}
