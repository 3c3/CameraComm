using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;

namespace CameraComm
{
    public class DataBuffer
    {
        private byte[] buff;
        private int idx;

        public DataBuffer()
        {
            buff = new byte[1048576];
            idx = 0;
        }

        public int Size
        {
            get { return idx; }
        }

        public void ReadFromPort(SerialPort sp)
        {
            int read = sp.Read(buff, idx, sp.BytesToRead);
            idx += read;
        }

        public void Clear()
        {
            idx = 0;
        }

        public void SaveToFile(string path)
        {
            if (idx == 0) return;
            byte[] fileBuff = new byte[idx-10];
            for (int i = 5; i < idx - 5; i++) fileBuff[i-5] = buff[i];
            File.WriteAllBytes(path, fileBuff);
            Clear();
        }
    }
}
