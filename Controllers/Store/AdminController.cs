/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.

    This file is part of MTD OrderMaker.
    MTD OrderMaker is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see  https://www.gnu.org/licenses/.
*/

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
