// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureServiceApiBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Payment.Heidelpay
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.OData.Builder;
    using Plugin.Payment.Heidelpay.Components;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines a block which configures the OData model
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Microsoft.AspNetCore.OData.Builder.ODataConventionModelBuilder,
    ///         Microsoft.AspNetCore.OData.Builder.ODataConventionModelBuilder,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("Plugin.Payment.Heidelpay:blocks:ConfigureServiceApi")]
    public class ConfigureServiceApiBlock : PipelineBlock<ODataConventionModelBuilder, ODataConventionModelBuilder, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="modelBuilder">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="ODataConventionModelBuilder"/>.
        /// </returns>
        public override Task<ODataConventionModelBuilder> Run(ODataConventionModelBuilder modelBuilder, CommercePipelineExecutionContext context)
        {
            Condition.Requires(modelBuilder).IsNotNull($"{this.Name}: The argument cannot be null.");
            modelBuilder.AddEntityType(typeof(HeidelpayPaymentComponent));

            var configuration = modelBuilder.Action("AddHeidelpayPayment");
            configuration.Parameter<string>("cartId");
            configuration.Parameter<HeidelpayPaymentComponent>("payment");
            configuration.ReturnsFromEntitySet<CommerceCommand>("Commands");
            
            var updateConfiguration = modelBuilder.Action("RequestPayment");
            updateConfiguration.Parameter<string>("orderId");
            updateConfiguration.Returns<string>();

            return Task.FromResult(modelBuilder);
        }
    }
}
