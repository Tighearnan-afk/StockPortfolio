using Portfolio.Model;

namespace Portfolio.Service.TestDouble
{
    /// <summary>
    /// The MockClient acts as a test double for the <see cref="IMarketClient"/> interface for 
    /// unit testing. It returns pre-packaged values for all queries. 
    /// </summary>
    public class MockClient : IMarketClient
    {
        /// <summary>
        /// This function returns a test doubl AssetQuote object for the named asset symbol.
        /// </summary>
        /// <param name="assetSymbol">assetSymbol indicates the symbol or icon that a specific 
        /// asset is identified with</param>
        /// <returns>a test double AssetQuote with test data</returns>
        /// <exception cref="NotImplementedException"></exception>
        public AssetQuote GetQuote(string assetSymbol)
        {
            AssetQuote fakeAssetQuote = new AssetQuote();
            fakeAssetQuote.AssetSymbol = assetSymbol;
            fakeAssetQuote.AssetFullName = "Fake Asset";
            fakeAssetQuote.AssetType = AssetType.Equity;
            fakeAssetQuote.RegularMarketOpen = 150.0m;
            fakeAssetQuote.AssetQuoteValue = 155.0m;
            fakeAssetQuote.AssetQuoteTimeStamp = DateTime.Now;
            fakeAssetQuote.RegularMarketChange = 5.5m;
            fakeAssetQuote.RegularMarketChangePercentage = 3;
            fakeAssetQuote.RegularMarketPreviousClose = 125m;

            return fakeAssetQuote;

            //throw new NotImplementedException();
        }

        /// <summary>
        /// A list of test double AssetQuotes for each of the named assets in the 
        /// </summary>
        /// <param name="assetSymbols">A list of asset symbols to get test quotes for.</param>
        /// <returns>A list of test asset quotes</returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<AssetQuote> GetQuote(List<string> assetSymbols)
        {
            List<AssetQuote> assets = new List<AssetQuote>(); //initialise list we will return
            int addNum = 1; //just to make each quote different to test if the loop works

            //iterate through given symbols list, getting quote data for each symbol
            for (int i = 0; i < assetSymbols.Count; i++) {
                AssetQuote fakeAssetQuote = new AssetQuote();
                fakeAssetQuote.AssetSymbol = assetSymbols[i];
                fakeAssetQuote.AssetFullName = "Fake Asset";
                fakeAssetQuote.AssetType = AssetType.Equity;
                fakeAssetQuote.RegularMarketOpen = 150.0m;
                fakeAssetQuote.AssetQuoteValue = 155.0m;
                fakeAssetQuote.AssetQuoteTimeStamp = DateTime.Now;
                fakeAssetQuote.RegularMarketChange = 5.5m;
                fakeAssetQuote.RegularMarketChangePercentage = 3 + addNum;
                fakeAssetQuote.RegularMarketPreviousClose = 125m;

                assets.Add(fakeAssetQuote);
                addNum++;
            }
            return assets;

            //throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a list of trending stocks for a region from an exchange.
        /// </summary>
        /// <param name="region">is the region to look up.</param>
        /// <returns>a list of asset symbols for the specified region</returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<String> GetTrendingStocksForRegion(string region)
        {
            List<String> fakeStocksForRegion = new List<String>();
            List<AssetQuote> fakeAssetQuotes = new List<AssetQuote>();
            AssetQuote fakeAssetQuote1 = new AssetQuote();
            AssetQuote fakeAssetQuote2 = new AssetQuote();
            fakeAssetQuote1.AssetSymbol = "AAPL";
            fakeAssetQuote1.AssetRegion = "US";
            fakeAssetQuote2.AssetSymbol = "MSFT";
            fakeAssetQuote2.AssetRegion = "US";

            fakeAssetQuotes.Add(fakeAssetQuote1);
            fakeAssetQuotes.Add(fakeAssetQuote2);

            foreach (AssetQuote asset in fakeAssetQuotes)
            {
                if(asset.AssetRegion == region)
                {
                    fakeStocksForRegion.Add(asset.AssetSymbol);
                }
            }

            return fakeStocksForRegion;
        }

    }
}
