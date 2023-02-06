using Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server._Repository
{
    static class Logs
    {
        private static string path = "C:\\Users\\GameMax\\source\\repos\\NetTechLab3\\Server\\Logs.txt";

        public static void SaveProduct(string command, Product product = null)
        {
            string text = command + "\t" + DateTime.Now;

            if (product != null)
                 text = text + "\n" + product.ToString();            

            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLineAsync(text);
            }
        }

        public static void SaveAdmin(string command, string name)
        {
            string text = command + "\t" + DateTime.Now + "\n" + name;

            /*if(admin != null)
                text = text + "\n" + admin.ToString();*/

            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLineAsync(text);
            }
        }
    }
}
