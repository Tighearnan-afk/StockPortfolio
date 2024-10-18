using Portfolio.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.Service
{
    /// <summary>
    /// This interface displays asset quote and symbol, information including asset history,
    /// echange information, current and past values. 
    /// </summary>
    public interface IMarketClient
    {
        /// <summary>
        /// Gets an assetquote from an exchange for the asset.
        /// </summary>
        /// <param name="assetSymbol"></param>
        /// <returns>An AssetQuote <see cref="AssetQuote"/></returns>
        AssetQuote GetQuote(string assetSymbol);

        /// <summary>
        /// Gets a list of AsseQuotes <see cref="AssetQuote"/>from an exchange for the asset.
        /// </summary>
        /// <param name="assetSymbols">is a list of asset symbols to get quotes for.</param>
        /// <returns>A list of AssetQuotes <see cref="AssetQuote"/></returns>
        List<AssetQuote> GetQuote(List<string> assetSymbols);


        /// <summary>
        /// Gets a list of trending stocks for a region from an exchange.
        /// </summary>
        /// <param name="region">is the region to look up.</param>
        /// <returns>a list of asset symbols for the specified region</returns>
        public List<String> GetTrendingStocksForRegion(string region);

    }
}
