using System.Threading.Tasks;
using MusicPlayer.Models.Dirble;

namespace MusicPlayer.Helpers
{
    /// <summary>
    /// The dirble client (subset of the api, see https://dirble.com/api-doc).
    /// </summary>
    internal class DirbleClient : RestClient
    {
        /// <summary>
        /// The number of results per page (limited by 30). 
        /// </summary>
        private readonly int _pageSize = 30;

        /// <summary>
        /// Gets the query string (api token).
        /// </summary>
        private static string QueryString
        {
            get
            {
                var token = IOHelper.ReadEmbeddedResourceText("MusicPlayer.Resources.DirbleApiKey.dirbleapikey");
                return !string.IsNullOrWhiteSpace(token) ? $"token={token}" : null;
            }
        }

        /// <summary>
        /// Initializes the dirble client.
        /// </summary>
        public DirbleClient() : base("http://api.dirble.com/v2/", QueryString)
        {
        }

        /// <summary>
        /// Gets the radio stations (pageSize = 250).
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>The radio stations.</returns>
        public async Task<RadioStation[]> GetStations(int page = 0)
        {
            return await Get<RadioStation[]>("stations", $"page={page}&per_page={_pageSize}");
        }

        /// <summary>
        /// Gets the radio stations (pageSize = 250).
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>The radio stations.</returns>
        public async Task<RadioStation[]> GetPopularStations(int page = 0)
        {
            return await Get<RadioStation[]>("stations/popular", $"page={page}&per_page={_pageSize}");
        }
    }
}
