﻿/*
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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.Areas.Identity.Data;
using Mtd.OrderMaker.Web.Data;
using Mtd.OrderMaker.Web.Services;

namespace Mtd.OrderMaker.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandler _userHandler;

        public IndexModel(OrderMakerContext context, UserHandler userHandler)
        {
            _context = context;
            _userHandler = userHandler;
        }

        public IList<MtdForm> Forms { get; set; }
        public string SearchText { get; set; }

        public async Task<IActionResult> OnGetAsync(string searchText)
        {
            WebAppUser user = await _userHandler._userManager.GetUserAsync(HttpContext.User);
            
            List<string> formIds = await _userHandler.GetFormIdsAsync(user, RightsType.View); 

            IQueryable<MtdForm> query = _context.MtdForm
                    .Include(x => x.MtdGroupNavigation)
                    .Include(x=>x.MtdFormHeader)
                    .Include(x => x.MtdFormDesk)
                    .Where(x=> formIds.Contains(x.Id));

            if (searchText != null) {
                query = query.Where(x => x.Name.Contains(searchText));
                SearchText = searchText;
            }
                         

            Forms = await query.ToListAsync();
            return Page();
        }

    }
}