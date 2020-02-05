using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace ImgUp
{
    public static class Processor
    {
        private const int QUALITY = 55;
        public static string Resize(string inputPath, int maxHeight)
        {
            var ext = Path.GetExtension(inputPath);
            using (var image = new Bitmap(System.Drawing.Image.FromFile(inputPath)))
            {
                int width, height;
                if (image.Height > maxHeight && maxHeight > 0)
                {
                    width = Convert.ToInt32(image.Width * maxHeight / (double)image.Height);
                    height = maxHeight;
                }
                else
                {
                    width = image.Width;
                    height = image.Height;
                }
                var resized = new Bitmap(width, height);
                using (var graphics = Graphics.FromImage(resized))
                {
                    graphics.CompositingQuality = CompositingQuality.HighSpeed;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.DrawImage(image, 0, 0, width, height);
                    string resizedPath = inputPath.Replace(".png", $"-h{maxHeight}.png");
                    using (var output = File.Open(resizedPath, FileMode.Create))
                    {
                        var qualityParamId = Encoder.Quality;
                        var encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(qualityParamId, QUALITY);
                        var codec = ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == ImageFormat.Png.Guid);
                        resized.Save(output, codec, encoderParameters);
                    }
                    return resizedPath;
                }
            }
        }
    }
}