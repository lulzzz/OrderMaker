using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.Areas.Identity.Data;
using Mtd.OrderMaker.Web.Data;
using Mtd.OrderMaker.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.Controllers.Store
{
    [Route("api/store/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandler _userHandler;

        public AdminController(OrderMakerContext context, UserHandler userHandler)
        {
            _context = context;
            _userHandler = userHandler;
        }


        [HttpPost("setowner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostSetOwnerAsync()
        {

            string idStore = Request.Form["setowner-id-store"];
            string idUser = Request.Form["setowner-id-user"];
             
            WebAppUser webAppUser = await _userHandler._userManager.FindByIdAsync(idUser);

            if (webAppUser != null)
            {

                MtdStoreOwner mtdStoreOwner = await _context.MtdStoreOwner.FindAsync(idStore);
                if (mtdStoreOwner == null)
                {
                    mtdStoreOwner = new MtdStoreOwner
                    {
                        Id = idStore,
                        UserId = webAppUser.Id,
                        UserName = webAppUser.Title
                    };

                    await _context.MtdStoreOwner.AddAsync(mtdStoreOwner);
                    await _context.SaveChangesAsync();

                    return Ok();

                }

                mtdStoreOwner.UserId = webAppUser.Id;
                mtdStoreOwner.UserName = webAppUser.Title;
                _context.Entry(mtdStoreOwner).State = EntityState.Modified; 
                await _context.SaveChangesAsync();

            }

            return Ok();
        }

    }
}
