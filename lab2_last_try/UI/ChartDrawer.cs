using System;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using NumericalMethodsApp.Models;

namespace NumericalMethodsApp.UI
{
    /// <summary>
    /// Класс для отрисовки графиков
    /// </summary>
    public static class ChartDrawer
    {
        // построение графика функции
        public static void DrawFunctionGraph(Chart chart, Equation equation, double a, double b)
        {
            chart.Series.Clear();

            var series = new Series("f(x)")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Blue,
                BorderWidth = 2
            };

            var zeroSeries = new Series("y=0")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Red,
                BorderWidth = 1,
                BorderDashStyle = ChartDashStyle.Dash
            };

            double step = (b - a) / 500;
            for (double x = a; x <= b; x += step)
            {
                double y = equation.Func(x);
                if (Math.Abs(y) < 1000)
                    series.Points.AddXY(x, y);
            }

            zeroSeries.Points.AddXY(a, 0);
            zeroSeries.Points.AddXY(b, 0);

            chart.Series.Add(series);
            chart.Series.Add(zeroSeries);

            chart.ChartAreas[0].AxisX.Title = "x";
            chart.ChartAreas[0].AxisY.Title = "f(x)";
            chart.ChartAreas[0].RecalculateAxesScale();
        }

        // отметка корня на графике
        public static void MarkRootOnGraph(Chart chart, double root)
        {
            try
            {
                var rootSeries = new Series("Найденный корень")
                {
                    ChartType = SeriesChartType.Point,
                    Color = Color.Green,
                    MarkerSize = 14,
                    MarkerStyle = MarkerStyle.Diamond,
                    BorderWidth = 2,
                    BorderColor = Color.Black
                };

                rootSeries.Points.AddXY(root, 0);
                rootSeries.Points[0].Label = $"  x = {root:F6}";
                rootSeries.Points[0].LabelForeColor = Color.DarkGreen;
                rootSeries.Points[0].Font = new Font("Arial", 9, FontStyle.Bold);
                chart.Series.Add(rootSeries);
                chart.Invalidate();
            }
            catch { }
        }

        // построение графиков для системы
        public static void DrawSystemGraph(Chart chart, SystemEquation system, int systemIndex, NewtonResult lastSolution)
        {
            chart.Series.Clear();

            var series1 = new Series("f1(x,y) = 0")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Blue,
                BorderWidth = 2
            };

            var series2 = new Series("f2(x,y) = 0")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Red,
                BorderWidth = 2
            };

            if (systemIndex == 0) // Система 1
            {
                for (double x = -2; x <= 2; x += 0.01)
                {
                    double y = (2 - Math.Sin(x)) / 2;

                    if (y >= -2 && y <= 2)
                    {
                        series1.Points.AddXY(x, y);
                    }   
                }

                for (double y = -2; y <= 2; y += 0.01)
                {
                    double x = 0.7 - Math.Cos(y - 1);

                    if (x >= -2 && x <= 2)
                    {
                        series2.Points.AddXY(x, y);
                    }    
                }
            }

            else if (systemIndex == 1) // Система 2
            {
                for (double x = -2; x <= 2; x += 0.01)
                {
                    double y = 1.5 - Math.Cos(x);

                    if (y >= -2 && y <= 2)
                    {
                        series2.Points.AddXY(x, y);
                    }
                }

                for (double y = -2; y <= 2; y += 0.01)
                {
                    double x = (1 + Math.Sin(y - 0.5)) / 2;

                    if (x >= -2 && x <= 2)
                    {
                        series1.Points.AddXY(x, y);
                    }
                }
            }

            chart.Series.Add(series1);
            chart.Series.Add(series2);

            if (lastSolution != null)
            {
                SetChartScaleAroundSolution(chart, lastSolution);

                var solutionPoint = new Series("Решение")
                {
                    ChartType = SeriesChartType.Point,
                    Color = Color.Green,
                    MarkerSize = 15,
                    MarkerStyle = MarkerStyle.Diamond,
                    BorderWidth = 3,
                    BorderColor = Color.Black
                };

                solutionPoint.Points.AddXY(lastSolution.X, lastSolution.Y);
                solutionPoint.Points[0].Label = $"  ({lastSolution.X:F4}, {lastSolution.Y:F4})";
                solutionPoint.Points[0].LabelForeColor = Color.DarkGreen;
                solutionPoint.Points[0].Font = new Font("Arial", 9, FontStyle.Bold);
                chart.Series.Add(solutionPoint);
            }
            else
            {
                chart.ChartAreas[0].AxisX.Minimum = -2;
                chart.ChartAreas[0].AxisX.Maximum = 2;
                chart.ChartAreas[0].AxisY.Minimum = -2;
                chart.ChartAreas[0].AxisY.Maximum = 2;
            }

            ConfigureChartAppearance(chart);
        }

        private static void SetChartScaleAroundSolution(Chart chart, NewtonResult solution)
        {
            double minX = -2, maxX = 2, minY = -2, maxY = 2;

            if (solution.X < minX) minX = solution.X - 0.5;
            if (solution.X > maxX) maxX = solution.X + 0.5;
            if (solution.Y < minY) minY = solution.Y - 0.5;
            if (solution.Y > maxY) maxY = solution.Y + 0.5;

            chart.ChartAreas[0].AxisX.Minimum = minX;
            chart.ChartAreas[0].AxisX.Maximum = maxX;
            chart.ChartAreas[0].AxisY.Minimum = minY;
            chart.ChartAreas[0].AxisY.Maximum = maxY;
        }

        private static void ConfigureChartAppearance(Chart chart)
        {
            chart.ChartAreas[0].AxisX.Title = "x";
            chart.ChartAreas[0].AxisY.Title = "y";
            chart.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
            chart.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;

            chart.Legends.Clear();
            var legend = new Legend();
            legend.DockedToChartArea = chart.ChartAreas[0].Name;
            legend.Docking = Docking.Top;
            chart.Legends.Add(legend);

            chart.ChartAreas[0].BackColor = Color.White;
            chart.ChartAreas[0].AxisX.LineColor = Color.Black;
            chart.ChartAreas[0].AxisY.LineColor = Color.Black;

            chart.Invalidate();
            chart.Update();
        }
    }
}
