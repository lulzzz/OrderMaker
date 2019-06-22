using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.Data;

namespace Mtd.OrderMaker.Web.Areas.Config.Pages.Approval
{
    public class CreateModel : PageModel
    {
        private readonly OrderMakerContext _context;
        
        public CreateModel(OrderMakerContext context)
        {
            _context = context;
        
        }

        [BindProperty]
        public MtdApproval MtdApproval { get; set; }
        public IList<MtdForm> MtdForms { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            MtdApproval = new MtdApproval
            {
                Id = Guid.NewGuid().ToString()
            };

            MtdForms = await _context.MtdForm.ToListAsync();
            ViewData["Forms"] = new SelectList(MtdForms.OrderBy(x => x.Sequence), "Id", "Name");


            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            _context.MtdApproval.Add(MtdApproval);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Edit", new { id = MtdApproval.Id });
        }
    }
}