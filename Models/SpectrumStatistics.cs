using System;
using System.Collections.Generic;
using System.Text;

namespace SpectralDataProcessing.Models;

public record SpectrumStatistics(
    int SpectrumNumber, //номер измерения
    int ValidCount, //количество корректных значений
    int MissingCount, //пропуски
    int ZeroCount, //значения, равные 0
    int NegativeCount, //отрицательныые значения
    double Min,
    double Max,
    double Mean, //среднее арифм
    double StandardDeviation, //разброс относительно среднего
    double Q1, //первые 25%
    double Median, //медиана 50%
    double Q3, //последние 25%
    int OutlierCount //количество выбросов (выход за границы) по правилу 1,5хIQR
);
