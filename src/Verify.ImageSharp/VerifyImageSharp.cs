namespace VerifyTests;

public static class VerifyImageSharp
{
    public static bool Initialized { get; private set; }

    public static void Initialize(double ssimThreshold = 1.0)
    {
        if (Initialized)
        {
            throw new("Already Initialized");
        }

        Initialized = true;

        InnerVerifier.ThrowIfVerifyHasBeenRun();
        VerifierSettings.RegisterStreamConverter("bmp", ConvertBmp);
        VerifierSettings.RegisterStreamConverter("gif", ConvertGif);
        VerifierSettings.RegisterStreamConverter("jpg", ConvertJpg);
        VerifierSettings.RegisterStreamConverter("png", ConvertPng);
        VerifierSettings.RegisterStreamConverter("tif", ConvertTiff);

        if (ssimThreshold < 1.0)
        {
            Task<CompareResult> Compare(Stream received, Stream verified, IReadOnlyDictionary<string, object> context)
            {
                var threshold = context.TryGetValue("ImageSharpSsimThreshold", out var value)
                    ? (double) value
                    : ssimThreshold;
                var ssim = SsimComparer.Calculate(received, verified);
                var result = ssim >= threshold
                    ? CompareResult.Equal
                    : CompareResult.NotEqual($"SSIM: {ssim:F6}, threshold: {threshold:F6}");
                return Task.FromResult(result);
            }

            VerifierSettings.RegisterStreamComparer("bmp", Compare);
            VerifierSettings.RegisterStreamComparer("gif", Compare);
            VerifierSettings.RegisterStreamComparer("jpg", Compare);
            VerifierSettings.RegisterStreamComparer("png", Compare);
            VerifierSettings.RegisterStreamComparer("tif", Compare);
        }

        var encoder = new PngEncoder();
        VerifierSettings.RegisterFileConverter<Image>((image, context) => ConvertImage(null, image, context, "png", encoder));
    }

    public static void SsimThreshold(this VerifySettings settings, double threshold) =>
        settings.Context["ImageSharpSsimThreshold"] = threshold;

    public static SettingsTask SsimThreshold(this SettingsTask settings, double threshold)
    {
        settings.CurrentSettings.SsimThreshold(threshold);
        return settings;
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

    static ConversionResult ConvertBmp(string? name, Stream stream, IReadOnlyDictionary<string, object> context) =>
        Convert<BmpEncoder>(name, BmpDecoder.Instance, stream, "bmp", context);

    static ConversionResult ConvertGif(string? name, Stream stream, IReadOnlyDictionary<string, object> context) =>
        Convert<GifEncoder>(name, GifDecoder.Instance, stream, "gif", context);

    static ConversionResult ConvertJpg(string? name, Stream stream, IReadOnlyDictionary<string, object> context) =>
        Convert<JpegEncoder>(name, JpegDecoder.Instance, stream, "jpg", context);

    static ConversionResult ConvertPng(string? name, Stream stream, IReadOnlyDictionary<string, object> context) =>
        Convert<PngEncoder>(name, PngDecoder.Instance, stream, "png", context);

    static ConversionResult ConvertTiff(string? name, Stream stream, IReadOnlyDictionary<string, object> context) =>
        Convert<TiffEncoder>(name, TiffDecoder.Instance, stream, "tif", context);

    static ConversionResult Convert<TEncoder>(string? name, IImageDecoder decoder, Stream stream, string extension, IReadOnlyDictionary<string, object> context)
        where TEncoder : IImageEncoder, new()
    {
        stream.Position = 0;
        using var image = decoder.Decode(new(), stream);
        stream.Position = 0;
        return ConvertImage(name, image, context, extension, new TEncoder());
    }

    static ConversionResult ConvertImage(string? name, Image image, IReadOnlyDictionary<string, object> context, string extension, IImageEncoder encoder)
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
        return new(info, [new(extension, stream, name)]);
    }
}