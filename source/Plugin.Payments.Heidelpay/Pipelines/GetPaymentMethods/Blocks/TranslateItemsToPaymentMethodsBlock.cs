using Sitecore.Commerce.Core;

namespace Plugin.Payment.Heidelpay.Pipelines.GetPaymentMethods.Blocks
{
    public class TranslateItemsToPaymentMethodsBlock : Sitecore.Commerce.Plugin.Management.TranslateItemsToPaymentMethodsBlock
    {
        public TranslateItemsToPaymentMethodsBlock(CommerceCommander commander) : base(commander)
        {
        }

        protected override string GetPaymentTemplateName(string paymentOptionType)
        {
            return paymentOptionType == "5" ? "Heidelpay" : base.GetPaymentTemplateName(paymentOptionType);
        }
    }
}
