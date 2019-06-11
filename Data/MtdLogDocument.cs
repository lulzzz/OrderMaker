using System;
using System.Collections.Generic;

namespace Mtd.OrderMaker.Web.Data
{
    public partial class MtdLogDocument
    {
        public int Id { get; set; }
        public string MtdStore { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime TimeCh { get; set; }

        public virtual MtdStore MtdStoreNavigation { get; set; }
    }
}
