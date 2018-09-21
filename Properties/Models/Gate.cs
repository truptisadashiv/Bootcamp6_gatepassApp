using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatePass1.Models
{
    public class Gate
    {
        public int serialNo { get; set; }
        public string employeeId { get; set; }
        public string fromLocation{ get; set; }
        public string toLocation { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public string businessJustification { get; set; }
        public string issuedTo { get; set; }
        public string status { get; set; }
       
      

    }
}
