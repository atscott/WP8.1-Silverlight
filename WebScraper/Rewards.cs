using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper
{
    public class Rewards
    {
        public string CategoryDescription { get; set;}
        public IList<string> Items {get;set;}

        public Rewards()
        {
            Items = new List<string>();
        }
    }
}
