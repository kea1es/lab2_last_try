using System;
using System.Collections.Generic;
using NumericalMethodsApp.Models;

namespace NumericalMethodsApp.Solvers
{
    /// <summary>
    /// Класс для решения нелинейных уравнений
    /// </summary>
    public static class EquationSolver
    {
        // метод половинного деления
        public static (double Root, int Iterations) BisectionMethod(Equation equation, double a, double b, double eps)
        {
            int iterations = 0;
            double f_a = equation.Func(a);
            double f_b = equation.Func(b);

            if (f_a * f_b >= 0)
                throw new Exception("Метод половинного деления: f(a) и f(b) должны иметь разные знаки!");

            double c = a;
            while ((b - a) / 2 > eps)
            {
                iterations++;
                c = (a + b) / 2;
                double f_c = equation.Func(c);

                if (f_c == 0) break;

                if (f_a * f_c < 0)
                {
                    b = c;
                    f_b = f_c;
                }
                else
                {
                    a = c;
                    f_a = f_c;
                }
            }

            return (c, iterations);
        }

        // метод секущих
        public static (double Root, int Iterations) SecantMethod(Equation equation, double a, double b, double eps)
        {
            int iterations = 0;
            double x0 = a, x1 = b;
            double f0 = equation.Func(x0);
            double f1 = equation.Func(x1);

            while (Math.Abs(x1 - x0) > eps && iterations < 1000)
            {
                iterations++;
                if (Math.Abs(f1 - f0) < 1e-12) break;

                double x2 = x1 - f1 * (x1 - x0) / (f1 - f0);
                x0 = x1;
                f0 = f1;
                x1 = x2;
                f1 = equation.Func(x1);
            }

            return (x1, iterations);
        }

        // метод простой итерации
        public static IterationResult SimpleIterationMethod(Equation equation, double a, double b, double eps)
        {
            double lambda = -1.0 / FindMaxDerivative(equation, a, b);
            Func<double, double> phi = x => x + lambda * equation.Func(x);

            double maxDerivative = FindMaxDerivativeDerivative(phi, a, b);
            bool convergent = maxDerivative < 1;

            if (!convergent)
            {
                return new IterationResult { Success = false };
            }

            double x0 = (a + b) / 2;
            int iterations = 0;
            double xPrev = x0;
            double xCurrent = phi(xPrev);

            while (Math.Abs(xCurrent - xPrev) > eps && iterations < 1000)
            {
                iterations++;
                xPrev = xCurrent;
                xCurrent = phi(xPrev);
            }

            return new IterationResult
            {
                Root = xCurrent,
                Iterations = iterations,
                Convergent = true,
                Success = true
            };
        }

        // поиск максимума производной
        private static double FindMaxDerivative(Equation equation, double a, double b)
        {
            double max = Math.Abs(equation.Derivative(a));
            for (double x = a; x <= b; x += (b - a) / 100)
            {
                double d = Math.Abs(equation.Derivative(x));
                if (d > max) max = d;
            }
            return max;
        }

        // поиск максимума производной для итерационной функции
        private static double FindMaxDerivativeDerivative(Func<double, double> phi, double a, double b)
        {
            double h = 1e-6;
            double max = 0;

            for (double x = a; x <= b; x += (b - a) / 100)
            {
                double d = Math.Abs((phi(x + h) - phi(x)) / h);
                if (d > max) max = d;
            }

            return max;
        }

        // поиск интервалов с корнями
        public static List<double> FindIntervals(Equation equation, double a, double b, int steps)
        {
            List<double> intervals = new List<double>();
            double step = (b - a) / steps;
            double prevX = a;
            double prevF = equation.Func(prevX);

            for (double x = a + step; x <= b; x += step)
            {
                double f = equation.Func(x);
                if (prevF * f < 0)
                {
                    intervals.Add((prevX + x) / 2);
                }
                prevX = x;
                prevF = f;
            }

            return intervals;
        }
    }
}