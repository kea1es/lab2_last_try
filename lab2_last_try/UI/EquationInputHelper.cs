using System;
using System.Globalization;
using System.Windows.Forms;
using NumericalMethodsApp.Helpers;
using NumericalMethodsApp.Models;
using NumericalMethodsApp.Solvers;

namespace NumericalMethodsApp.UI
{
    // Класс для валидации и форматирования ввода уравнений
    public static class EquationInputHelper
    {
        public static string NormalizeDecimalSeparator(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return input.Replace(',', '.');
        }

        public static double ParseDouble(string input)
        {
            return double.Parse(NormalizeDecimalSeparator(input), CultureInfo.InvariantCulture);
        }


        public static void ValidateInput(double a, double b, double eps)
        {
            if (a >= b)
            {
                throw new Exception("Левая граница должна быть меньше правой");
            }
                
            if (eps <= 0)
            {
                throw new Exception("Погрешность должна быть положительной");
            }

        }


        public static void ValidateEpsilon(double eps)
        {
            if (eps <= 0)
            {
                throw new Exception("Погрешность должна быть положительной");
            }
                
        }


        public static void CheckRootsExistence(Equation equation, double a, double b)
        {
            double f_a = equation.Func(a);
            double f_b = equation.Func(b);

            if (f_a * f_b > 0)
            {
                var roots = EquationSolver.FindIntervals(equation, a, b, 100);

                if (roots.Count == 0)
                {
                    throw new Exception("На заданном интервале нет корней");
                }

                else if (roots.Count > 1)
                {
                    MessageBox.Show($"Найдено {roots.Count} корней. Будет найден первый корень.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        // Чтение данных из файла через диалог
        public static (double a, double b, double eps) ReadFromFileDialog()
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.Filter = "Текстовые файлы|*.txt|Все файлы|*.*";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    return FileHelper.ReadEquationData(fileDialog.FileName);
                }
                return (0, 0, 0);
            }
        }

        // Сохранение результата в файл через диалог
        public static void SaveToFileDialog(string result)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Текстовые файлы|*.txt";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    FileHelper.SaveResult(sfd.FileName, result);
                    MessageBox.Show("Результаты сохранены в файл", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}