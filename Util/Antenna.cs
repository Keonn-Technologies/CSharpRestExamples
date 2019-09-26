using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public class Antenna
    {
        public int readerPort, mux1, mux2, orientation, x, y, z, power, sensitivity;
        public String location;

        public Antenna(int readerPort, int mux1, int mux2)
        {
            this.readerPort = readerPort;
            this.mux1 = mux1;
            this.mux2 = mux2;

            this.orientation = 0;
            this.x = this.y = this.z = 0;
            this.power = this.sensitivity = 0;
            this.location = "loc_id";
        }

        public Antenna(int readerPort, int mux1, int mux2, int orientation, String location, int x, int y, int z)
        {
            this.readerPort = readerPort;
            this.mux1 = mux1;
            this.mux2 = mux2;
            this.orientation = orientation;
            this.location = location;
            this.x = x;
            this.y = y;
            this.z = z;
            this.power = this.sensitivity = 0;
        }

        public Antenna(String readerPort, String mux1, String mux2, String orientation, String location, String x, String y, String z)
        {
            try
            {
                this.readerPort = int.Parse(readerPort);
                this.mux1 = int.Parse(mux1);
                this.mux2 = int.Parse(mux2);
                this.orientation = int.Parse(orientation);
                this.location = location;
                this.x = int.Parse(x);
                this.y = int.Parse(y);
                this.z = int.Parse(z);
                this.power = this.sensitivity = 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Source);
            }
        }

        public Antenna(int readerPort, int mux1, int mux2, int orientation, String location, int x, int y, int z, int power, int sensitivity)
        {
            this.readerPort = readerPort;
            this.mux1 = mux1;
            this.mux2 = mux2;
            this.orientation = orientation;
            this.location = location;
            this.x = x;
            this.y = y;
            this.z = z;
            this.power = power;
            this.sensitivity = sensitivity;
        }

        public Antenna(String readerPort, String mux1, String mux2, String orientation, String location,
            String x, String y, String z, String power, String sensitivity)
        {
            try
            {
                this.readerPort = int.Parse(readerPort);
                this.mux1 = int.Parse(mux1);
                this.mux2 = int.Parse(mux2);
                this.orientation = int.Parse(orientation);
                this.location = location;
                this.x = int.Parse(x);
                this.y = int.Parse(y);
                this.z = int.Parse(z);
                this.power = int.Parse(power);
                this.sensitivity = int.Parse(sensitivity);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Source);
            }
        }

        public void setPower(int power)
        {
            this.power = power;
        }

        public void setPower(String power)
        {
            try
            {
                this.power = int.Parse(power);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Source);
            }
        }

        public void setSensitivity(int sensitivity)
        {
            this.sensitivity = sensitivity;
        }

        public void setSensitivity(String sensitivity)
        {
            try
            {
                this.sensitivity = int.Parse(sensitivity);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Source);
            }
        }

        public override string ToString()
        {
            return "Antenna [readerPort=" + readerPort + ", mux1=" + mux1
                    + ", mux2=" + mux2 + ", orientation=" + orientation
                    + ", location=" + location + ", x=" + x + ", y=" + y
                    + ", z=" + z + ", power=" + power + ", sensitivity="
                    + sensitivity + "]";
        }

    }
}
