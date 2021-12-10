using IronXL;
namespace UltrasoundVisualization
{
    internal static class UltrasoundVisualization
    {
        internal delegate void CalculationsOutput(Dictionary<string, double [] []> input, double [] frequencies);


        internal enum FrequencyUnits : int
        {
            Hz = 1,
            MHz = 1_000_000,

        }

        internal enum DistanceUnits : int
        {
            m = 1,
            mm

        }
        /// <summary>
        /// Обчислює повздовжну роздільну здатність
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="frequency"></param>
        /// <returns>Значення</returns>
        internal static double CalculateR(int speed, double frequency,
            FrequencyUnits frUnit = FrequencyUnits.Hz,
            DistanceUnits dstUnit = DistanceUnits.m
            ) => speed / (2 * frequency * (int) frUnit) * ((dstUnit == DistanceUnits.m) ? 1 : 1000);

        internal static void Print(Dictionary<string, double [] []> output, double [] frequencies, CalculationsOutput calculationsOutput)
        {
            calculationsOutput(output, frequencies);
        }
        internal static Dictionary<string, double [] []> CalculateArray(Dictionary<string, int []> environmentSpeeds, double [] frequencies, FrequencyUnits frUnit, DistanceUnits dstUnit)
        {
            var superQuery = from environmentSpeed in environmentSpeeds //в кожній парі назваСередовища/значення швидкостей
                             select                                     //вибрати
                             (
                               environmentSpeed.Key,                    //назву середовища

                               //Масив, який містить масиви для обчислених значень R для кожної частоти
                               (from frequency in frequencies           //в кожній частоті
                                select                                  //вибрати
                                            (
                                                from speed in environmentSpeed.Value //в кожній швидкості (якщо є діяпазон)
                                                select CalculateR(speed, frequency, frUnit, dstUnit)

                                            ).ToArray()
                                ).ToArray()
                               );
            var results = superQuery.ToDictionary(x => x.Item1, y => y.Item2);
            return results;
        }

        internal static void ConsoleOutput(Dictionary<string, double [] []> output, double [] frequencies)
        {
            byte padding = 16;
            string valuesToPrint;
            int tempLength;


            Console.Write("Частота, МГЦ:".PadRight(padding));
            foreach (var frequency in frequencies)
            {
                Console.Write($"{frequency}".PadRight(padding) + "|"); //значення з однією цифрою після коми
            }
            Console.WriteLine("\n" + new string('-', padding * (frequencies.Length + 1)));

            Console.WriteLine("Середовище:".PadRight(padding) + "|\n");


            foreach (var result in output)
            {
                Console.Write($"{result.Key}".PadRight(padding) + "|");
                for (int i = 0; i < frequencies.Length; i++)
                {
                    tempLength = result.Value [i].Length;
                    valuesToPrint = tempLength == 1 ? result.Value [i] [0].ToString("0.00") : $"{result.Value [i] [0]:0.00}-{result.Value [i] [1]:0.00}";
                    Console.Write($"{valuesToPrint}".PadRight(padding));

                }
                Console.WriteLine("\n");
            }
        }
        internal static void ExcelOutput(Dictionary<string, double [] []> output, double [] frequencies)
        {
            int tempLength;
            string valuesToPrint;
            char letter = 'A';


            WorkBook xlWorkbook = WorkBook.Create(ExcelFileFormat.XLSX);
            xlWorkbook.Metadata.Author = "Maksym Syvash";
            WorkSheet xlSheet = xlWorkbook.CreateWorkSheet("R");

            //add data and styles;
            xlSheet ["A1"].Value = "Частота, МГц";
            for (int i = 0; i < frequencies.Length; i++)
            {
                letter = (char) (((int) letter) + 1);
                xlSheet [letter.ToString() + 1].Value = frequencies [i];
            }

            xlSheet ["A3"].Value = "Середовище";

            short row = 4;
            foreach (var result in output)
            {

                xlSheet ["A" + row].Value = result.Key;
                letter = 'A';
                for (int i = 0; i < frequencies.Length; i++)
                {
                    letter = (char) (((int) letter) + 1);
                    tempLength = result.Value [i].Length;
                    valuesToPrint = tempLength == 1 ? result.Value [i] [0].ToString("0.00") : $"{result.Value [i] [0]:0.00}-{result.Value [i] [1]:0.00}";
                    xlSheet [letter.ToString() + row].Value = valuesToPrint;
                }
                ++row;
            }

            xlWorkbook.SaveAs("results.xlsx");

        }



    }
}
