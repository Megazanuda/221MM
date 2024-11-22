using System;
using System.Linq;
using System.Windows;

namespace _221MM
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        public int[] suppliers;
        public int[] consumers;

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Считывание данных
                suppliers = SuppliersTextBox.Text.Split(',').Select(int.Parse).ToArray();
                consumers = ConsumersTextBox.Text.Split(',').Select(int.Parse).ToArray();

                var costMatrix = ParseCostMatrix(CostMatrixTextBox.Text, consumers, suppliers);

                // Проверка на соответствие
                if (suppliers.Length == 0 || consumers.Length == 0 || costMatrix.GetLength(0) == 0 || costMatrix.GetLength(1) == 0)
                {
                    ResultTextBlock.Text = "Пожалуйста, введите валидные данные.";
                    return;
                }

                int[,] result;

                // Проверяем, какой метод был выбран
                if (NorthWestButton.IsChecked == true)
                {
                    result = SolveTransportationProblemNorthWest(suppliers, consumers, costMatrix);
                }
                else if (MinElementsButton.IsChecked == true)
                {
                    result = SolveTransportationProblemMinElements(suppliers, consumers, costMatrix);
                }
                else
                {
                    ResultTextBlock.Text = "Выберите метод решения задачи.";
                    return;
                }

                // Отображаем результат
                DisplayResults(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private int[,] ParseCostMatrix(string input, int[] consumers1, int[] suppliers1)
        {
            var lines = input.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int size = lines.Length;

            // Проверка на сбалансированность задачи
            int totalSuppliers = suppliers1.Sum();
            int totalConsumers = consumers1.Sum();

            if (totalSuppliers == totalConsumers)
            {
                // Если задача сбалансирована, то просто заполняем матрицу
                var matrix = new int[size, consumers1.Length];
                for (int i = 0; i < size; i++)
                {
                    var values = lines[i].Split(',').Select(int.Parse).ToArray();
                    for (int j = 0; j < consumers1.Length; j++)
                    {
                        matrix[i, j] = values[j];
                    }
                }
                return matrix;
            }
            else
            {
                // Добавляем фиктивную строку или столбец
                if (totalSuppliers > totalConsumers)
                {
                    // Добавляем фиктивного потребителя
                    var matrix = new int[size, consumers1.Length + 1];
                    for (int i = 0; i < size; i++)
                    {
                        var values = lines[i].Split(',').Select(int.Parse).ToArray();
                        for (int j = 0; j < consumers1.Length; j++)
                        {
                            matrix[i, j] = values[j];
                        }
                    }
                    // Устанавливаем нулевые затраты для фиктивного потребителя
                    for (int i = 0; i < size; i++)
                    {
                        matrix[i, consumers1.Length] = 0;
                    }
                    
                    Array.Resize(ref consumers, consumers.Length + 1);
                    consumers[consumers.Length - 1] = totalSuppliers - totalConsumers;
                    return matrix;
                }
                else
                {
                    // Добавляем фиктивного поставщика
                    var matrix = new int[size + 1, consumers1.Length];
                    for (int i = 0; i < size; i++)
                    {
                        var values = lines[i].Split(',').Select(int.Parse).ToArray();
                        for (int j = 0; j < consumers1.Length; j++)
                        {
                            matrix[i, j] = values[j];
                        }
                    }
                    // Устанавливаем нулевые затраты для фиктивного поставщика
                    for (int j = 0; j < consumers1.Length; j++)
                    {
                        matrix[size, j] = 0;
                    }
                    Array.Resize(ref suppliers, suppliers.Length + 1);
                    suppliers[suppliers.Length - 1] = totalConsumers - totalSuppliers;
                    return matrix;
                }
            }
        }

        // Метод северо-западного угла
        private int[,] SolveTransportationProblemNorthWest(int[] suppliers, int[] consumers, int[,] costMatrix)
        {

            int n = costMatrix.GetLength(1);
            int m = costMatrix.GetLength(0);
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

        // Метод минимальных элементов
        private int[,] SolveTransportationProblemMinElements(int[] suppliers, int[] consumers, int[,] costMatrix)
        {
            int m = suppliers.Length;
            int n = consumers.Length;
            int[,] result = new int[m, n];
            int[] remainingSuppliers = (int[])suppliers.Clone();
            int[] remainingConsumers = (int[])consumers.Clone();

            while (remainingSuppliers.Any(s => s > 0) && remainingConsumers.Any(c => c > 0))
            {
                int minCost = int.MaxValue;
                int minRow = -1, minCol = -1;

                // Находим минимальный элемент в матрице стоимостей
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (remainingSuppliers[i] > 0 && remainingConsumers[j] > 0 && costMatrix[i, j] < minCost)
                        {
                            minCost = costMatrix[i, j];
                            minRow = i;
                            minCol = j;
                        }
                    }
                }

                // Определяем количество перевозок
                int shipment = Math.Min(remainingSuppliers[minRow], remainingConsumers[minCol]);
                result[minRow, minCol] = shipment;

                // Обновляем оставшиеся поставки и потребности
                remainingSuppliers[minRow] -= shipment;
                remainingConsumers[minCol] -= shipment;
            }

            return result;
        }

        // Отображаем результат
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
