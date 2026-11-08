namespace SpectralDataProcessing.Models;

public sealed class DataProcessingResult
{
    public SpectralDataset Dataset { get; }
    public double[,] CorrectedSpectra { get; }
    public double[,] NormalizedSpectra { get; }

    public IReadOnlyList<SpectrumStatistics> RawStatistics { get; }
    public IReadOnlyList<SpectrumStatistics> CorrectedStatistics { get; }
    public IReadOnlyList<SpectrumStatistics> NormalizedStatistics { get; }

    public double[,] TrainingData { get; }
    public double[,] TestingData { get; }

    public DataProcessingResult(
        SpectralDataset dataset,
        double[,] correctedSpectra,
        double[,] normalizedSpectra,
        IReadOnlyList<SpectrumStatistics> rawStatistics,
        IReadOnlyList<SpectrumStatistics> correctedStatistics,
        IReadOnlyList<SpectrumStatistics> normalizedStatistics,
        double[,] trainingData,
        double[,] testingData)
    {
        Dataset = dataset;
        CorrectedSpectra = correctedSpectra;
        NormalizedSpectra = normalizedSpectra;
        RawStatistics = rawStatistics;
        CorrectedStatistics = correctedStatistics;
        NormalizedStatistics = normalizedStatistics;
        TrainingData = trainingData;
        TestingData = testingData;
    }
}