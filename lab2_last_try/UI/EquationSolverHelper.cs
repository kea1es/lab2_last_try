using System;
using NumericalMethodsApp.Models;
using NumericalMethodsApp.Solvers;

namespace NumericalMethodsApp.UI
{
    public static class EquationSolverHelper
    {
        public static (bool success, double root, int iterations, string error) Solve(
            Equation equation, double a, double b, double eps, int methodIndex)
        {
            switch (methodIndex)
            {
                case 0: // Метод половинного деления
                    try
                    {
                        var result = EquationSolver.BisectionMethod(equation, a, b, eps);
                        return (true, result.Root, result.Iterations, null);
                    }

                    catch (Exception ex)
                    {
                        return (false, 0, 0, ex.Message);
                    }

                case 1: // Метод секущих
                    try
                    {
                        var result = EquationSolver.SecantMethod(equation, a, b, eps);
                        return (true, result.Root, result.Iterations, null);
                    }

                    catch (Exception ex)
                    {
                        return (false, 0, 0, ex.Message);
                    }

                case 2: // Метод простой итерации
                    var iterationResult = EquationSolver.SimpleIterationMethod(equation, a, b, eps);
                    if (iterationResult.Success)
                    {
                        return (true, iterationResult.Root, iterationResult.Iterations, null);
                    }

                    else
                    {
                        return (false, 0, 0, "Не удалось найти φ(x) с условием сходимости |φ'(x)| < 1");
                    }

                default:
                    return (false, 0, 0, "Метод не выбран");
            }
        }
    }
}