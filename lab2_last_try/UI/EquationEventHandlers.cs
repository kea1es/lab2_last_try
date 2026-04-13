using NumericalMethodsApp.Models;
using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace NumericalMethodsApp.UI
{
    public class EquationEventHandlers
    {
        private MainForm mainForm;
        private ComboBox chooseEquation;
        private ComboBox chooseMethod;
        private TextBox leftBorder;
        private TextBox rightBorder;
        private TextBox epsilon;
        private RadioButton rbtnFileInput;
        private RadioButton rbtnFileOutput;
        private RichTextBox txtResult;
        private Chart chartEquation;

        public EquationEventHandlers(MainForm mainForm,
            ComboBox chooseEquation,
            ComboBox chooseMethod,
            TextBox leftBorder,
            TextBox rightBorder,
            TextBox epsilon,
            RadioButton rbtnFileInput,
            RadioButton rbtnFileOutput,
            RichTextBox txtResult,
            Chart chartEquation)
        {
            this.mainForm = mainForm;
            this.chooseEquation = chooseEquation;
            this.chooseMethod = chooseMethod;
            this.leftBorder = leftBorder;
            this.rightBorder = rightBorder;
            this.epsilon = epsilon;
            this.rbtnFileInput = rbtnFileInput;
            this.rbtnFileOutput = rbtnFileOutput;
            this.txtResult = txtResult;
            this.chartEquation = chartEquation;
        }

        
        public void BtnSolve_Click(object sender, EventArgs e)
        {
            try
            {
                double a, b, eps;

                if (rbtnFileInput.Checked)
                {
                    var data = EquationInputHelper.ReadFromFileDialog();

                    if (data.a == 0 && data.b == 0 && data.eps == 0) return;

                    a = data.a;
                    b = data.b;
                    eps = data.eps;
                }

                else
                {
                    a = EquationInputHelper.ParseDouble(leftBorder.Text);
                    b = EquationInputHelper.ParseDouble(rightBorder.Text);
                    eps = EquationInputHelper.ParseDouble(epsilon.Text);
                }

                EquationInputHelper.ValidateInput(a, b, eps);

                Equation equation = mainForm.GetEquation(chooseEquation.SelectedIndex);
                EquationInputHelper.CheckRootsExistence(equation, a, b);

                string result = ResultFormatter.FormatEquationHeader(equation, a, b, eps, chooseMethod.SelectedItem.ToString());

                var solutionResult = EquationSolverHelper.Solve(equation, a, b, eps, chooseMethod.SelectedIndex);

                if (solutionResult.success)
                {
                    result += ResultFormatter.FormatEquationSuccess(equation, solutionResult.root, solutionResult.iterations);
                    ChartDrawer.DrawFunctionGraph(chartEquation, equation, a, b);
                    ChartDrawer.MarkRootOnGraph(chartEquation, solutionResult.root);
                }

                else
                {
                    result += ResultFormatter.FormatError(solutionResult.error);
                    ChartDrawer.DrawFunctionGraph(chartEquation, equation, a, b);
                }

                if (rbtnFileOutput.Checked)
                {
                    EquationInputHelper.SaveToFileDialog(result);
                }

                txtResult.Text = result;
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        public void BtnDrawGraph_Click(object sender, EventArgs e)
        {
            try
            {
                double a = EquationInputHelper.ParseDouble(leftBorder.Text);
                double b = EquationInputHelper.ParseDouble(rightBorder.Text);

                Equation equation = mainForm.GetEquation(chooseEquation.SelectedIndex);
                ChartDrawer.DrawFunctionGraph(chartEquation, equation, a, b);
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}