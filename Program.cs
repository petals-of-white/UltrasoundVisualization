using System.Diagnostics;
using static UltrasoundVisualization.UltrasoundVisualization;

//Швидкість звуку в різних середовищах
Dictionary<string, int []> environmentSpeeds = new()
{
    { "Повітря", new int [] { 343 } },
    { "Вода", new int [] { 1480 } },
    { "Легені", new int [] { 400, 1200 } },
    { "Жирова тканина", new int [] { 1350, 1470 } },
    { "Мозок", new int [] { 1520, 1570 } },
    { "Кров", new int [] { 1540, 1600 } },
    { "Печінка", new int [] { 1550, 1610 } },
    { "М'язова тканина", new int [] { 1560, 1620 } },
    { "Нирка", new int [] { 1560 } },
    { "М'які тканина", new int [] { 1540 } },
    { "Кісткова тканина", new int [] { 2500, 4300 } },
    { "Каміння печінки", new int [] { 1420, 2200 } }
};

double [] frequencies = new double [] //in megahertz
{
    1.7,
    12,
    20,
    50
};

var result = CalculateArray(environmentSpeeds, frequencies, FrequencyUnits.MHz, DistanceUnits.mm);

Print(result, frequencies, ConsoleOutput);
Print(result, frequencies, ExcelOutput);




