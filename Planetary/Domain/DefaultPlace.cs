using Newtonsoft.Json;
using System;

namespace Planetary.Domain
{
    public class DefaultPlace
    {
        public const string DefaultPlaceId = "00000000-0000-0000-0000-000000000000";

        [JsonProperty("id")]
        public Guid Id { get; set; }
        public string Place { get; set; }

        public DefaultPlace()
        {
            Id = Guid.Empty;
        }
    }
}
