using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Threading;

namespace Nekl_proj
{
    public partial class Form1 : Form
    {
        static double scale = 1.0;
        static PictureBox LeftBox = null;
        static PictureBox TopBox = null;
        static PictureBox WaterLeft = null;
        static TextBox tbox = null;
        static Size SettingsSize = new Size();
        static System.Timers.Timer Timer;
        static double WaterLevel = 100.0;
        static double time = 0;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            SettingsSize.Width = 200;
            SettingsSize.Height = Size.Height;
            TopBox = new PictureBox
            {
                Location = new Point(5, 5),
                Size = new Size(Size.Width - SettingsSize.Width - 5, Size.Height/2 - 5),
                BackColor = Color.Cyan,
                BorderStyle = BorderStyle.FixedSingle
            };

            LeftBox = new PictureBox
            {
                Location = new Point(5, Size.Height / 2 + 5),
                Size = new Size(Size.Width - SettingsSize.Width - 5, Size.Height/2 - 50),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            WaterLeft = new PictureBox
            {
                Location = new Point(5, LeftBox.Location.Y + LeftBox.Size.Height - (int)WaterLevel),
                Size = new Size(LeftBox.Size.Width, (int)WaterLevel),
                BackColor = Color.Cyan,
                BorderStyle = BorderStyle.FixedSingle
            };


            Controls.Add(WaterLeft);
            Controls.Add(LeftBox);
            Controls.Add(TopBox);
            
            timer1.Start();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            
        }

        private void TextBox1_Enter(object sender, EventArgs e)
        {
            
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            double CurrentLevel = 40;
            WaterLeft.Location = new Point(5, LeftBox.Location.Y + LeftBox.Size.Height - (int)(WaterLevel + CurrentLevel* Math.Sin(time)));
            WaterLeft.Size = new Size(LeftBox.Size.Width, (int)(WaterLevel + CurrentLevel* Math.Sin(time)));
            time += 0.05;
        }
    }
}
