using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogSearching.Service
{
    class OrderAttribute : Attribute
    {
        public int Order { get; private set; }
        public OrderAttribute(int order)
        {
            Order = order;
        }
    }
}
