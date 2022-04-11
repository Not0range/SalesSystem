using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesSystem
{
    public class Deal
    {
        public static List<Deal> deals = new List<Deal>();

        public int id;
        public Client client;
        public List<(Product product, int count)> products;
        public DateTime dealDate;

        public decimal Price
        {
            get
            {
                return products.Sum(t => t.product.price * t.count);
            }
        }

        public static Deal GenerateFromString(string data)
        {
            var strs = data.Split(new string[]{"\t\t"}, 
                StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length != 4)
                throw new ArgumentException();

            var deal = new Deal
            {
                id = int.Parse(strs[0]),
                client = Client.clients.First(t => t.id == int.Parse(strs[1])),
                products = new List<(Product product, int count)>(),
                dealDate = DateTime.Parse(strs[3])
            };
            var products = strs[2].Split(new string[] { "\t" }, 
                StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in products)
            {
                var pr = p.Split(' ');
                deal.products.Add((Product.products.First(t => t.id == int.Parse(pr[0])), 
                    int.Parse(pr[1])));
            }
            return deal;
        }

        public string ConvertToString()
        {
            return string.Join("\t\t", id, client.id, string.Join("\t", 
                products.Select(t => t.product.id + " " + t.count)), dealDate.ToString("dd.MM.yyyy"));
        }
    }

    public class Product
    {
        public static List<Product> products = new List<Product>();

        public int id;
        public string title;
        public string description;
        public string category;
        public decimal price;

        public override string ToString()
        {
            return string.Format("{0} ({1})", title, category);
        }

        public static Product GenerateFromString(string data)
        {
            var strs = data.Split(new string[] { "\t\t" },
                StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length != 5)
                throw new ArgumentException();
            return new Product
            {
                id = int.Parse(strs[0]),
                title = strs[1],
                description = strs[2],
                category = strs[3],
                price = decimal.Parse(strs[4])
            };
        }

        public string ConvertToString()
        {
            return string.Join("\t\t", id, title, description, category, price);
        }
    }

    public class Client
    {
        public static List<Client> clients = new List<Client>();

        public int id;
        public string name;
        public string address;
        public string email;
        public string phone;

        public override string ToString()
        {
            return name;
        }

        public static Client GenerateFromString(string data)
        {
            var strs = data.Split(new string[] { "\t\t" },
                StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length != 5)
                throw new ArgumentException();
            return new Client
            {
                id = int.Parse(strs[0]),
                name = strs[1],
                address = strs[2],
                email = strs[3],
                phone = strs[4]
            };
        }

        public string ConvertToString()
        {
            return string.Join("\t\t", id, name, address, email, phone);
        }
    }
}
