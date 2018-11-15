using Sitecore.Commerce.Core;


namespace Plugin.Payment.Heidelpay.Models
{
    public class PaymentRequested : Model
    {
        public PaymentRequested(string redirectUrl)
        {
            RedirectUrl = redirectUrl;
        }

        public string RedirectUrl { get; set; }
    }
}
