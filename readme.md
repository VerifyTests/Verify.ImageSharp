# <img src="/src/icon.png" height="30px"> Verify.ImageSharp

[![Discussions](https://img.shields.io/badge/Verify-Discussions-yellow?svg=true&label=)](https://github.com/orgs/VerifyTests/discussions)
[![Build status](https://ci.appveyor.com/api/projects/status/o30f8u47l7vv5844?svg=true)](https://ci.appveyor.com/project/SimonCropp/Verify-ImageSharp)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.ImageSharp.svg)](https://www.nuget.org/packages/Verify.ImageSharp/)

Extends [Verify](https://github.com/VerifyTests/Verify) to allow verification of documents via [ImageSharp](https://github.com/SixLabors/ImageSharp).

**See [Milestones](../../milestones?state=closed) for release notes.**


## NuGet

 * https://nuget.org/packages/Verify.ImageSharp


## Usage

<!-- snippet: enable -->
<a id='snippet-enable'></a>
```cs
[ModuleInitializer]
public static void Init() =>
    VerifyImageSharp.Initialize();
```
<sup><a href='/src/Tests/ModuleInit.cs#L3-L9' title='Snippet source file'>snippet source</a> | <a href='#snippet-enable' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Verify a file

<!-- snippet: VerifyImageFile -->
<a id='snippet-VerifyImageFile'></a>
```cs
[Test]
public Task VerifyImageFile() =>
    VerifyFile("sample.jpg");
```
<sup><a href='/src/Tests/Samples.cs#L7-L13' title='Snippet source file'>snippet source</a> | <a href='#snippet-VerifyImageFile' title='Start of snippet'>anchor</a></sup>
<a id='snippet-VerifyImageFile-1'></a>
```cs
[Test]
public Task VerifyImageFileWithCustomEncoder() =>
    VerifyFile("sample.jpg")
        .EncodeAsPng();
```
<sup><a href='/src/Tests/Samples.cs#L15-L22' title='Snippet source file'>snippet source</a> | <a href='#snippet-VerifyImageFile-1' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Two files are produced


#### Info file:

Samples.VerifyImageFile.verified.txt

<!-- snippet: Samples.VerifyImageFile.verified.txt -->
<a id='snippet-Samples.VerifyImageFile.verified.txt'></a>
```txt
{
  Width: 1599,
  Height: 1066,
  HorizontalResolution: 1.0,
  VerticalResolution: 1.0
}
```
<sup><a href='/src/Tests/Samples.VerifyImageFile.verified.txt#L1-L6' title='Snippet source file'>snippet source</a> | <a href='#snippet-Samples.VerifyImageFile.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Image file:

Samples.VerifyImageFile.verified.jpg

<img src="/src/Tests/Samples.VerifyImageFile.verified.jpg" width="200px">


### Verify an SixLabors.ImageSharp.Image

An instance if an `SixLabors.ImageSharp.Image` can be verified using the following:

<!-- snippet: VerifyImage -->
<a id='snippet-VerifyImage'></a>
```cs
[Test]
public Task VerifyImage()
{
    var image = new Image<Rgba32>(11, 11)
    {
        [5, 5] = Rgba32.ParseHex("#0000FF")
    };
    return Verify(image);
}
```
<sup><a href='/src/Tests/Samples.cs#L24-L36' title='Snippet source file'>snippet source</a> | <a href='#snippet-VerifyImage' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## File Samples

http://file-examples.com/


## Icon

[Swirl](https://thenounproject.com/term/pattern/2719636/) designed by [BÖCK, RU](https://thenounproject.com/titaniclast/) from [The Noun Project](https://thenounproject.com).
