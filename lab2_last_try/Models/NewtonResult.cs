using System.Collections.Generic;

namespace NumericalMethodsApp.Models
{
    public class NewtonResult
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int Iterations { get; set; }
        public List<(double dx, double dy)> Errors { get; set; }
    }
}