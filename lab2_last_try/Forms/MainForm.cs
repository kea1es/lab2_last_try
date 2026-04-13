using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using NumericalMethodsApp.Models;
using NumericalMethodsApp.UI;

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

        // Данные
        private List<Equation> equations;
        private List<SystemEquation> systems;
        private NewtonResult lastSolution = null;

        // Обработчики событий
        private EquationEventHandlers equationHandlers;
        private SystemEventHandlers systemHandlers;

        public MainForm()
        {
            InitializeComponents();
            InitializeData();
            InitializeEventHandlers();
            SubscribeEvents();
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

            btnSolveSystem = new Button { Text = "Решить систему (Метод Ньютона)", Location = new Point(20, y), Size = new Size(220, 35), BackColor = Color.White };
            btnDrawSystemGraph = new Button { Text = "Построить графики", Location = new Point(260, y), Size = new Size(150, 35), BackColor = Color.White };

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
        }

        private void InitializeEventHandlers()
        {
            // Создаем обработчики для уравнений
            equationHandlers = new EquationEventHandlers(
                this,
                chooseEquation,
                chooseMethod,
                leftBorder,
                rightBorder,
                epsilon,
                rbtnFileInput,
                rbtnFileOutput,
                txtResult,
                chartEquation
            );

            // Создаем обработчики для систем
            systemHandlers = new SystemEventHandlers(
                this,
                cmbSystem,
                x0,
                y0,
                epsilonSystem,
                resultSystem,
                chartSystem
            );
        }

        private void SubscribeEvents()
        {
            btnSolve.Click += equationHandlers.BtnSolve_Click;
            btnDrawGraph.Click += equationHandlers.BtnDrawGraph_Click;
            btnSolveSystem.Click += systemHandlers.BtnSolveSystem_Click;
            btnDrawSystemGraph.Click += systemHandlers.BtnDrawSystemGraph_Click;
        }

        public Equation GetEquation(int index)
        {
            return equations[index];
        }

        public SystemEquation GetSystem(int index)
        {
            return systems[index];
        }

        public void SetLastSolution(NewtonResult solution)
        {
            lastSolution = solution;
        }

        public NewtonResult GetLastSolution()
        {
            return lastSolution;
        }

        private void InitializeComponent() { }
    }
}