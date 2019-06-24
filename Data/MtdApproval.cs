using System;
using System.Collections.Generic;

namespace Mtd.OrderMaker.Web.Data
{
    public partial class MtdApproval
    {
        public MtdApproval()
        {
            MtdApprovalStage = new HashSet<MtdApprovalStage>();            
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MtdForm { get; set; }

        public virtual MtdForm MtdFormNavigation { get; set; }
        public virtual ICollection<MtdApprovalStage> MtdApprovalStage { get; set; }        
    }
}
