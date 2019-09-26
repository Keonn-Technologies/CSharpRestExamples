using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util
{
    public class Device
    {

        public enum ReadMode
        {
            AUTONOMOUS, SEQUENTIAL
        }

        public String id;
        public String family;
        public String advanNetVersion;

        public Device(String id, String family)
        {
            this.id = id;
            this.family = family;
            this.advanNetVersion = "";
        }

        public Device(String id, String family, String advanNetVersion)
        {
            this.id = id;
            this.family = family;
            this.advanNetVersion = advanNetVersion;
        }
    }
}
