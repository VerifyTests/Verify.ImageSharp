using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using Verify;

public static class VerifyImageSharp
{
    public static void Initialize()
    {
        SharedVerifySettings.RegisterFileConverter("bmp", "bmp", ConvertBmp);
        SharedVerifySettings.RegisterFileConverter("gif", "gif", ConvertGif);
        SharedVerifySettings.RegisterFileConverter("jpg", "jpg", ConvertJpg);
        SharedVerifySettings.RegisterFileConverter("png", "png", ConvertPng);
        SharedVerifySettings.RegisterFileConverter("bmp", ConvertBmpImage, IsImage);
        SharedVerifySettings.RegisterFileConverter("gif", ConvertGifImage, IsImage);
        SharedVerifySettings.RegisterFileConverter("jpg", ConvertJpgImage, IsImage);
        SharedVerifySettings.RegisterFileConverter("png", ConvertPngImage, IsImage);
    }

    private static bool IsImage(object target)
    {
        return target is Image;
    }

    static ConversionResult ConvertBmpImage(object image, VerifySettings settings)
    {
        return Convert((Image)image, new BmpEncoder());
    }

    static ConversionResult ConvertGifImage(object image, VerifySettings settings)
    {
        return Convert((Image)image, new GifEncoder());
    }

    static ConversionResult ConvertJpgImage(object image, VerifySettings settings)
    {
        return Convert((Image)image, new JpegEncoder());
    }

    static ConversionResult ConvertPngImage(object image, VerifySettings settings)
    {
        return Convert((Image)image, new PngEncoder());
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