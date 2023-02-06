using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Model
{
    internal class Product
    {
        public int IdProduct { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string Status { get; set; }

        public override string ToString()
        {
            return string.Format($"Id - {IdProduct}, Name - {Name}, Descriprion - {Description}, Price - {Price}, Status - {Status}");
        }
    }
}
