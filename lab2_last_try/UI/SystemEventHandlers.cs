using NumericalMethodsApp.Models;
using NumericalMethodsApp.Solvers;
using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace NumericalMethodsApp.UI
{
    public class SystemEventHandlers
    {
        private MainForm mainForm;
        private ComboBox cmbSystem;
        private TextBox x0;
        private TextBox y0;
        private TextBox epsilonSystem;
        private RichTextBox resultSystem;
        private Chart chartSystem;

        public SystemEventHandlers(MainForm mainForm,
            ComboBox cmbSystem,
            TextBox x0,
            TextBox y0,
            TextBox epsilonSystem,
            RichTextBox resultSystem,
            Chart chartSystem)
        {
            this.mainForm = mainForm;
            this.cmbSystem = cmbSystem;
            this.x0 = x0;
            this.y0 = y0;
            this.epsilonSystem = epsilonSystem;
            this.resultSystem = resultSystem;
            this.chartSystem = chartSystem;
        }

        public void BtnSolveSystem_Click(object sender, EventArgs e)
        {
            try
            {
                double x_0 = EquationInputHelper.ParseDouble(x0.Text);
                double y_0 = EquationInputHelper.ParseDouble(y0.Text);
                double eps = EquationInputHelper.ParseDouble(epsilonSystem.Text);

                EquationInputHelper.ValidateEpsilon(eps);

                SystemEquation system = mainForm.GetSystem(cmbSystem.SelectedIndex);
                var result = SystemSolver.NewtonMethod(system, x_0, y_0, eps);
                mainForm.SetLastSolution(result);

                ChartDrawer.DrawSystemGraph(chartSystem, system, cmbSystem.SelectedIndex, result);

                string output = ResultFormatter.FormatSystemResult(system, x_0, y_0, eps, result);
                resultSystem.Text = output;

                MessageBox.Show("Система решена. Точка решения на графике.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void BtnDrawSystemGraph_Click(object sender, EventArgs e)
        {
            try
            {
                SystemEquation system = mainForm.GetSystem(cmbSystem.SelectedIndex);
                ChartDrawer.DrawSystemGraph(chartSystem, system, cmbSystem.SelectedIndex, mainForm.GetLastSolution());
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}