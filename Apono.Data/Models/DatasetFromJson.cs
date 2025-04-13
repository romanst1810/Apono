using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apono.Data.Models
{
    public class DatasetFromJson
    {
        public List<Citizen> citizens { get; set; } = new();
        public List<Role> roles { get; set; } = new();
    }
}
