using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesSystem
{
    static partial class Program
    {
        static List<string> clientsTable = new List<string>();
        static MenuState ClientsMenu()
        {
            int prevBuffSize = Console.BufferWidth;
            var state = MenuState.FirstOrCancel;

            do
            {
                if (state == MenuState.Choise)
                    WriteClientsTable();

                clientsTable.ForEach(t => Console.WriteLine(t));

                Console.WriteLine();
                if (state == MenuState.Error)
                    Console.WriteLine("Ошибка ввода");

                state = WriteMenu(false, (() => AddEditClient(), "Добавить клиента"),
                                         (EditClient, "Редактировать клиента"),
                                         (DeleteClient, "Удалить клиента"));
                Console.Clear();
            } while (state != MenuState.Exit);
            Console.BufferWidth = prevBuffSize;
            return MenuState.Choise;
        }

        private static void WriteClientsTable()
        {
            string[] columns = new string[] { "ID", "Наименование", "Адрес", "Эл. почта", "Номер телефона" };
            int[] widths = new int[columns.Length];
            clientsTable.Clear();
            for (int i = 0; i < columns.Length; i++)
                widths[i] = columns[i].Length;

            foreach (var c in Client.clients)
            {
                if (c.id.ToString().Length > widths[0])
                    widths[0] = c.id.ToString().Length;
                if (c.name.Length > widths[1])
                    widths[1] = c.name.Length;
                if (c.address.Length > widths[2])
                    widths[2] = c.address.Length;
                if (c.email.Length > widths[3])
                    widths[3] = c.email.Length;
                if (c.phone.Length > widths[4])
                    widths[4] = c.phone.Length;
            }
            for (int i = 0; i < widths.Length; i++)
                widths[i] += 3;

            int w = widths.Sum() + Environment.NewLine.Length;
            if (w > Console.BufferWidth)
                Console.BufferWidth = widths.Sum() + Environment.NewLine.Length;

            var temp = new (string, int)[columns.Length];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = (columns[i], widths[i]);
            WriteTable(clientsTable, temp);

            foreach (var c in Client.clients)
            {
                temp[0] = (c.id.ToString(), widths[0]);
                temp[1] = (c.name, widths[1]);
                temp[2] = (c.address, widths[2]);
                temp[3] = (c.email, widths[3]);
                temp[4] = (c.phone, widths[4]);
                WriteTable(clientsTable, temp);
            }
        }

        static MenuState AddEditClient(Client c = null)
        {
            string name, address, email, phone;

            name = ReadLine(String.Format("Введите наименование клиента{0}: ",
                c != null ? " (" + c.name + ")" : ""), c != null);
            address = ReadLine(String.Format("Введите адрес клиента{0}: ",
                c != null ? " (" + c.address + ")" : ""), c != null);
            email = ReadLine(String.Format("Введите эл. почту клиента{0}: ",
                c != null ? " (" + c.email + ")" : ""), c != null);
            phone = ReadLine(String.Format("Введите номер телефона клиента{0}: ",
                c != null ? " (" + c.phone + ")" : ""), c != null);

            Console.Clear();
            Console.WriteLine("Наименование: {0}", string.IsNullOrWhiteSpace(name) ? c.name : name);
            Console.WriteLine("Адрес: {0}", string.IsNullOrWhiteSpace(address) ? c.address : address);
            Console.WriteLine("Эл. почта: {0}", string.IsNullOrWhiteSpace(email) ? c.email : email);
            Console.WriteLine("Номер телефона: {0}", string.IsNullOrWhiteSpace(phone) ? c.phone : phone);
            Console.WriteLine("{0} данного клиента?", c == null ? "Добавить" : "Сохранить");
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

            if (c == null)
            {
                Client.clients.Add(new Client
                {
                    id = Client.clients.Count > 0 ? Client.clients.Last().id + 1 : 0,
                    name = name,
                    address = address,
                    email = email,
                    phone = phone
                });
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(name)) c.name = name;
                if (!string.IsNullOrWhiteSpace(address)) c.address = address;
                if (!string.IsNullOrWhiteSpace(email)) c.email = email;
                if (!string.IsNullOrWhiteSpace(phone)) c.phone = phone;
            }
            return MenuState.Choise;
        }

        static MenuState EditClient()
        {
            clientsTable.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();

            Console.Write("Введите ID клиента, который необходимо отредактировать: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Client c;
            if (!success || (c = Client.clients.FirstOrDefault(t => t.id == id)) == null)
                return MenuState.Error;

            Console.Clear();
            return AddEditClient(c);
        }

        static MenuState DeleteClient()
        {
            clientsTable.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();

            Console.Write("Введите ID клиента, который необходимо удалить: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Client c;
            if (!success || (c = Client.clients.FirstOrDefault(t => t.id == id)) == null)
                return MenuState.Error;

            Client.clients.Remove(c);
            Deal.deals.RemoveAll(t => t.client == c);
            return MenuState.Choise;
        }
    }
}
