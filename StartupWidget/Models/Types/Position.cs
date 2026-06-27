using System;
using System.Collections.Generic;
using System.Text;

namespace StartupWidget.Models.Types
{
    public struct Position
    {
        public float x { get; set; }
        public float y { get; set; }

        public Position()
        {
            x = 0; y = 0;
        }
        public Position(float x, float y)
        {
            this.x = x; this.y = y;
        }
    }
}