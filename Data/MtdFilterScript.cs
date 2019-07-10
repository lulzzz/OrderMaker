using System;
using System.Collections.Generic;

namespace Mtd.OrderMaker.Web.Data
{
    public partial class MtdFilterScript
    {
        public int Id { get; set; }
        public int MtdFilter { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Script { get; set; }
        public sbyte Apply { get; set; }

        public virtual MtdFilter MtdFilterNavigation { get; set; }
    }
}
