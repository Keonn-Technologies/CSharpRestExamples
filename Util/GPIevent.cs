using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public class GPIevent
    {
        //A class for the Gpi events
        private string deviceId;
        private int line;
        private bool lowToHigh;

        public GPIevent()
        {
            this.deviceId = "null";
            this.line = -1;
            this.lowToHigh = false;
        }

        public GPIevent(string deviceId, int line, bool lowToHigh)
        {
            this.deviceId = deviceId;
            this.line = line;
            this.lowToHigh = lowToHigh;
        }

        public void setDeviceId(string deviceId)
        {
            this.deviceId = deviceId;
        }
        public string getDeviceId()
        {
            return this.deviceId;
        }

        public void setLine(int line)
        {
            this.line = line;
        }
        public void setLine(string line)
        {
            this.line = Convert.ToInt32(line);
        }
        public int getLine()
        {
            return this.line;
        }

        public void setLowToHigh(bool lowToHigh)
        {
            this.lowToHigh = lowToHigh;
        }
        public void setLowToHigh(string lowToHigh)
        {
            this.lowToHigh = Convert.ToBoolean(lowToHigh);
        }
        public bool getLowToHigh()
        {
            return this.lowToHigh;
        }

        public string ToString()
        {
            string gpi = "";

            gpi = "GPI Event - Device: " + this.deviceId;
            gpi += " - Line: " + this.line.ToString();
            gpi += " - LowToHigh: " + this.lowToHigh.ToString();

            return gpi;
        }
    }
}
