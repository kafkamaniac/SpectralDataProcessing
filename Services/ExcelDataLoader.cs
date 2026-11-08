
using ClosedXML.Excel;
using SpectralDataProcessing.Models;
using System.Globalization;

namespace SpectralDataProcessing.Services;

public class ExcelDataLoader
{
    private const string SheetName = "ИСиТ_data";
    private const int PointCount = 128;
    private const int MeasurementCount = 50;
    private const int SignalStartRow = 5;
    private const int BackgroundStartRow = 138;
    private const int ReferenceStartRow = 271;

    private static readonly CultureInfo RuCulture = CultureInfo.GetCultureInfo("ru-RU"); //русский язык и зпт

    public SpectralDataset Load(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Файл с данными не найден.", filePath);

        using var workbook = new XLWorkbook(filePath);

        if (!workbook.TryGetWorksheet(SheetName, out var worksheet))
            throw new InvalidDataException($"Лист {SheetName} не найден.");

        double[,] signals = new double[PointCount, MeasurementCount];
        double[] background = new double[PointCount];
        double[,] referenceSpectra = new double[PointCount, MeasurementCount];

        for (int i = 0; i < PointCount; i++)
        {
            for (int j = 0; j < MeasurementCount; j++)
            {
                signals[i, j] = ReadValue(worksheet.Cell(SignalStartRow + i, j + 1));
            }
        }

        for (int i = 0; i < PointCount; i++)
        {
            background[i] = ReadValue(worksheet.Cell(BackgroundStartRow + i, 1));
        }

        for (int i = 0; i < PointCount; i++)
        {
            for (int j = 0; j < MeasurementCount; j++)
            {
                referenceSpectra[i, j] = ReadValue(worksheet.Cell(ReferenceStartRow + i, j + 1));
            }
        }

        return new SpectralDataset(signals, background, referenceSpectra);
    }

    private static double ReadValue(IXLCell cell)
    {
        string value = cell.GetString().Trim();
        if (string.IsNullOrWhiteSpace(value))
            return double.NaN;

        if (double.TryParse(value, NumberStyles.Float, RuCulture, out double result))
        {
            return result;
        }

        throw new InvalidDataException($"Некорректное значение {value} в ячейке {cell.Address}.");
    }
}
