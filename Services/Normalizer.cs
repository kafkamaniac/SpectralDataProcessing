
namespace SpectralDataProcessing.Services;

public class Normalizer
{
    public double[,] Normalize(double[,] spectra)
    {
        int pointCount = spectra.GetLength(0);
        int measurementCount = spectra.GetLength(1);

        double[,] normalized = new double[pointCount, measurementCount];

        for (int j = 0; j < measurementCount; j++)
        {
            var values = new List<double>();

            for (int i = 0; i < pointCount; i++)
            {
                double value = spectra[i, j];

                if (double.IsFinite(value))
                {
                    values.Add(value);
                }
            }
            if (values.Count == 0)
            {
                for (int i = 0; i < pointCount; i++)
                    normalized[i, j] = double.NaN;

                continue;
            }
            double min = values.Min();
            double max = values.Max();

            for (int i = 0; i < pointCount; i++)
            {
                double value = spectra[i, j];

                if (!double.IsFinite(value))
                {
                    normalized[i, j] = double.NaN;
                }
                else if (max == min)
                {
                    normalized[i, j] = 0;
                }
                else
                {
                    normalized[i, j] =
                        (value - min) / (max - min);
                }
            }
        }
        return normalized;
    }
}
