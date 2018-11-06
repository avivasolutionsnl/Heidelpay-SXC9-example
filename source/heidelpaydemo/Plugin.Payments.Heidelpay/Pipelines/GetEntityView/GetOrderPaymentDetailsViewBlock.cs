using Plugin.Payment.Heidelpay.Components;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Commerce.Plugin.Payments;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Payment.Heidelpay.Pipelines.GetEntityView
{
    /// <summary>
    /// Adds a view to display the heidel pay details in the order details.
    /// The code is largely based on the default implementation for a Federated payment in 
    /// Sitecore.Commerce.Plugin.Payments.GetOrderPaymentDetailsViewBlock
    /// </summary>
    [PipelineDisplayName("Plugin.Payment.Heidelpay:Blocks:GetOrderPaymentDetailsViewBlock")]
    public class GetOrderPaymentDetailsViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly GetOnHoldOrderCartCommand _getOnHoldOrderCartCommand;

        public GetOrderPaymentDetailsViewBlock(GetOnHoldOrderCartCommand getOnHoldOrderCartCommand)
          : base((string)null)
        {
            _getOnHoldOrderCartCommand = getOnHoldOrderCartCommand;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull(string.Format("{0}: The argument cannot be null.", Name));

            var request = context.CommerceContext.GetObject<EntityViewArgument>();

            if (string.IsNullOrEmpty(request != null ? request.ViewName : null) ||
                !request.ViewName.Equals(context.GetPolicy<KnownPaymentsViewsPolicy>().OrderPayments, StringComparison.OrdinalIgnoreCase) &&
                !request.ViewName.Equals(context.GetPolicy<KnownOrderViewsPolicy>().Master, StringComparison.OrdinalIgnoreCase) &&
                !request.ViewName.Equals(context.GetPolicy<KnownPaymentsViewsPolicy>().OrderPaymentDetails, StringComparison.OrdinalIgnoreCase) ||
                (!(request.Entity is Order) || !string.IsNullOrEmpty(request.ForAction)))
            {
                return entityView;
            }

            Order order = (Order)request.Entity;
            if (!order.HasComponent<PaymentComponent>())
                return entityView;

            List<PaymentComponent> payments;
            if (order.HasComponent<OnHoldOrderComponent>())
            {
                payments = (await _getOnHoldOrderCartCommand.Process(context.CommerceContext, order)).Components.OfType<PaymentComponent>().ToList();
            }
            else
            {
                payments = order.Components.OfType<PaymentComponent>().ToList();
            }

            HeidelpayPaymentComponent heidelpayComponent = null;

            if (request.ViewName.Equals(context.GetPolicy<KnownOrderViewsPolicy>().Master, StringComparison.OrdinalIgnoreCase) ||
                request.ViewName.Equals(context.GetPolicy<KnownPaymentsViewsPolicy>().OrderPayments, StringComparison.OrdinalIgnoreCase))
            {
                EntityView childPaymentView = entityView;
                if (request.ViewName.Equals(context.GetPolicy<KnownOrderViewsPolicy>().Master, StringComparison.OrdinalIgnoreCase))
                {
                    childPaymentView = entityView.ChildViews.Cast<EntityView>().FirstOrDefault(
                        ev => ev.Name.Equals(context.GetPolicy<KnownPaymentsViewsPolicy>().OrderPayments, StringComparison.OrdinalIgnoreCase));
                }

                if (childPaymentView != null)
                {
                    childPaymentView.ChildViews
                        .Where(cv => cv.Name.Equals(context.GetPolicy<KnownPaymentsViewsPolicy>().OrderPaymentDetails, StringComparison.OrdinalIgnoreCase))
                        .Cast<EntityView>().ToList().ForEach(paymentView =>
                        {
                            heidelpayComponent = payments.FirstOrDefault(s => s.Id.Equals(paymentView.ItemId, StringComparison.OrdinalIgnoreCase)) as HeidelpayPaymentComponent;
                            if (heidelpayComponent != null)
                                PopulatePaymentDetails(paymentView, heidelpayComponent);
                        });
                }

                return entityView;
            }

            heidelpayComponent = payments.FirstOrDefault(s => s.Id.Equals(request.ItemId, StringComparison.OrdinalIgnoreCase)) as HeidelpayPaymentComponent;
            if (heidelpayComponent != null)
                PopulatePaymentDetails(entityView, heidelpayComponent);
            return entityView;
        }

        protected virtual void PopulatePaymentDetails(EntityView view, HeidelpayPaymentComponent heidelpayPayment)
        {
            if (view == null || heidelpayPayment == null)
                return;

            view.Properties.Add(new ViewProperty
            {
                Name = "ItemId",
                IsReadOnly = true,
                IsHidden = true,
                RawValue = heidelpayPayment.Id
            });

            view.Properties.Add(new ViewProperty
            {
                Name = "Type",
                IsReadOnly = true,
                RawValue = heidelpayPayment.GetType().Name
            });

            view.Properties.Add(new ViewProperty
            {
                Name = nameof(heidelpayPayment.Amount),
                IsReadOnly = true,
                RawValue = heidelpayPayment.Amount
            });

            view.Properties.Add(new ViewProperty
            {
                Name = nameof(heidelpayPayment.IsSettled),
                IsReadOnly = true,
                RawValue = heidelpayPayment.IsSettled
            });

            view.Properties.Add(new ViewProperty
            {
                Name = nameof(heidelpayPayment.SettledAt),
                IsReadOnly = true,
                RawValue = heidelpayPayment.SettledAt
            });

            PopulateBillingParty(view, heidelpayPayment);
        }

        protected virtual void PopulateBillingParty(EntityView view, HeidelpayPaymentComponent heidelpayPayment)
        {
            Party billingParty = heidelpayPayment.BillingParty;

            view.Properties.Add(new ViewProperty
            {
                Name = "FirstName",
                IsReadOnly = true,
                RawValue = billingParty.FirstName
            });

            view.Properties.Add(new ViewProperty
            {
                Name = "LastName",
                IsReadOnly = true,
                RawValue = billingParty.LastName
            });

            view.Properties.Add(new ViewProperty
            {
                Name = "Address1",
                IsReadOnly = true,
                RawValue = billingParty.Address1
            });

            view.Properties.Add(new ViewProperty
            {
                Name = "Address2",
                IsReadOnly = true,
                RawValue = billingParty.Address2
            });

            view.Properties.Add(new ViewProperty
            {
                Name = "City",
                IsReadOnly = true,
                RawValue = billingParty.City
            });

            view.Properties.Add(new ViewProperty
            {
                Name = "State",
                IsReadOnly = true,
                RawValue = billingParty.State
            });

            view.Properties.Add(new ViewProperty
            {
                Name = "CountryCode",
                IsReadOnly = true,
                RawValue = billingParty.CountryCode,
                IsHidden = true
            });

            view.Properties.Add(new ViewProperty
            {
                Name = "ZipPostalCode",
                IsReadOnly = true,
                RawValue = billingParty.ZipPostalCode
            });

            view.Properties.Add(new ViewProperty
            {
                Name = "PhoneNumber",
                IsReadOnly = true,
                RawValue = billingParty.PhoneNumber
            });
        }
    }
}
