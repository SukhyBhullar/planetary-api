using Newtonsoft.Json;
using System;

namespace Planetary.Domain
{
    public class DefaultPlace
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        public string Place { get; set; }

        public DefaultPlace()
        {
            Id = Guid.Empty;
        }
    }
}
