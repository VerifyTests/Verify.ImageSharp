using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

namespace VerifyTests;

public static class VerifyImageSharp
{
    public static void Initialize()
    {
        VerifierSettings.RegisterFileConverter("png", ConvertPng);
        VerifierSettings.RegisterFileConverter("bmp", ConvertBmp);
        VerifierSettings.RegisterFileConverter("gif", ConvertGif);
        VerifierSettings.RegisterFileConverter("jpg", ConvertJpg);
        VerifierSettings.RegisterFileConverter(ConvertPngImage, (o, extension, _) => IsImage(o, extension, "png"));
        VerifierSettings.RegisterFileConverter(ConvertBmpImage, (o, extension, _) => IsImage(o, extension, "bmp"));
        VerifierSettings.RegisterFileConverter(ConvertGifImage, (o, extension, _) => IsImage(o, extension, "gif"));
        VerifierSettings.RegisterFileConverter(ConvertJpgImage, (o, extension, _) => IsImage(o, extension, "jpg"));
    }

    static bool IsImage(object target, string? extension, string requiredExtension)
    {
        return target is Image; // && requiredExtension == extension;
    }

    static ConversionResult ConvertBmpImage(object image, IReadOnlyDictionary<string, object> context)
    {
        return Convert((Image) image, "bmp", new BmpEncoder());
    }

    static ConversionResult ConvertGifImage(object image, IReadOnlyDictionary<string, object> context)
    {
        return Convert((Image) image, "gif", new GifEncoder());
    }

    static ConversionResult ConvertJpgImage(object image, IReadOnlyDictionary<string, object> context)
    {
        return Convert((Image) image, "jpg", new JpegEncoder());
    }

    static ConversionResult ConvertPngImage(object image, IReadOnlyDictionary<string, object> context)
    {
        return Convert((Image) image, "png", new PngEncoder());
    }

    static ConversionResult Convert(Image image, string extension, IImageEncoder encoder)
    {
        var stream = new MemoryStream();
        var info = image.GetInfo();
        image.Save(stream, encoder);
        stream.Position = 0;
        return new(info, extension, stream);
    }

    static ConversionResult ConvertBmp(Stream stream, IReadOnlyDictionary<string, object> context)
    {
        return Convert(stream, "bmp", new BmpDecoder());
    }

    static ConversionResult ConvertGif(Stream stream, IReadOnlyDictionary<string, object> context)
    {
        return Convert(stream, "gif", new GifDecoder());
    }

    static ConversionResult ConvertJpg(Stream stream, IReadOnlyDictionary<string, object> context)
    {
        return Convert(stream, "jpg", new JpegDecoder());
    }

    static ConversionResult ConvertPng(Stream stream, IReadOnlyDictionary<string, object> context)
    {
        return Convert(stream, "png", new PngDecoder());
    }

    static ConversionResult Convert(Stream stream, string extension, IImageDecoder decoder)
    {
        var image = Image.Load(stream, decoder);
        stream.Position = 0;
        var info = image.GetInfo();
        return new(info, extension, stream);
    }
}