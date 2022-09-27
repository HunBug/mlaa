using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Emgu.CV.Reg;
using System.Windows.Controls;

namespace Mlaa.Model
{
    internal class VideoFrameImageSource : IFrameImageSource
    {
        private VideoCapture VideoCapture;
        public VideoFrameImageSource(string videoFilePath)
        {
            VideoFilePath = videoFilePath;
            VideoCapture = new VideoCapture(VideoFilePath);
            CurrentFrameIndex = 0;
        }
        public string VideoFilePath { get; private set; }
        public int FrameCount { get => (int)Math.Round(VideoCapture.Get(Emgu.CV.CvEnum.CapProp.FrameCount)); }
        public int FrameWidth { get => (int)Math.Round(VideoCapture.Get(Emgu.CV.CvEnum.CapProp.FrameWidth)); }
        public int FrameHeight { get => (int)Math.Round(VideoCapture.Get(Emgu.CV.CvEnum.CapProp.FrameHeight)); }
        public int CurrentFrameIndex { get; private set; }

        public Image<Bgr,byte> GetFrameBitmap(int frameIndex)
        {
            CurrentFrameIndex = frameIndex;
            _ = VideoCapture.Set(Emgu.CV.CvEnum.CapProp.PosFrames, CurrentFrameIndex);
            var frame = VideoCapture.QueryFrame();
            if (frame == null)
            {
                throw new Exception("Failed to read frame from video file.");
            }
            var frameBitmap = frame.ToImage<Bgr, byte>();
            return frameBitmap;
        }
    }
}
