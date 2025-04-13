using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apono.Data.Models
{
    public class Citizen
    {
        public string name { get; set; } = string.Empty;
        public List<string>? roles { get; set; }
        public List<string>? allowed_places { get; set; }
    }
}
