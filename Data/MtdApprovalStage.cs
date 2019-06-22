using System;
using System.Collections.Generic;

namespace Mtd.OrderMaker.Web.Data
{
    public partial class MtdApprovalStage
    {
        public MtdApprovalStage()
        {
            MtdLogApproval = new HashSet<MtdLogApproval>();
            MtdStoreApproval = new HashSet<MtdStoreApproval>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MtdApproval { get; set; }
        public int Stage { get; set; }
        public string UserId { get; set; }
        public string BlockParts { get; set; }
        

        public virtual MtdApproval MtdApprovalNavigation { get; set; }
        public virtual ICollection<MtdLogApproval> MtdLogApproval { get; set; }
        public virtual ICollection<MtdStoreApproval> MtdStoreApproval { get; set; }
    }
}
