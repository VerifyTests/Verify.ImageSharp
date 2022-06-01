using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

#region TestDefinition

[TestFixture]
public class Samples
{
    static Samples() =>
        VerifyImageSharp.Initialize();

    #endregion

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
}