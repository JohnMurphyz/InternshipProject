using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Models
{


    public class ScheduledQueryModel
    {

        // I need to change these back to DateTime type & Bool

        public string Query { get; set; }

        public DateTime ScheduledTime { get; set; }


    }
}
