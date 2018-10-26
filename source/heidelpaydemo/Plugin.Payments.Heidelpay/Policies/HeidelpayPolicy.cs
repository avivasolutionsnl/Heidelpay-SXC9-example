using Sitecore.Commerce.Core;

namespace Plugin.Payment.Heidelpay.Policies
{
    public class HeidelpayPolicy : Policy
    {
        public string TransactionChannel { get; set; }

        public string TransactionMode { get; set; }

        public string SecuritySender { get; set; }

        public string UserLogin { get; set; }

        public string UserPassword{ get; set; }

        public string FrontendResponseUrl { get; set; }
    }
}
