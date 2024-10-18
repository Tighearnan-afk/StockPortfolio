using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Portfolio.Model;
using static System.Net.Mime.MediaTypeNames;

namespace Portfolio.Service.Live
{
    /// <summary>
    /// The YahooClient class implements the MarketClient interface <see cref="IMarketClient"/> and
    /// makes calls to the Yahoo finance API. 
    /// </summary>
    public class YahooClient : IMarketClient
    {
        /// <summary>
        /// The base URL for the web API
        /// </summary>
        public string BaseURL { get; set; }

        private HttpClient client;
        
        /// <summary>
        /// A list of API keys for the Yahoo finance API. If a key is invalid or you have reached 
        /// the maximum number of requests then you can use another key.
        /// </summary>
        public List<String> APIKeys { get; set; }

        public YahooClient(String url, List<String> apiKeys)
        {
            BaseURL = url; // not needed
            APIKeys = apiKeys;
            client = new HttpClient();
            try
            {
                client.BaseAddress = new Uri(BaseURL);
            }
            catch(UriFormatException e)
            {

            }
        }

        /// <summary>
        /// A sample implementation of the GetQuote method that uses the http://yfapi.net API 
        /// service to retrieve data on the specified asset in the assetSymbol parameter.
        /// The method adds additional elements to the request URL and header information
        /// such as the API key. 
        /// <see cref="https://financeapi.net/ "/> for more information on the API endpoints.
        /// </summary>
        /// <param name="assetSymbol">the name of the asset to query</param>
        /// <returns>an empty asset quote</returns>
        public AssetQuote GetQuote(string assetSymbol)
        {
            // Example get Quote from YahooFinance API
            String endpoint = @"/v6/finance/quote";
            String parameters = @"region=US&lang=en&symbols=" + assetSymbol;
            String url = endpoint + "?" + parameters;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Add("x-api-key", APIKeys[2]);

            var task = client.SendAsync(requestMessage);
            var response = task.Result;
            response.EnsureSuccessStatusCode();
            string responseBody = response.Content.ReadAsStringAsync().Result;
            return ParseQuoteResponse(responseBody)[0];
        }

        private List<AssetQuote> ParseQuoteResponse(string responseBody)
        {
            List <AssetQuote> quotes = new List<AssetQuote>();

            //Deserealize the JSON Response
            try
            {
                Root rawResult = JsonSerializer.Deserialize<Root>(responseBody);
            
            if(rawResult.quoteResponse is not null)
            {
                AssetQuote assetQuote = new AssetQuote();
                foreach (Result quote in rawResult.quoteResponse.result)
                {
                    switch(quote.quoteType)
                    {
                        case "EQUITY":
                             assetQuote.AssetType = AssetType.Equity;
                            break;
                        case "CURRENCY":
                             assetQuote.AssetType = AssetType.Currency;
                            break;
                        default:
                             assetQuote.AssetType = AssetType.Cryptocurrency;
                            break;
                    }
                    //Read the values from the quote object and assign to the assetQuote
                    assetQuote.AssetSymbol = quote.symbol;
                    assetQuote.AssetFullName = quote.longName;
                    assetQuote.AssetQuoteValue = (decimal)quote.regularMarketPrice;
                    assetQuote.RegularMarketPreviousClose = (decimal)quote.regularMarketPreviousClose;
                    assetQuote.RegularMarketOpen = (decimal)quote.regularMarketOpen;
                    assetQuote.RegularMarketChange = (decimal)quote.regularMarketChange;
                    assetQuote.RegularMarketChangePercentage = (float)quote.regularMarketChangePercent;
                    DateTime dateTime = new DateTime(1970, 1 ,1 ,0, 0, 0, 0, DateTimeKind.Utc);
                    assetQuote.AssetQuoteTimeStamp = dateTime;
                    quotes.Add(assetQuote);
                }
            }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return quotes;
        }

        /// <summary>
        /// Gets a list of AssetQuotes <see cref="AssetQuote"/>from an exchange for the asset.
        /// </summary>
        /// <param name="assetSymbols">is a list of asset symbols to get quotes for.</param>
        /// <returns>A list of AssetQuotes <see cref="AssetQuote"/></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<AssetQuote> GetQuote(List<string> assetSymbols)
        {
            List<AssetQuote> assetQuotes = new List<AssetQuote>();
            foreach (string symbol in assetSymbols)
            {
                String endpoint = @"/v6/finance/quote";
                String parameters = @"region=US&lang=en&symbols=" + symbol;
                String url = endpoint + "?" + parameters;
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                requestMessage.Headers.Add("x-api-key", APIKeys[2]);

                var task = client.SendAsync(requestMessage);
                var response = task.Result;
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().Result;

                AssetQuote assetQuote = ParseQuoteResponse(responseBody)[0];
                assetQuotes.Add(assetQuote);    
            }
            return assetQuotes;
        }

        private List<string> ParseTrendingResponse(string responseBody)
        {
            List<string> symbols = new List<string>();

            //Deserealize the JSON Response
            RootRegion rawResult = JsonSerializer.Deserialize<RootRegion>(responseBody);

            if (rawResult.finance is not null)
            {
                foreach (ResultRegion result in rawResult.finance.result)
                {
                    foreach (Quote quote in result.quotes)
                    {
                        symbols.Add(quote.symbol);
                    }
                }
            }
            return symbols;
        }


        /// <summary>
        /// Gets a list of trending stocks for a region from an exchange.
        /// </summary>
        /// <param name="region">is the region to look up.</param>
        /// <returns>a list of asset symbols for the specified region</returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<string> GetTrendingStocksForRegion(string region)
        {
            String endpoint = @"/v1/finance/trending/" + region;
            String parameters = @"region=" + region;
            String url = endpoint + "?" + parameters;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Add("x-api-key", APIKeys[0]);

            var task = client.SendAsync(requestMessage);
            var response = task.Result;
            response.EnsureSuccessStatusCode();
            string responseBody = response.Content.ReadAsStringAsync().Result;

            return ParseTrendingResponse(responseBody);
        }
    }
}

