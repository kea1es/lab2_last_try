using System;

namespace NumericalMethodsApp.Models
{
    public class SystemEquation
    {
        public string Name { get; }
        public Func<double, double, double> F1 { get; }
        public Func<double, double, double> F2 { get; }
        public Func<double, double, double> Df1dx { get; }
        public Func<double, double, double> Df1dy { get; }
        public Func<double, double, double> Df2dx { get; }
        public Func<double, double, double> Df2dy { get; }

        public SystemEquation(string name,
            Func<double, double, double> f1,
            Func<double, double, double> f2,
            Func<double, double, double> df1dx,
            Func<double, double, double> df1dy,
            Func<double, double, double> df2dx,
            Func<double, double, double> df2dy)
        {
            Name = name;
            F1 = f1;
            F2 = f2;
            Df1dx = df1dx;
            Df1dy = df1dy;
            Df2dx = df2dx;
            Df2dy = df2dy;
        }
    }
}