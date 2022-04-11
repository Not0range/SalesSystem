using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesSystem
{
    static partial class Program
    {
        static void Main(string[] args)
        {
            ReadData();
            WriteProductsTable();
            WriteClientsTable();
            WriteDealsTable();

            Console.Title = "Система управления продажами";

            var state = MenuState.Choise;

            do
            {
                if (state == MenuState.Choise)
                {
                    Console.Write(new string('*', Console.BufferWidth));
                    Console.WriteLine("Добро пожаловать в систему управления продажами");
                    Console.Write(new string('*', Console.BufferWidth));
                }
                
                if (state == MenuState.Error)
                    Console.WriteLine("Ошибка ввода");
                state = WriteMenu(true, (ProductMenu, "Работа со списком товаров"),
                                (ClientsMenu, "Работа со списком клиентов"),
                                (DealsMenu, "Работа со списком сделок"));
            } while(state != MenuState.Exit);
            SaveData();
        }

        static void ReadData()
        {
            bool error = false;
            var reader = new StreamReader(new FileStream("products.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite));
            while (!reader.EndOfStream)
            {
                try
                {
                    Product.products.Add(Product.GenerateFromString(reader.ReadLine()));
                }
                catch
                {
                    error = true;
                }
            }
            reader.Close();

            reader = new StreamReader(new FileStream("clients.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite));
            while (!reader.EndOfStream)
            {
                try
                {
                    Client.clients.Add(Client.GenerateFromString(reader.ReadLine()));
                }
                catch
                {
                    error = true;
                }
            }
            reader.Close();

            reader = new StreamReader(new FileStream("deals.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite));
            while (!reader.EndOfStream)
            {
                try
                {
                    Deal.deals.Add(Deal.GenerateFromString(reader.ReadLine()));
                }
                catch
                {
                    error = true;
                }
            }
            reader.Close();

            if (error)
            {
                Console.WriteLine("При чтении файла были обнаружены некорректные записи.");
                Console.WriteLine("Они не были добавлены в таблицы и будут удалены");
                Console.WriteLine("Нажмите любую клавишу, чтобы продолжить");
                Console.ReadKey();
            }
        }

        static void SaveData()
        {
            var writer = new StreamWriter("products.txt");
            foreach (var p in Product.products)
                writer.WriteLine(p.ConvertToString());
            writer.Close();

            writer = new StreamWriter("clients.txt");
            foreach (var c in Client.clients)
                writer.WriteLine(c.ConvertToString());
            writer.Close();

            writer = new StreamWriter("deals.txt");
            foreach (var d in Deal.deals)
                writer.WriteLine(d.ConvertToString());
            writer.Close();
        }

        static MenuState WriteMenu(bool main, params (Func<MenuState> action, string text)[] lines)
        {
            do
            {
                Console.WriteLine("Выберите необходимое действие");
                for (int i = 0; i < lines.Length && i < 9; i++)
                    Console.WriteLine("{0} - {1}", i + 1, lines[i].text);

                if (main)
                    Console.WriteLine("{0} - Выход из программы", lines.Length + 1 < 10 ? lines.Length + 1 : 0);
                else
                    Console.WriteLine("{0} - В предыдущее меню", lines.Length + 1 < 10 ? lines.Length + 1 : 0);

                var key = Console.ReadKey(true);
                Console.Clear();
                if (lines.Length >= 10 && key.KeyChar < '0' || lines.Length < 10 && key.KeyChar <= '0' || key.KeyChar > '9' ||
                    key.KeyChar.ToInt() > lines.Length + 1)
                    return MenuState.Error;
                else if (lines.Length >= 10 && key.KeyChar == '0' || key.KeyChar.ToInt() == lines.Length + 1)
                    return MenuState.Exit;
                else
                    return lines[key.KeyChar.ToInt() - 1].action.Invoke();
            } while (true);
        }

        static void WriteTable(List<string> list, params (string text, int width)[] fields)
        {
            list.Add(string.Join("", fields.Select(f => f.text.PadRight(f.width))));
        }

        static string ReadLine(string text, bool allowEmpty = false)
        {
            bool success;
            string str;
            do
            {
                Console.Write(text);
                str = Console.ReadLine().Trim();
                success = !string.IsNullOrWhiteSpace(str) || allowEmpty;
                if (!success)
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.Write(new string('\0', Console.BufferWidth));
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                }
            } while (!success);
            return str;
        }

        static int ToInt(this char c)
        {
            return int.Parse(c.ToString());
        }

        enum MenuState
        {
            Choise,
            FirstOrCancel,
            Exit, 
            Error
        }
    }
}
