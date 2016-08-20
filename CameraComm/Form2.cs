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
using System.IO;

namespace CameraComm
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            comboBox2.Items.Add(new CameraResolution("640x480", 0));
            for (int i = 1; i < 11; i++) comboBox2.Items.Add(new CameraResolution(String.Format("res {0}", i), (byte)i));
            comboBox2.Items.Add(new CameraResolution("320x240", 11));
            for (int i = 12; i < 22; i++) comboBox2.Items.Add(new CameraResolution(String.Format("res {0}", i), (byte)i));
        }

        Camera camera;

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            foreach (string port in SerialPort.GetPortNames()) comboBox1.Items.Add(port);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                camera = new Camera(comboBox1.Text, int.Parse(textBox1.Text));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                camera = null;
                return;
            }

            button1.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (camera == null) return;
            camera.Reset();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (camera == null) return;
            if (!camera.StopFrame()) MessageBox.Show("Failed");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (camera == null) return;
            if (!camera.ResumeFrame()) MessageBox.Show("Failed");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (camera == null) return;
            if (!camera.StepFrame()) MessageBox.Show("Failed");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (camera == null) return;
            int len = camera.GetImageLength();
            if (len == -1) MessageBox.Show("Failed");
            label2.Text = len.ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int offset = (int)numericUpDown1.Value;
            int len = (int)numericUpDown2.Value;

            byte[] buff = new byte[len];
            if (!camera.ReadImage(offset, len, buff, 0)) MessageBox.Show("Failed");
            else
            {
                Console.WriteLine(Hexxer.ToHexString(buff, len));
            }
        }

        int imgCount = 0;
        string fileName = "img_{0}.jpg";
        private void button9_Click(object sender, EventArgs e)
        {
            int len = camera.GetImageLength();
            if (len == -1)
            {
                MessageBox.Show("fail");
                return;
            }

            byte[] imgBuff = new byte[len];
            camera.ReadImage(0, len, imgBuff, 0);

            File.WriteAllBytes(String.Format(fileName, imgCount++), imgBuff);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            CameraResolution res = (CameraResolution)comboBox2.SelectedItem;
            if (!camera.ChangeResolution(res)) MessageBox.Show("couldn't change");
        }
    }
}
