using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace CameraComm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SerialPort serialPort;
        DataBuffer buffer = new DataBuffer();

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            foreach (string port in SerialPort.GetPortNames()) comboBox1.Items.Add(port);
        }

        void OpenPort()
        {
            if (serialPort != null) return;
            int baud = int.Parse(textBox1.Text);
            try
            {
                serialPort = new SerialPort(comboBox1.Text, baud, Parity.None, 8);
                serialPort.DataReceived += serialPort_DataReceived;
                serialPort.Open();
            }
            catch(Exception e)
            {
                serialPort = null;
                Console.WriteLine(e);
                return;
            }
            
            
            button1.Text = "Close";
        }

        void ClosePort()
        {
            if (serialPort == null) return;
            try
            {
                serialPort.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }            
            serialPort = null;
            button1.Text = "Open";
        }

        void SendData()
        {
            if (serialPort == null) return;
            byte[] bytes;
            try
            {
                bytes = Hexxer.ToBytes(textBox2.Text);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return;
            }

            serialPort.Write(bytes, 0, bytes.Length);
        }

        byte[] incBuff = new byte[16384];

        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (radioButton1.Checked)
            {
                //if(nlTimer.Enabled) nlTimer.Stop();
                int read = serialPort.Read(incBuff, 0, serialPort.BytesToRead);
                Console.Write(Hexxer.ToHexString(incBuff, read));
                nlTimer.Start();
            }
            else buffer.ReadFromPort(serialPort);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (serialPort == null) OpenPort();
            else ClosePort();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //label3.Text = buffer.Size.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            buffer.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if(sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                buffer.SaveToFile(sfd.FileName);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine();
            SendData();
        }

        private void nlTimer_Tick(object sender, EventArgs e)
        {
            Console.WriteLine();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Console.WriteLine();
        }

        byte[] resetBuff = { 0x56, 0, 0x26, 0x00 };

        private void timer1_Tick(object sender, EventArgs e)
        {
            Console.WriteLine(resetBuff[1]);
            serialPort.Write(resetBuff, 0, 4);

            if (resetBuff[1] != 255) resetBuff[1]++;
            else
            {
                timer1.Stop();
                resetBuff[1] = 0;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }
    }
}
