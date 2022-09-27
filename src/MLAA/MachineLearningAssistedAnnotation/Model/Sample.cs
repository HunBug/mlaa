using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mlaa.Model
{
    internal class Sample
    {
        public int FrameIndex { get; set; } = 0;
        public List<Annotation> Annotations { get; set; } = new List<Annotation>();
    }
}
