using SixLabors.ImageSharp.PixelFormats;

namespace VerifyTests;

public static class SsimComparer
{
    const double k1 = 0.01;
    const double k2 = 0.03;
    const double l = 255.0;
    const double c1 = k1 * l * k1 * l;
    const double c2 = k2 * l * k2 * l;

    public static double Calculate(Stream received, Stream verified)
    {
        using var imgReceived = Image.Load<Rgba32>(received);
        using var imgVerified = Image.Load<Rgba32>(verified);

        if (imgReceived.Width != imgVerified.Width ||
            imgReceived.Height != imgVerified.Height)
        {
            return 0;
        }

        var width = imgReceived.Width;
        var height = imgReceived.Height;
        var pixelCount = (double) (width * height);

        double sumR1 = 0, sumG1 = 0, sumB1 = 0;
        double sumR2 = 0, sumG2 = 0, sumB2 = 0;
        double sumR1Sq = 0, sumG1Sq = 0, sumB1Sq = 0;
        double sumR2Sq = 0, sumG2Sq = 0, sumB2Sq = 0;
        double sumR12 = 0, sumG12 = 0, sumB12 = 0;

        imgReceived.ProcessPixelRows(imgVerified, (accessorReceived, accessorVerified) =>
        {
            for (var y = 0; y < height; y++)
            {
                var row1 = accessorReceived.GetRowSpan(y);
                var row2 = accessorVerified.GetRowSpan(y);

                for (var x = 0; x < width; x++)
                {
                    var p1 = row1[x];
                    var p2 = row2[x];

                    double r1 = p1.R, g1 = p1.G, b1 = p1.B;
                    double r2 = p2.R, g2 = p2.G, b2 = p2.B;

                    sumR1 += r1;
                    sumG1 += g1;
                    sumB1 += b1;
                    sumR2 += r2;
                    sumG2 += g2;
                    sumB2 += b2;

                    sumR1Sq += r1 * r1;
                    sumG1Sq += g1 * g1;
                    sumB1Sq += b1 * b1;
                    sumR2Sq += r2 * r2;
                    sumG2Sq += g2 * g2;
                    sumB2Sq += b2 * b2;

                    sumR12 += r1 * r2;
                    sumG12 += g1 * g2;
                    sumB12 += b1 * b2;
                }
            }
        });

        var ssimR = CalculateChannel(pixelCount, sumR1, sumR2, sumR1Sq, sumR2Sq, sumR12);
        var ssimG = CalculateChannel(pixelCount, sumG1, sumG2, sumG1Sq, sumG2Sq, sumG12);
        var ssimB = CalculateChannel(pixelCount, sumB1, sumB2, sumB1Sq, sumB2Sq, sumB12);

        return (ssimR + ssimG + ssimB) / 3.0;
    }

    static double CalculateChannel(double n, double sum1, double sum2, double sum1Sq, double sum2Sq, double sum12)
    {
        var mu1 = sum1 / n;
        var mu2 = sum2 / n;
        var sigma1Sq = sum1Sq / n - mu1 * mu1;
        var sigma2Sq = sum2Sq / n - mu2 * mu2;
        var sigma12 = sum12 / n - mu1 * mu2;

        return (2 * mu1 * mu2 + c1) * (2 * sigma12 + c2) /
               ((mu1 * mu1 + mu2 * mu2 + c1) * (sigma1Sq + sigma2Sq + c2));
    }
}
