using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LINQtoEbay;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            EbayQuery<EbayItem> queryable = new EbayQuery<EbayItem>();

            int i = 0;
            string ipod = "ipod";
            var query = (from it in queryable
                         where it.Description == "ipod" && it.BuyItNowAvailable
                         orderby it.EndTime descending
                         select it);

            foreach (var it in query)
            {
                i++;
                Console.WriteLine(it.ToString());
            }

            Console.WriteLine("Items={0}", i);
            Console.ReadLine();
        }
    }
}