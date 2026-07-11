using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

[TestFixture]
public class SsimTests
{
    [Test]
    public void IdenticalImages()
    {
        using var image = new Image<Rgba32>(100, 100);
        var stream1 = Encode(image);
        var stream2 = Encode(image);
        var ssim = SsimComparer.Calculate(stream1, stream2);
        Assert.That(ssim, Is.EqualTo(1.0));
    }

    [Test]
    public void CompletelyDifferentImages()
    {
        using var black = new Image<Rgba32>(100, 100);
        using var white = new Image<Rgba32>(100, 100);
        for (var y = 0; y < 100; y++)
        {
            for (var x = 0; x < 100; x++)
            {
                white[x, y] = new(255, 255, 255, 255);
            }
        }

        var ssim = SsimComparer.Calculate(Encode(black), Encode(white));
        Assert.That(ssim, Is.LessThan(0.1));
    }

    [Test]
    public void SlightlyDifferentImages()
    {
        using var image1 = new Image<Rgba32>(100, 100);
        using var image2 = image1.Clone();

        // change a few pixels slightly
        for (var x = 0; x < 10; x++)
        {
            image2[x, 0] = new(10, 10, 10, 255);
        }

        var ssim = SsimComparer.Calculate(Encode(image1), Encode(image2));
        Assert.That(ssim, Is.GreaterThan(0.99));
        Assert.That(ssim, Is.LessThan(1.0));
    }

    [Test]
    public void DifferentSizeReturnsZero()
    {
        using var small = new Image<Rgba32>(50, 50);
        using var large = new Image<Rgba32>(100, 100);
        var ssim = SsimComparer.Calculate(Encode(small), Encode(large));
        Assert.That(ssim, Is.EqualTo(0));
    }

    #region SsimThreshold

    [Test]
    public Task VerifyImageWithSsimThreshold()
    {
        var image = new Image<Rgba32>(11, 11)
        {
            [5, 5] = Color.ParseHex("#0000FF").ToPixel<Rgba32>()
        };
        return Verify(image)
            .SsimThreshold(0.95);
    }

    #endregion

    static MemoryStream Encode(Image image)
    {
        var stream = new MemoryStream();
        image.Save(stream, new PngEncoder());
        stream.Position = 0;
        return stream;
    }
}
