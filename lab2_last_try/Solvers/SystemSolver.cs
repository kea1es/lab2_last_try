using System;
using System.Collections.Generic;
using NumericalMethodsApp.Models;

namespace NumericalMethodsApp.Solvers
{
    public static class SystemSolver
    {
        // метод Ньютона для решения систем
        public static NewtonResult NewtonMethod(SystemEquation system, double x0, double y0, double eps)
        {
            int iterations = 0;
            double x = x0, y = y0;
            List<(double dx, double dy)> errors = new List<(double, double)>();

            while (iterations < 1000)
            {
                iterations++;

                double f1 = system.F1(x, y);
                double f2 = system.F2(x, y);

                double df1dx = system.Df1dx(x, y);
                double df1dy = system.Df1dy(x, y);
                double df2dx = system.Df2dx(x, y);
                double df2dy = system.Df2dy(x, y);

                double det = df1dx * df2dy - df1dy * df2dx;
                if (Math.Abs(det) < 1e-12)
                {
                    throw new Exception("Якобиан вырожден!");
                }

                double dx = (-f1 * df2dy + f2 * df1dy) / det;
                double dy = (-f2 * df1dx + f1 * df2dx) / det;

                errors.Add((Math.Abs(dx), Math.Abs(dy)));

                x += dx;
                y += dy;

                if (Math.Abs(dx) < eps && Math.Abs(dy) < eps)
                {
                    break;
                }     
            }

            return new NewtonResult { X = x, Y = y, Iterations = iterations, Errors = errors };
        }
    }
}