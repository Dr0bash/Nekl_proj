﻿using System;
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
        static PictureBox ContLeftBack = null;
        static PictureBox ContLeftMiddle = null;
        static PictureBox ContLeftFront = null;
        static TextBox tbox = null;
        static Size SettingsSize = new Size();
        static Color WaterColor = Color.FromArgb(0, 162, 211);
        static System.Timers.Timer Timer;
        static Container[,,] Containers = new Container[3, 5, 4];
        static TrackBar WavePowerBar = null;
        static Label WavePowerText = null;
        static TrackBar AmplBar = null;
        static Label AmplText = null;
        static PictureBox[,] ContainersWeb = new PictureBox[3, 5];
        static Control[] allObj = new Control[150];
        static Button Begin = null;
        static Point PlaceForCont = new Point(0,0);
        static Tuple<Point, Point> TrosLeft = null;
        static Tuple<Point, Point> TrosTop = null;
        static double rope_length = 0;

        static double Ampl = 20;
        static double WaterLevel = 120.0;
        static double WavePower = 0.05;
        static double time = 0;

        static PointF wind = new PointF(0, 0);
        static PointF cur_wind = new PointF(0, 0);
        static bool wind_changed = false;
        static PointF wind_change_speed = new PointF(0,0);

        static bool[,,] ContainerPlaced = new bool[3, 5, 4];
        public static Physics.Point3D ContainerLocation;
        public static int PcontHeight;


        static double eps = 0.1;

        //graph variables for right introducing
        static int krestikloc = 5;
        static int contslocfront = krestikloc + 15;
        static int contslocmiddle = contslocfront + 21;
        static int contslocback = contslocmiddle + 21;
        PictureBox CurrentContPlace = null;
        static int[] oldlocs = new int[20];
        int level = 0;
        Random r = new Random();
        Pen trospen = new Pen(Brushes.Black, 2);


        //animation variable
        bool animation = false;
        int animationspeed = 4; // тестовая штука для проверки(контейнер просто идет вниз)

        //отклонение контейнера на leftbox тест на перекрытие
        static int testbias = 50;

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

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 5; j++)
                    for (int k = 0; k < 4; k++)
                        ContainerPlaced[i, j, k] = false;

            

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
            int sch = krestikloc;
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

            CurrentContPlace = (PictureBox)allObj[krestikloc];

            //containersFrontForLeft
            int o = 0;
            Size consize = new Size((int)(LeftBox.Size.Width / 8.48), (int)(LeftBox.Size.Height / 13.9));
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 5; ++j)
                {

                    allObj[sch] = new PictureBox
                    {
                        Location = new Point(-500, 0),
                        Size = consize,
                        BorderStyle = BorderStyle.None,
                        BackColor = Color.White,
                        Tag = i * 10 + j
                    };
                    oldlocs[o] = ShipLeft.Location.Y - i * consize.Height;
                    ((PictureBox)allObj[sch]).Paint += new PaintEventHandler(ContsLeft_Paint);
                    ((PictureBox)allObj[sch]).Dock = DockStyle.None;
                    ++o;
                    sch += 1;
                }
            }

            sch += 1; //между группами контейнеров(front and middle) нужно ставить новый контейнер

            //containersMiddleForLeft
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 5; ++j)
                {

                    allObj[sch] = new PictureBox
                    {
                        Location = new Point(-500, 0),
                        Size = consize,
                        BorderStyle = BorderStyle.None,
                        BackColor = Color.White,
                        Tag = i * 10 + j
                    };
                    ((PictureBox)allObj[sch]).Paint += new PaintEventHandler(ContsLeft_Paint);
                    ((PictureBox)allObj[sch]).Dock = DockStyle.None;
                    sch += 1;
                }
            }

            sch += 1; //между группами контейнеров(middle and back) нужно ставить новый контейнер

            //containersBackForLeft
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 5; ++j)
                {

                    allObj[sch] = new PictureBox
                    {
                        Location = new Point(ShipLeft.Location.X + (ShipLeft.Size.Width / 24) + j * (int)(consize.Width + ShipLeft.Size.Width / 94.5), ShipLeft.Location.Y - i * (consize.Height)),
                        Size = consize,
                        BorderStyle = BorderStyle.None,
                        BackColor = Color.White,
                        Tag = i * 10 + j
                    };
                    ((PictureBox)allObj[sch]).Paint += new PaintEventHandler(ContsLeft_Paint);
                    ((PictureBox)allObj[sch]).Dock = DockStyle.None;
                    sch += 1;
                }
            }


            ContTop = new PictureBox
            {
                Location = new Point(ShipTop.Location.X + (ShipTop.Size.Width / 24), ShipTop.Location.Y + ShipTop.Location.Y + ShipTop.Size.Height / 10 - 17),
                Size = new Size((int)(ShipTop.Size.Width / 6.74), (int)(ShipTop.Size.Height / 4.22)),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Yellow
            };

            ContTop.Location = new Point(ContainersWeb[0, 0].Location.X + ContainersWeb[0, 0].Size.Width / 2 - ContTop.Size.Width / 2, ContainersWeb[0, 0].Location.Y + ContainersWeb[0, 0].Size.Height / 2 - ContTop.Size.Height / 2);

            ContLeftFront = new PictureBox
            {
                Location = new Point(LeftBox.Location.X, LeftBox.Location.Y + (int)(LeftBox.Size.Height/4.3)),
                Size = new Size((int)(LeftBox.Size.Width / 8.48), (int)(LeftBox.Size.Height / 13.9)),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Yellow
            };

            ContLeftFront.Location = new Point(ContainersWeb[0, 0].Location.X + ContainersWeb[0, 0].Size.Width / 2 - ContLeftFront.Size.Width / 2, ContLeftFront.Location.Y);

            ContLeftMiddle = new PictureBox
            {
                Location = new Point(0, 0),
                Size = new Size((int)(LeftBox.Size.Width / 8.48), (int)(LeftBox.Size.Height / 13.9)),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Yellow
            };

            ContLeftMiddle.Location = new Point(-500, ContLeftFront.Location.Y);

            ContLeftBack = new PictureBox
            {
                Location = new Point(0, 0),
                Size = new Size((int)(LeftBox.Size.Width / 8.48), (int)(LeftBox.Size.Height / 13.9)),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Yellow
            };

            ContLeftBack.Location = new Point(-500, ContLeftFront.Location.Y);

            ContLeft = ContLeftFront;

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
            allObj[3] = ContLeftFront;
            allObj[4] = ShipLeft;
            //5-19 krestiki
            //20-39 boxesfront left
            allObj[40] = ContLeftMiddle;
            //41-60 boxesmiddle left
            allObj[61] = ContLeftBack;
            //62-81 boxesback left
            allObj[100] = LulkaLeft;
            allObj[101] = ReikaLeft;
            allObj[102] = ShipTop;
            allObj[104] = WaterLeft;
            allObj[105] = LeftBox;
            allObj[106] = TopBox;
            allObj[107] = WavePowerText;
            allObj[108] = WavePowerBar;
            allObj[109] = AmplText;
            allObj[110] = AmplBar;
            allObj[111] = Begin;

            // Начальная длина тросса
            // rope_length = Math.Sqrt(Math.Pow(TrosTop.Item1.X - TrosTop.Item2.X, 2) + Math.Pow(TrosTop.Item1.Y - TrosTop.Item2.Y, 2) + Math.Pow(TrosLeft.Item1.Y - TrosLeft.Item2.Y, 2));
           
            Controls.AddRange(allObj);

            timer1.Start();

            ContainerLocation = new Physics.Point3D(ContTop.Location.X, ContTop.Location.Y, ContLeft.Location.Y);

        }

        //Paint event for Tros
        private void LeftBox_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawLine(trospen, TrosLeft.Item1.X, TrosLeft.Item1.Y, TrosLeft.Item2.X, TrosLeft.Item2.Y);
        }

        //for tros
        private void ContsLeft_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {

            var currentCont = sender as PictureBox;
            int begx = LeftBox.Location.X - currentCont.Location.X;
            int begy = LeftBox.Location.Y - currentCont.Location.Y;
            
            Graphics g = e.Graphics;
            int addition = 0;
            if (currentCont.BorderStyle == BorderStyle.None)
                addition = 1;


            g.DrawLine(trospen, begx + TrosLeft.Item1.X + addition, begy + TrosLeft.Item1.Y, begx + TrosLeft.Item2.X + addition, begy + TrosLeft.Item2.Y);
            
        }
        
        //top tros
        private void ContTop_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawLine(trospen, TrosTop.Item1.X, TrosTop.Item1.Y, TrosTop.Item2.X, TrosTop.Item2.Y);
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
            CurrentContPlace = ContPlace;
            LulkaLeft.Location = new Point(ContPlace.Location.X + ContPlace.Size.Width / 2 - LulkaLeft.Size.Width / 2, LulkaLeft.Location.Y);
            LulkaTop.Location = new Point(ContPlace.Location.X + ContPlace.Size.Width / 2 - LulkaTop.Size.Width / 2, ContPlace.Location.Y + ContPlace.Size.Height / 2 - LulkaTop.Size.Height / 2);
            PlaceForCont = new Point((int)ContPlace.Tag / 10, (int)ContPlace.Tag % 10);
            ContLeft.Location = new Point(ContPlace.Location.X + ContPlace.Size.Width / 2 - ContLeft.Size.Width / 2, ContLeft.Location.Y);
            ContTop.Location = new Point(ContPlace.Location.X + ContPlace.Size.Width / 2 - ContTop.Size.Width / 2, ContPlace.Location.Y + ContPlace.Size.Height / 2 - ContTop.Size.Height / 2);
            TrosLeft = new Tuple<Point, Point>(new Point(LulkaLeft.Location.X + LulkaLeft.Size.Width / 2 - LeftBox.Location.X, LulkaLeft.Location.Y + LulkaLeft.Size.Height - 1 - LeftBox.Location.Y), new Point(ContLeft.Location.X + ContLeft.Size.Width / 2 - LeftBox.Location.X, ContLeft.Location.Y - LeftBox.Location.Y));
            TrosTop = new Tuple<Point, Point>(new Point(ContTop.Size.Width / 2, ContTop.Size.Height / 2), new Point(-ContTop.Location.X + LulkaTop.Location.X + LulkaTop.Size.Width / 2, -ContTop.Location.Y + LulkaTop.Location.Y + LulkaTop.Size.Height / 2));
            Begin.Enabled = true;

            ContainerLocation = new Physics.Point3D(ContTop.Location.X, ContTop.Location.Y, ContLeft.Location.Y);
            PcontHeight = ContLeft.Size.Height;

            LeftBox.Refresh();
        }

        


        private void Begin_Click(object sender, EventArgs e)
        {
            level = 0;
            int y = LeftBox.Location.Y + (int)(LeftBox.Size.Height / 4.3);
            for (int i = krestikloc; i < krestikloc+15; ++i)
            {
                ((PictureBox)Controls[i]).Image = null;
                Controls[i].Enabled = false;
            }
            if (PlaceForCont.X == 0)
            {
                ContLeftBack.BackColor = ContLeft.BackColor;
                ContLeft = ContLeftBack;
                ContLeft.Location = new Point(CurrentContPlace.Location.X + CurrentContPlace.Size.Width / 2 - ContLeft.Size.Width / 2, ContLeft.Location.Y);
                ContLeftMiddle.Location = new Point(-500, y);
                ContLeftFront.Location = new Point(-500, y);
            }
            else if (PlaceForCont.X == 1)
            {
                ContLeftMiddle.BackColor = ContLeft.BackColor;
                ContLeft = ContLeftMiddle;
                ContLeft.Location = new Point(CurrentContPlace.Location.X + CurrentContPlace.Size.Width / 2 - ContLeft.Size.Width / 2, ContLeft.Location.Y);
                ContLeftBack.Location = new Point(-500, y);
                ContLeftFront.Location = new Point(-500, y);
            }
            else
            {
                ContLeftFront.BackColor = ContLeft.BackColor;
                ContLeft = ContLeftFront;
                ContLeft.Location = new Point(CurrentContPlace.Location.X + CurrentContPlace.Size.Width / 2 - ContLeft.Size.Width / 2, ContLeft.Location.Y);
                ContLeftBack.Location = new Point(-500, y);
                ContLeftMiddle.Location = new Point(-500, y);
            }

            if (ContainerPlaced[PlaceForCont.X, PlaceForCont.Y, 0])
            {
                if (ContainerPlaced[PlaceForCont.X, PlaceForCont.Y, 1])
                {
                    if (ContainerPlaced[PlaceForCont.X, PlaceForCont.Y, 2])
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

            for (int i = PlaceForCont.X + 1; i < 3; ++i)
                for (int j = 0; j < 5; ++j)
                {
                    if (ContainerPlaced[i, j, 0])
                        TurnOffTros(i, j, 0);
                }

            animation = true;
            Begin.Enabled = false;
        }

        private void TurnOffTros(int i, int j, int lvl)
        {
            ((PictureBox)Controls[contslocback + lvl*5 + j]).Paint -= new PaintEventHandler(ContsLeft_Paint);
            ((PictureBox)Controls[contslocmiddle + lvl * 5 + j]).Paint -= new PaintEventHandler(ContsLeft_Paint);
            ((PictureBox)Controls[contslocfront + lvl * 5 + j]).Paint -= new PaintEventHandler(ContsLeft_Paint);
            if (lvl < 3 && ContainerPlaced[i, j, lvl + 1])
                TurnOffTros(i, j, lvl + 1);
        }

        private void After_animation()
        {
            //logics
            ContainerPlaced[PlaceForCont.X, PlaceForCont.Y, level] = true;
            Containers[PlaceForCont.X, PlaceForCont.Y, level] = new Container(new Physics.Point3D(ContainerLocation.x, ContainerLocation.y, ContainerLocation.z));

            //LeftBox
            //bool f = true;
            //for (int i = PlaceForCont.X + 1; i < 3; ++i)
            //{
            //    if (ContainerPlaced[i, PlaceForCont.Y, level])
            //    {
            //        f = false;
            //        break;
            //    }
            //}
            //if (f)
            //{
            //    ((PictureBox)Controls[contslocback + level * 5 + PlaceForCont.Y]).BackColor = ContLeft.BackColor;
            //    ((PictureBox)Controls[contslocback + level * 5 + PlaceForCont.Y]).BorderStyle = BorderStyle.FixedSingle;
            //}
            if (PlaceForCont.X > 0)
            {
                if (PlaceForCont.X == 1)
                {
                    Point loc = ((PictureBox)Controls[contslocback + 5 * level + PlaceForCont.Y]).Location;
                    ((PictureBox)Controls[contslocmiddle + 5 * level + PlaceForCont.Y]).Location = new Point(loc.X, loc.Y);
                    ((PictureBox)Controls[contslocmiddle + 5 * level + PlaceForCont.Y]).BackColor = ContLeft.BackColor;
                    ((PictureBox)Controls[contslocmiddle + 5 * level + PlaceForCont.Y]).BorderStyle = BorderStyle.FixedSingle;
                }
                else
                {
                    Point loc = ((PictureBox)Controls[contslocback + 5 * level + PlaceForCont.Y]).Location;
                    ((PictureBox)Controls[contslocfront + 5 * level + PlaceForCont.Y]).Location = new Point(loc.X, loc.Y);
                    ((PictureBox)Controls[contslocfront + 5 * level + PlaceForCont.Y]).BackColor = ContLeft.BackColor;
                    ((PictureBox)Controls[contslocfront + 5 * level + PlaceForCont.Y]).BorderStyle = BorderStyle.FixedSingle;
                }
            }
            else
            {
                ((PictureBox)Controls[contslocback + 5 * level + PlaceForCont.Y]).BackColor = ContLeft.BackColor;
                ((PictureBox)Controls[contslocback + 5 * level + PlaceForCont.Y]).BorderStyle = BorderStyle.FixedSingle;
            }

            ContainersWeb[PlaceForCont.X, PlaceForCont.Y].BackColor = ContLeft.BackColor;
            Color c = Color.FromArgb(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255));
            ContLeft.BackColor = c;

            //TopBox
            ContTop.BackColor = c;
            for (int i = krestikloc; i < krestikloc + 15; ++i)
            {
                int temp = (int)((PictureBox)Controls[i]).Tag;
                if (!ContainerPlaced[temp / 10, temp % 10, 3])
                {
                    ((PictureBox)Controls[i]).Image = Properties.Resources.disabled_cross;
                    Controls[i].Enabled = true;
                }
            }

            //TurnOnAllTros
            for (int i = contslocback; i < contslocback + 20; ++i)
            {
                ((PictureBox)Controls[i]).Paint -= new PaintEventHandler(ContsLeft_Paint);
                ((PictureBox)Controls[i]).Paint += new PaintEventHandler(ContsLeft_Paint);
            }

            for (int i = contslocmiddle; i < contslocmiddle + 20; ++i)
            {
                ((PictureBox)Controls[i]).Paint -= new PaintEventHandler(ContsLeft_Paint);
                ((PictureBox)Controls[i]).Paint += new PaintEventHandler(ContsLeft_Paint);
            }

            for (int i = contslocfront; i < contslocfront + 20; ++i)
            {
                ((PictureBox)Controls[i]).Paint -= new PaintEventHandler(ContsLeft_Paint);
                ((PictureBox)Controls[i]).Paint += new PaintEventHandler(ContsLeft_Paint);
            }


            Begin.Enabled = true;
            if (ContainerPlaced[PlaceForCont.X, PlaceForCont.Y, 3])
                Begin.Enabled = false;
            var ContPlace = ContainersWeb[PlaceForCont.X, PlaceForCont.Y];
            ContLeft.Location = new Point(ContPlace.Location.X + ContPlace.Size.Width / 2 - ContLeft.Size.Width / 2, LeftBox.Location.Y + (int)(LeftBox.Size.Height / 4.3));
            TrosLeft = new Tuple<Point, Point>(new Point(LulkaLeft.Location.X + LulkaLeft.Size.Width / 2 - LeftBox.Location.X, LulkaLeft.Location.Y + LulkaLeft.Size.Height - 1 - LeftBox.Location.Y), new Point(ContLeft.Location.X + ContLeft.Size.Width / 2 - LeftBox.Location.X, ContLeft.Location.Y - LeftBox.Location.Y));
        }


        private void Timer1_Tick(object sender, EventArgs e)
        {
            //water and ship
            WaterLeft.Location = new Point(5, LeftBox.Location.Y + LeftBox.Size.Height - (int)(WaterLevel + Ampl * Math.Sin(time)));
            WaterLeft.Size = new Size(LeftBox.Size.Width, (int)(WaterLevel + Ampl * Math.Sin(time)));
            ShipLeft.Location = new Point(ShipLeft.Location.X, LeftBox.Location.Y + (int)(LeftBox.Size.Height / 1.088) - (int)(WaterLevel + Ampl * Math.Sin(time)));

            //containers

            Size consize = ContLeft.Size;
            
            for (int i = contslocback; i < contslocback + 20; ++i)
            {
                ((PictureBox)Controls[i]).Location = new Point(((PictureBox)Controls[i]).Location.X, oldlocs[i - contslocback] - (int)(Ampl*Math.Sin(time)) - ContLeft.Size.Height + 1);
                ((PictureBox)Controls[i]).Refresh();
            }

            for (int i = contslocmiddle; i < contslocmiddle + 20; ++i)
            {
                ((PictureBox)Controls[i]).Location = new Point(((PictureBox)Controls[i]).Location.X, oldlocs[i - contslocmiddle] - (int)(Ampl * Math.Sin(time)) - ContLeft.Size.Height + 1);
                ((PictureBox)Controls[i]).Refresh();
            }

            for (int i = contslocfront; i < contslocfront + 20; ++i)
            {
                ((PictureBox)Controls[i]).Location = new Point(((PictureBox)Controls[i]).Location.X, oldlocs[i - contslocfront] - (int)(Ampl * Math.Sin(time)) - ContLeft.Size.Height + 1);
                ((PictureBox)Controls[i]).Refresh();
            }

            time += WavePower*0.8;

            if (animation)
            {
                var ContPlace = ContainersWeb[PlaceForCont.X, PlaceForCont.Y];
                ContLeft.Location = new Point(ContPlace.Location.X + ContPlace.Size.Width / 2 - ContLeft.Size.Width / 2 + testbias, ContLeft.Location.Y + animationspeed);
                TrosLeft = new Tuple<Point, Point>(new Point(LulkaLeft.Location.X + LulkaLeft.Size.Width / 2 - LeftBox.Location.X, LulkaLeft.Location.Y + LulkaLeft.Size.Height - 1 - LeftBox.Location.Y), new Point(ContLeft.Location.X + ContLeft.Size.Width / 2 - LeftBox.Location.X, ContLeft.Location.Y - LeftBox.Location.Y));
                if ((ContLeft.Location.Y + ContLeft.Size.Height) >= ShipLeft.Location.Y - level * ContLeft.Size.Height)
                {
                    animation = false;
                    After_animation();
                }
            }



            //фезека

            // Tuple<Point, Point> TrosLeft2 = TrosLeft;
            // Physics.Point3D point3dtemp = Physics.Container_Movement(new PointF(10, 1), 4, rope_length);
            // TrosLeft = new Tuple<Point, Point>(TrosLeft2.Item1, new Point(TrosLeft2.Item1.X + Convert.ToInt32(point3dtemp.x), Convert.ToInt32(TrosLeft2.Item1.Y + point3dtemp.z)));
            // ContLeft.Location = new Point(TrosLeft.Item2.X - ContLeft.Size.Width / 2, TrosLeft.Item2.Y + LeftBox.Location.Y);

            //Tros = new Tuple<Point, Point>(new Point(Tros.Item1.X + 1, Tros.Item1.Y), new Point(Tros.Item2.X, Tros.Item2.Y));
            LeftBox.Refresh();

            if (wind_changed)
                if (Math.Abs(wind.X - cur_wind.X) + Math.Abs(wind.Y - cur_wind.Y) > eps)
                    cur_wind = Physics.Wind_to_destination(cur_wind, wind_change_speed);
        }
    }

    public class Container
    {
        private static Physics.Point3D Location;

        public Container(Physics.Point3D l)
        {
            Location = l;
        }
    }
}
