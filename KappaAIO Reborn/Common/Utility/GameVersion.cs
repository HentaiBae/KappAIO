using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Version = System.Version;

namespace KappAIO_Reborn.Common.Utility
{
    public static class GameVersion
    {
        private const string _versionUrl = "http://ddragon.leagueoflegends.com/api/versions.json";
        private static Version _cachedVersion;

        public static Version CurrentPatch()
        {
            if (_cachedVersion != null)
            {
                return _cachedVersion;
            }

            var WebClient = new WebClient();
            using (WebClient)
            {
                var request = Task.Run(async () => await WebClient.DownloadStringTaskAsync(_versionUrl));
                var versionJson = JArray.Parse(request.Result);
                var stringversion = versionJson.First.ToObject<string>();
                _cachedVersion = new Version(stringversion);
            }
            
            return _cachedVersion;
        }
    }
}
