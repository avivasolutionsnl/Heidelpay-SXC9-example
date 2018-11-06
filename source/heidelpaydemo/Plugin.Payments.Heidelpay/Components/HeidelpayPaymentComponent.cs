using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Payments;
using System;

namespace Plugin.Payment.Heidelpay.Components
{
    public class HeidelpayPaymentComponent : PaymentComponent
    {
        public HeidelpayPaymentComponent()
        {
            IsSettled = false;
        }

        public bool IsSettled { get; set; }

        public DateTimeOffset? SettledAt { get; set; }

        public Party BillingParty { get; set; }
    }
}
