using System;
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
            VerifierSettings.RegisterFileConverter(ConvertImage, IsImage);
        }

        private static bool IsImage(object target)
        {
            return target is Image;
        }

        public static void TargetExtension(this VerifySettings settings, string extension)
        {
            Guard.AgainstNull(settings, nameof(settings));
            Guard.AgainstNullOrEmpty(extension, nameof(extension));
            settings.Data["VerifyImageSharpTargetExtension"] = extension;
        }
        static string GetTargetExtension(this VerifySettings settings)
        {
            if (settings.Data.TryGetValue("VerifyImageSharpTargetExtension", out var extension))
            {
                return (string) extension;
            }
            throw new Exception("VerifySettings.TargetExtension() must be used to set the output extension.");
        }

        static ConversionResult ConvertImage(object image, VerifySettings settings)
        {
            return Convert((Image) image, GetTargetExtension(settings), new BmpEncoder());
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