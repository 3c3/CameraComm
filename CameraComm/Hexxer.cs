using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraComm
{
    public abstract class Hexxer
    {
        public static byte ToByte(string s)
        {
            if (s.Length > 2) throw new FormatException();
            byte result = 0;
            foreach(char c in s)
            {
                result*=16;
                if (c >= '0' && c <= '9') result += (byte)(c - '0');
                else if (c >= 'A' && c <= 'F') result += (byte)(c - 'A' + 10);
                else throw new FormatException();
            }
            return result;
        }

        public static byte[] ToBytes(string s)
        {
            List<byte> bytes = new List<byte>();
            s = s.ToUpper();
            String[] parts = s.Split(' ');
            foreach (string part in parts) bytes.Add(ToByte(part));
            return bytes.ToArray();
        }

        static char[] symbols = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' }; 

        public static string ToHexString(byte[] bytes, int lenght)
        {
            string result = "";
            for (int i = 0; i < lenght; i++ )
            {
                byte b = bytes[i];
                int high = b >> 4;
                int low = b & 15;
                result += symbols[high];
                result += symbols[low];
                result += " ";
            }
            return result;
        }
    }
}
