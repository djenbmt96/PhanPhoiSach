//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BookStore.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class agency_debt_item
    {
        public int agency_debt_item_id { get; set; }
        public int fk_agency_debt { get; set; }
        public int fk_book { get; set; }
        public Nullable<int> agency_debt_item_quantity { get; set; }
        public Nullable<decimal> agency_debt_item_money { get; set; }
    
        public virtual agency_debt agency_debt { get; set; }
        public virtual book book { get; set; }
    }
}
