using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using NumericalMethodsApp.Models;
using NumericalMethodsApp.Solvers;
using NumericalMethodsApp.UI;
using NumericalMethodsApp.Helpers;
using System.Windows.Forms.DataVisualization.Charting;

namespace NumericalMethodsApp
{
    public class MainForm : Form
    {
        // Компоненты для уравнений
        private ComboBox chooseEquation;
        private ComboBox chooseMethod;
        private TextBox leftBorder, rightBorder, epsilon;
        private RadioButton rbtnKeyboardInput, rbtnFileInput;
        private RadioButton rbtnScreenOutput, rbtnFileOutput;
        private Button btnSolve, btnDrawGraph;
        private RichTextBox txtResult;
        private Chart chartEquation;

        // Компоненты для систем
        private ComboBox cmbSystem;
        private TextBox x0, y0, epsilonSystem;
        private Button btnSolveSystem, btnDrawSystemGraph;
        private RichTextBox resultSystem;
        private Chart chartSystem;

        private List<Equation> equations;
        private List<SystemEquation> systems;
        private NewtonResult lastSolution = null;

        public MainForm()
        {
            InitializeComponents();
            InitializeData();
        }

        private void InitializeComponents()
        {
            this.Text = "Численные методы. Решение нелинейных уравнений и систем";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            var tabControl = new TabControl { Dock = DockStyle.Fill };
            var tabEquations = new TabPage("Нелинейные уравнения");
            var tabSystems = new TabPage("Системы нелинейных уравнений");

            tabControl.TabPages.Add(tabEquations);
            tabControl.TabPages.Add(tabSystems);

            SetupEquationsTab(tabEquations);
            SetupSystemsTab(tabSystems);

            this.Controls.Add(tabControl);
        }

        private void SetupEquationsTab(TabPage tab)
        {
            int y = 20;

            var lableEquation = new Label { Text = "Выберите уравнение:", Location = new Point(20, y), Size = new Size(150, 25) };
            chooseEquation = new ComboBox { Location = new Point(180, y), Width = 400, DropDownStyle = ComboBoxStyle.DropDownList };
            y += 35;

            var lableMethod = new Label { Text = "Выберите метод:", Location = new Point(20, y), Size = new Size(150, 25) };
            chooseMethod = new ComboBox { Location = new Point(180, y), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            chooseMethod.Items.AddRange(new string[] { "Метод половинного деления", "Метод секущих", "Метод простой итерации" });
            chooseMethod.SelectedIndex = 0;

            y += 35;

            var lableLeft = new Label { Text = "Левая граница a:", Location = new Point(20, y), Size = new Size(120, 25) };
            leftBorder = new TextBox { Text = "0", Location = new Point(150, y), Width = 100 };
            var lableRight = new Label { Text = "Правая граница b:", Location = new Point(270, y), Size = new Size(120, 25) };
            rightBorder = new TextBox { Text = "2", Location = new Point(400, y), Width = 100 };

            y += 35;

            var lableEps = new Label { Text = "Погрешность ε:", Location = new Point(20, y), Size = new Size(100, 25) };
            epsilon = new TextBox { Text = "0.0001", Location = new Point(130, y), Width = 100 };

            y += 35;

            var groupInput = CreateInputGroup(ref y);
            var groupOutput = CreateOutputGroup(ref y);

            y += 80;

            btnSolve = new Button { Text = "Решить уравнение", Location = new Point(20, y), Size = new Size(150, 35), BackColor = Color.White };
            btnDrawGraph = new Button { Text = "Построить график", Location = new Point(190, y), Size = new Size(150, 35), BackColor = Color.White };

            y += 50;


            var lableResult = new Label { Text = "Результаты:", Location = new Point(20, y), Size = new Size(100, 25) };
            y += 25;

            txtResult = new RichTextBox { Location = new Point(20, y), Size = new Size(550, 200), Font = new Font("Consolas", 10), ReadOnly = true };
            y += 210;

            chartEquation = new Chart { Location = new Point(600, 20), Size = new Size(550, 450) };
            chartEquation.ChartAreas.Add(new ChartArea());
            chartEquation.ChartAreas[0].AxisX.Title = "x";
            chartEquation.ChartAreas[0].AxisY.Title = "f(x)";

            tab.Controls.AddRange(new Control[] {
                lableEquation, chooseEquation, lableMethod, chooseMethod, lableLeft, leftBorder,
                lableRight, rightBorder, lableEps, epsilon, groupInput, groupOutput,
                btnSolve, btnDrawGraph, lableResult, txtResult, chartEquation
            });
        }

        private GroupBox CreateInputGroup(ref int y)
        {
            var groupInput = new GroupBox { Text = "Ввод данных", Location = new Point(20, y), Size = new Size(250, 70) };

            rbtnKeyboardInput = new RadioButton { Text = "С клавиатуры", Location = new Point(10, 20), Checked = true };
            rbtnFileInput = new RadioButton { Text = "Из файла", Location = new Point(10, 45) };

            groupInput.Controls.AddRange(new Control[] { rbtnKeyboardInput, rbtnFileInput });
            return groupInput;
        }

        private GroupBox CreateOutputGroup(ref int y)
        {
            var groupOutput = new GroupBox { Text = "Вывод результатов", Location = new Point(290, y), Size = new Size(250, 70) };

            rbtnScreenOutput = new RadioButton { Text = "На экран", Location = new Point(10, 20), Checked = true };
            rbtnFileOutput = new RadioButton { Text = "В файл", Location = new Point(10, 45) };

            groupOutput.Controls.AddRange(new Control[] { rbtnScreenOutput, rbtnFileOutput });
            return groupOutput;
        }

        private void SetupSystemsTab(TabPage tab)
        {
            int y = 20;

            var lableSystem = new Label { Text = "Выберите систему:", Location = new Point(20, y), Size = new Size(150, 25) };
            cmbSystem = new ComboBox { Location = new Point(180, y), Width = 400, DropDownStyle = ComboBoxStyle.DropDownList };

            y += 40;


            var lableX0 = new Label { Text = "x₀ =", Location = new Point(20, y), Size = new Size(40, 25) };
            x0 = new TextBox { Text = "-0.25", Location = new Point(70, y), Width = 100 };
            var lableY0 = new Label { Text = "y₀ =", Location = new Point(190, y), Size = new Size(40, 25) };
            y0 = new TextBox { Text = "1.12", Location = new Point(240, y), Width = 100 };

            y += 40;


            var lableEps = new Label { Text = "Погрешность ε:", Location = new Point(20, y), Size = new Size(100, 25) };
            epsilonSystem = new TextBox { Text = "0.0001", Location = new Point(130, y), Width = 100 };

            y += 50;


            btnSolveSystem = new Button { Text = "Решить систему (Метод Ньютона)", Location = new Point(20, y), Size = new Size(220, 35), BackColor = Color.LightGreen };
            btnDrawSystemGraph = new Button { Text = "Построить графики", Location = new Point(260, y), Size = new Size(150, 35), BackColor = Color.LightBlue };

            y += 50;


            var lableResult = new Label { Text = "Результаты:", Location = new Point(20, y), Size = new Size(100, 25) };
            y += 25;


            resultSystem = new RichTextBox { Location = new Point(20, y), Size = new Size(550, 350), Font = new Font("Consolas", 10), ReadOnly = true };
            y += 370;


            chartSystem = new Chart { Location = new Point(600, 20), Size = new Size(550, 450) };
            chartSystem.ChartAreas.Add(new ChartArea());
            chartSystem.ChartAreas[0].AxisX.Title = "x";
            chartSystem.ChartAreas[0].AxisY.Title = "y";

            tab.Controls.AddRange(new Control[] {
                lableSystem, cmbSystem, lableX0, x0, lableY0, y0, lableEps, epsilonSystem,
                btnSolveSystem, btnDrawSystemGraph, lableResult, resultSystem, chartSystem
            });
        }

        private void InitializeData()
        {
            equations = new List<Equation>
            {
                new Equation("f(x) = 2x³ + 3,41x² - 23,74x + 2,95",
                    x => 2*Math.Pow(x,3) + 3.41*Math.Pow(x,2) - 23.74*x + 2.95,
                    x => 6*Math.Pow(x,2) + 6.82*x - 23.74),

                new Equation("f(x) = x³ + 2,84x² - 5,606x - 14,766",
                    x => Math.Pow(x,3) + 2.84*Math.Pow(x,2) - 5.606*x - 14.766,
                    x => 3*Math.Pow(x,2) + 5.68*x - 5.606),

                new Equation("f(x) = sin(x) - x/2",
                    x => Math.Sin(x) - x/2,
                    x => Math.Cos(x) - 0.5),

                new Equation("f(x) = cos(x) - x",
                    x => Math.Cos(x) - x,
                    x => -Math.Sin(x) - 1),

                new Equation("f(x) = e^(-x) - x",
                    x => Math.Exp(-x) - x,
                    x => -Math.Exp(-x) - 1)
            };

            foreach (var eq in equations)
            {
                chooseEquation.Items.Add(eq.Name);
            }
            chooseEquation.SelectedIndex = 0;


            systems = new List<SystemEquation>
            {
                new SystemEquation(
                    "Система 1: sin(x) + 2y = 2, x + cos(y-1) = 0,7",
                    (x, y) => Math.Sin(x) + 2*y - 2,
                    (x, y) => x + Math.Cos(y-1) - 0.7,
                    (x, y) => Math.Cos(x),
                    (x, y) => 2,
                    (x, y) => 1,
                    (x, y) => -Math.Sin(y-1)
                ),

                new SystemEquation(
                    "Система 2: 2x - sin(y-0,5) = 1, y + cos(x) = 1,5",
                    (x, y) => 2*x - Math.Sin(y-0.5) - 1,
                    (x, y) => y + Math.Cos(x) - 1.5,
                    (x, y) => 2,
                    (x, y) => -Math.Cos(y-0.5),
                    (x, y) => -Math.Sin(x),
                    (x, y) => 1
                )
            };

            foreach (var sys in systems)
            {
                cmbSystem.Items.Add(sys.Name);
            }
            cmbSystem.SelectedIndex = 0;

            btnSolve.Click += BtnSolve_Click;
            btnDrawGraph.Click += BtnDrawGraph_Click;
            btnSolveSystem.Click += BtnSolveSystem_Click;
            btnDrawSystemGraph.Click += BtnDrawSystemGraph_Click;
        }

        private void BtnSolve_Click(object sender, EventArgs e)
        {
            try
            {
                double a, b, eps;

                if (rbtnFileInput.Checked)
                {
                    using (OpenFileDialog fileDialog = new OpenFileDialog())
                    {
                        fileDialog.Filter = "Текстовые файлы|*.txt|Все файлы|*.*";
                        if (fileDialog.ShowDialog() == DialogResult.OK)
                        {
                            var data = FileHelper.ReadEquationData(fileDialog.FileName);
                            a = data.a;
                            b = data.b;
                            eps = data.eps;
                        }
                        else return;
                    }
                }

                else
                {
                    a = double.Parse(change(leftBorder.Text), CultureInfo.InvariantCulture);
                    b = double.Parse(change(rightBorder.Text), CultureInfo.InvariantCulture);
                    eps = double.Parse(change(epsilon.Text), CultureInfo.InvariantCulture);
                }

                ValidateInput(a, b, eps);

                Equation equation = equations[chooseEquation.SelectedIndex];
                CheckRootsExistence(equation, a, b);

                string result = BuildResultHeader(equation, a, b, eps);

                var solutionResult = SolveEquation(equation, a, b, eps);

                if (solutionResult.success)
                {
                    result += BuildSuccessResult(solutionResult);
                    ChartDrawer.DrawFunctionGraph(chartEquation, equation, a, b);
                    ChartDrawer.MarkRootOnGraph(chartEquation, solutionResult.root);
                }

                else
                {
                    result += $"Ошибка: {solutionResult.error}\n\n";
                    ChartDrawer.DrawFunctionGraph(chartEquation, equation, a, b);
                }

                if (rbtnFileOutput.Checked)
                {
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Текстовые файлы|*.txt";
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            FileHelper.SaveResult(sfd.FileName, result);
                            MessageBox.Show("Результаты сохранены в файл", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }

                txtResult.Text = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ValidateInput(double a, double b, double eps)
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

        private void CheckRootsExistence(Equation equation, double a, double b)
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

        private string BuildResultHeader(Equation equation, double a, double b, double eps)
        {
            string result = $"{"=".PadRight(60, '=')}\n";
            result += $"Решение уравнения: {equation.Name}\n";
            result += $"Интервал: [{a}, {b}], ε = {eps}\n";
            result += $"Выбранный метод: {chooseMethod.SelectedItem}\n";
            result += $"{"=".PadRight(60, '=')}\n\n";
            return result;
        }

        private (bool success, double root, int iterations, string error) SolveEquation(Equation equation, double a, double b, double eps)
        {
            switch (chooseMethod.SelectedIndex)
            {
                case 0:
                    try
                    {
                        var result = EquationSolver.BisectionMethod(equation, a, b, eps);
                        return (true, result.Root, result.Iterations, null);
                    }

                    catch (Exception ex)
                    {
                        return (false, 0, 0, ex.Message);
                    }


                case 1:
                    try
                    {
                        var result = EquationSolver.SecantMethod(equation, a, b, eps);
                        return (true, result.Root, result.Iterations, null);
                    }

                    catch (Exception ex)
                    {
                        return (false, 0, 0, ex.Message);
                    }


                case 2:
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

        private string BuildSuccessResult((bool success, double root, int iterations, string error) solution)
        {
            string result = $"Итог:\n";
            result += $"  Корень: x = {solution.root:F10}\n";
            result += $"  f(x) = {equations[chooseEquation.SelectedIndex].Func(solution.root):E10}\n";
            result += $"  Кол-во итераций: {solution.iterations}\n\n";
            return result;
        }

        private void BtnDrawGraph_Click(object sender, EventArgs e)
        {
            try
            {
                double a = double.Parse(change(leftBorder.Text), CultureInfo.InvariantCulture);
                double b = double.Parse(change(rightBorder.Text), CultureInfo.InvariantCulture);

                Equation equation = equations[chooseEquation.SelectedIndex];
                ChartDrawer.DrawFunctionGraph(chartEquation, equation, a, b);
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSolveSystem_Click(object sender, EventArgs e)
        {
            try
            {
                double x_0 = double.Parse(change(x0.Text), CultureInfo.InvariantCulture);
                double y_0 = double.Parse(change(y0.Text), CultureInfo.InvariantCulture);
                double eps = double.Parse(change(epsilonSystem.Text), CultureInfo.InvariantCulture);

                if (eps <= 0)
                {
                    throw new Exception("Погрешность должна быть положительной");
                }


                SystemEquation system = systems[cmbSystem.SelectedIndex];
                var result = SystemSolver.NewtonMethod(system, x_0, y_0, eps);
                lastSolution = result;

                ChartDrawer.DrawSystemGraph(chartSystem, system, cmbSystem.SelectedIndex, lastSolution);

                string output = BuildSystemResult(system, x_0, y_0, eps, result);
                resultSystem.Text = output;

                MessageBox.Show("Система решена. Точка решения на графике.", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string BuildSystemResult(SystemEquation system, double x0, double y0, double eps, NewtonResult result)
        {
            string output = $"{"=".PadRight(60, '=')}\n";
            output += $"Решение системы: {system.Name}\n";
            output += $"Начальные приближения: x₀ = {x0}, y₀ = {y0}\n";
            output += $"Погрешность: ε = {eps}\n";
            output += $"{"=".PadRight(60, '=')}\n\n";
            output += $"Итог:\n";
            output += $"  x = {result.X:F10}\n";
            output += $"  y = {result.Y:F10}\n\n";
            output += $"Проверка:\n";
            output += $"  f1(x,y) = {system.F1(result.X, result.Y):E10}\n";
            output += $"  f2(x,y) = {system.F2(result.X, result.Y):E10}\n\n";
            output += $"Кол-во итераций: {result.Iterations}\n\n";
            output += $"Вектор погрешностей:\n";

            for (int i = 0; i < result.Errors.Count && i < 20; i++)
            {
                output += $"  Итерация {i + 1}: |Δx| = {result.Errors[i].dx:F10}, |Δy| = {result.Errors[i].dy:F10}\n";
            }

            return output;
        }

        private void InitializeComponent()
        {

        }

        private void BtnDrawSystemGraph_Click(object sender, EventArgs e)
        {
            try
            {
                SystemEquation system = systems[cmbSystem.SelectedIndex];
                ChartDrawer.DrawSystemGraph(chartSystem, system, cmbSystem.SelectedIndex, lastSolution);
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string change(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            } 

            return input.Replace(',', '.');
        }
    }

}