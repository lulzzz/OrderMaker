using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mtd.OrderMaker.Web.Data;

namespace Mtd.OrderMaker.Web.Areas.Config.Pages.Approval
{
    public class StageCreateModel : PageModel
    {
        private readonly OrderMakerContext _context;

        public StageCreateModel(OrderMakerContext context)
        {
            _context = context;
        }

        public MtdApproval MtdApproval { get; set; }
        public async Task<IActionResult> OnGetAsync(string idApproval)
        {
            MtdApproval = await _context.MtdApproval.FindAsync(idApproval);
            if (MtdApproval == null)
            {
                return NotFound();
            }
            
            return Page();
        }
        
    }
}