using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Model
{
    internal class Admin
    {
        public int IdAdmin { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        public override string ToString()
        {
            return string.Format($"Id - {IdAdmin}, Name - {Name}, Password - {Password}");
        }

    }
}
