/*
    OrderMaker - http://ordermaker.org
    Copyright(c) 2019 Oleg Bruev. All rights reserved.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.Data;

namespace Mtd.OrderMaker.Web.Controllers.Config.Approval
{
    [Route("api/config/approval")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DataController : ControllerBase
    {

        private readonly OrderMakerContext _context;


        public DataController(OrderMakerContext context)
        {
            _context = context;
        }

        [HttpPost("delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDeleteAsync()
        {
            string approvalId = Request.Form["IdApproval"];
            if (approvalId == null)
            {
                return NotFound();
            }

            MtdApproval mtdApproval = new MtdApproval { Id = approvalId };
            _context.MtdApproval.Remove(mtdApproval);
            await _context.SaveChangesAsync();
            return Ok();
        }
     
    }
}