﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Payment.Heidelpay
{
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;
    using Plugin.Payment.Heidelpay.Pipelines.GetPaymentMethods.Blocks;
    using Plugin.Payment.Heidelpay.Pipelines.RequestPayment;
    using Plugin.Payment.Heidelpay.Pipelines.RequestPayment.Blocks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Payments;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;

    /// <summary>
    /// The Habitat configure class.
    /// </summary>
    /// <seealso cref="IConfigureSitecore" />
    public class ConfigureSitecore : IConfigureSitecore
    {
        /// <summary>
        /// The configure services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);
            services.RegisterAllCommands(assembly);

            services.Sitecore().Pipelines(
                config =>
                    config
                        .ConfigurePipeline<IConfigureServiceApiPipeline>(configure => configure.Add<ConfigureServiceApiBlock>())
                        .ConfigurePipeline<IGetPaymentMethodsPipeline>(d =>
                        {
                            d.Replace<Sitecore.Commerce.Plugin.Management.TranslateItemsToPaymentMethodsBlock, TranslateItemsToPaymentMethodsBlock>();
                        })
                        .ConfigurePipeline<IGetPaymentOptionsPipeline>(d =>
                        {
                            d.Replace<Sitecore.Commerce.Plugin.Management.TranslateItemsToPaymentOptionsBlock, TranslateItemsToPaymentOptionsBlock>();
                        })
                        .AddPipeline<IRequestPaymentPipeline, RequestPaymentPipeline>(d =>
                        {
                            d.Add<RequestPaymentBlock>();
                        })
                        .ConfigurePipeline<IRunningPluginsPipeline>(c =>
                        {
                            c.Add<RegisteredPluginBlock>().After<RunningPluginsBlock>();
                        }));
        }
    }
}