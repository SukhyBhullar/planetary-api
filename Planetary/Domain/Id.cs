using Newtonsoft.Json;
using System;

namespace Planetary.Domain
{
    public class Identified
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        public string UserId { get; set; }
    }

}
