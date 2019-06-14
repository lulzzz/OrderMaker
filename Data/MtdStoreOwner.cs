using System;
using System.Collections.Generic;

namespace Mtd.OrderMaker.Web.Data
{
    public partial class MtdStoreOwner
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }

        public virtual MtdStore IdNavigation { get; set; }
    }
}
