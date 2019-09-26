using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public class TagData
    {
        public String hexEpc;
        public long ts;
        public Location loc;

        public TagData(String hexEpc, String ts)
        {
            this.hexEpc = hexEpc;
            try
            {
                this.ts = long.Parse(ts);
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("Argument execption: " + ane.Source);
                this.ts = 0;
            }
            catch (FormatException fe)
            {
                Console.WriteLine("Format execption: " + fe.Source);
                this.ts = 0;
            }
            catch (OverflowException ofe)
            {
                Console.WriteLine("Overflow execption: " + ofe.Source);
                this.ts = 0;
            }
        }

        public void addLocation(String loc)
        {
            this.loc = new Location(loc);
        }
    }
}
