using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mlaa.Model
{
    internal interface IFrameImageSource
    {
        string Path { get; }
        int FrameCount { get; }
        int FrameWidth { get; }
        int FrameHeight { get; }
        int CurrentFrameIndex { get; }

        Image<Bgr, byte> GetFrameBitmap(int frameIndex);
    }
}
