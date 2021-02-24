using System;
using System.Collections.Generic;
using System.Text;

namespace WebexAPITestApp.Models
{
    public class WebexPeople
    {
        public List<Item> items { get; set; }
    }

    public class Item
    {
        public string id { get; set; }
        public List<string> emails { get; set; }
        public string displayName { get; set; }
    }
}
