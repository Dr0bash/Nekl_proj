using System;

namespace Nekl_proj
{
    public class WorldCharacteristics
    {
        public const double MaxRopeDownSpeed = 3.0;         // м/c
        public const double MaxRopeUpSpeed = 1.0;           // м/c
        public const double MaxHorizontalCraneSpeed = 4.0;  // м/c
        
        public const double MaxWindSpeed = 15;              // м/c
        public const double MaxWaveHeight = 8;              // м
        
        public const double TimeDimension = 0.1;            // c

        public double CurrentRopeLength = 0;               // м
    }
}