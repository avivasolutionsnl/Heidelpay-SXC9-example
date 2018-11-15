using Sitecore.Commerce.Engine.Connect.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace website.Models
{
    public class CartModel
    {
        public Sitecore.Commerce.Entities.Carts.Cart Cart { get; set; }

        public IEnumerable<CommerceSellableItemSearchResultItem> SellableItems { get; set; }
    }
}