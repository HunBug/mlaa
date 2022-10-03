using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mlaa.Model
{
    internal class ImageFolderImageSource : IFrameImageSource
    {
        private List<string> imagePaths = new List<string>();
        private Image<Bgr, byte> currentImage = null!;
        public ImageFolderImageSource(string imageFolderPath)
        {
            FolderPath = imageFolderPath;
            InitializeImages();
            _ = GetFrameBitmap(0);
        }

        private void InitializeImages()
        {
            //check if folder exists
            if (!Directory.Exists(FolderPath))
            {
                throw new Exception($"Folder {FolderPath} does not exist");
            }
            //Load all supported images
            imagePaths = new List<string>();
            foreach (string imagePath in Directory.EnumerateFiles(FolderPath))
            {
                if (imagePath.EndsWith(".jpg") || imagePath.EndsWith(".png"))
                {
                    imagePaths.Add(imagePath);
                }
            }
        }
        
        public string Path { get => FolderPath; }

        public int FrameCount => imagePaths.Count;

        public int FrameWidth => currentImage.Width;

        public int FrameHeight => currentImage.Height;

        public int CurrentFrameIndex { get; private set; }

        public string FolderPath { get; }

        public Image<Bgr, byte> GetFrameBitmap(int frameIndex)
        {
            //Load image at index
            CurrentFrameIndex = frameIndex;
            currentImage = new Image<Bgr, byte>(imagePaths[CurrentFrameIndex]);
            return currentImage;
        }
    }
}