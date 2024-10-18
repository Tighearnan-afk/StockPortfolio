namespace Portfolio.Model
{
    /// <summary>
    /// A simple C# enumeration for different types of assets
    /// </summary>
    public enum AssetType
    {
        Equity, // aka stock
        Cryptocurrency,
        Currency
    }

    /// <summary>
    /// The Asset class represents the base class for any assets within the portfolio. Assets 
    /// can be either stock or cryptocurrencies. 
    /// </summary>
    public class Asset
    {
        /// <summary>
        /// The symbol of the asset eg "APPL" or "BTC-USD"
        /// </summary>
        public string AssetSymbol { get; set; }

        /// <summary>
        /// The full name of the asset symobl, eg "Apple Inc" or "Bitcoin USD".
        /// </summary>
        public string AssetFullName { get; set; }

        /// <summary>
        /// The date and time the asset was originally purchased.
        /// </summary>
        public DateTime AssetPurchaseDateTime { get; set; }


        /// <summary>
        /// The type of asset (stock, currency et cetera), <see cref="AssetType"/> enumeration.
        /// </summary>
        public AssetType AssetType { get; set; }

        /// <summary>
        /// The quoted price each unit cost USD at the time of purchase.
        /// </summary>
        public decimal PurchaseCost { get; set; }

        /// <summary>
        /// The number of units of the asset purchased at the date and time originally purchased.
        /// </summary>
        public int UnitsPurchased { get; set; }

        /// <summary>
        /// The amount the Asset is sold for when SellAsset is called
        /// </summary>
        public decimal SalePrice { get; set; }

        // TODO - Design and Implement the Asset class as you see fit. Justify the design decisions
        // that you make within the implementation.

    }
}
