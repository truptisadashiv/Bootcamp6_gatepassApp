using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatePass1.Models
{
    public class ViewModel
    {
        public IEnumerable<Gate> Gates { get; set; }
        public IEnumerable<Asset> Assets { get; set; }
    }
}
