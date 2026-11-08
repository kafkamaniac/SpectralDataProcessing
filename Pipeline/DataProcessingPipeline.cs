
using SpectralDataProcessing.Models;

namespace SpectralDataProcessing.Services;

public class DataProcessingPipeline
{
    private readonly ExcelDataLoader loader = new();
    private readonly BackgroundCorrector corrector = new();
    private readonly DataAnalyzer analyzer = new();
    private readonly Normalizer normalizer = new();
    private readonly DataSplitter splitter = new();

    public DataProcessingResult Process(string filePath)
    {
        var dataset = loader.Load(filePath);
        var rawStatistics = analyzer.Analyze(dataset.Signals);
        double[,] correctedSpectra = corrector.Correct(dataset.Signals, dataset.Background);
        var correctedStatistics = analyzer.Analyze(correctedSpectra);
        double[,] normalizedSpectra = normalizer.Normalize(correctedSpectra);
        var normalizedStatistics = analyzer.Analyze(normalizedSpectra);
        var (trainingData, testingData) = splitter.Split(normalizedSpectra);

        return new DataProcessingResult(
            dataset,
            correctedSpectra,
            normalizedSpectra,
            rawStatistics,
            correctedStatistics,
            normalizedStatistics,
            trainingData,
            testingData
        );
    }
}
