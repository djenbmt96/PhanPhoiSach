using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Areas.Administrator.ViewsModels
{
    public class big_purchase_order
    {
        public purchase_order purchase_order{get;set;}
        public purchase_order_item purchase_order_item { get; set; }
    }
}