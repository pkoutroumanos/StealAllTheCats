namespace StealAllTheCats.Options
{
    /// <summary>
    /// Represents configuration settings for the external Cat API service.
    /// </summary>
    public class CatApiOptions
    {
        /// <summary>
        /// Base URL for the Cat API.
        /// </summary>
        public string BaseUrl { get; set; } = default!;
        /// <summary>
        /// The API key used for authenticating requests to the Cat API.
        /// </summary>
        public string ApiKey { get; set; } = default!;
    }
}
