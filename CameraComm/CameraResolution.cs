using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraComm
{
    public class CameraResolution
    {
        string name;
        byte value;

        public byte Value
        {
            get { return value; }
        }

        public CameraResolution(string name, byte val)
        {
            this.name = name;
            this.value = val;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
