# <img src="/src/icon.png" height="30px"> Verify.ImageSharp

[![Build status](https://ci.appveyor.com/api/projects/status/o30f8u47l7vv5844?svg=true)](https://ci.appveyor.com/project/SimonCropp/Verify-ImageSharp)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.ImageSharp.svg)](https://www.nuget.org/packages/Verify.ImageSharp/)

Extends [Verify](https://github.com/SimonCropp/Verify) to allow verification of documents via [ImageSharp](https://github.com/SixLabors/ImageSharp).


toc


## NuGet package

https://nuget.org/packages/Verify.ImageSharp/


## Usage

Given a test with the following definition:

snippet: TestDefinition


### Verify a file

snippet: VerifyImageFile

Two files are produced

#### Info file:

Samples.VerifyImageFile.info.verified.txt

snippet: Samples.VerifyImageFile.info.verified.txt

#### Image file:

Samples.VerifyImageFile.verified.jpg

<img src="/src/Tests/Samples.VerifyImageFile.verified.jpg" width="200px">


### Verify an Image

An instance if an `Image` can be verified using the following:

snippet: VerifyImage


## File Samples

http://file-examples.com/


## Release Notes

See [closed milestones](../../milestones?state=closed).


## Icon

[Swirl](https://thenounproject.com/term/pattern/2719636/) designed by [B�CK, RU](https://thenounproject.com/titaniclast/) from [The Noun Project](https://thenounproject.com/creativepriyanka).