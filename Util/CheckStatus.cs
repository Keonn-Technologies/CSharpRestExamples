using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    class CheckStatus
    {
        //when debug is true the program will print all the data sent by AdvanNet
        private bool debug;
        //List of Keonn Systems
        List<String> keonnSystemsIPaddress;

        public CheckStatus(List<String> keonnSystemsIPaddress, bool debug)
        {
            this.keonnSystemsIPaddress = keonnSystemsIPaddress;
            this.debug = debug;
        }

        public void checkStatus()
        {
            foreach (String keonnSystemIPaddress in keonnSystemsIPaddress)
            {
                RESTUtil util = new RESTUtil(keonnSystemIPaddress, this.debug);
                HashSet<Device> devices = util.parseDevices(false);
                if (devices == null)
                    return;
                foreach (Device d in devices)
                {
                    Console.WriteLine("Status of " + d.id + " is " + util.isRunning(d, false));
                }
            }
        }
    }
}
