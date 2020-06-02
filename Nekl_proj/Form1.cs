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
        static PictureBox SettingsBox = null;
        public static PictureBox WaterLeft = null;
        public static PictureBox ShipLeft = null;
        static PictureBox ShipTop = null;
        static PictureBox ReikaLeft = null;
        static PictureBox LulkaLeft = null;
        static PictureBox LulkaTop = null;
        static PictureBox ContTop = null;
        static PictureBox ContLeft = null;
        static PictureBox ContLeftBack = null;
        static PictureBox ContLeftMiddle = null;
        static PictureBox ContLeftFront = null;
        static PictureBox WindDirectionBox = null;
        static Label WindDirectionBoxText = null;
        static TextBox tbox = null;
        static Size SettingsSize = new Size();
        static Color WaterColor = Color.FromArgb(0, 162, 211);
        static System.Timers.Timer Timer;
        static Container[,,] Containers = new Container[3, 5, 4];
        static TrackBar WeightBar = null;
        static Label WeightText = null;
        static TrackBar AmplBar = null;
        static Label AmplText = null;
        static Label WaveFunctionText = null;
        static RadioButton w1 = null;
        static RadioButton w2 = null;
        static RadioButton w3 = null;
        static RadioButton w4 = null;

        static PictureBox[,] ContainersWeb = new PictureBox[3, 5];
        static Control[] allObj = new Control[150];
        static Button Begin = null;
        static Point PlaceForCont = new Point(0, 0);
        static Tuple<Point, Point> TrosLeft = null;
        static Tuple<Point, Point> TrosTop = null;
        static double rope_length = 0;
        static double begin_rope_length = 0;

        static double WaterLevel = 120.0;
        static double Ampl = WaterLevel / 6.0;
        static double WavePower = 0.05;
        static double time = 0;

        static PointF wind = new PointF(0, 0);
        static PointF cur_wind = new PointF(0, 0);
        public static PointF final_wind = new PointF(0, 0);
        static bool wind_changed = false;
        static PointF wind_change_speed = new PointF(0, 0);
        static double max_wind_power = 6;
        static double wind_change_speed_fraction = 0.01;

        static int weight = 20;

        static bool[,,] ContainerPlaced = new bool[3, 5, 4];
        static int[,] contGridTop = new int[3, 5];
        public static Physics.Point3D ContainerLocation;
        public static int PcontHeight;
        static double waveFunctionNum = 1;


        public static Size ScreenSize;
        public static bool speedAlarm = false;

        static double eps = 0.1;

        //graph variables for right introducing
        static int krestikloc = 84;
        static int contslocfront = 16;
        static int contslocmiddle = contslocfront + 21;
        static int contslocback = contslocmiddle + 21;
        PictureBox CurrentContPlace = null;
        static int[] oldlocs = new int[20];
        int level = 0;
        Random r = new Random();
        Pen trospen = new Pen(Brushes.Black, 2);
        static bool start = true;

        //Позиция мышки внутри WindDirectionBox
        static Point CoordInWindBox = new Point(0, 0);

        //animation variable
        bool animation = false;

        //тест
        int mistake = 0;


        //логика

        private Logic logic;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Visible = false;
            bool truesize = false;
            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width == 1920 &&
                System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height == 1080)
            {
                WindowState = FormWindowState.Maximized;
                ScreenSize = Size;
                WindowState = FormWindowState.Normal;
                Size = new Size(1936, 1046);
                Location = new Point(0, 0);
                truesize = true;
            }
            else
            {
                WindowState = FormWindowState.Maximized;
                ScreenSize = Size;
                WindowState = FormWindowState.Normal;
                Size = new Size(ScreenSize.Width, ScreenSize.Height - Size.Height / 64);
                //Size = new Size(1280, 720);
                //ScreenSize = Size;
            }


            WaterLevel = ScreenSize.Height / 9.0;
            Ampl = WaterLevel / 6.0;
            Location = new Point(-7, 0);
            SettingsSize.Width = (int) (Size.Width / 7);
            SettingsSize.Height = Size.Height;

            for (int i = 0; i < 3; i++)
            for (int j = 0; j < 5; j++)
            {
                contGridTop[i, j] = 0;
                for (int k = 0; k < 4; k++)
                    ContainerPlaced[i, j, k] = false;
            }

            TopBox = new PictureBox
            {
                Location = new Point(0, 0),
                Size = new Size(Size.Width - SettingsSize.Width - 5, (int) (Size.Height / 3.0) + 10),
                BackColor = WaterColor,
                BorderStyle = BorderStyle.FixedSingle
            };

            LeftBox = new PictureBox
            {
                Location = new Point(0, TopBox.Location.Y + TopBox.Height),
                Size = new Size(Size.Width - SettingsSize.Width - 5,
                    (int) (Size.Height - TopBox.Height - (Size.Height / 20.92))),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            WorldCharacteristics.MaxRopeDownSpeed =
                1.7 * Size.Height / 1046 + (truesize ? 0 : 0.7); //+ (638-LeftBox.Size.Height)*0.0010182;
            WorldCharacteristics.MaxHorizontalCraneSpeed = 20 * Size.Width / 1936.0;
            logic = new Logic(WorldCharacteristics.MaxHorizontalCraneSpeed, WorldCharacteristics.MaxRopeDownSpeed);

            SettingsBox = new PictureBox
            {
                Location = new Point(TopBox.Location.X + TopBox.Size.Width, 0),
                Size = new Size(SettingsSize.Width, SettingsSize.Height - 50),
                BackColor = Color.Transparent,
                BorderStyle = BorderStyle.FixedSingle
            };

            WaterLeft = new PictureBox
            {
                Location = new Point(5, LeftBox.Location.Y + LeftBox.Size.Height - (int) WaterLevel),
                Size = new Size(LeftBox.Size.Width, (int) WaterLevel),
                BackColor = WaterColor,
                BorderStyle = BorderStyle.FixedSingle
            };

            ShipLeft = new PictureBox
            {
                Location = new Point(LeftBox.Location.X + LeftBox.Size.Width / 6,
                    LeftBox.Location.Y + (int) (LeftBox.Size.Height / 1.088) - (int) WaterLevel),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size((int) (LeftBox.Size.Width / 1.25), (int) (LeftBox.Size.Height / 5.5)),
                BackColor = Color.White,
                Image = Properties.Resources.ShipLeft,
                Padding = new Padding(0)
            };

            ShipTop = new PictureBox
            {
                Location = new Point(TopBox.Location.X + TopBox.Size.Width / 6,
                    TopBox.Location.Y + TopBox.Size.Height / 32),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size((int) (TopBox.Size.Width / 1.25), (int) (TopBox.Size.Height - TopBox.Size.Height / 16)),
                BackColor = WaterColor,
                Image = Properties.Resources.ShipTop
            };

            ShipTop.Paint += ShipTop_Paint;

            WeightText = new Label
            {
                Location = new Point(TopBox.Width + 15, TopBox.Height / 64),
                Text = "Вес контейнера",
                Font = new Font("Arial", SettingsSize.Width / 20, FontStyle.Regular),
                AutoSize = true
            };

            WeightBar = new TrackBar
            {
                Location = new Point(TopBox.Width + 5, WeightText.Location.Y + WeightText.Size.Height + 5),
                Size = new Size(SettingsSize.Width - 20, 20),
                TickFrequency = 1,
                Minimum = 15,
                Maximum = 28,
                Value = weight,
                SmallChange = 1,
                LargeChange = 1
            };

            WeightBar.ValueChanged += WeightBar_Changed;

            AmplText = new Label
            {
                Location = new Point(TopBox.Width + 15, WeightBar.Location.Y + WeightBar.Size.Height + 5),
                Text = "Амплитуда волны",
                Font = new Font("Arial", SettingsSize.Width / 20, FontStyle.Regular),
                AutoSize = true
            };

            AmplBar = new TrackBar
            {
                Location = new Point(TopBox.Width + 5, AmplText.Location.Y + AmplText.Size.Height + 10),
                Size = new Size(SettingsSize.Width - 20, 10),
                TickFrequency = 5,
                Minimum = (int) (Ampl),
                Maximum = (int) (WaterLevel / 3),
                Value = (int) (Ampl),
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
                        Location = new Point(
                            ShipTop.Location.X + (ShipTop.Size.Width / 24) + (int) (j * ShipTop.Size.Width / 6.34),
                            ShipTop.Location.Y + ShipTop.Size.Height / 10 + (int) (i * ShipTop.Size.Height / 3.6)),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Size = new Size((int) (ShipTop.Size.Width / 6.74), (int) (ShipTop.Size.Height / 4.22)),
                        BackColor = Color.FromArgb(128, 128, 128),
                        BorderStyle = BorderStyle.None,
                        Padding = new Padding(0),
                        Image = Properties.Resources.disabled_cross,
                        Tag = i * 10 + j
                    };
                    ContainersWeb[i, j].MouseEnter += ContainersWeb_MouseEnter;
                    ContainersWeb[i, j].MouseLeave += ContainersWeb_MouseLeave;
                    ContainersWeb[i, j].MouseClick += ContainersWeb_MouseClick;
                    ContainersWeb[i, j].Paint += Krestik_Paint;
                    allObj[sch] = ContainersWeb[i, j];
                    sch += 1;
                }
            }

            CurrentContPlace = (PictureBox) allObj[krestikloc];

            sch = contslocfront;
            //containersFrontForLeft
            int o = 0;
            Size consize = new Size((int) (LeftBox.Size.Width / 8.48), (int) (LeftBox.Size.Height / 13.9));
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
                    ((PictureBox) allObj[sch]).Paint += new PaintEventHandler(ContsLeft_Paint);
                    ((PictureBox) allObj[sch]).Dock = DockStyle.None;
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
                    ((PictureBox) allObj[sch]).Paint += new PaintEventHandler(ContsLeft_Paint);
                    ((PictureBox) allObj[sch]).Dock = DockStyle.None;
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
                        Location = new Point(
                            ShipLeft.Location.X + (ShipLeft.Size.Width / 24) +
                            j * (int) (consize.Width + ShipLeft.Size.Width / 94.5),
                            ShipLeft.Location.Y - i * (consize.Height)),
                        Size = consize,
                        BorderStyle = BorderStyle.None,
                        BackColor = Color.White,
                        Tag = i * 10 + j
                    };
                    ((PictureBox) allObj[sch]).Paint += new PaintEventHandler(ContsLeft_Paint);
                    ((PictureBox) allObj[sch]).Dock = DockStyle.None;
                    sch += 1;
                }
            }

            ContTop = new PictureBox
            {
                Location = new Point(ShipTop.Location.X + (ShipTop.Size.Width / 24),
                    ShipTop.Location.Y + ShipTop.Location.Y + ShipTop.Size.Height / 10 - 17),
                Size = new Size((int) (ShipTop.Size.Width / 6.74), (int) (ShipTop.Size.Height / 4.22)),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Yellow
            };

            ContTop.Location =
                new Point(ContainersWeb[0, 0].Location.X + ContainersWeb[0, 0].Size.Width / 2 - ContTop.Size.Width / 2,
                    ContainersWeb[0, 0].Location.Y + ContainersWeb[0, 0].Size.Height / 2 - ContTop.Size.Height / 2);

            ContLeftFront = new PictureBox
            {
                Location = new Point(LeftBox.Location.X, LeftBox.Location.Y + (int) (LeftBox.Size.Height / 4.3)),
                Size = new Size((int) (LeftBox.Size.Width / 8.48), (int) (LeftBox.Size.Height / 13.9)),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Yellow
            };

            ContLeftFront.Location =
                new Point(
                    ContainersWeb[0, 0].Location.X + ContainersWeb[0, 0].Size.Width / 2 - ContLeftFront.Size.Width / 2,
                    ContLeftFront.Location.Y);

            ContLeftMiddle = new PictureBox
            {
                Location = ContLeftFront.Location,
                Size = new Size((int) (LeftBox.Size.Width / 8.48), (int) (LeftBox.Size.Height / 13.9)),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Yellow,
                Visible = false
            };

            ContLeftBack = new PictureBox
            {
                Location = ContLeftFront.Location,
                Size = new Size((int) (LeftBox.Size.Width / 8.48), (int) (LeftBox.Size.Height / 13.9)),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Yellow,
                Visible = false
            };

            ContLeft = ContLeftFront;

            ReikaLeft = new PictureBox
            {
                Location = new Point(LeftBox.Location.X + (int) (LeftBox.Size.Width / 64),
                    LeftBox.Location.Y + (int) (LeftBox.Size.Height / 32)),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size((int) (LeftBox.Size.Width / 1.03), LeftBox.Size.Height / 8),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                Image = Properties.Resources.reika_krana_left
            };

            LulkaLeft = new PictureBox
            {
                Location = new Point(LeftBox.Location.X + (int) (LeftBox.Size.Width / 2),
                    LeftBox.Location.Y + (int) (LeftBox.Size.Height / 32)),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size((int) (LeftBox.Size.Width / 19), LeftBox.Size.Height / 8),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                Image = Properties.Resources.lulka
            };

            LulkaLeft.Location =
                new Point(
                    ContainersWeb[0, 0].Location.X + ContainersWeb[0, 0].Size.Width / 2 - LulkaLeft.Size.Width / 2,
                    LulkaLeft.Location.Y);

            LulkaTop = new PictureBox
            {
                Location = new Point(TopBox.Location.X + (int) (TopBox.Size.Width / 2),
                    TopBox.Location.Y + (int) (TopBox.Size.Height / 2)),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size((int) (TopBox.Size.Width / 33), (int) (TopBox.Size.Height / 8)),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                Image = Properties.Resources.lulka
            };

            LulkaTop.Location =
                new Point(ContainersWeb[0, 0].Location.X + ContainersWeb[0, 0].Size.Width / 2 - LulkaTop.Size.Width / 2,
                    ContainersWeb[0, 0].Location.Y + ContainersWeb[0, 0].Size.Height / 2 - LulkaTop.Size.Height / 2);

            WaveFunctionText = new Label
            {
                Location = new Point(TopBox.Width + 15, AmplBar.Location.Y + AmplBar.Size.Height + 10),
                Text = "Функция волны",
                Font = new Font("Arial", SettingsSize.Width / 20, FontStyle.Regular),
                AutoSize = true
            };

            w1 = new RadioButton
            {
                Location =
                    new Point(TopBox.Width + 15, WaveFunctionText.Location.Y + WaveFunctionText.Size.Height + 10),
                Text = "Sin(x)",
                AutoSize = true,
                Font = new Font("Arial", SettingsSize.Width / 20, FontStyle.Regular),
                Checked = true,
                Tag = 1
            };

            w2 = new RadioButton
            {
                Location = new Point(TopBox.Width + 15, w1.Location.Y + w1.Size.Height + 10),
                Text = "Sin(2*x)*Sin(0.5*x)^2",
                AutoSize = true,
                Font = new Font("Arial", SettingsSize.Width / 20, FontStyle.Regular),
                Tag = 2
            };

            w3 = new RadioButton
            {
                Location = new Point(TopBox.Width + 15, w2.Location.Y + w2.Size.Height + 10),
                Text = "(sin(x)+0.4*sin(2x)^2)/2",
                AutoSize = true,
                Font = new Font("Arial", SettingsSize.Width / 20, FontStyle.Regular),
                Tag = 3
            };

            w4 = new RadioButton
            {
                Location = new Point(TopBox.Width + 15, w3.Location.Y + w3.Size.Height + 10),
                Text = "1-|sin(x)|",
                AutoSize = true,
                Font = new Font("Arial", SettingsSize.Width / 20, FontStyle.Regular),
                Tag = 4
            };

            w1.Click += Check_Click;
            w2.Click += Check_Click;
            w3.Click += Check_Click;
            w4.Click += Check_Click;

            WindDirectionBoxText = new Label
            {
                Location = new Point(TopBox.Width + 15, w4.Location.Y + w4.Size.Height + 10),
                Text = "Направление и сила ветра",
                Font = new Font("Arial", SettingsSize.Width / 20, FontStyle.Regular),
                AutoSize = true
            };

            WindDirectionBox = new PictureBox
            {
                Location = new Point(TopBox.Width + SettingsSize.Width / 5,
                    WindDirectionBoxText.Location.Y + WindDirectionBoxText.Size.Height + 5),
                Size = new Size(SettingsSize.Width / 2, SettingsSize.Width / 2),
                BorderStyle = BorderStyle.None,
                BackColor = Color.Transparent
            };

            WindDirectionBox.Click += WindDirectionBox_Click;

            Begin = new Button
            {
                Location = new Point(TopBox.Width + (int) (SettingsSize.Width / 3.5),
                    WindDirectionBox.Location.Y + WindDirectionBox.Size.Height + 20),
                Text = "Начать",
                Font = new Font("Arial", SettingsSize.Width / 16, FontStyle.Regular),
                AutoSize = true
            };

            Begin.Click += Begin_Click;

            TopBox.Paint += TopBox_Paint;


            // Dock the PictureBox to the form and set its background to white.
            LeftBox.Dock = DockStyle.None;
            ContTop.Dock = DockStyle.None;
            WindDirectionBox.Dock = DockStyle.None;
            // Connect the Paint event of the PictureBox to the event handler method.
            TrosLeft = new Tuple<Point, Point>(
                new Point(LulkaLeft.Location.X + LulkaLeft.Size.Width / 2 - LeftBox.Location.X,
                    LulkaLeft.Location.Y + LulkaLeft.Size.Height - 1 - LeftBox.Location.Y),
                new Point(ContLeft.Location.X + ContLeft.Size.Width / 2 - LeftBox.Location.X,
                    ContLeft.Location.Y - LeftBox.Location.Y));
            TrosTop = new Tuple<Point, Point>(new Point(ContTop.Size.Width / 2, ContTop.Size.Height / 2),
                new Point(-ContTop.Location.X + LulkaTop.Location.X + LulkaTop.Size.Width / 2,
                    -ContTop.Location.Y + LulkaTop.Location.Y + LulkaTop.Size.Height / 2));
            LeftBox.Paint += new System.Windows.Forms.PaintEventHandler(LeftBox_Paint);
            ContTop.Paint += new System.Windows.Forms.PaintEventHandler(ContTop_Paint);
            WindDirectionBox.Paint += new System.Windows.Forms.PaintEventHandler(WindDirectionBox_Paint);

            //settings part
            allObj[1] = WeightText;
            allObj[2] = WeightBar;
            allObj[3] = AmplText;
            allObj[4] = AmplBar;
            allObj[5] = WindDirectionBoxText;
            allObj[6] = WindDirectionBox;
            allObj[7] = WaveFunctionText;
            allObj[8] = w1;
            allObj[9] = w2;
            allObj[10] = w3;
            allObj[11] = w4;
            allObj[12] = Begin;
            allObj[13] = SettingsBox;

            //Left part
            allObj[14] = ContLeftFront;
            allObj[15] = ShipLeft;
            //16-35 boxesfront left
            allObj[36] = ContLeftMiddle;
            //37-56 boxesmiddle left
            allObj[57] = ContLeftBack;
            //58-77 boxesback left
            allObj[78] = LulkaLeft;
            allObj[79] = ReikaLeft;
            allObj[80] = WaterLeft;
            allObj[81] = LeftBox;

            //Top part
            allObj[82] = LulkaTop;
            allObj[83] = ContTop;
            //84-98 krestiki
            allObj[99] = ShipTop;
            allObj[100] = TopBox;

            Controls.AddRange(allObj);


            // Начальная длина тросса
            rope_length = Math.Sqrt(Math.Pow(TrosTop.Item1.X - TrosTop.Item2.X, 2) +
                                    Math.Pow(TrosTop.Item1.Y - TrosTop.Item2.Y, 2) +
                                    Math.Pow(TrosLeft.Item1.Y - TrosLeft.Item2.Y, 2));
            begin_rope_length = rope_length;


            timer1.Start();

            ContainerLocation = new Physics.Point3D(ContTop.Location.X, ContTop.Location.Y, ContLeft.Location.Y);
            PcontHeight = ContLeft.Size.Height;
        }

        private void Check_Click(object sender, EventArgs e)
        {
            var r = sender as RadioButton;
            switch (r.Tag)
            {
                case 1:
                    waveFunctionNum = 1;
                    break;
                case 2:
                    waveFunctionNum = 2;
                    break;
                case 3:
                    waveFunctionNum = 3;
                    break;
                case 4:
                    waveFunctionNum = 4;
                    break;
            }
        }

        private double WaveFunc(double x)
        {
            switch (waveFunctionNum)
            {
                case 1:
                    return Physics.WaveFun1(x);
                case 2:
                    return Physics.WaveFun2(x);
                case 3:
                    return Physics.WaveFun3(x);
                case 4:
                    return Physics.WaveFun4(x);
                default:
                    return Physics.WaveFun1(x);
            }
        }

        //Paint for windbox
        private void WindDirectionBox_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Size s = WindDirectionBox.Size;
            Graphics g = e.Graphics;
            Pen circlePen = new Pen(Brushes.Black, 1);
            g.DrawEllipse(circlePen, 0, 0, WindDirectionBox.Width - 1, WindDirectionBox.Height - 1);
            //g.DrawLine(circlePen, s.Width / 2, s.Height / 2, MousePosition.X - WindDirectionBox.Location.X + Location.X, MousePosition.Y - WindDirectionBox.Location.Y - Location.Y);
            if (start)
            {
                g.DrawLine(circlePen, s.Width / 2, s.Height / 2, s.Width / 2, s.Height / 2);
                start = false;
            }
            else
            {
                double rx = s.Width / 2;
                double ry = s.Height / 2;
                double x = CoordInWindBox.X - rx;
                double y = CoordInWindBox.Y - ry;
                double radius = rx;
                double LineLength = Math.Sqrt(x * x + y * y);
                if (LineLength > radius)
                {
                    double relx = x;
                    double rely = y;
                    relx = relx / LineLength * radius;
                    rely = rely / LineLength * radius;
                    x = relx + rx;
                    y = rely + ry;
                    g.DrawLine(circlePen, s.Width / 2, s.Height / 2, (int) x, (int) y);
                }
                else
                {
                    g.DrawLine(circlePen, s.Width / 2, s.Height / 2, CoordInWindBox.X, CoordInWindBox.Y);
                }
            }
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


            g.DrawLine(trospen, begx + TrosLeft.Item1.X + addition, begy + TrosLeft.Item1.Y,
                begx + TrosLeft.Item2.X + addition, begy + TrosLeft.Item2.Y);
        }

        private void TopBox_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawLine(trospen, TrosTop.Item1.X, TrosTop.Item1.Y, TrosTop.Item2.X, TrosTop.Item2.Y);
        }

        //top tros
        private void ContTop_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            var currentCont = sender as PictureBox;
            int begx = TopBox.Location.X - currentCont.Location.X;
            int begy = TopBox.Location.Y - currentCont.Location.Y;

            Graphics g = e.Graphics;

            g.DrawLine(trospen, begx + TrosTop.Item1.X + 1, begy + TrosTop.Item1.Y, begx + TrosTop.Item2.X + 1,
                begy + TrosTop.Item2.Y);
        }

        private void ShipTop_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            var currentCont = sender as PictureBox;
            int begx = TopBox.Location.X - currentCont.Location.X;
            int begy = TopBox.Location.Y - currentCont.Location.Y;

            Graphics g = e.Graphics;

            g.DrawLine(trospen, begx + TrosTop.Item1.X + 1, begy + TrosTop.Item1.Y + 1, begx + TrosTop.Item2.X + 1,
                begy + TrosTop.Item2.Y + 1);
        }

        private void Krestik_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            var currentCont = sender as PictureBox;
            int begx = TopBox.Location.X - currentCont.Location.X;
            int begy = TopBox.Location.Y - currentCont.Location.Y;

            Graphics g = e.Graphics;

            g.DrawLine(trospen, begx + TrosTop.Item1.X + 1, begy + TrosTop.Item1.Y + 1, begx + TrosTop.Item2.X + 1,
                begy + TrosTop.Item2.Y + 1);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
        }

        private void WeightBar_Changed(object sender, EventArgs e)
        {
            weight = WeightBar.Value;
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
            LulkaLeft.Location = new Point(ContPlace.Location.X + ContPlace.Size.Width / 2 - LulkaLeft.Size.Width / 2,
                LulkaLeft.Location.Y);
            LulkaTop.Location = new Point(ContPlace.Location.X + ContPlace.Size.Width / 2 - LulkaTop.Size.Width / 2,
                ContPlace.Location.Y + ContPlace.Size.Height / 2 - LulkaTop.Size.Height / 2);
            PlaceForCont = new Point((int) ContPlace.Tag / 10, (int) ContPlace.Tag % 10);
            ContLeft.Location = new Point(ContPlace.Location.X + ContPlace.Size.Width / 2 - ContLeft.Size.Width / 2,
                ContLeft.Location.Y);
            ContLeftBack.Location = new Point(ContPlace.Location.X + ContPlace.Size.Width / 2 - ContLeft.Size.Width / 2,
                ContLeft.Location.Y);
            ContLeftMiddle.Location =
                new Point(ContPlace.Location.X + ContPlace.Size.Width / 2 - ContLeft.Size.Width / 2,
                    ContLeft.Location.Y);
            ContLeftFront.Location =
                new Point(ContPlace.Location.X + ContPlace.Size.Width / 2 - ContLeft.Size.Width / 2,
                    ContLeft.Location.Y);
            //ContTop.Location = new Point(ContPlace.Location.X + ContPlace.Size.Width / 2 - ContTop.Size.Width / 2, ContPlace.Location.Y + ContPlace.Size.Height / 2 - ContTop.Size.Height / 2);
            TrosLeft = new Tuple<Point, Point>(
                new Point(LulkaLeft.Location.X + LulkaLeft.Size.Width / 2 - LeftBox.Location.X,
                    LulkaLeft.Location.Y + LulkaLeft.Size.Height - 1 - LeftBox.Location.Y),
                new Point(ContLeft.Location.X + ContLeft.Size.Width / 2 - LeftBox.Location.X,
                    ContLeft.Location.Y - LeftBox.Location.Y));
            //TrosTop = new Tuple<Point, Point>(new Point(ContTop.Size.Width / 2, ContTop.Size.Height / 2), new Point(-ContTop.Location.X + LulkaTop.Location.X + LulkaTop.Size.Width / 2, -ContTop.Location.Y + LulkaTop.Location.Y + LulkaTop.Size.Height / 2));
            Begin.Enabled = true;

            ContainerLocation = new Physics.Point3D(ContTop.Location.X, ContTop.Location.Y, ContLeft.Location.Y);

            TopBox.Refresh();
            LeftBox.Refresh();
        }

        private void WindDirectionBox_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs) e;
            CoordInWindBox = new Point(me.Location.X, me.Location.Y);

            Size s = WindDirectionBox.Size;
            double rx = s.Width / 2;
            double ry = s.Height / 2;
            wind = new PointF((float) ((CoordInWindBox.X - rx) / rx * max_wind_power),
                (float) ((CoordInWindBox.Y - ry) / ry * max_wind_power));
            wind_changed = true;
            wind_change_speed = new PointF((float) ((wind.X - cur_wind.X) * wind_change_speed_fraction),
                (float) ((wind.Y - cur_wind.Y) * wind_change_speed_fraction));


            WindDirectionBox.Refresh();
        }


        private void Begin_Click(object sender, EventArgs e)
        {
            level = 0;
            AmplBar.Enabled = false;
            WeightBar.Enabled = false;
            w1.Enabled = false;
            w2.Enabled = false;
            w3.Enabled = false;
            w4.Enabled = false;
            int y = LeftBox.Location.Y + (int) (LeftBox.Size.Height / 4.3);
            for (int i = krestikloc; i < krestikloc + 15; ++i)
            {
                ((PictureBox) Controls[i]).Image = null;
                Controls[i].Enabled = false;
            }

            if (PlaceForCont.X == 0)
            {
                ContLeftBack.BackColor = ContLeft.BackColor;
                ContLeftBack.Visible = true;
                ContLeft = ContLeftBack;
                ContLeftMiddle.Visible = false;
                ContLeftFront.Visible = false;
                ////ContLeft.Location = new Point(CurrentContPlace.Location.X + CurrentContPlace.Size.Width / 2 - ContLeft.Size.Width / 2, ContLeft.Location.Y);
                //ContLeft.Location = new Point(Math.Max(ContLeftMiddle.Location.X, ContLeftFront.Location.X), ContLeft.Location.Y);
                //ContLeftMiddle.Location = new Point(-500, y);
                //ContLeftFront.Location = new Point(-500, y);
            }
            else if (PlaceForCont.X == 1)
            {
                ContLeftMiddle.BackColor = ContLeft.BackColor;
                ContLeftMiddle.Visible = true;
                ContLeft = ContLeftMiddle;
                ContLeftFront.Visible = false;
                ContLeftBack.Visible = false;
                ////ContLeft.Location = new Point(CurrentContPlace.Location.X + CurrentContPlace.Size.Width / 2 - ContLeft.Size.Width / 2, ContLeft.Location.Y);
                //ContLeft.Location = new Point(Math.Max(ContLeftBack.Location.X, ContLeftFront.Location.X), ContLeft.Location.Y);
                //ContLeftBack.Location = new Point(-500, y);
                //ContLeftFront.Location = new Point(-500, y);
            }
            else
            {
                ContLeftFront.BackColor = ContLeft.BackColor;
                ContLeftFront.Visible = true;
                ContLeft = ContLeftFront;
                ContLeftMiddle.Visible = false;
                ContLeftBack.Visible = false;
                ////ContLeft.Location = new Point(CurrentContPlace.Location.X + CurrentContPlace.Size.Width / 2 - ContLeft.Size.Width / 2, ContLeft.Location.Y);
                //ContLeft.Location = new Point(Math.Max(ContLeftBack.Location.X, ContLeftMiddle.Location.X), ContLeft.Location.Y);
                //ContLeftBack.Location = new Point(-500, y);
                //ContLeftMiddle.Location = new Point(-500, y);
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
            ((PictureBox) Controls[contslocback + lvl * 5 + j]).Paint -= new PaintEventHandler(ContsLeft_Paint);
            ((PictureBox) Controls[contslocmiddle + lvl * 5 + j]).Paint -= new PaintEventHandler(ContsLeft_Paint);
            ((PictureBox) Controls[contslocfront + lvl * 5 + j]).Paint -= new PaintEventHandler(ContsLeft_Paint);
            if (lvl < 3 && ContainerPlaced[i, j, lvl + 1])
                TurnOffTros(i, j, lvl + 1);
        }

        private void After_animation()
        {
            //logics
            ContainerPlaced[PlaceForCont.X, PlaceForCont.Y, level] = true;
            contGridTop[PlaceForCont.X, PlaceForCont.Y] += 1;
            Containers[PlaceForCont.X, PlaceForCont.Y, level] =
                new Container(new Physics.Point3D(ContainerLocation.x, ContainerLocation.y, ContainerLocation.z));

            ContTop.Location =
                new Point(CurrentContPlace.Location.X + CurrentContPlace.Size.Width / 2 - ContTop.Size.Width / 2,
                    CurrentContPlace.Location.Y + CurrentContPlace.Size.Height / 2 - ContTop.Size.Height / 2);
            TrosTop = new Tuple<Point, Point>(
                new Point(LulkaTop.Location.X + LulkaTop.Size.Width / 2 - TopBox.Location.X,
                    LulkaTop.Location.Y + LulkaTop.Size.Height / 2 - 1 - TopBox.Location.Y),
                new Point(ContTop.Location.X + ContTop.Size.Width / 2 - TopBox.Location.X,
                    ContTop.Location.Y - TopBox.Location.Y + ContTop.Size.Height / 2));
            //LeftBox
            if (PlaceForCont.X > 0)
            {
                if (PlaceForCont.X == 1)
                {
                    Point loc = ((PictureBox) Controls[contslocback + 5 * level + PlaceForCont.Y]).Location;
                    ((PictureBox) Controls[contslocmiddle + 5 * level + PlaceForCont.Y]).Location =
                        new Point(loc.X, loc.Y);
                    ((PictureBox) Controls[contslocmiddle + 5 * level + PlaceForCont.Y]).BackColor = ContLeft.BackColor;
                    ((PictureBox) Controls[contslocmiddle + 5 * level + PlaceForCont.Y]).BorderStyle =
                        BorderStyle.FixedSingle;
                }
                else
                {
                    Point loc = ((PictureBox) Controls[contslocback + 5 * level + PlaceForCont.Y]).Location;
                    ((PictureBox) Controls[contslocfront + 5 * level + PlaceForCont.Y]).Location =
                        new Point(loc.X, loc.Y);
                    ((PictureBox) Controls[contslocfront + 5 * level + PlaceForCont.Y]).BackColor = ContLeft.BackColor;
                    ((PictureBox) Controls[contslocfront + 5 * level + PlaceForCont.Y]).BorderStyle =
                        BorderStyle.FixedSingle;
                }
            }
            else
            {
                ((PictureBox) Controls[contslocback + 5 * level + PlaceForCont.Y]).BackColor = ContLeft.BackColor;
                ((PictureBox) Controls[contslocback + 5 * level + PlaceForCont.Y]).BorderStyle =
                    BorderStyle.FixedSingle;
            }

            ContainersWeb[PlaceForCont.X, PlaceForCont.Y].BackColor = ContLeft.BackColor;
            Color c = Color.FromArgb(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255));
            ContLeft.BackColor = c;

            //TurnOnAllTros
            for (int i = contslocback; i < contslocback + 20; ++i)
            {
                ((PictureBox) Controls[i]).Paint -= new PaintEventHandler(ContsLeft_Paint);
                ((PictureBox) Controls[i]).Paint += new PaintEventHandler(ContsLeft_Paint);
            }

            for (int i = contslocmiddle; i < contslocmiddle + 20; ++i)
            {
                ((PictureBox) Controls[i]).Paint -= new PaintEventHandler(ContsLeft_Paint);
                ((PictureBox) Controls[i]).Paint += new PaintEventHandler(ContsLeft_Paint);
            }

            for (int i = contslocfront; i < contslocfront + 20; ++i)
            {
                ((PictureBox) Controls[i]).Paint -= new PaintEventHandler(ContsLeft_Paint);
                ((PictureBox) Controls[i]).Paint += new PaintEventHandler(ContsLeft_Paint);
            }

            //TopBox
            var tr = TrosTop;
            ContTop.BackColor = c;
            for (int i = krestikloc; i < krestikloc + 15; ++i)
            {
                int temp = (int) ((PictureBox) Controls[i]).Tag;
                if (!ContainerPlaced[temp / 10, temp % 10, 3])
                {
                    Controls[i].Enabled = true;
                    ((PictureBox) Controls[i]).Image = Properties.Resources.disabled_cross;
                    TrosTop = new Tuple<Point, Point>(
                        new Point(((PictureBox) Controls[i]).Location.X, ((PictureBox) Controls[i]).Location.Y),
                        new Point(((PictureBox) Controls[i]).Location.X, ((PictureBox) Controls[i]).Location.Y));
                    ((PictureBox) Controls[i]).Refresh();
                }
            }

            TrosTop = tr;

            Begin.Enabled = true;
            if (ContainerPlaced[PlaceForCont.X, PlaceForCont.Y, 3])
                Begin.Enabled = false;
            var ContPlace = ContainersWeb[PlaceForCont.X, PlaceForCont.Y];
            ContLeft.Location = new Point(ContPlace.Location.X + ContPlace.Size.Width / 2 - ContLeft.Size.Width / 2,
                LeftBox.Location.Y + (int) (LeftBox.Size.Height / 4.3));
            TrosLeft = new Tuple<Point, Point>(
                new Point(LulkaLeft.Location.X + LulkaLeft.Size.Width / 2 - LeftBox.Location.X,
                    LulkaLeft.Location.Y + LulkaLeft.Size.Height - 1 - LeftBox.Location.Y),
                new Point(ContLeft.Location.X + ContLeft.Size.Width / 2 - LeftBox.Location.X,
                    ContLeft.Location.Y - LeftBox.Location.Y));

            ContTop.Refresh();
            TopBox.Refresh();

            AmplBar.Enabled = true;
            WeightBar.Enabled = true;
            w1.Enabled = true;
            w2.Enabled = true;
            w3.Enabled = true;
            w4.Enabled = true;
        }

        static double AdditionToLeftX = 0;
        static double AdditionToTopY = 0;
        static Point begLocLulLeft;
        static Point begLocLulTop;
        static Tuple<Point, Point> begLocTrosLeft;
        static Tuple<Point, Point> begLocTrosTop;

        private void Timer1_Tick(object sender, EventArgs e)
        {
            // Wave function
            // WaveFun1 - sin(x)
            // WaveFun2 - sin(2*x)*sin(0.5*x)^2
            // WaveFun3 - (sin(x)+0.4sin(2x)^2)/2
            // WaveFun4 - 1-|sin(x)|
            double waveFunction = WaveFunc(time);

            //water and ship
            WaterLeft.Location = new Point(5,
                LeftBox.Location.Y + LeftBox.Size.Height - (int) (WaterLevel + Ampl * waveFunction));
            WaterLeft.Size = new Size(LeftBox.Size.Width, (int) (WaterLevel + Ampl * waveFunction));
            ShipLeft.Location = new Point(ShipLeft.Location.X,
                LeftBox.Location.Y + (int) (LeftBox.Size.Height / 1.088) - (int) (WaterLevel + Ampl * waveFunction));

            //containers

            ContainerLocation.z = ContLeft.Location.Y;

            Size consize = ContLeft.Size;

            for (int i = contslocback; i < contslocback + 20; ++i)
            {
                ((PictureBox) Controls[i]).Location = new Point(((PictureBox) Controls[i]).Location.X,
                    oldlocs[i - contslocback] - (int) (Ampl * waveFunction) - ContLeft.Size.Height + 1);
                ((PictureBox) Controls[i]).Refresh();
            }

            for (int i = contslocmiddle; i < contslocmiddle + 20; ++i)
            {
                ((PictureBox) Controls[i]).Location = new Point(((PictureBox) Controls[i]).Location.X,
                    oldlocs[i - contslocmiddle] - (int) (Ampl * waveFunction) - ContLeft.Size.Height + 1);
                ((PictureBox) Controls[i]).Refresh();
            }

            for (int i = contslocfront; i < contslocfront + 20; ++i)
            {
                ((PictureBox) Controls[i]).Location = new Point(((PictureBox) Controls[i]).Location.X,
                    oldlocs[i - contslocfront] - (int) (Ampl * waveFunction) - ContLeft.Size.Height + 1);
                ((PictureBox) Controls[i]).Refresh();
            }

            time += WavePower * 0.8;
            var ContPlace = ContainersWeb[PlaceForCont.X, PlaceForCont.Y];


            begLocLulLeft =
                new Point(CurrentContPlace.Location.X + CurrentContPlace.Size.Width / 2 - LulkaLeft.Size.Width / 2,
                    LulkaLeft.Location.Y);
            begLocLulTop =
                new Point(CurrentContPlace.Location.X + CurrentContPlace.Size.Width / 2 - LulkaTop.Size.Width / 2,
                    CurrentContPlace.Location.Y + CurrentContPlace.Size.Height / 2 - LulkaTop.Size.Height / 2);
            begLocTrosLeft =
                new Tuple<Point, Point>(
                    new Point(CurrentContPlace.Location.X + CurrentContPlace.Size.Width / 2, TrosLeft.Item1.Y),
                    TrosLeft.Item2);
            begLocTrosTop =
                new Tuple<Point, Point>(
                    new Point(CurrentContPlace.Location.X + CurrentContPlace.Size.Width / 2,
                        CurrentContPlace.Location.Y + CurrentContPlace.Size.Height / 2), TrosTop.Item2);

            //фезека
            Physics.Point3D point3dtemp = Physics.Container_Movement(final_wind, weight, rope_length);

            Tuple<Point, Point> TrosLeft2 = begLocTrosLeft;
            TrosLeft = new Tuple<Point, Point>(TrosLeft2.Item1,
                new Point(TrosLeft2.Item1.X + Convert.ToInt32(point3dtemp.x),
                    Convert.ToInt32(TrosLeft2.Item1.Y + point3dtemp.z)));

            Tuple<Point, Point> TrosTop2 = begLocTrosTop;
            TrosTop = new Tuple<Point, Point>(
                new Point(CurrentContPlace.Location.X + CurrentContPlace.Size.Width / 2,
                    CurrentContPlace.Location.Y + CurrentContPlace.Size.Height / 2),
                new Point(TrosTop2.Item1.X + Convert.ToInt32(point3dtemp.x),
                    Convert.ToInt32(TrosTop2.Item1.Y + point3dtemp.y)));

            if (wind_changed)
            {
                if (Math.Abs(wind.X - cur_wind.X) + Math.Abs(wind.Y - cur_wind.Y) > eps)
                    cur_wind = Physics.Wind_to_destination(cur_wind, wind_change_speed);
                else
                {
                    wind = cur_wind;
                    wind_changed = false;
                }
            }

            final_wind = Physics.WindChangeByContainers(contGridTop, cur_wind, PlaceForCont);

            if (animation)
            {
                if ((ContLeft.Location.Y + ContLeft.Size.Height) >= ShipLeft.Location.Y - level * ContLeft.Size.Height)
                {
                    rope_length = begin_rope_length;
                    AdditionToLeftX = 0;
                    AdditionToTopY = 0;
                    animation = false;
                    var x = logic.DeviationCompensation(((ContLeft.Location.X + ContLeft.Size.Width / 2) -
                                                         (CurrentContPlace.Location.X + CurrentContPlace.Size.Width / 2)
                        ));
                    var y = logic.HeightCompensation(
                        ((ContLeft.Location.X + ContLeft.Size.Width / 2) -
                         (CurrentContPlace.Location.X + CurrentContPlace.Size.Width / 2)),
                        (ShipLeft.Location.Y - contGridTop[PlaceForCont.X, PlaceForCont.Y] * PcontHeight -
                         (ContLeft.Location.Y + ContLeft.Size.Height)) / 10.0);
                    var yTop = logic.DeviationCompensation((ContTop.Location.Y + ContTop.Size.Height / 2) -
                                                           (CurrentContPlace.Location.Y +
                                                            CurrentContPlace.Size.Height / 2));
                    AdditionToLeftX += x;
                    AdditionToTopY += yTop;
                    int addX = (int) Math.Truncate(AdditionToLeftX);
                    int addYTop = (int) Math.Truncate(AdditionToTopY);

                    LulkaLeft.Location = new Point(begLocLulLeft.X + addX, begLocLulLeft.Y);
                    TrosLeft = new Tuple<Point, Point>(new Point(TrosLeft.Item1.X + addX, TrosLeft.Item1.Y),
                        new Point(TrosLeft.Item2.X + addX, TrosLeft.Item2.Y));

                    LulkaTop.Location = new Point(begLocLulTop.X + addX, begLocLulTop.Y + addYTop);
                    TrosTop = new Tuple<Point, Point>(new Point(TrosTop.Item1.X + addX, TrosTop.Item1.Y + addYTop),
                        new Point(TrosTop.Item2.X + addX, TrosTop.Item2.Y + addYTop));
                    After_animation();
                }
                else
                {
                    var x = logic.DeviationCompensation(((ContLeft.Location.X + ContLeft.Size.Width / 2) -
                                                         (CurrentContPlace.Location.X + CurrentContPlace.Size.Width / 2)
                        ));
                    var y = logic.HeightCompensation(
                        ((ContLeft.Location.X + ContLeft.Size.Width / 2) -
                         (CurrentContPlace.Location.X + CurrentContPlace.Size.Width / 2)),
                        (ShipLeft.Location.Y - contGridTop[PlaceForCont.X, PlaceForCont.Y] * PcontHeight -
                         (ContLeft.Location.Y + ContLeft.Size.Height)) / (10.0 * Size.Width / 1936.0));
                    var yTop = logic.DeviationCompensation((ContTop.Location.Y + ContTop.Size.Height / 2) -
                                                           (CurrentContPlace.Location.Y +
                                                            CurrentContPlace.Size.Height / 2));
                    //x = speedAlarm ? (x*0.7) : x;
                    AdditionToLeftX += x;
                    AdditionToTopY += yTop;
                    int addX = (int) Math.Truncate(AdditionToLeftX);
                    int addYTop = (int) Math.Truncate(AdditionToTopY);

                    LulkaLeft.Location = new Point(begLocLulLeft.X + addX, begLocLulLeft.Y);
                    TrosLeft = new Tuple<Point, Point>(new Point(TrosLeft.Item1.X + addX, TrosLeft.Item1.Y),
                        new Point(TrosLeft.Item2.X + addX, TrosLeft.Item2.Y));

                    LulkaTop.Location = new Point(begLocLulTop.X + addX, begLocLulTop.Y + addYTop);
                    TrosTop = new Tuple<Point, Point>(new Point(TrosTop.Item1.X + addX, TrosTop.Item1.Y + addYTop),
                        new Point(TrosTop.Item2.X + addX, TrosTop.Item2.Y + addYTop));

                    //фезека
                    rope_length += speedAlarm ? (0) : y;

                    //test
                    //if (mistake == 0)
                    //    for (int i = Math.Max(0, PlaceForCont.X - 1); i < Math.Min(3, PlaceForCont.X + 1); ++i)
                    //        for (int j = Math.Max(0, PlaceForCont.Y - 1); j < Math.Min(5, PlaceForCont.Y + 1); ++j)
                    //            if (((ShipLeft.Location.Y - ContLeft.Location.Y) / ContLeft.Size.Height) <= 5 && !(i == PlaceForCont.X && j == PlaceForCont.Y) && TestCollision(ContainersWeb[i, j], ContTop))
                    //            {
                    //                mistake += 1;
                    //            }

                    //if (mistake == 1)
                    //{
                    //    mistake += 1;
                    //    textBox1.Visible = true;
                    //    textBox1.Location = new Point(Size.Width / 2, Size.Height / 2);
                    //    textBox1.Text = "ВРЕЗАЛИСЬ";
                    //    textBox1.Font = new Font("Arial", 20, FontStyle.Regular);
                    //    textBox1.Size = new Size(200, textBox1.Size.Height);
                    //}
                }
            }

            ContLeft.Location = new Point(TrosLeft.Item2.X - ContLeft.Size.Width / 2,
                TrosLeft.Item2.Y + LeftBox.Location.Y);
            ContTop.Location = new Point(TrosTop.Item2.X - ContTop.Size.Width / 2,
                TrosTop.Item2.Y + TopBox.Location.Y - ContTop.Size.Height / 2);
            //TrosTop = new Tuple<Point, Point>(new Point(LulkaTop.Location.X + LulkaTop.Size.Width / 2 - TopBox.Location.X, LulkaTop.Location.Y + LulkaTop.Size.Height / 2 - 1 - TopBox.Location.Y), new Point(ContTop.Location.X + ContTop.Size.Width / 2 - TopBox.Location.X, ContTop.Location.Y - TopBox.Location.Y + ContTop.Size.Height / 2));


            //WeightText.Text = cur_wind.X + " " + cur_wind.Y;
            //WeightText.Text = PlaceForCont.X + " " + PlaceForCont.Y;
            //WeightText.Text = "" + contGridTop[PlaceForCont.X,PlaceForCont.Y];
            //WeightText.Text = "" + ContLeft.Location.Y + " > " + (Form1.ShipLeft.Location.Y - contGridTop[PlaceForCont.X, PlaceForCont.Y] * PcontHeight);
            //final_wind = cur_wind;

            //WeightText.Text = speedAlarm + "";
            ShipTop.Refresh();
            ContTop.Refresh();
            TopBox.Refresh();
            LeftBox.Refresh();
        }

        private bool TestCollision(PictureBox a, PictureBox b)
        {
            var aRect = new Rectangle(
                a.Location.X,
                a.Location.Y,
                a.Width,
                a.Height);

            var bRect = new Rectangle(
                b.Location.X,
                b.Location.Y,
                b.Width,
                b.Height);

            return aRect.IntersectsWith(bRect);
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