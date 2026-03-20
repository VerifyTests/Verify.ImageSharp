using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

[TestFixture]
public class Samples
{
    #region VerifyImageFile

    [Test]
    public Task VerifyImageFile() =>
        VerifyFile("sample.jpg");

    #endregion

    #region VerifyImageFile

    [Test]
    public Task VerifyImageFileWithCustomEncoder() =>
        VerifyFile("sample.jpg")
            .EncodeAsPng();

    #endregion

    #region VerifyImage

    [Test]
    public Task VerifyImage()
    {
        var image = new Image<Rgba32>(11, 11)
        {
            [5, 5] = Rgba32.ParseHex("#0000FF")
        };
        return Verify(image);
    }

    #endregion

    #region VerifyImageWithCustomEncoder

    [Test]
    public Task VerifyImageWithCustomEncoder()
    {
        var image = new Image<Rgba32>(11, 11)
        {
            [5, 5] = Rgba32.ParseHex("#0000FF")
        };
        return Verify(image)
            .EncodeAsJpeg();
    }

    #endregion

    [Test]
    public Task StreamWithNonZeroPosition()
    {
        var image = new Image<Rgba32>(11, 11)
        {
            [5, 5] = Rgba32.ParseHex("#FF0000")
        };
        var stream = new MemoryStream();
        image.SaveAsPng(stream);
        // intentionally leave stream.Position at end
        return Verify(stream, "png");
    }
}