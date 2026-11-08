
namespace SpectralDataProcessing.Services;

internal class DataSplitter
{
    public (double[,] Training, double[,] Testing) Split(
        double[,] spectra,
        double trainRatio = 0.8,
        int seed = 42)
    {
        int pointCount = spectra.GetLength(0);
        int measurementCount = spectra.GetLength(1);

        if (trainRatio <= 0 || trainRatio >= 1)
            throw new ArgumentException(
                "Доля обучения должна быть между 0 и 1.");

        // Создаём индексы всех измерений
        int[] indices = Enumerable.Range(0, measurementCount).ToArray();

        // Перемешиваем индексы
        Random random = new Random(seed);

        indices = indices
            .OrderBy(x => random.Next())
            .ToArray();

        // Определяем размеры выборок
        int trainCount = (int)(measurementCount * trainRatio);
        int testCount = measurementCount - trainCount;

        double[,] training = new double[pointCount, trainCount];
        double[,] testing = new double[pointCount, testCount];

        // Заполняем обучающую выборку
        for (int j = 0; j < trainCount; j++)
        {
            int sourceColumn = indices[j];

            for (int i = 0; i < pointCount; i++)
                training[i, j] = spectra[i, sourceColumn];
        }

        // Заполняем тестовую выборку
        for (int j = 0; j < testCount; j++)
        {
            int sourceColumn = indices[trainCount + j];

            for (int i = 0; i < pointCount; i++)
                testing[i, j] = spectra[i, sourceColumn];
        }

        return (training, testing);
    }
}
