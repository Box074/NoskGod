using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Wave
{
    internal abstract class WavePointRangeProvider : WavePointProvider
    {
        public override bool HasPoint(float x, float width)
        {
            return x >= MinX && x <= MaxX;
        }
        public void Move(float offset)
        {
            MinX += offset;
            MaxX += offset;
        }
        public virtual float MinX { get; set; }
        public virtual float MaxX { get; set; }
    }
}
