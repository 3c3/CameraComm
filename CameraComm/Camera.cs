using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace CameraComm
{
    public class Camera
    {
        static byte[] header = { 0x56, 0x00 };
        SerialPort port;
        byte lastError;

        public Camera(string portName, int baud)
        {
            port = new SerialPort(portName, baud, Parity.None, 8);
            port.Open();
        }

        public int LastError
        {
            get { return lastError; }
        }

        public void Reset()
        {
            port.Write(header, 0, 2);
            byte[] cmdBuff = { 0x26, 0x00 };
            port.Write(cmdBuff, 0, 2);

            byte[] recvBuff = new byte[4];
            while (port.BytesToRead < 4) ;
            port.Read(recvBuff, 0, 4);

            Thread.Sleep(500);
            byte[] dumpBuff = new byte[128];
            port.Read(dumpBuff, 0, port.BytesToRead);
        }

        public bool StopFrame()
        {
            port.Write(header, 0, 2);
            byte[] cmdBuff = { 0x36, 0x01, 0x00 };
            port.Write(cmdBuff, 0, 3);

            byte[] recvBuff = new byte[5];
            while (port.BytesToRead < 5);

            port.Read(recvBuff, 0, 5);
            byte state = recvBuff[3];
            if (state != 0) lastError = state;

            return state == 0;
        }

        public bool ResumeFrame()
        {
            port.Write(header, 0, 2);
            byte[] cmdBuff = { 0x36, 0x01, 0x03 };
            port.Write(cmdBuff, 0, 3);

            byte[] recvBuff = new byte[5];
            while (port.BytesToRead < 5) ;

            port.Read(recvBuff, 0, 5);
            byte state = recvBuff[3];
            if (state != 0) lastError = state;

            return state == 0;
        }

        public bool StepFrame()
        {
            return ResumeFrame() && StopFrame();
            // По някаква причина не работи (тествано на 640x480)
            /*
            port.Write(header, 0, 2);
            byte[] cmdBuff = { 0x36, 0x01, 0x02 };
            port.Write(cmdBuff, 0, 3);

            byte[] recvBuff = new byte[5];
            while (port.BytesToRead < 5) ;

            port.Read(recvBuff, 0, 5);
            byte state = recvBuff[3];
            if (state != 0) lastError = state;

            return state == 0;
            */
        }

        public int GetImageLength()
        {
            port.Write(header, 0, 2);
            byte[] cmdBuff = { 0x34, 0x00, 0x01 };
            port.Write(cmdBuff, 0, 3);

            byte[] recvBuff = new byte[5];
            while (port.BytesToRead < 5) ;

            port.Read(recvBuff, 0, 5);
            byte state = recvBuff[3];
            if (state != 0)
            {
                lastError = state;
                return -1;
            }

            int len = recvBuff[4];
            if (len != 4) return -1;

            port.Read(recvBuff, 0, 4);
            return (recvBuff[0] << 24) | (recvBuff[1] << 16) | (recvBuff[2] << 8) | (recvBuff[0]);
        }

        public bool ReadImage(int offset, int length, byte[] recvBuff, int buffOffset)
        {
            if (offset%8 != 0) return false;
            port.Write(header, 0, 2);//1    2     3     4     5     6     7     8     9     10    11    12    13
            byte[] cmdBuff = { 0x32, 0x0C, 0x00, 0x0A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2F, 0xCC, 0x0B, 0xB8 };

            cmdBuff[4] = (byte)(offset >> 24);
            cmdBuff[5] = (byte)((offset >> 16) & 0xFF);
            cmdBuff[6] = (byte)((offset >> 8) & 0xFF);
            cmdBuff[7] = (byte)(offset & 0xFF);

            cmdBuff[8] = (byte)(length >> 24);
            cmdBuff[9] = (byte)((length >> 16) & 0xFF);
            cmdBuff[10] = (byte)((length >> 8) & 0xFF);
            cmdBuff[11] = (byte)(length & 0xFF);

            port.Write(cmdBuff, 0, 14);
            while (port.BytesToRead < 5) ;
            port.Read(cmdBuff, 0, 5);
            byte state = cmdBuff[3];
            if(state != 0)
            {
                lastError = state;
                return false;
            }

            int read = 0;
            while(read < length)
            {
                if(port.BytesToRead > 0)
                {
                    int btr = port.BytesToRead;
                    int max = length - read;
                    if (btr > max) btr = max;
                    read += port.Read(recvBuff, buffOffset + read, btr);
                    Console.WriteLine(read);
                }
            }
            //while (port.BytesToRead < length) ;
            //port.Read(recvBuff, buffOffset, length);

            while (port.BytesToRead < 5) ;
            port.Read(cmdBuff, 0, 5);

            return true;
        }

        public bool ChangeResolution(CameraResolution res)
        {
            byte[] cmdBuff = {0x56, 0x00, 0x31, 0x05, 0x04, 0x01, 0x00, 0x19, res.Value };
            port.Write(cmdBuff, 0, cmdBuff.Length);

            while (port.BytesToRead < 5) ;
            port.Read(cmdBuff, 0, 5);
            byte state = cmdBuff[3];
            if(state!=0)
            {
                lastError = state;
                return false;
            }
            
            

            return true;
        }

        private bool ArrCompare(byte[] arr1, byte[] arr2)
        {
            if (arr1.Length != arr2.Length) return false;
            for (int i = 0; i < arr1.Length; i++) if (arr1[i] != arr2[i]) return false;
            return true;
        }
    }
}
