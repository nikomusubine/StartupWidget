using System;
using System.Collections.Generic;
using System.Text;

namespace StartupWidget.Models.Types
{
    public struct Position
    {
        public double x { get; set; }
        public double y { get; set; }

        public Position()
        {
            x = 0; y = 0;
        }
        public Position(double x, double y)
        {
            this.x = x; this.y = y;
        }
    }
}