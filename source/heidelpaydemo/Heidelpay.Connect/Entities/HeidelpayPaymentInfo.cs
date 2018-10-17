using Sitecore.Commerce.Entities.Carts;
using System;

namespace Heidelpay.Connect.Entities
{
    public class HeidelpayPaymentInfo : PaymentInfo
    {
        public Decimal Amount { get; set; }
    }
}