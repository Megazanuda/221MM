using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;




namespace _221MM
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Считывание данных
                var suppliers = SuppliersTextBox.Text.Split(',').Select(int.Parse).ToArray();
                var consumers = ConsumersTextBox.Text.Split(',').Select(int.Parse).ToArray();
                var costMatrix = ParseCostMatrix(CostMatrixTextBox.Text);

                // Проверка на соответствие
                if (suppliers.Length == 0 || consumers.Length == 0 || costMatrix.Length == 0)
                {
                    ResultTextBlock.Text = "Пожалуйста, введите валидные данные.";
                    return;
                }
                //if (suppliers.Sum() != consumers.Sum())
                //{
                //    if (suppliers.Sum() < consumers.Sum())
                //    {
                //        costMatrix[costMatrix.Length].
                //    }
                //}

                // Решение транспортной задачи методом северо-западного угла
                var result = SolveTransportationProblem(suppliers, consumers, costMatrix);
                DisplayResults(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private int[,] ParseCostMatrix(string input)
        {
            
            
            var lines = input.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int size = lines.Length;
            var matrix = new int[size+1, size];

            for (int i = 0; i < size; i++)
            {
                var values = lines[i].Split(',').Select(int.Parse).ToArray();
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = values[j];
                }
            }
            
            return matrix;
        }

        private int[,] SolveTransportationProblem(int[] suppliers, int[] consumers, int[,] costMatrix)
        {
            int m = suppliers.Length;
            int n = consumers.Length;
            int[,] result = new int[m, n];

            int i = 0, j = 0;

            while (i < m && j < n)
            {
                int shipment = Math.Min(suppliers[i], consumers[j]);
                result[i, j] = shipment;
                suppliers[i] -= shipment;
                consumers[j] -= shipment;

                if (suppliers[i] == 0) i++;
                if (consumers[j] == 0) j++;
            }

            return result;
        }

        private void DisplayResults(int[,] result)
        {
            string output = "Опорный план:\n";

            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    output += result[i, j] + "\t";
                }
                output += "\n";
            }

            ResultTextBlock.Text = output;
        }
    }
}
