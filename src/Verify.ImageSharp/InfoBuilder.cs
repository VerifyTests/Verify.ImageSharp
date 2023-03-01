static class InfoBuilder
{
    public static object GetInfo(this Image image)
    {
        var metadata = image.Metadata;
        return new
        {
            image.Width,
            image.Height,
            metadata.HorizontalResolution,
            metadata.VerticalResolution,
            metadata.ResolutionUnits,
        };
    }
}