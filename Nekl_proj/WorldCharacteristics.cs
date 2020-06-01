//using System;

//namespace Nekl_proj
//{
//    public class WorldCharacteristics
//    {
//        public const double MaxRopeDownSpeed = 3.0;         // м/c
//        public const double MaxRopeUpSpeed = 1.0;           // м/c
//        public const double MaxHorizontalCraneSpeed = 4.0;  // м/c

//        public const double MaxWindSpeed = 15;              // м/c
//        public const double MaxWaveHeight = 8;              // м

//        public const double TimeDimension = 1;            // c

//        public double CurrentRopeLength = 0;               // м
//    }
//}

using System;

namespace Nekl_proj
{
    public class WorldCharacteristics
    {
        public static double MaxRopeDownSpeed; //System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height/1080.0;         // м/c
        public const double MaxRopeUpSpeed = 0.7;           // м/c
        public static double MaxHorizontalCraneSpeed;  // м/c

        public const double MaxWindSpeed = 30;              // м/c
        public const double MaxWaveHeight = 4;              // м

        public static double TimeDimension = 1;            // c

        public double CurrentRopeLength = 0;               // м
    }
}