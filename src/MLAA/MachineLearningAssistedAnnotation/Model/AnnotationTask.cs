using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mlaa.Model
{
    internal class AnnotationTask
    {
        public AnnotationTask(IFrameImageSource frameImageSource)
        {
            FrameImageSource = frameImageSource;
        }
        public IFrameImageSource FrameImageSource { get; private set; }
        public List<Sample> Samples { get; private set; } = new List<Sample>();
    }
}
