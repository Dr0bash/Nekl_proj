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
        static PictureBox ShipLeft = null;
        static PictureBox ShipTop = null;
        static TextBox tbox = null;
        static Size SettingsSize = new Size();
        static Color WaterColor = Color.FromArgb(0,162,211);
        static System.Timers.Timer Timer;
        static PictureBox[,,] Containers = new PictureBox[5,3,4];
        static TrackBar WavePowerBar = null;
        static Label WavePowerText = null;
        static TrackBar AmplBar = null;
        static Label AmplText = null;
        static PictureBox[,] ContainersWeb = new PictureBox[3, 5];

        static double Ampl = 20;
        static double WaterLevel = 140.0;
        static double WavePower = 0.05;
        static double time = 0;

        static PointF wind = new PointF(0, 0);
        static PointF cur_wind = new PointF(0, 0);
        static bool wind_changed = false;
        static PointF wind_change_speed = new PointF(0,0);

        static double eps = 0.1;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Size = new Size(1280, 720);
            WindowState = FormWindowState.Maximized;
            SettingsSize.Width = 400;
            SettingsSize.Height = Size.Height;
            
            TopBox = new PictureBox
            {
                Location = new Point(5, 5),
                Size = new Size(Size.Width - SettingsSize.Width - 5, (int)(Size.Height/3.0) + 10),
                BackColor = WaterColor,
                BorderStyle = BorderStyle.FixedSingle
            };

            LeftBox = new PictureBox
            {
                Location = new Point(5, TopBox.Location.Y + TopBox.Height + 5),
                Size = new Size(Size.Width - SettingsSize.Width - 5, Size.Height - TopBox.Height - 50),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            WaterLeft = new PictureBox
            {
                Location = new Point(5, LeftBox.Location.Y + LeftBox.Size.Height - (int)WaterLevel),
                Size = new Size(LeftBox.Size.Width, (int)WaterLevel),
                BackColor = WaterColor,
                BorderStyle = BorderStyle.FixedSingle
            };

            ShipLeft = new PictureBox
            {
                Location = new Point(LeftBox.Location.X + LeftBox.Size.Width / 12, LeftBox.Location.Y + (int)(LeftBox.Size.Height / 1.088) - (int)WaterLevel),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size((int)(LeftBox.Size.Width / 1.25), (int)(LeftBox.Size.Height / 5.5)),
                BackColor = Color.White,
                Image = Properties.Resources.ShipLeft,
                Padding = new Padding(0)
            };

            ShipTop = new PictureBox
            {
                Location = new Point(TopBox.Location.X + TopBox.Size.Width / 12, TopBox.Location.Y + TopBox.Size.Height / 32),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size((int)(TopBox.Size.Width / 1.25), (int)(TopBox.Size.Height - TopBox.Size.Height / 16)), 
                BackColor = WaterColor,
                Image = Properties.Resources.ShipTop
            };

            WavePowerText = new Label
            {
                Location = new Point(TopBox.Width + 15, 30),
                Size = new Size(SettingsSize.Width - 30, 20),
                Text = "Скорость прилива/отлива",
                Font = new Font("Arial", 14, FontStyle.Regular)
            };

            WavePowerBar = new TrackBar
            {
                Location = new Point(TopBox.Width + 5, 80),
                Size = new Size(SettingsSize.Width - 20, 10),
                TickFrequency = 1,
                Minimum = 1,
                Maximum = 10,
                Value = (int)(WavePower * 100),
                SmallChange = 1,
                LargeChange = 1
            };

            WavePowerBar.ValueChanged += WavePowerBar_Changed;

            AmplText = new Label
            {
                Location = new Point(TopBox.Width + 15, WavePowerBar.Location.Y + WavePowerBar.Size.Height + 10),
                Size = new Size(SettingsSize.Width - 30, 20),
                Text = "Амплитуда волны",
                Font = new Font("Arial", 14, FontStyle.Regular)
            };

            AmplBar = new TrackBar
            {
                Location = new Point(TopBox.Width + 5, AmplText.Location.Y + AmplText.Size.Height + 10),
                Size = new Size(SettingsSize.Width - 20, 10),
                TickFrequency = 5,
                Minimum = 10,
                Maximum = 50,
                Value = (int)(Ampl),
                SmallChange = 1,
                LargeChange = 1
            };

            AmplBar.ValueChanged += AmplBar_Changed;

            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 5; ++j)
                {
                    ContainersWeb[i, j] = new PictureBox
                    {
                        Location = new Point(ShipTop.Location.X + (ShipTop.Size.Width / 24) + (int)(j* ShipTop.Size.Width / 6.34), ShipTop.Location.Y + ShipTop.Size.Height / 10 + (int)(i * ShipTop.Size.Height / 3.6)),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Size = new Size((int)(ShipTop.Size.Width / 6.74), (int)(ShipTop.Size.Height / 4.22)),
                        Image = Properties.Resources.disabled_cross,
                        BorderStyle = BorderStyle.None
                        //BorderStyle = BorderStyle.FixedSingle
                    };
                    ContainersWeb[i, j].MouseEnter += ContainersWeb_MouseEnter;
                    ContainersWeb[i, j].MouseLeave += ContainersWeb_MouseLeave;
                    ContainersWeb[i, j].MouseClick += ContainersWeb_MouseClick;
                    Controls.Add(ContainersWeb[i, j]);
                }
            }




            Controls.Add(ShipTop);
            Controls.Add(ShipLeft);
            Controls.Add(WaterLeft);
            Controls.Add(LeftBox);
            Controls.Add(TopBox);
            Controls.Add(WavePowerText);
            Controls.Add(WavePowerBar);
            Controls.Add(AmplText);
            Controls.Add(AmplBar);

            timer1.Start();
            
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            
        }

        private void WavePowerBar_Changed(object sender, EventArgs e)
        {
            WavePower = WavePowerBar.Value / 100.0;
        }

        private void AmplBar_Changed(object sender, EventArgs e)
        {
            Ampl = AmplBar.Value;
        }

        private void ContainersWeb_MouseEnter(object sender, EventArgs e)
        {
            PictureBox ContPlace = sender as PictureBox;
            ContPlace.Image = Properties.Resources.enabled_cross;
        }

        private void ContainersWeb_MouseLeave(object sender, EventArgs e)
        {
            PictureBox ContPlace = sender as PictureBox;
            ContPlace.Image = Properties.Resources.disabled_cross;
        }
        private void ContainersWeb_MouseClick(object sender, EventArgs e)
        {
            //начало поставки контейнера
        }


        private void Timer1_Tick(object sender, EventArgs e)
        {
            
            WaterLeft.Location = new Point(5, LeftBox.Location.Y + LeftBox.Size.Height - (int)(WaterLevel + Ampl * Math.Sin(time)));
            WaterLeft.Size = new Size(LeftBox.Size.Width, (int)(WaterLevel + Ampl * Math.Sin(time)));
            ShipLeft.Location = new Point(ShipLeft.Location.X, LeftBox.Location.Y + (int)(LeftBox.Size.Height / 1.088) - (int)(WaterLevel + Ampl * Math.Sin(time)));

            time += WavePower*0.8;

            if (wind_changed)
                if (Math.Abs(wind.X - cur_wind.X) + Math.Abs(wind.Y - cur_wind.Y) > eps)
                    cur_wind = Physics.Wind_to_destination(cur_wind, wind_change_speed);
        }
    }
}
