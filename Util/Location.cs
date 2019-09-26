using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public class Location
    {
        public int x, y, z;

        public Location(String loc)
        {
            string[] coordinates = loc.Split(',');

            if (coordinates.Length >= 4)
            {
                x = int.Parse(coordinates[1]);
                y = int.Parse(coordinates[2]);
                z = int.Parse(coordinates[3]);
            }

        }

        public override String ToString()
        {
            return "x: " + x + ", y: " + y + ", z: " + z;
        }
    }
}
