using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter_API
{
    // Root Class is a Main Class. API Return rates in a rates it Return All Currency Name With Value
    public class Root
    {
        public Rate rates { get; set; }  // get all Record in rates and Set in Rate Class as Currency Name Wise
        public long timestamp;
        public string license;
    }
}
