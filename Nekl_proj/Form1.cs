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
        static PictureBox ReikaLeft = null;
        static PictureBox LulkaLeft = null;
        static PictureBox LulkaTop = null;
        static PictureBox ContTop = null;
        static PictureBox ContLeft = null;
        static TextBox tbox = null;
        static Size SettingsSize = new Size();
        static Color WaterColor = Color.FromArgb(0, 162, 211);
        static System.Timers.Timer Timer;
        static PictureBox[,,] Containers = new PictureBox[5, 3, 4];
        static TrackBar WavePowerBar = null;
        static Label WavePowerText = null;
        static TrackBar AmplBar = null;
        static Label AmplText = null;
        static PictureBox[,] ContainersWeb = new PictureBox[3, 5];
        static Control[] allObj = new Control[150];
        static Button Begin = null;
        static Tuple<int, int> PlaceForCont = new Tuple<int, int>(0,0);
        static Tuple<Point, Point> TrosLeft = null;
        static Tuple<Point, Point> TrosTop = null;

        static double Ampl = 20;
        static double WaterLevel = 120.0;
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
            WindowState = FormWindowState.Maximized;
            Size ScreenSize = Size;
            WindowState = FormWindowState.Normal;
            Size = new Size(ScreenSize.Width, ScreenSize.Height - Size.Height/64);
            //Size = new Size(1280, 720);
            Location = new Point(-7, 0);
            SettingsSize.Width = (int)(Size.Width / 7);
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
                Location = new Point(LeftBox.Location.X + LeftBox.Size.Width / 6, LeftBox.Location.Y + (int)(LeftBox.Size.Height / 1.088) - (int)WaterLevel),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size((int)(LeftBox.Size.Width / 1.25), (int)(LeftBox.Size.Height / 5.5)),
                BackColor = Color.White,
                Image = Properties.Resources.ShipLeft,
                Padding = new Padding(0)
            };

            ShipTop = new PictureBox
            {
                Location = new Point(TopBox.Location.X + TopBox.Size.Width / 6, TopBox.Location.Y + TopBox.Size.Height / 32),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size((int)(TopBox.Size.Width / 1.25), (int)(TopBox.Size.Height - TopBox.Size.Height / 16)), 
                BackColor = WaterColor,
                Image = Properties.Resources.ShipTop
            };

            WavePowerText = new Label
            {
                Location = new Point(TopBox.Width + 15, 30),
                Text = "Скорость прилива/отлива",
                Font = new Font("Arial", SettingsSize.Width / 20, FontStyle.Regular),
                AutoSize = true
            };

            WavePowerBar = new TrackBar
            {
                Location = new Point(TopBox.Width + 5, 80),
                Size = new Size(SettingsSize.Width, 20),
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
                Text = "Амплитуда волны",
                Font = new Font("Arial", SettingsSize.Width / 20, FontStyle.Regular),
                AutoSize = true
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

            //krestiki
            int sch = 10;
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 5; ++j)
                {
                    ContainersWeb[i, j] = new PictureBox
                    {
                        Location = new Point(ShipTop.Location.X + (ShipTop.Size.Width / 24) + (int)(j * ShipTop.Size.Width / 6.34), ShipTop.Location.Y + ShipTop.Size.Height / 10 + (int)(i * ShipTop.Size.Height / 3.6)),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Size = new Size((int)(ShipTop.Size.Width / 6.74), (int)(ShipTop.Size.Height / 4.22)),
                        BackColor = Color.FromArgb(128,128,128),
                        BorderStyle = BorderStyle.None,
                        Padding = new Padding(0),
                        Image = Properties.Resources.disabled_cross,
                        Tag = i*10 + j
                    };
                    ContainersWeb[i, j].MouseEnter += ContainersWeb_MouseEnter;
                    ContainersWeb[i, j].MouseLeave += ContainersWeb_MouseLeave;
                    ContainersWeb[i, j].MouseClick += ContainersWeb_MouseClick;
                    allObj[sch] = ContainersWeb[i, j];
                    sch += 1;
                }
            }

            //containers
            //Size consize = new Size((int)(LeftBox.Size.Width / 8.48), (int)(LeftBox.Size.Height / 13.9));
            //for (int i = 0; i < 4; ++i)
            //{
            //    for (int j = 0; j < 5; ++j)
            //    {

            //        allObj[sch] = new PictureBox
            //        {
            //            Location = new Point(ShipLeft.Location.X + (ShipLeft.Size.Width / 24) + j * (int)(consize.Width + ShipLeft.Size.Width / 94.5), ShipLeft.Location.Y - i * (consize.Height)),
            //            Size = consize,
            //            BorderStyle = BorderStyle.None,
            //            BackColor = Color.White

            //            Tag = i * 10 + j
            //        };
            //        sch += 1;
            //    }
            //}


            ContTop = new PictureBox
            {
                Location = new Point(ShipTop.Location.X + (ShipTop.Size.Width / 24), ShipTop.Location.Y + ShipTop.Location.Y + ShipTop.Size.Height / 10 - 17),
                Size = new Size((int)(ShipTop.Size.Width / 6.74), (int)(ShipTop.Size.Height / 4.22)),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Yellow
            };

            ContTop.Location = new Point(ContainersWeb[0, 0].Location.X + ContainersWeb[0, 0].Size.Width / 2 - ContTop.Size.Width / 2, ContainersWeb[0, 0].Location.Y + ContainersWeb[0, 0].Size.Height / 2 - ContTop.Size.Height / 2);

            ContLeft = new PictureBox
            {
                Location = new Point(LeftBox.Location.X, LeftBox.Location.Y + (int)(LeftBox.Size.Height/4.3)),
                Size = new Size((int)(LeftBox.Size.Width / 8.48), (int)(LeftBox.Size.Height / 13.9)),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Yellow
            };

            ContLeft.Location = new Point(ContainersWeb[0, 0].Location.X + ContainersWeb[0, 0].Size.Width / 2 - ContLeft.Size.Width / 2, ContLeft.Location.Y);

            ReikaLeft = new PictureBox
            {
                Location = new Point(LeftBox.Location.X + (int)(LeftBox.Size.Width/64), LeftBox.Location.Y + (int)(LeftBox.Size.Height / 32)),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size((int)(LeftBox.Size.Width/1.03), LeftBox.Size.Height / 8),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                Image = Properties.Resources.reika_krana_left
            };

            LulkaLeft = new PictureBox
            {
                Location = new Point(LeftBox.Location.X + (int)(LeftBox.Size.Width / 2), LeftBox.Location.Y + (int)(LeftBox.Size.Height / 32)),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size((int)(LeftBox.Size.Width / 19), LeftBox.Size.Height / 8),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                Image = Properties.Resources.lulka
            };

            LulkaLeft.Location = new Point(ContainersWeb[0,0].Location.X + ContainersWeb[0, 0].Size.Width / 2 - LulkaLeft.Size.Width / 2, LulkaLeft.Location.Y);

            LulkaTop = new PictureBox
            {
                Location = new Point(TopBox.Location.X + (int)(TopBox.Size.Width / 2), TopBox.Location.Y + (int)(TopBox.Size.Height / 2)),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size((int)(TopBox.Size.Width / 33), (int)(TopBox.Size.Height / 8)),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                Image = Properties.Resources.lulka
            };

            LulkaTop.Location = new Point(ContainersWeb[0, 0].Location.X + ContainersWeb[0, 0].Size.Width / 2 - LulkaTop.Size.Width / 2, ContainersWeb[0, 0].Location.Y + ContainersWeb[0, 0].Size.Height / 2 - LulkaTop.Size.Height / 2);


            Begin = new Button
            {
                Location = new Point(TopBox.Width + SettingsSize.Width / 3 + 5, AmplBar.Location.Y + AmplBar.Size.Height + 5),
                Text = "Начать",
                Font = new Font("Arial", SettingsSize.Width / 16, FontStyle.Regular),
                AutoSize = true
            };

            Begin.Click += Begin_Click;

            // Dock the PictureBox to the form and set its background to white.
            LeftBox.Dock = DockStyle.None;
            ContTop.Dock = DockStyle.None;
            // Connect the Paint event of the PictureBox to the event handler method.
            TrosLeft = new Tuple<Point, Point>(new Point(LulkaLeft.Location.X + LulkaLeft.Size.Width / 2 - LeftBox.Location.X, LulkaLeft.Location.Y + LulkaLeft.Size.Height - 1 - LeftBox.Location.Y), new Point(ContLeft.Location.X + ContLeft.Size.Width / 2 - LeftBox.Location.X, ContLeft.Location.Y - LeftBox.Location.Y));
            TrosTop = new Tuple<Point, Point>(new Point(ContTop.Size.Width / 2, ContTop.Size.Height / 2), new Point(-ContTop.Location.X+ LulkaTop.Location.X + LulkaTop.Size.Width/2,-ContTop.Location.Y + LulkaTop.Location.Y + LulkaTop.Size.Height/2));
            LeftBox.Paint += new System.Windows.Forms.PaintEventHandler(LeftBox_Paint);
            ContTop.Paint += new System.Windows.Forms.PaintEventHandler(ContTop_Paint);


            allObj[1] = LulkaTop;
            allObj[2] = ContTop;
            allObj[3] = ContLeft;
            //10-34 krestiki
            //35-54 boxes left
            allObj[100] = LulkaLeft;
            allObj[101] = ReikaLeft;
            allObj[102] = ShipTop;
            allObj[103] = ShipLeft;
            allObj[104] = WaterLeft;
            allObj[105] = LeftBox;
            allObj[106] = TopBox;
            allObj[107] = WavePowerText;
            allObj[108] = WavePowerBar;
            allObj[109] = AmplText;
            allObj[110] = AmplBar;
            allObj[111] = Begin;
           
            Controls.AddRange(allObj);

            timer1.Start();
            
        }

        private void LeftBox_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawLine(System.Drawing.Pens.Black, TrosLeft.Item1.X, TrosLeft.Item1.Y, TrosLeft.Item2.X, TrosLeft.Item2.Y);
        }

        private void ContTop_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawLine(System.Drawing.Pens.Black, TrosTop.Item1.X, TrosTop.Item1.Y, TrosTop.Item2.X, TrosTop.Item2.Y);
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
            PictureBox ContPlace = sender as PictureBox;
            LulkaLeft.Location = new Point(ContPlace.Location.X + ContPlace.Size.Width / 2 - LulkaLeft.Size.Width / 2, LulkaLeft.Location.Y);
            LulkaTop.Location = new Point(ContPlace.Location.X + ContPlace.Size.Width / 2 - LulkaTop.Size.Width / 2, ContPlace.Location.Y + ContPlace.Size.Height / 2 - LulkaTop.Size.Height / 2);
            PlaceForCont = new Tuple<int, int>((int)ContPlace.Tag / 10, (int)ContPlace.Tag % 10);
            ContLeft.Location = new Point(ContPlace.Location.X + ContPlace.Size.Width / 2 - ContLeft.Size.Width / 2, ContLeft.Location.Y);
            ContTop.Location = new Point(ContPlace.Location.X + ContPlace.Size.Width / 2 - ContTop.Size.Width / 2, ContPlace.Location.Y + ContPlace.Size.Height / 2 - ContTop.Size.Height / 2);
            TrosLeft = new Tuple<Point, Point>(new Point(LulkaLeft.Location.X + LulkaLeft.Size.Width / 2 - LeftBox.Location.X, LulkaLeft.Location.Y + LulkaLeft.Size.Height - 1 - LeftBox.Location.Y), new Point(ContLeft.Location.X + ContLeft.Size.Width / 2 - LeftBox.Location.X, ContLeft.Location.Y - LeftBox.Location.Y));
            TrosTop = new Tuple<Point, Point>(new Point(ContTop.Size.Width / 2, ContTop.Size.Height / 2), new Point(-ContTop.Location.X + LulkaTop.Location.X + LulkaTop.Size.Width / 2, -ContTop.Location.Y + LulkaTop.Location.Y + LulkaTop.Size.Height / 2));
            LeftBox.Refresh();
        }

        private void Begin_Click(object sender, EventArgs e)
        {
            int level = 0;
            for (int i = 4; i < 19; ++i)
            {
                ((PictureBox)Controls[i]).Image = null;
                Controls[i].Enabled = false;
            }
            if (Containers[PlaceForCont.Item1, PlaceForCont.Item2, 0] != null)
            {
                if (Containers[PlaceForCont.Item1, PlaceForCont.Item2, 1] != null)
                {
                    if (Containers[PlaceForCont.Item1, PlaceForCont.Item2, 2] != null)
                    {
                        level = 3;
                    }
                    else
                    {
                        level = 2;
                    }
                }
                else
                {
                    level = 1;
                }
            }
            else
                level = 0;

            //TODO
            //анимация

            for (int i = 4; i < 19; ++i)
            {
                int temp = (int)((PictureBox)Controls[i]).Tag;
                if (Containers[temp / 10, temp % 10, 3] == null)
                {
                    ((PictureBox)Controls[i]).Image = Properties.Resources.disabled_cross;
                    Controls[i].Enabled = true;
                }
            }
            //for (int i = 0; i < 3; ++i)
            // for (int j = 0; j < 5; ++j)
            //Controls[ContainersWeb[i, j]].Enabled = false;
            //анимация
            //ContainersWeb[PlaceForCont.Item1, PlaceForCont.Item2].BackColor = Color.Yellow;

            //Controls[ContainersWeb[0,0].Name].BackColor = Color.Yellow;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            
            WaterLeft.Location = new Point(5, LeftBox.Location.Y + LeftBox.Size.Height - (int)(WaterLevel + Ampl * Math.Sin(time)));
            WaterLeft.Size = new Size(LeftBox.Size.Width, (int)(WaterLevel + Ampl * Math.Sin(time)));
            ShipLeft.Location = new Point(ShipLeft.Location.X, LeftBox.Location.Y + (int)(LeftBox.Size.Height / 1.088) - (int)(WaterLevel + Ampl * Math.Sin(time)));

            time += WavePower*0.8;

            //Tros = new Tuple<Point, Point>(new Point(Tros.Item1.X + 1, Tros.Item1.Y), new Point(Tros.Item2.X, Tros.Item2.Y));
            //LeftBox.Refresh();

            if (wind_changed)
                if (Math.Abs(wind.X - cur_wind.X) + Math.Abs(wind.Y - cur_wind.Y) > eps)
                    cur_wind = Physics.Wind_to_destination(cur_wind, wind_change_speed);
        }
    }
}
