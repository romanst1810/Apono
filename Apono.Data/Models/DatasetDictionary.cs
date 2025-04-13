using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apono.Data.Models
{
    public class DatasetDictionary
    {
        public Dictionary<string, Citizen> _citizens = new();
        public Dictionary<string, Role> _roles = new ();
    }
}
