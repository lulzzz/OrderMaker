using System;
using System.Collections.Generic;

namespace Mtd.OrderMaker.Web.Data
{
    public partial class MtdLogApproval
    {
        public int Id { get; set; }
        public string MtdApproval { get; set; }
        public int MtdApprovalStage { get; set; }
        public string UserId { get; set; }
        public DateTime Timech { get; set; }
        public sbyte Result { get; set; }

        public virtual MtdApproval MtdApprovalNavigation { get; set; }
        public virtual MtdApprovalStage MtdApprovalStageNavigation { get; set; }
    }
}
