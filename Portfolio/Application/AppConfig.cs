namespace Portfolio.Application
{
    /// <summary>
    /// The AppConfig class is used to store application configuration settings.
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// If InDevelopment is true the code is not deployed to production. 
        /// </summary>
        public bool InDevelopment { get; set; }

        /// <summary>
        /// If InTest is true we are using the MockClient to get asset quotes.
        /// </summary>
        public bool InTest { get; set; }

        /// <summary>
        /// The BaseURL is core URL for the live market client to retrieve live market data. 
        /// </summary>
        public String BaseURL { get; set; }

        /// <summary>
        /// A list of API key strings for the live market data API.
        /// </summary>
        public List<string> API_keys { get; set; }

    }
}
