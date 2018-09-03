using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Models.Settings;
using Business.Interfaces;
using Business.Models.KeyPhrases;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Extensions.Options;
using Microsoft.Rest;

namespace Azure
{
    public sealed class TextAnalyticsService : ITextAnalyticsService
    {
        private static AzureCognitiveServices _connectionOptions;

        private ITextAnalyticsClient _analyticsClient;
        private readonly object _clientLock = new object();

        public TextAnalyticsService(IOptions<AzureCognitiveServices> options)
        {
            _connectionOptions = options.Value;
        }

        public async Task<KeyPhrasesResponseModel> GetKeyPhrasesAsync(KeyPhrasesRequestModel model)
        {
            var client = GetClient;
            KeyPhraseBatchResult result2 = await client.KeyPhrasesAsync(model.Map());

            return result2.Map();
        }

        /// <summary>
        /// Container for subscription credentials. Make sure to enter your valid key.
        /// </summary>
        private class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Add(_connectionOptions.SubscriptionKeyName, _connectionOptions.SubscriptionKey);
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }

        private ITextAnalyticsClient GetClient
        {
            get
            {
                if (_analyticsClient == null)
                {
                    lock (_clientLock)
                    {
                        if (_analyticsClient == null)
                        {
                            _analyticsClient = new TextAnalyticsClient(new ApiKeyServiceClientCredentials())
                            {
                                
                                Endpoint = _connectionOptions.EndPoint
                            };
                        }
                    }
                }

                return _analyticsClient;
            }
        }
    }
}
