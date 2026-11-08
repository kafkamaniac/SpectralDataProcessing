using ScottPlot;

namespace SpectralDataProcessing.Visualization;

public class PlotService
{
    public void SaveSpectrum(
        double[,] spectra,
        int measurementIndex,
        string title,
        string yAxisLabel,
        string filePath)
    {
        int pointCount = spectra.GetLength(0);
        int measurementCount = spectra.GetLength(1);

        if (measurementIndex < 0 || measurementIndex >= measurementCount)
        {
            throw new ArgumentOutOfRangeException(nameof(measurementIndex));
        }

        double[] values = new double[pointCount];

        for (int i = 0; i < pointCount; i++)
            values[i] = spectra[i, measurementIndex];

        var plot = new ScottPlot.Plot();

        plot.Add.Signal(values);
        plot.Title(title);
        plot.XLabel("Номер точки спектра");
        plot.YLabel(yAxisLabel);

        string? directory = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        plot.SavePng(filePath, 1200, 700); 
    }

    public void SaveComparisonSpectrum(
    double[] firstSpectrum,
    double[] secondSpectrum,
    string firstLabel,
    string secondLabel,
    string title,
    string filePath)
    {
        if (firstSpectrum.Length != secondSpectrum.Length)
            throw new ArgumentException("Спектры должны иметь одинаковую длину.");

        int pointCount = firstSpectrum.Length;

        double[] x = Enumerable.Range(1, pointCount).Select(i => (double)i).ToArray();

        var plot = new ScottPlot.Plot();


        var firstLine = plot.Add.Scatter(x, firstSpectrum);
        firstLine.Color = ScottPlot.Colors.Red;
        firstLine.LinePattern = ScottPlot.LinePattern.Dashed;
        firstLine.LineWidth = 2;
        firstLine.MarkerSize = 0;
        firstLine.LegendText = firstLabel;

        var secondLine = plot.Add.Scatter(x, secondSpectrum);
        secondLine.Color = ScottPlot.Colors.Blue;
        secondLine.LineWidth = 2;
        secondLine.MarkerSize = 0;
        secondLine.LegendText = secondLabel;

        plot.Title(title);
        plot.XLabel("Номер точки спектра");
        plot.YLabel("Нормированная интенсивность");

        plot.Axes.SetLimitsY(0, 1);

        plot.ShowLegend();

        string? directory = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        plot.SavePng(filePath, 1200, 700);
    }

}