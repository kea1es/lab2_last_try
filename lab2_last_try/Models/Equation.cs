using System;

namespace NumericalMethodsApp.Models
{
    public class Equation
    {
        public string Name { get; }
        public Func<double, double> Func { get; }
        public Func<double, double> Derivative { get; }


        public Equation(string name, Func<double, double> func, Func<double, double> derivative)
        {
            Name = name;
            Func = func;
            Derivative = derivative;
        }
    }
}