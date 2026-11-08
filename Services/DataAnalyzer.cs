using SpectralDataProcessing.Models;

namespace SpectralDataProcessing.Services;

public class DataAnalyzer
{
    public IReadOnlyList<SpectrumStatistics> Analyze(double[,] spectra)
    {
        int pointCount = spectra.GetLength(0); //кол-во строк (веществ)
        int measurementCount = spectra.GetLength(1); //кол-во столбцов (замеров)

        var results = new List<SpectrumStatistics>();

        for (int j = 0; j < measurementCount; j++)
        {
            var values = new List<double>();

            int missing = 0;
            int zeros = 0;
            int negatives = 0;

            for (int i = 0; i < pointCount; i++)
            {
                double value = spectra[i, j];

                if (!double.IsFinite(value))
                {
                    missing++;
                    continue;
                }

                values.Add(value);

                if (value == 0)
                    zeros++;

                if (value < 0)
                    negatives++;
            }

            if (values.Count == 0)
            {
                results.Add(new SpectrumStatistics(
                    j + 1, 0, missing, zeros, negatives,
                    double.NaN, double.NaN, double.NaN,
                    double.NaN, double.NaN, double.NaN,
                    double.NaN, 0));

                continue;
            }

            double min = values.Min();
            double max = values.Max();
            double mean = values.Average();

            double variance = values
                .Select(x => Math.Pow(x - mean, 2))
                .Average();

            double stdDev = Math.Sqrt(variance);

            values.Sort();

            double q1 = Quantile(values, 0.25);
            double median = Quantile(values, 0.50);
            double q3 = Quantile(values, 0.75);

            double iqr = q3 - q1;
            double lowerBound = q1 - 1.5 * iqr;
            double upperBound = q3 + 1.5 * iqr;

            int outliers = values.Count(
                x => x < lowerBound || x > upperBound);

            results.Add(new SpectrumStatistics(
                j + 1,
                values.Count,
                missing,
                zeros,
                negatives,
                min,
                max,
                mean,
                stdDev,
                q1,
                median,
                q3,
                outliers));
        }

        return results;
    }

    private static double Quantile(List<double> sorted, double probability)
    {
        double position = (sorted.Count - 1) * probability;

        int lowerIndex = (int)Math.Floor(position);
        int upperIndex = (int)Math.Ceiling(position);

        if (lowerIndex == upperIndex)
            return sorted[lowerIndex];

        double fraction = position - lowerIndex;

        return sorted[lowerIndex] * (1 - fraction)
             + sorted[upperIndex] * fraction;
    }

    public void Print(
        IReadOnlyList<SpectrumStatistics> statistics,
        string title)
    {
        Console.WriteLine($"\n=== {title} ===");

        Console.WriteLine(
            "Спектр | Пропуски | Нули | Отриц. | Среднее | Мин. | Макс. | Выбросы");

        foreach (var s in statistics)
        {
            Console.WriteLine(
                $"{s.SpectrumNumber,6} | " +
                $"{s.MissingCount,8} | " +
                $"{s.ZeroCount,4} | " +
                $"{s.NegativeCount,6} | " +
                $"{s.Mean,8:F5} | " +
                $"{s.Min,7:F4} | " +
                $"{s.Max,7:F4} | " +
                $"{s.OutlierCount,7}");
        }
    }
}