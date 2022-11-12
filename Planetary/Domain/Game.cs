using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planetary.Domain
{
    public class Game : Identified
    {
        public string CharacterName { get; set; }
    }
}
