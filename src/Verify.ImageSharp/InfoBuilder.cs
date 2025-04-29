static class InfoBuilder
{
    public static object GetInfo(this Image image)
    {
        var data = image.Metadata;
        return new
        {
            image.Width,
            image.Height,
            data.HorizontalResolution,
            data.VerticalResolution,
            data.ResolutionUnits
        };
    }
}