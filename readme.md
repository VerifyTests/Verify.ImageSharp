# <img src="/src/icon.png" height="30px"> Verify.ImageSharp

[![Build status](https://ci.appveyor.com/api/projects/status/o30f8u47l7vv5844?svg=true)](https://ci.appveyor.com/project/SimonCropp/Verify-ImageSharp)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.ImageSharp.svg)](https://www.nuget.org/packages/Verify.ImageSharp/)

Extends [Verify](https://github.com/VerifyTests/Verify) to allow verification of documents via [ImageSharp](https://github.com/SixLabors/ImageSharp).

Support is available via a [Tidelift Subscription](https://tidelift.com/subscription/pkg/nuget-verify?utm_source=nuget-verify&utm_medium=referral&utm_campaign=enterprise).

<a href='https://dotnetfoundation.org' alt='Part of the .NET Foundation'><img src='https://raw.githubusercontent.com/VerifyTests/Verify/master/docs/dotNetFoundation.svg' height='30px'></a><br>
Part of the <a href='https://dotnetfoundation.org' alt=''>.NET Foundation</a>

<!-- toc -->
## Contents

  * [Usage](#usage)
    * [Verify a file](#verify-a-file)
    * [Verify an Image](#verify-an-image)
  * [File Samples](#file-samples)
  * [Security contact information](#security-contact-information)<!-- endToc -->


## NuGet package

https://nuget.org/packages/Verify.ImageSharp/


## Usage

Given a test with the following definition:

<!-- snippet: TestDefinition -->
<a id='snippet-testdefinition'></a>
```cs
[TestFixture]
public class Samples
{
    static Samples()
    {
        VerifyImageSharp.Initialize();
    }
```
<sup><a href='/src/Tests/Samples.cs#L8-L16' title='Snippet source file'>snippet source</a> | <a href='#snippet-testdefinition' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Verify a file

<!-- snippet: VerifyImageFile -->
<a id='snippet-verifyimagefile'></a>
```cs
[Test]
public Task VerifyImageFile()
{
    return Verifier.VerifyFile("sample.jpg");
}
```
<sup><a href='/src/Tests/Samples.cs#L18-L26' title='Snippet source file'>snippet source</a> | <a href='#snippet-verifyimagefile' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Two files are produced


#### Info file:

Samples.VerifyImageFile.info.verified.txt

<!-- snippet: Samples.VerifyImageFile.info.verified.txt -->
<a id='snippet-Samples.VerifyImageFile.info.verified.txt'></a>
```txt
{
  Width: 1599,
  Height: 1066,
  HorizontalResolution: 1.0,
  VerticalResolution: 1.0
}
```
<sup><a href='/src/Tests/Samples.VerifyImageFile.info.verified.txt#L1-L6' title='Snippet source file'>snippet source</a> | <a href='#snippet-Samples.VerifyImageFile.info.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Image file:

Samples.VerifyImageFile.verified.jpg

<img src="/src/Tests/Samples.VerifyImageFile.verified.jpg" width="200px">


### Verify an Image

An instance if an `Image` can be verified using the following:

<!-- snippet: VerifyImage -->
<a id='snippet-verifyimage'></a>
```cs
[Test]
public Task VerifyImage()
{
    var image = new Image<Rgba32>(11, 11)
    {
        [5, 5] = Rgba32.ParseHex("#0000FF")
    };
    return Verifier.Verify(image).UseExtension("png");
}
```
<sup><a href='/src/Tests/Samples.cs#L28-L39' title='Snippet source file'>snippet source</a> | <a href='#snippet-verifyimage' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## File Samples

http://file-examples.com/


## Security contact information

To report a security vulnerability, use the [Tidelift security contact](https://tidelift.com/security). Tidelift will coordinate the fix and disclosure.


## Icon

[Swirl](https://thenounproject.com/term/pattern/2719636/) designed by [B�CK, RU](https://thenounproject.com/titaniclast/) from [The Noun Project](https://thenounproject.com/creativepriyanka).
