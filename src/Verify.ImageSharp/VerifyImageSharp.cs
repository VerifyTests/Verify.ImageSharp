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
            VerifierSettings.RegisterFileConverter("bmp", ConvertBmp);
            VerifierSettings.RegisterFileConverter("gif", ConvertGif);
            VerifierSettings.RegisterFileConverter("jpg", ConvertJpg);
            VerifierSettings.RegisterFileConverter("png", ConvertPng);
            VerifierSettings.RegisterFileConverter(ConvertBmpImage, IsImage);
            VerifierSettings.RegisterFileConverter(ConvertGifImage, IsImage);
            VerifierSettings.RegisterFileConverter(ConvertJpgImage, IsImage);
            VerifierSettings.RegisterFileConverter(ConvertPngImage, IsImage);
        }

        private static bool IsImage(object target)
        {
            return target is Image;
        }

        static ConversionResult ConvertBmpImage(object image, VerifySettings settings)
        {
            return Convert((Image) image, "bmp", new BmpEncoder());
        }

        static ConversionResult ConvertGifImage(object image, VerifySettings settings)
        {
            return Convert((Image) image, "gif", new GifEncoder());
        }

        static ConversionResult ConvertJpgImage(object image, VerifySettings settings)
        {
            return Convert((Image) image, "jpg", new JpegEncoder());
        }

        static ConversionResult ConvertPngImage(object image, VerifySettings settings)
        {
            return Convert((Image) image, "png", new PngEncoder());
        }

        static ConversionResult Convert(Image image, string extension, IImageEncoder encoder)
        {
            var stream = new MemoryStream();
            var info = image.GetInfo();
            image.Save(stream, encoder);
            stream.Position = 0;
            return new ConversionResult(info, extension, stream);
        }

        static ConversionResult ConvertBmp(Stream stream, VerifySettings settings)
        {
            return Convert(stream, "bmp", new BmpDecoder());
        }

        static ConversionResult ConvertGif(Stream stream, VerifySettings settings)
        {
            return Convert(stream, "gif", new GifDecoder());
        }

        static ConversionResult ConvertJpg(Stream stream, VerifySettings settings)
        {
            return Convert(stream, "jpg", new JpegDecoder());
        }

        static ConversionResult ConvertPng(Stream stream, VerifySettings settings)
        {
            return Convert(stream, "png", new PngDecoder());
        }

        static ConversionResult Convert(Stream stream, string extension, IImageDecoder decoder)
        {
            var image = Image.Load(stream, decoder);
            stream.Position = 0;
            var info = image.GetInfo();
            return new ConversionResult(info, extension, stream);
        }
    }
}