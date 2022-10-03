using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mlaa.Model
{
    internal class Annotation
    {
        public Rectangle BoundingBox { get; set; }
        public double Confidence { get; set; }
        public AnnotationSource AnnotationSource { get; set; }
        public string? Label { get; set; }
    }
}
