using Sitecore.Commerce.Core;

namespace Plugin.Payment.Heidelpay.Pipelines.GetPaymentMethods.Blocks
{
    public class TranslateItemsToPaymentOptionsBlock : Sitecore.Commerce.Plugin.Management.TranslateItemsToPaymentOptionsBlock
    {
        public TranslateItemsToPaymentOptionsBlock(CommerceCommander commander) : base(commander)
        {
        }

        protected override string GetPaymentTemplateName(string paymentOptionType)
        {
            return paymentOptionType == "5" ? "Heidelpay" : base.GetPaymentTemplateName(paymentOptionType);
        }
    }
}
