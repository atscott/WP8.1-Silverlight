using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebScraper
{
    public class Merchant
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public IList<Rewards> Rewards { get; set; }
        public string Location { get; set; }
        public string Url { get; set; }
        public IList<string> NotesAndConditions { get; set; }
        public string Neighborhood { get; set; }
    }
}
