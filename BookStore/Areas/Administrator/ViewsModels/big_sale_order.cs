using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Areas.Administrator.ViewsModels
{
    public class big_sale_order
    {
        public Models.sale_order sale_order { get; set; }
        public Models.sale_order_item sale_order_item { get; set; }
    }
}