namespace NumericalMethodsApp.Models
{
    public class IterationResult
    {
        public bool Success { get; set; }
        public double Root { get; set; }
        public int Iterations { get; set; }
        public bool Convergent { get; set; }
    }
}
