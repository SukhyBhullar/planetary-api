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
        public string CallSign { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
