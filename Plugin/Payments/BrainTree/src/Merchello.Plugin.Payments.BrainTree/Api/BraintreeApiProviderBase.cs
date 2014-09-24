﻿namespace Merchello.Plugin.Payments.Braintree.Api
{
    using System;

    using global::Braintree;

    using Merchello.Core;
    using Merchello.Plugin.Payments.Braintree.Persistence.Factories;

    using Umbraco.Core.Cache;

    /// <summary>
    /// A base class for local Braintree services.
    /// </summary>
    internal abstract class BraintreeApiProviderBase
    {
        /// <summary>
        /// The <see cref="BraintreeApiRequestFactory"/>.
        /// </summary>
        private readonly Lazy<BraintreeApiRequestFactory> _requestFactory = new Lazy<BraintreeApiRequestFactory>(() => new BraintreeApiRequestFactory());  


        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeApiProviderBase"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        /// <param name="braintreeGateway">
        /// The <see cref="BraintreeGateway"/>.
        /// </param>
        protected BraintreeApiProviderBase(IMerchelloContext merchelloContext, BraintreeGateway braintreeGateway)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(braintreeGateway, "braintreeGateway");

            this.MerchelloContext = merchelloContext;

            this.BraintreeGateway = braintreeGateway;
        }

        /// <summary>
        /// Gets the merchello context.
        /// </summary>
        protected IMerchelloContext MerchelloContext { get; private set; }

        /// <summary>
        /// Gets the braintree gateway.
        /// </summary>
        protected BraintreeGateway BraintreeGateway { get; private set; }

        /// <summary>
        /// Gets the runtime cache.
        /// </summary>
        protected IRuntimeCacheProvider RuntimeCache 
        { 
            get { return this.MerchelloContext.Cache.RuntimeCache; } 
        }

        /// <summary>
        /// Gets the request factory.
        /// </summary>
        protected BraintreeApiRequestFactory RequestFactory
        {
            get
            {
                return _requestFactory.Value;
            }
        }

        /// <summary>
        /// The try get cached.
        /// </summary>
        /// <param name="cacheKey">
        /// The cache key.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        protected T TryGetCached<T>(string cacheKey)
        {
            return (T)this.RuntimeCache.GetCacheItem(cacheKey);
        }
    }
}