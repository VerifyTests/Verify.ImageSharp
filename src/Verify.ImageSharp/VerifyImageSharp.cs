using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

namespace VerifyTests
{
    public static class VerifyImageSharp
    {
        public static void Initialize()
        {
            VerifierSettings.RegisterFileConverter("bmp", "bmp", ConvertBmp);
            VerifierSettings.RegisterFileConverter("gif", "gif", ConvertGif);
            VerifierSettings.RegisterFileConverter("jpg", "jpg", ConvertJpg);
            VerifierSettings.RegisterFileConverter("png", "png", ConvertPng);
            VerifierSettings.RegisterFileConverter("bmp", ConvertBmpImage, IsImage);
            VerifierSettings.RegisterFileConverter("gif", ConvertGifImage, IsImage);
            VerifierSettings.RegisterFileConverter("jpg", ConvertJpgImage, IsImage);
            VerifierSettings.RegisterFileConverter("png", ConvertPngImage, IsImage);
        }

        private static bool IsImage(object target)
        {
            return target is Image;
        }

        static ConversionResult ConvertBmpImage(object image, VerifySettings settings)
        {
            return Convert((Image) image, new BmpEncoder());
        }

        static ConversionResult ConvertGifImage(object image, VerifySettings settings)
        {
            return Convert((Image) image, new GifEncoder());
        }

        static ConversionResult ConvertJpgImage(object image, VerifySettings settings)
        {
            return Convert((Image) image, new JpegEncoder());
        }

        static ConversionResult ConvertPngImage(object image, VerifySettings settings)
        {
            return Convert((Image) image, new PngEncoder());
        }

        static ConversionResult Convert(Image image, IImageEncoder encoder)
        {
            var stream = new MemoryStream();
            var info = image.GetInfo();
            image.Save(stream, encoder);
            stream.Position = 0;
            return new ConversionResult(info, new List<Stream> {stream});
        }

        static ConversionResult ConvertBmp(Stream stream, VerifySettings settings)
        {
            return Convert(stream, new BmpDecoder());
        }

        static ConversionResult ConvertGif(Stream stream, VerifySettings settings)
        {
            return Convert(stream, new GifDecoder());
        }

        static ConversionResult ConvertJpg(Stream stream, VerifySettings settings)
        {
            return Convert(stream, new JpegDecoder());
        }

        static ConversionResult ConvertPng(Stream stream, VerifySettings settings)
        {
            return Convert(stream, new PngDecoder());
        }

        static ConversionResult Convert(Stream stream, IImageDecoder decoder)
        {
            var image = Image.Load(stream, decoder);
            stream.Position = 0;
            var info = image.GetInfo();
            return new ConversionResult(info, new List<Stream> {stream});
        }
    }
}