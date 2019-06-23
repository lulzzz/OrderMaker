using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.Areas.Identity.Data;
using Mtd.OrderMaker.Web.Data;
using Mtd.OrderMaker.Web.Services;

namespace Mtd.OrderMaker.Web.Areas.Config.Pages.Approval
{
    public class StageEditModel : PageModel
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandlerTrial _userHandler;

        public StageEditModel(OrderMakerContext context, UserHandlerTrial userHandler)
        {
            _context = context;
            _userHandler = userHandler;
        }
        
        public MtdApproval MtdApproval { get; set; }
        public MtdApprovalStage MtdApprovalStage { get; set; }
        public IList<MtdFormPart> MtdFormParts { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {

            MtdApprovalStage = await _context.MtdApprovalStage.FindAsync(id);

            if (MtdApprovalStage == null)
            {
                return NotFound();
            }

            MtdApproval = await _context.MtdApproval.Include(x => x.MtdFormNavigation).Where(x => x.Id == MtdApprovalStage.MtdApproval).FirstOrDefaultAsync();
                        
            MtdFormParts = await _context.MtdFormPart.Where(x => x.MtdForm == MtdApproval.MtdForm).OrderBy(x => x.Sequence).ToListAsync();
            IList<WebAppUser> webAppUsers = await _userHandler.Users.ToListAsync();
            ViewData["Users"] = new SelectList(webAppUsers, "Id", "Title");
            return Page();
        }
    }
}