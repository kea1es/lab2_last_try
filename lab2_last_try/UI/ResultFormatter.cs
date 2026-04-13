using NumericalMethodsApp.Models;

namespace NumericalMethodsApp.UI
{
    public static class ResultFormatter
    {
        public static string FormatEquationHeader(Equation equation, double a, double b, double eps, string methodName)
        {
            string result = new string('-', 50) + "\n";
            result += $"Решение уравнения: {equation.Name}\n";
            result += $"Интервал: [{a}, {b}], ε = {eps}\n";
            result += $"Выбранный метод: {methodName}\n";
            result += new string('-', 50) + "\n\n";
            return result;
        }

        public static string FormatEquationSuccess(Equation equation, double root, int iterations)
        {
            string result = "Итог:\n";
            result += $"Корень: x = {root:F10}\n";
            result += $"f(x) = {equation.Func(root):E10}\n";
            result += $"Кол-во итераций: {iterations}\n\n";
            return result;
        }

        public static string FormatError(string error)
        {
            return $"Ошибка: {error}\n\n";
        }


        public static string FormatSystemResult(SystemEquation system, double x0, double y0, double eps, NewtonResult result)
        {
            string output = new string('-', 60) + "\n";
            output += $"Решение системы: {system.Name}\n";
            output += $"Начальные приближения: x₀ = {x0}, y₀ = {y0}\n";
            output += $"Погрешность: ε = {eps}\n";
            output += new string('-', 60) + "\n\n";

            output += "Итог:\n";
            output += $"x = {result.X:F10}\n";
            output += $"y = {result.Y:F10}\n\n";

            output += "Проверка:\n";
            output += $"f1(x,y) = {system.F1(result.X, result.Y):E10}\n";
            output += $"f2(x,y) = {system.F2(result.X, result.Y):E10}\n\n";

            output += $"Кол-во итераций: {result.Iterations}\n\n";
            output += "Вектор погрешностей:\n";

            for (int i = 0; i < result.Errors.Count && i < 20; i++)
            {
                output += $"  Итерация {i + 1}: |Δx| = {result.Errors[i].dx:F10}, |Δy| = {result.Errors[i].dy:F10}\n";
            }

            return output;
        }
    }
}