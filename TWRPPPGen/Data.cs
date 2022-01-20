namespace TWRPPPGen
{
    internal struct Data
    {
        /// <summary>
        /// The Cookie container.
        /// </summary>
        private static CookieContainer _cookieContainer = new();
        /// <summary>
        /// Used to make a new HttpClient
        /// </summary>
        public static readonly HttpClientHandler _handler = new()
        {
            SslProtocols = System.Security.Authentication.SslProtocols.Tls12,
            UseCookies = true,
            CookieContainer = _cookieContainer,
            AutomaticDecompression = DecompressionMethods.All
        };
        /// <summary>
        /// Store the runtime information for the OS of the host machine
        /// </summary>
        public static OSPlatform CurrentOS = OSPlatform.Windows;
        /// <summary>
        /// The HttpClient used to perform Http requests.
        /// </summary>
        public static HttpClient client = new();
        public static readonly string PathToAIK = Environment.CurrentDirectory + @"\Android Image Kitchen\";
    }
}
