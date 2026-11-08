using SpectralDataProcessing.Services;
using SpectralDataProcessing.Visualization;

namespace SpectralDataProcessing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = Path.Combine(AppContext.BaseDirectory,"Data","input.xlsx");

            try
            {
                var loader = new ExcelDataLoader();
                
                var dataset = loader.Load(filePath);

                Console.WriteLine("\nПервые значения Signals:");

                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine(
                        $"Точка {i + 1}: " +
                        string.Join(" | ",
                            Enumerable.Range(0, 5)
                                .Select(j => dataset.Signals[i, j].ToString("F5")))
                    );
                }

                Console.WriteLine("\nПервые значения ReferenceSpectra:");

                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine(
                        $"Точка {i + 1}: " +
                        string.Join(" | ",
                            Enumerable.Range(0, 5)
                                .Select(j => dataset.ReferenceSpectra[i, j].ToString("F5")))
                    );
                }

                Console.WriteLine("\nСравнение диапазонов:");

                for (int j = 0; j < 50; j++)
                {
                    double signalMin = Enumerable.Range(0, 128)
                        .Select(i => dataset.Signals[i, j])
                        .Where(double.IsFinite)
                        .Min();

                    double signalMax = Enumerable.Range(0, 128)
                        .Select(i => dataset.Signals[i, j])
                        .Where(double.IsFinite)
                        .Max();

                    double referenceMin = Enumerable.Range(0, 128)
                        .Select(i => dataset.ReferenceSpectra[i, j])
                        .Where(double.IsFinite)
                        .Min();

                    double referenceMax = Enumerable.Range(0, 128)
                        .Select(i => dataset.ReferenceSpectra[i, j])
                        .Where(double.IsFinite)
                        .Max();

                    Console.WriteLine(
                        $"Измерение {j + 1}: " +
                        $"Signals [{signalMin:F2}; {signalMax:F2}] | " +
                        $"Reference [{referenceMin:F5}; {referenceMax:F5}]");
                }

                Console.WriteLine("Файл успешно загружен!");
                Console.WriteLine();

                Console.WriteLine($"Исходные сигналы: {dataset.Signals.GetLength(0)} x {dataset.Signals.GetLength(1)}");

                Console.WriteLine($"Фоновый сигнал: {dataset.Background.Length}");

                Console.WriteLine($"Контрольные спектры: {dataset.ReferenceSpectra.GetLength(0)} " +
                    $"x {dataset.ReferenceSpectra.GetLength(1)}");

                //КОРРЕКЦИЯ
                var corrector = new BackgroundCorrector();

                double[,] correctedSpectra = corrector.Correct(dataset.Signals, dataset.Background);

                //АНАЛИЗ

                var analyzer = new DataAnalyzer();

                var rawStatistics = analyzer.Analyze(dataset.Signals);
                var correctedStatistics = analyzer.Analyze(correctedSpectra);

                //РИСУЕМ
                var plotService = new PlotService();
                string resultsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Results");

                plotService.SaveSpectrum(dataset.Signals, 0, "Исходный спектр — измерение 1", "Интенсивность",
                    Path.Combine(resultsDirectory, "raw_spectrum.png"));

                plotService.SaveSpectrum(correctedSpectra, 0, "Спектр после коррекции фона — измерение 1",
                    "Относительная величина",
                    Path.Combine(resultsDirectory, "corrected_spectrum.png"));

                Console.WriteLine($"\nГрафики сохранены в: {resultsDirectory}");

                //НОРМАЛИЗАЦИЯ

                var normalizer = new Normalizer();
                double[,] normalizedSignals = normalizer.Normalize(correctedSpectra);

                double[] firstSpectrum = new double[128];
                double[] secondSpectrum = new double[128];

                for (int i = 0; i < 128; i++)
                {
                    firstSpectrum[i] = normalizedSignals[i, 0];
                    secondSpectrum[i] = normalizedSignals[i, 1];
                }

                plotService.SaveComparisonSpectrum(
                    firstSpectrum,
                    secondSpectrum,
                    "Измерение 1",
                    "Измерение 2",
                    "Сравнение нормализованных спектров",
                    Path.Combine(resultsDirectory, "normalized_comparison.png")
                );

                // ПОВТОРНЫЙ АНАЛИЗ ПОСЛЕ ОБРАБОТКИ
                var normalizedStatistics = analyzer.Analyze(normalizedSignals);

                analyzer.Print(
                    normalizedStatistics,
                    "Статистика после обработки и нормализации"
                );

                // РАЗДЕЛЕНИЕ НА ВЫБОРКИ
                var splitter = new DataSplitter();

                var (trainingData, testingData) =
                    splitter.Split(normalizedSignals);

                Console.WriteLine("\nРазделение данных:");

                Console.WriteLine(
                    $"Обучающая выборка: {trainingData.GetLength(0)} x {trainingData.GetLength(1)}");

                Console.WriteLine(
                    $"Тестовая выборка: {testingData.GetLength(0)} x {testingData.GetLength(1)}");

            }
            catch (Exception ex)
            {
                Console.WriteLine("ОШИБКА:");
                Console.WriteLine(ex.Message);
            }

        }

        
    }
}
