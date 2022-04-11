using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesSystem
{
    static partial class Program
    {
        static List<string> dealsTable = new List<string>();
        static MenuState DealsMenu()
        {
            int prevBuffSize = Console.BufferWidth;
            var state = MenuState.FirstOrCancel;

            do
            {
                if (state == MenuState.Choise)
                    WriteDealsTable();

                dealsTable.ForEach(t => Console.WriteLine(t));

                Console.WriteLine();
                if (state == MenuState.Error)
                    Console.WriteLine("Ошибка ввода");

                state = WriteMenu(false, (() => AddEditDeal(), "Добавить сделку"),
                                         (EditDeal, "Редактировать сделку"),
                                         (DeleteDeal, "Удалить сделку"));
                Console.Clear();
            } while (state != MenuState.Exit);
            Console.BufferWidth = prevBuffSize;
            return MenuState.Choise;
        }

        private static void WriteDealsTable()
        {
            string[] columns = new string[] { "ID", "Клиент", "Товары", "Дата сделки", "Стоимость" };
            int[] widths = new int[columns.Length];

            dealsTable.Clear();
            for (int i = 0; i < columns.Length; i++)
                widths[i] = columns[i].Length;

            foreach (var d in Deal.deals)
            {
                int max;
                if (d.id.ToString().Length > widths[0])
                    widths[0] = d.id.ToString().Length;
                if (d.client.ToString().Length > widths[1])
                    widths[1] = d.client.ToString().Length;
                max = d.products.Max(t => t.ToString().Length);
                if (max > widths[2])
                    widths[2] = max;
                max = d.dealDate.ToString("dd.MM.yy").Length;
                if (max > widths[3])
                    widths[3] = max;
                if (d.Price.ToString().Length > widths[4])
                    widths[4] = d.Price.ToString().Length;
            }
            for (int i = 0; i < widths.Length; i++)
                widths[i] += 3;

            int w = widths.Sum() + Environment.NewLine.Length;
            if (w > Console.BufferWidth)
                Console.BufferWidth = widths.Sum() + Environment.NewLine.Length;

            var temp = new (string, int)[columns.Length];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = (columns[i], widths[i]);
            WriteTable(dealsTable, temp);

            foreach (var d in Deal.deals)
            {
                temp[0] = (d.id.ToString(), widths[0]);
                temp[1] = (d.client.ToString(), widths[1]);
                temp[2] = (d.products[0].ToString(), widths[2]);
                temp[3] = (d.dealDate.ToString("dd.MM.yy"), widths[3]);
                temp[4] = (d.Price.ToString(), widths[4]);
                WriteTable(dealsTable, temp);

                for (int i = 1; i < d.products.Count; i++)
                {
                    temp[0] = ("", widths[0]);
                    temp[1] = ("", widths[1]);
                    temp[2] = (d.products[i].ToString(), widths[2]);
                    temp[3] = ("", widths[3]);
                    temp[4] = ("", widths[4]);
                    WriteTable(dealsTable, temp);
                }
            }
        }

        static MenuState AddEditDeal(Deal d = null)
        {
            string dateStr, clientStr;
            int client = 0;
            var products = new List<(Product product, int count)>();
            DateTime date = DateTime.Now;
            bool success;

            if (d != null)
                d.products.ForEach(t => products.Add(t));

            clientsTable.ForEach(t => Console.WriteLine(t));
            do
            {
                clientStr = ReadLine(String.Format("Введите ID клиента{0}: ",
                    d != null ? " (" + d.client.id + ")" : ""), d != null);
                if (string.IsNullOrWhiteSpace(clientStr))
                    break;

                success = int.TryParse(clientStr, out client) && Client.clients.Any(t => t.id == client);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            ConsoleKeyInfo key;
            do
            {
                Console.Clear();
                Console.WriteLine("Текущий список товаров:");
                products.ForEach(t => Console.WriteLine(t));
                Console.WriteLine();
                Console.WriteLine("1 - Добавить товар в список");
                Console.WriteLine("2 - Удалить товар из списка");
                if (products.Count > 0)
                    Console.WriteLine("3 - Продолжить");
                key = Console.ReadKey(true);
                if (key.KeyChar == '1')
                    AddProductToList(products);
                else if (key.KeyChar == '2')
                    RemoveProductFromList(products); 
            } while (key.KeyChar != '3');
            Console.Clear();

            do
            {
                dateStr = ReadLine(String.Format("Введите дату сделки{0}: ",
                    d != null ? " (" + d.dealDate.ToString("dd.MM.yy") + ")" : ""), d != null);
                if (string.IsNullOrWhiteSpace(dateStr))
                    break;

                success = DateTime.TryParse(dateStr, out date);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            Console.Clear();
            Console.WriteLine("Клиент: {0}", string.IsNullOrWhiteSpace(clientStr) ? 
                d.client : Client.clients.First(t => t.id == client));
            Console.WriteLine("Товары: {0}", products[0]);
            foreach (var t in products.Skip(1))
                Console.WriteLine("{0}{1}", new string(' ', "Товары: ".Length), t);
            Console.WriteLine("Дата сделки: {0}", string.IsNullOrWhiteSpace(dateStr) ? 
                d.dealDate.ToString("dd.MM.yy") : date.ToString("dd.MM.yy"));
            Console.WriteLine("{0} данную сделку?", d == null ? "Добавить" : "Сохранить");
            Console.WriteLine("1 - Да");
            Console.WriteLine("2 - Нет");
            do
            {
                var k = Console.ReadKey(true);
                if (k.KeyChar == '1')
                    break;
                else if (k.KeyChar == '2')
                    return MenuState.FirstOrCancel;

            } while (true);

            if (d == null)
            {
                Deal.deals.Add(new Deal
                {
                    id = Deal.deals.Count > 0 ? Deal.deals.Last().id + 1 : 0,
                    client = Client.clients.First(t => t.id == client),
                    products = products,
                    dealDate = date
                });
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(clientStr)) d.client = Client.clients.First(t => t.id == client);
                if (!string.IsNullOrWhiteSpace(dateStr)) d.dealDate = date;
                d.products = products;
            }
            return MenuState.Choise;
        }

        private static void AddProductToList(List<(Product product, int count)> list)
        {
            Console.Clear();
            productTable.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();

            string tempStr;
            int id, count = 0;
            bool success;
            Product p = null;
            do
            {
                tempStr = ReadLine("Введите ID товара: ");
                if (string.IsNullOrWhiteSpace(tempStr))
                    break;

                success = int.TryParse(tempStr, out id) && 
                    ((p = Product.products.FirstOrDefault(t => t.id == id)) != null);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            do
            {
                tempStr = ReadLine("Введите количество товара: ");
                if (string.IsNullOrWhiteSpace(tempStr))
                    break;

                success = int.TryParse(tempStr, out count) && count > 0;
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            list.Add((p, count));
        }

        private static void RemoveProductFromList(List<(Product product, int count)> list)
        {
            Console.Clear();
            for (int i = 0; i < list.Count; i++)
                Console.WriteLine("{0}. {1}", i + 1, list[i]);
            Console.WriteLine();
            Console.Write("Введите номер позиции, которую необходимо удалить: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            if (!success || id <= 0 || id > list.Count)
                return;

            list.RemoveAt(id - 1);
        }

        static MenuState EditDeal()
        {
            dealsTable.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();

            Console.Write("Введите ID сделки, которую необходимо отредактировать: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Deal d;
            if (!success || (d = Deal.deals.FirstOrDefault(t => t.id == id)) == null)
                return MenuState.Error;

            Console.Clear();
            return AddEditDeal(d);
        }

        static MenuState DeleteDeal()
        {
            dealsTable.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();

            Console.Write("Введите ID сделки, которую необходимо удалить: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Deal d;
            if (!success || (d = Deal.deals.FirstOrDefault(t => t.id == id)) == null)
                return MenuState.Error;

            Deal.deals.Remove(d);
            return MenuState.Choise;
        }
    }
}
