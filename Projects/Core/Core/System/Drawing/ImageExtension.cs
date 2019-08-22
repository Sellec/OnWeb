namespace System.Drawing
{
    /// <summary>
    /// </summary>
    public static class ImageExtension
    {
        /// <summary>
        /// Создает новое изображение на основе <paramref name="sourceImg"/> и приводит его размеры к ширине <paramref name="newWidth"/> и высоте <paramref name="newHeight"/> пропорционально размерам <paramref name="sourceImg"/>.
        /// </summary>
        public static Image Resize(this Image sourceImg, int newWidth, int newHeight)
        {
            if (sourceImg != null)
            {
                if (sourceImg.Width < newWidth || newWidth == 0) newWidth = sourceImg.Width;
                if (sourceImg.Height < newHeight || newHeight == 0) newHeight = sourceImg.Height;

                var coeff = Math.Max(sourceImg.Width / (float)newWidth, sourceImg.Height / (float)newHeight);

                var newWidth2 = (int)(sourceImg.Width / coeff);
                var newHeight2 = (int)(sourceImg.Height / coeff);

                var newImage = new Bitmap(newWidth2, newHeight2, sourceImg.PixelFormat);
                using (var gr = Graphics.FromImage(newImage))
                {
                    //gr.SmoothingMode = SmoothingMode.HighQuality;
                    // gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    gr.DrawImage(sourceImg, new Rectangle(0, 0, newImage.Width, newImage.Height));
                }

                return newImage;
            }

            return null;
        }

    }
}
