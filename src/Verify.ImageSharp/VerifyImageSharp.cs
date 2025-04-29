using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tiff;

namespace VerifyTests;

public static class VerifyImageSharp
{
    public static bool Initialized { get; private set; }

    public static void Initialize()
    {
        if (Initialized)
        {
            throw new("Already Initialized");
        }

        Initialized = true;

        InnerVerifier.ThrowIfVerifyHasBeenRun();
        VerifierSettings.RegisterFileConverter("bmp", ConvertBmp);
        VerifierSettings.RegisterFileConverter("gif", ConvertGif);
        VerifierSettings.RegisterFileConverter("jpg", ConvertJpg);
        VerifierSettings.RegisterFileConverter("png", ConvertPng);
        VerifierSettings.RegisterFileConverter("tif", ConvertTiff);

        var encoder = new PngEncoder();
        VerifierSettings.RegisterFileConverter<Image>((image, context) => ConvertImage(image, context, "png", encoder));
    }

    static void EncodeAs<TEncoder>(this VerifySettings settings, string extension, IImageEncoder? encoder)
        where TEncoder : IImageEncoder, new()
    {
        settings.Context["ImageSharpEncoder"] = encoder ?? new TEncoder();
        settings.Context["ImageSharpExtension"] = extension;
    }

    public static void EncodeAsPng(this VerifySettings settings, PngEncoder? encoder = null) =>
        settings.EncodeAs<PngEncoder>("png", encoder);

    public static SettingsTask EncodeAsPng(this SettingsTask settings, PngEncoder? encoder = null)
    {
        settings.CurrentSettings.EncodeAsPng(encoder);
        return settings;
    }

    public static SettingsTask EncodeAsGif(this SettingsTask settings, GifEncoder? encoder = null)
    {
        settings.CurrentSettings.EncodeAsGif(encoder);
        return settings;
    }

    public static void EncodeAsGif(this VerifySettings settings, GifEncoder? encoder = null) =>
        settings.EncodeAs<GifEncoder>("gif", encoder);

    public static SettingsTask EncodeAsTiff(this SettingsTask settings, TiffEncoder? encoder = null)
    {
        settings.CurrentSettings.EncodeAsTiff(encoder);
        return settings;
    }

    public static void EncodeAsTiff(this VerifySettings settings, TiffEncoder? encoder = null) =>
        settings.EncodeAs<TiffEncoder>("tif", encoder);

    public static SettingsTask EncodeAsBmp(this SettingsTask settings, BmpEncoder? encoder = null)
    {
        settings.CurrentSettings.EncodeAsBmp(encoder);
        return settings;
    }

    public static void EncodeAsBmp(this VerifySettings settings, BmpEncoder? encoder = null) =>
        settings.EncodeAs<BmpEncoder>("bmp", encoder);

    public static SettingsTask EncodeAsJpeg(this SettingsTask settings, JpegEncoder? encoder = null)
    {
        settings.CurrentSettings.EncodeAsJpeg(encoder);
        return settings;
    }

    public static void EncodeAsJpeg(this VerifySettings settings, JpegEncoder? encoder = null) =>
        settings.EncodeAs<JpegEncoder>("jpg", encoder);


    static ConversionResult ConvertBmp(Stream stream, IReadOnlyDictionary<string, object> context) =>
        Convert<BmpEncoder>(BmpDecoder.Instance, stream, "bmp", context);

    static ConversionResult ConvertGif(Stream stream, IReadOnlyDictionary<string, object> context) =>
        Convert<GifEncoder>(GifDecoder.Instance, stream, "gif", context);

    static ConversionResult ConvertJpg(Stream stream, IReadOnlyDictionary<string, object> context) =>
        Convert<JpegEncoder>(JpegDecoder.Instance, stream, "jpg", context);

    static ConversionResult ConvertPng(Stream stream, IReadOnlyDictionary<string, object> context) =>
        Convert<PngEncoder>(PngDecoder.Instance, stream, "png", context);

    static ConversionResult ConvertTiff(Stream stream, IReadOnlyDictionary<string, object> context) =>
        Convert<TiffEncoder>(TiffDecoder.Instance, stream, "tif", context);

    static ConversionResult Convert<TEncoder>(IImageDecoder decoder, Stream stream, string extension, IReadOnlyDictionary<string, object> context)
        where TEncoder : IImageEncoder, new()
    {
        using var image = decoder.Decode(new(), stream);
        stream.Position = 0;
        return ConvertImage(image, context, extension, new TEncoder());
    }

    static ConversionResult ConvertImage(Image image, IReadOnlyDictionary<string, object> context, string extension, IImageEncoder encoder)
    {
        if (context.TryGetValue("ImageSharpEncoder", out var encoderValue))
        {
            extension = (string) context["ImageSharpExtension"];
            encoder = (IImageEncoder) encoderValue;
        }

        var stream = new MemoryStream();
        var info = image.GetInfo();
        image.Save(stream, encoder);
        stream.Position = 0;
        return new(info, extension, stream);
    }
}