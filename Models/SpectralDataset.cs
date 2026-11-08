using System;
using System.Collections.Generic;
using System.Text;


namespace SpectralDataProcessing.Models;

public class SpectralDataset
{
    public double[,] Signals { get; set; }
    public double[] Background { get; set; }
    public double[,] ReferenceSpectra { get; set; }
    public int PointCount => Signals.GetLength(0);
    public int MeasurementCount => Signals.GetLength(1);

    public SpectralDataset(
        double[,] signals,
        double[] background,
        double[,] referenceSpectra)
    {
        Signals = signals;
        Background = background;
        ReferenceSpectra = referenceSpectra;
    }
}
