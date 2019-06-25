using System;
using System.Collections.Generic;

namespace Mtd.OrderMaker.Web.Data
{
    public partial class MtdStoreApproval
    {
        public string Id { get; set; }
        public int MtdApproveStage { get; set; }
        public string PartsApproved { get; set; }
        public sbyte Approved { get; set; }

        public virtual MtdStore IdNavigation { get; set; }
        public virtual MtdApprovalStage MdApproveStageNavigation { get; set; }
    }
}
