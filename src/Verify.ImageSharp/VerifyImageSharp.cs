using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using Verify;

public static class VerifyImageSharp
{
    public static void Initialize()
    {
        SharedVerifySettings.RegisterFileConverter("jpg", "jpg", ConvertJpg);
        SharedVerifySettings.RegisterFileConverter("bmp", "bmp", ConvertBmp);
        SharedVerifySettings.RegisterFileConverter("gif", "gif", ConvertGif);
    }

    static ConversionResult ConvertJpg(Stream stream, VerifySettings settings)
    {
        var image = Image.Load(stream, new JpegDecoder());
        return Convert(stream, image);
    }

    static ConversionResult ConvertBmp(Stream stream, VerifySettings settings)
    {
        var image = Image.Load(stream, new BmpDecoder());
        return Convert(stream, image);
    }

    static ConversionResult ConvertGif(Stream stream, VerifySettings settings)
    {
        var image = Image.Load(stream, new GifDecoder());
        return Convert(stream, image);
    }

    private static ConversionResult Convert(Stream stream, Image image)
    {
        stream.Position = 0;
        var metadata = image.Metadata;
        var info = new
        {
            image.Width,
            image.Height,
            metadata.HorizontalResolution,
            metadata.VerticalResolution,
            metadata.ResolutionUnits,
        };
        return new ConversionResult(info, new List<Stream> {stream});
    }
}