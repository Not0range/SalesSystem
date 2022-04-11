using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesSystem
{
    static partial class Program
    {
        static List<string> productTable = new List<string>();
        static MenuState ProductMenu()
        {
            int prevBuffSize = Console.BufferWidth;
            var state = MenuState.FirstOrCancel;

            do
            {
                if (state == MenuState.Choise)
                    WriteProductsTable();

                productTable.ForEach(t => Console.WriteLine(t));

                Console.WriteLine();
                if (state == MenuState.Error)
                    Console.WriteLine("Ошибка ввода");

                state = WriteMenu(false, (() => AddEditProduct(), "Добавить товар"),
                                         (EditProduct, "Редактировать товар"),
                                         (DeleteProduct, "Удалить товар"));
                Console.Clear();
            } while (state != MenuState.Exit);
            Console.BufferWidth = prevBuffSize;
            return MenuState.Choise;
        }

        private static void WriteProductsTable()
        {
            string[] columns = new string[] { "ID", "Название", "Описание", "Категория", "Цена" };
            int[] widths = new int[columns.Length];
            productTable.Clear();
            for (int i = 0; i < columns.Length; i++)
                widths[i] = columns[i].Length;

            foreach (var p in Product.products)
            {
                if (p.id.ToString().Length > widths[0])
                    widths[0] = p.id.ToString().Length;
                if (p.title.Length > widths[1])
                    widths[1] = p.title.Length;
                if (p.description.Length > widths[2])
                    widths[2] = p.description.Length;
                if (p.category.Length > widths[3])
                    widths[3] = p.category.Length;
                if (p.price.ToString().Length > widths[4])
                    widths[4] = p.price.ToString().Length;
            }
            for (int i = 0; i < widths.Length; i++)
                widths[i] += 3;

            int w = widths.Sum() + Environment.NewLine.Length;
            if (w > Console.BufferWidth)
                Console.BufferWidth = widths.Sum() + Environment.NewLine.Length;

            var temp = new (string, int)[columns.Length];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = (columns[i], widths[i]);
            WriteTable(productTable, temp);

            foreach (var p in Product.products)
            {
                temp[0] = (p.id.ToString(), widths[0]);
                temp[1] = (p.title, widths[1]);
                temp[2] = (p.description, widths[2]);
                temp[3] = (p.category, widths[3]);
                temp[4] = (p.price.ToString(), widths[4]);
                WriteTable(productTable, temp);
            }
        }

        static MenuState AddEditProduct(Product p = null)
        {
            string title, description, category, priceStr;
            decimal price = 0;
            bool success;

            title = ReadLine(String.Format("Введите название товара{0}: ",
                p != null ? " (" + p.title + ")" : ""), p != null);
            description = ReadLine(String.Format("Введите описание товара{0}: ",
                p != null ? " (" + p.description + ")" : ""), p != null);
            category = ReadLine(String.Format("Введите категорию товара{0}: ",
                p != null ? " (" + p.category + ")" : ""), p != null);
            do
            {
                priceStr = ReadLine(String.Format("Введите стоимость товара{0}: ",
                    p != null ? " (" + p.price + ")" : ""), p != null);
                if (string.IsNullOrWhiteSpace(priceStr))
                    break;

                success = decimal.TryParse(priceStr, out price);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            Console.Clear();
            Console.WriteLine("Название: {0}", string.IsNullOrWhiteSpace(title) ? p.title : title);
            Console.WriteLine("Описание: {0}", string.IsNullOrWhiteSpace(description) ? p.description : description);
            Console.WriteLine("Категория: {0}", string.IsNullOrWhiteSpace(category) ? p.category : category);
            Console.WriteLine("Цена: {0}", string.IsNullOrWhiteSpace(priceStr) ? p.price : price);
            Console.WriteLine("{0} данный товар?", p == null ? "Добавить" : "Сохранить");
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

            if (p == null)
            {
                Product.products.Add(new Product
                {
                    id = Product.products.Count > 0 ? Product.products.Last().id + 1 : 0,
                    title = title,
                    description = description,
                    category = category,
                    price = price
                });
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(title)) p.title = title;
                if (!string.IsNullOrWhiteSpace(description)) p.description = description;
                if (!string.IsNullOrWhiteSpace(category)) p.category = category;
                if (!string.IsNullOrWhiteSpace(priceStr)) p.price = price;
            }
            return MenuState.Choise;
        }

        static MenuState EditProduct()
        {
            productTable.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();

            Console.Write("Введите ID товара, который необходимо отредактирвоать: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Product p;
            if (!success || (p = Product.products.FirstOrDefault(t => t.id == id)) == null)
                return MenuState.Error;

            Console.Clear();
            return AddEditProduct(p);
        }

        static MenuState DeleteProduct()
        {
            productTable.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();

            Console.Write("Введите ID товара, который необходимо удалить: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Product p;
            if (!success || (p = Product.products.FirstOrDefault(t => t.id == id)) == null)
                return MenuState.Error;

            Product.products.Remove(p);
            Deal.deals.ForEach(t1 => t1.products.RemoveAll(t2 => t2.product == p));
            return MenuState.Choise;
        }
    }
}
