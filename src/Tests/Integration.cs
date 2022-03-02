using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

[TestFixture]
public class Integration
{
    static Integration()
    {
        VerifyImageSharp.Initialize();
    }

    private class Context
    {
        public Context([CallerMemberName] string? testMethodName = null, [CallerFilePath] string sourceFile = "")
        {
            FilePrefix = $"{nameof(Integration)}.{testMethodName}.";
            Folder = Path.GetDirectoryName(sourceFile)!;
        }

        public string FilePrefix { get; }

        public string Folder { get; }

        public string SearchPattern => FilePrefix + "*";

        public IEnumerable<string> GetFileKeys()
        {
            return GetFiles()
                .Select(file => Path.GetFileName(file).Substring(FilePrefix.Length))
                .OrderBy(key => key);
        }

        public void ClearFiles()
        {
            var files = GetFiles();
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        public IEnumerable<string> GetFiles()
        {
            return Directory.EnumerateFiles(Folder, SearchPattern);
        }

        public string GetFullName(string file)
        {
            return Path.Combine(Folder, FilePrefix + file);
        }
    }

    static readonly string[] expectedFiles =
    {
        "00.received.txt",
        "00.verified.txt",
        "01.received.png",
        "01.verified.png"
    };

    private const string expectedText =
@"{
  Width: 11,
  Height: 11,
  HorizontalResolution: 96.0,
  VerticalResolution: 96.0,
  ResolutionUnits: PixelsPerInch
}";

    static readonly Image<Rgba32> red = new(11, 11, Rgba32.ParseHex("#FF0000"));
    static readonly Image<Rgba32> green = new(11, 11, Rgba32.ParseHex("#00FF00"));

    [Test]
    public void VerifyFailsCorrectly_NoFilesExist()
    {
        var context = new Context();

        context.ClearFiles();

        var ex = Assert.ThrowsAsync(Is.AssignableTo<Exception>(), async () =>
        {
            await Verify(red); // .UseExtension("png");
        });
        Assert.AreEqual("VerifyException", ex?.GetType().Name, ex?.Message);
        Assert.AreEqual(expectedFiles, context.GetFileKeys());
    }

    [Test]
    public void VerifyFailsCorrectly_TextExistDifferentImageMissing()
    {
        var context = new Context();

        context.ClearFiles();

        var textFile = context.GetFullName(expectedFiles[1]);
        File.WriteAllText(textFile, "dummy");

        var ex = Assert.ThrowsAsync(Is.AssignableTo<Exception>(), async () =>
        {
            await Verify(red); // .UseExtension("png");
        });
        Assert.AreEqual("VerifyException", ex?.GetType().Name, ex?.Message);
        Assert.AreEqual(expectedFiles, context.GetFileKeys());
    }

    [Test]
    public void VerifyFailsCorrectly_TextExistDifferentImageDifferent()
    {
        var context = new Context();

        context.ClearFiles();

        var textFile = context.GetFullName(expectedFiles[1]);
        File.WriteAllText(textFile, "dummy");
        var imageFile = context.GetFullName(expectedFiles[3]);
        green.SaveAsPng(imageFile);

        var ex = Assert.ThrowsAsync(Is.AssignableTo<Exception>(), async () =>
        {
            await Verify(red); // .UseExtension("png");
        });
        Assert.AreEqual("VerifyException", ex?.GetType().Name, ex?.Message);
        Assert.AreEqual(expectedFiles, context.GetFileKeys());
    }

    [Test]
    public void VerifyFailsCorrectly_TextExistMatchingImageMissing()
    {
        var context = new Context();

        context.ClearFiles();

        var textFile = context.GetFullName(expectedFiles[1]);
        File.WriteAllText(textFile, expectedText);

        var ex = Assert.ThrowsAsync(Is.AssignableTo<Exception>(), async () =>
        {
            await Verify(red); // .UseExtension("png");
        });
        Assert.AreEqual("VerifyException", ex?.GetType().Name, ex?.Message);
        Assert.AreEqual(expectedFiles.Skip(1), context.GetFileKeys(), string.Join(", ", context.GetFileKeys()));
    }

    [Test]
    public void VerifyFailsCorrectly_TextExistMatchingImageDifferent()
    {
        var context = new Context();

        context.ClearFiles();

        var textFile = context.GetFullName(expectedFiles[1]);
        File.WriteAllText(textFile, expectedText);
        var imageFile = context.GetFullName(expectedFiles[3]);
        green.SaveAsPng(imageFile);

        var ex = Assert.ThrowsAsync(Is.AssignableTo<Exception>(), async () =>
        {
            await Verify(red); // .UseExtension("png");
        });
        Assert.AreEqual("VerifyException", ex?.GetType().Name, ex?.Message);
        Assert.AreEqual(expectedFiles, context.GetFileKeys(), string.Join(", ", context.GetFileKeys()));
    }

    [Test]
    public async Task VerifySucceeds_TextAndImageMatching()
    {
        var context = new Context();

        context.ClearFiles();

        var textFile = context.GetFullName(expectedFiles[1]);
        await File.WriteAllTextAsync(textFile, expectedText);
        var imageFile = context.GetFullName(expectedFiles[3]);
        await red.SaveAsPngAsync(imageFile);

        await Verify(red); // .UseExtension("png");

        Assert.AreEqual(expectedFiles.Where(file => file.Contains("verified")), context.GetFileKeys(), string.Join(", ", context.GetFileKeys()));
    }
}

