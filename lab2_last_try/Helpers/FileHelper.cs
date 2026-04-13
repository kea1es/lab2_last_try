using System;
using System.Globalization;
using System.IO;

namespace NumericalMethodsApp.Helpers
{
    public static class FileHelper
    {
        public static (double a, double b, double eps) ReadEquationData(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            return (
                double.Parse(lines[0], CultureInfo.InvariantCulture),
                double.Parse(lines[1], CultureInfo.InvariantCulture),
                double.Parse(lines[2], CultureInfo.InvariantCulture)
            );
        }

       
        public static void SaveResult(string filePath, string result)
        {
            File.WriteAllText(filePath, result);
        }
    }
}
