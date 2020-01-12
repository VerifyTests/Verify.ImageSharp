using System.Threading.Tasks;
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
}