using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

#region TestDefinition
[TestFixture]
public class Samples
{
    static Samples()
    {
        VerifyImageSharp.Initialize();
    }
    #endregion

    #region VerifyImageFile

    [Test]
    public Task VerifyImageFile()
    {
        return VerifyFile("sample.jpg");
    }

    #endregion

    #region VerifyImage

    [Test]
    public Task VerifyImage()
    {
        var image = new Image<Rgba32>(11, 11)
        {
            [5, 5] = Rgba32.ParseHex("#0000FF")
        };
        return Verify(image).UseExtension("png");
    }
    #endregion

    #region VerifyFailsCorrectly
    [Test]
    public void VerifyFailsCorrectly()
    {
        var image = new Image<Rgba32>(11, 11)
        {
            [5, 5] = Rgba32.ParseHex("#0001FF")
        };

        ClearFiles();

        var ex = Assert.ThrowsAsync(Is.AssignableTo<Exception>(), async () =>
        {
            await Verify(image); // .UseExtension("png");
        });

        Assert.AreEqual("VerifyException", ex?.GetType().Name);
    }

    private void ClearFiles([CallerFilePath] string sourceFile = "")
    {
        var folder = Path.GetDirectoryName(sourceFile);
        var files = Directory.EnumerateFiles(folder!, "*.received.png");
        foreach (var file in files)
        {
            File.Delete(file);
        }
    }
    #endregion
}