namespace Portfolio.Model
{
    /// <summary>
    /// The AssetQuote class is used to hold pricing information of assets, this includes:
    /// MarketPrice, marketChangePercentage, MarketChange and MarketPreviousClose values.
    /// </summary>
    public class AssetQuote
    {
        /// <summary>
        /// The full name of the asset e.g. Apple, Tesla, Barclays PLC, Bitcoin USD.
        /// </summary>
        public string AssetFullName { get; set; }

        /// <summary>
        /// The symbol of the asset e.g. APPL, TSLA, BARC or BTC-USD.
        /// </summary>
        public string AssetSymbol { get; set; }

        /// <summary>
        /// The type of asset being quoted for <see cref="AssetType"/>.
        /// </summary>
        public AssetType AssetType { get; set; }

        /// <summary>
        /// The date and time the asset quote was obtained.
        /// </summary>
        public DateTime AssetQuoteTimeStamp { get; set; }

        /// <summary>
        /// The value in USD of the named asset at the time of the quote.
        /// </summary>
        public decimal AssetQuoteValue { get; set; }

        /// <summary>
        /// The change (from opening price to now) in price expressed as USD
        /// </summary>
        public decimal RegularMarketChange { get; set; }

        /// <summary>
        /// The change (from opening price to now) in price expressed as a percentage.
        /// </summary>
        public float RegularMarketChangePercentage { get; set; }

        /// <summary>
        /// The price at which the asset closed the previous trading day.
        /// </summary>
        public decimal RegularMarketPreviousClose { get; set; }

        /// <summary>
        /// The price of the asset when the market opened.
        /// </summary>
        public decimal RegularMarketOpen { get; set; }

        // TODO - You should implement relevant methods for the class. You can add additional 
        // class members as required if you wish. Carefully consider the information that you can
        // retrieve from the finance API that you use and what information the user would like to 
        // view or may find useful 

        /// <summary>
        /// The region of the asset e.g. US.
        /// </summary>
        public string AssetRegion { get; set; }

    }
}
