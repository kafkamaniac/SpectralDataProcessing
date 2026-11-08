namespace SpectralDataProcessing.Services;

public class BackgroundCorrector
{
    public double[,] Correct(double[,] signals, double[] background)
    {
        int pointCount = signals.GetLength(0);
        int measurementCount = signals.GetLength(1);

        if (background.Length != pointCount)
        {
            throw new ArgumentException("Размер фонового сигнала не совпадает с числом точек спектра.");
        }

        var corrected = new double[pointCount, measurementCount];

        for (int i = 0; i < pointCount; i++)
        {
            double backgroundValue = background[i];

            for (int j = 0; j < measurementCount; j++)
            {
                double signalValue = signals[i, j];

                if (!double.IsFinite(signalValue) ||
                    !double.IsFinite(backgroundValue) ||
                    backgroundValue == 0)
                {
                    corrected[i, j] = double.NaN;
                    continue;
                }

                corrected[i, j] = (backgroundValue - signalValue) / backgroundValue;
            }
        }
        return corrected;
    }
}