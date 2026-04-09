using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
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
        using var img1 = Image.Load<Rgba32>(received);
        using var img2 = Image.Load<Rgba32>(verified);

        if (img1.Width != img2.Width ||
            img1.Height != img2.Height)
        {
            return 0;
        }

        return CalculateCore(img1, img2);
    }

    static double CalculateCore(Image<Rgba32> img1, Image<Rgba32> img2)
    {
        var width = img1.Width;
        var height = img1.Height;
        var pixelCount = (double) (width * height);

        double sumR1 = 0, sumG1 = 0, sumB1 = 0;
        double sumR2 = 0, sumG2 = 0, sumB2 = 0;
        double sumR1Sq = 0, sumG1Sq = 0, sumB1Sq = 0;
        double sumR2Sq = 0, sumG2Sq = 0, sumB2Sq = 0;
        double sumR12 = 0, sumG12 = 0, sumB12 = 0;

        img1.ProcessPixelRows(img2, (acc1, acc2) =>
        {
            for (var y = 0; y < height; y++)
            {
                var row1 = MemoryMarshal.Cast<Rgba32, uint>(acc1.GetRowSpan(y));
                var row2 = MemoryMarshal.Cast<Rgba32, uint>(acc2.GetRowSpan(y));
                var x = 0;

                if (Vector256.IsHardwareAccelerated)
                {
                    x = AccumulateVector256(
                        row1, row2, width,
                        ref sumR1, ref sumG1, ref sumB1,
                        ref sumR2, ref sumG2, ref sumB2,
                        ref sumR1Sq, ref sumG1Sq, ref sumB1Sq,
                        ref sumR2Sq, ref sumG2Sq, ref sumB2Sq,
                        ref sumR12, ref sumG12, ref sumB12);
                }
                else if (Vector128.IsHardwareAccelerated)
                {
                    x = AccumulateVector128(
                        row1, row2, width,
                        ref sumR1, ref sumG1, ref sumB1,
                        ref sumR2, ref sumG2, ref sumB2,
                        ref sumR1Sq, ref sumG1Sq, ref sumB1Sq,
                        ref sumR2Sq, ref sumG2Sq, ref sumB2Sq,
                        ref sumR12, ref sumG12, ref sumB12);
                }

                for (; x < width; x++)
                {
                    double r1 = (byte) row1[x], g1 = (byte) (row1[x] >> 8), b1 = (byte) (row1[x] >> 16);
                    double r2 = (byte) row2[x], g2 = (byte) (row2[x] >> 8), b2 = (byte) (row2[x] >> 16);

                    sumR1 += r1; sumG1 += g1; sumB1 += b1;
                    sumR2 += r2; sumG2 += g2; sumB2 += b2;
                    sumR1Sq += r1 * r1; sumG1Sq += g1 * g1; sumB1Sq += b1 * b1;
                    sumR2Sq += r2 * r2; sumG2Sq += g2 * g2; sumB2Sq += b2 * b2;
                    sumR12 += r1 * r2; sumG12 += g1 * g2; sumB12 += b1 * b2;
                }
            }
        });

        var ssimR = CalculateChannel(pixelCount, sumR1, sumR2, sumR1Sq, sumR2Sq, sumR12);
        var ssimG = CalculateChannel(pixelCount, sumG1, sumG2, sumG1Sq, sumG2Sq, sumG12);
        var ssimB = CalculateChannel(pixelCount, sumB1, sumB2, sumB1Sq, sumB2Sq, sumB12);

        return (ssimR + ssimG + ssimB) / 3.0;
    }

    static int AccumulateVector256(
        ReadOnlySpan<uint> row1, ReadOnlySpan<uint> row2, int width,
        ref double sumR1, ref double sumG1, ref double sumB1,
        ref double sumR2, ref double sumG2, ref double sumB2,
        ref double sumR1Sq, ref double sumG1Sq, ref double sumB1Sq,
        ref double sumR2Sq, ref double sumG2Sq, ref double sumB2Sq,
        ref double sumR12, ref double sumG12, ref double sumB12)
    {
        var vR1 = Vector256<float>.Zero; var vG1 = Vector256<float>.Zero; var vB1 = Vector256<float>.Zero;
        var vR2 = Vector256<float>.Zero; var vG2 = Vector256<float>.Zero; var vB2 = Vector256<float>.Zero;
        var vR1Sq = Vector256<float>.Zero; var vG1Sq = Vector256<float>.Zero; var vB1Sq = Vector256<float>.Zero;
        var vR2Sq = Vector256<float>.Zero; var vG2Sq = Vector256<float>.Zero; var vB2Sq = Vector256<float>.Zero;
        var vR12 = Vector256<float>.Zero; var vG12 = Vector256<float>.Zero; var vB12 = Vector256<float>.Zero;
        var mask = Vector256.Create(0x000000FFu);
        ref var ref1 = ref MemoryMarshal.GetReference(row1);
        ref var ref2 = ref MemoryMarshal.GetReference(row2);
        var x = 0;

        for (; x <= width - Vector256<uint>.Count; x += Vector256<uint>.Count)
        {
            var p1 = Vector256.LoadUnsafe(ref ref1, (nuint) x);
            var p2 = Vector256.LoadUnsafe(ref ref2, (nuint) x);

            var r1 = Vector256.ConvertToSingle((p1 & mask).AsInt32());
            var g1 = Vector256.ConvertToSingle((Vector256.ShiftRightLogical(p1, 8) & mask).AsInt32());
            var b1 = Vector256.ConvertToSingle((Vector256.ShiftRightLogical(p1, 16) & mask).AsInt32());
            var r2 = Vector256.ConvertToSingle((p2 & mask).AsInt32());
            var g2 = Vector256.ConvertToSingle((Vector256.ShiftRightLogical(p2, 8) & mask).AsInt32());
            var b2 = Vector256.ConvertToSingle((Vector256.ShiftRightLogical(p2, 16) & mask).AsInt32());

            vR1 += r1; vG1 += g1; vB1 += b1;
            vR2 += r2; vG2 += g2; vB2 += b2;
            vR1Sq += r1 * r1; vG1Sq += g1 * g1; vB1Sq += b1 * b1;
            vR2Sq += r2 * r2; vG2Sq += g2 * g2; vB2Sq += b2 * b2;
            vR12 += r1 * r2; vG12 += g1 * g2; vB12 += b1 * b2;
        }

        sumR1 += Vector256.Sum(vR1); sumG1 += Vector256.Sum(vG1); sumB1 += Vector256.Sum(vB1);
        sumR2 += Vector256.Sum(vR2); sumG2 += Vector256.Sum(vG2); sumB2 += Vector256.Sum(vB2);
        sumR1Sq += Vector256.Sum(vR1Sq); sumG1Sq += Vector256.Sum(vG1Sq); sumB1Sq += Vector256.Sum(vB1Sq);
        sumR2Sq += Vector256.Sum(vR2Sq); sumG2Sq += Vector256.Sum(vG2Sq); sumB2Sq += Vector256.Sum(vB2Sq);
        sumR12 += Vector256.Sum(vR12); sumG12 += Vector256.Sum(vG12); sumB12 += Vector256.Sum(vB12);
        return x;
    }

    static int AccumulateVector128(
        ReadOnlySpan<uint> row1, ReadOnlySpan<uint> row2, int width,
        ref double sumR1, ref double sumG1, ref double sumB1,
        ref double sumR2, ref double sumG2, ref double sumB2,
        ref double sumR1Sq, ref double sumG1Sq, ref double sumB1Sq,
        ref double sumR2Sq, ref double sumG2Sq, ref double sumB2Sq,
        ref double sumR12, ref double sumG12, ref double sumB12)
    {
        var vR1 = Vector128<float>.Zero; var vG1 = Vector128<float>.Zero; var vB1 = Vector128<float>.Zero;
        var vR2 = Vector128<float>.Zero; var vG2 = Vector128<float>.Zero; var vB2 = Vector128<float>.Zero;
        var vR1Sq = Vector128<float>.Zero; var vG1Sq = Vector128<float>.Zero; var vB1Sq = Vector128<float>.Zero;
        var vR2Sq = Vector128<float>.Zero; var vG2Sq = Vector128<float>.Zero; var vB2Sq = Vector128<float>.Zero;
        var vR12 = Vector128<float>.Zero; var vG12 = Vector128<float>.Zero; var vB12 = Vector128<float>.Zero;
        var mask = Vector128.Create(0x000000FFu);
        ref var ref1 = ref MemoryMarshal.GetReference(row1);
        ref var ref2 = ref MemoryMarshal.GetReference(row2);
        var x = 0;

        for (; x <= width - Vector128<uint>.Count; x += Vector128<uint>.Count)
        {
            var p1 = Vector128.LoadUnsafe(ref ref1, (nuint) x);
            var p2 = Vector128.LoadUnsafe(ref ref2, (nuint) x);

            var r1 = Vector128.ConvertToSingle((p1 & mask).AsInt32());
            var g1 = Vector128.ConvertToSingle((Vector128.ShiftRightLogical(p1, 8) & mask).AsInt32());
            var b1 = Vector128.ConvertToSingle((Vector128.ShiftRightLogical(p1, 16) & mask).AsInt32());
            var r2 = Vector128.ConvertToSingle((p2 & mask).AsInt32());
            var g2 = Vector128.ConvertToSingle((Vector128.ShiftRightLogical(p2, 8) & mask).AsInt32());
            var b2 = Vector128.ConvertToSingle((Vector128.ShiftRightLogical(p2, 16) & mask).AsInt32());

            vR1 += r1; vG1 += g1; vB1 += b1;
            vR2 += r2; vG2 += g2; vB2 += b2;
            vR1Sq += r1 * r1; vG1Sq += g1 * g1; vB1Sq += b1 * b1;
            vR2Sq += r2 * r2; vG2Sq += g2 * g2; vB2Sq += b2 * b2;
            vR12 += r1 * r2; vG12 += g1 * g2; vB12 += b1 * b2;
        }

        sumR1 += Vector128.Sum(vR1); sumG1 += Vector128.Sum(vG1); sumB1 += Vector128.Sum(vB1);
        sumR2 += Vector128.Sum(vR2); sumG2 += Vector128.Sum(vG2); sumB2 += Vector128.Sum(vB2);
        sumR1Sq += Vector128.Sum(vR1Sq); sumG1Sq += Vector128.Sum(vG1Sq); sumB1Sq += Vector128.Sum(vB1Sq);
        sumR2Sq += Vector128.Sum(vR2Sq); sumG2Sq += Vector128.Sum(vG2Sq); sumB2Sq += Vector128.Sum(vB2Sq);
        sumR12 += Vector128.Sum(vR12); sumG12 += Vector128.Sum(vG12); sumB12 += Vector128.Sum(vB12);
        return x;
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
