using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

#region TestDefinition
public class Samples :
    VerifyBase
{
    public Samples(ITestOutputHelper output) :
        base(output)
    {
    }

    static Samples()
    {
        VerifyImageSharp.Initialize();
    }
    #endregion

    #region VerifyImageFile

    [Fact]
    public Task VerifyImageFile()
    {
        return VerifyFile("sample.jpg");
    }

    #endregion

    #region VerifyImage

    [Fact]
    public Task VerifyImage()
    {
        var image = new Image<Rgba32>(400, 400)
        {
            [200, 200] = Rgba32.White
        };
        var settings = new VerifySettings();
        settings.UseExtension("png");
        return Verify(image, settings);
    }
    #endregion
}