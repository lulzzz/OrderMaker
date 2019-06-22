using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.Data;

namespace Mtd.OrderMaker.Web.Areas.Config.Pages.Approval
{
    public class EditModel : PageModel
    {
        private readonly OrderMakerContext _context;

        public EditModel(OrderMakerContext context)
        {
            _context = context;
        }
        [BindProperty]
        public MtdApproval MtdApproval { get; set; }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MtdApproval = await _context.MtdApproval.Include(x => x.MtdFormNavigation).FirstOrDefaultAsync(x => x.Id == id);

            if (MtdApproval == null)
            {
                return NotFound();
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(MtdApproval).State = EntityState.Modified;
            _context.Entry(MtdApproval).Property(x => x.MtdForm).IsModified = false;
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }


    }
}