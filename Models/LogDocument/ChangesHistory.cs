using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.Models.LogDocument
{
    public class ChangesHistory
    {
        public string CreateByUser { get; set; } = "Unknown";
        public string CreateByTime { get; set; } = "Unknown";
        public string LastEditedUser { get; set; } = "Unknown";
        public string LastEditedTime { get; set; } = "Unknown";
    }

}

