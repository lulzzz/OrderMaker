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

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mtd.OrderMaker.Web.Areas.Identity.Data;
using Mtd.OrderMaker.Web.Data;
using Mtd.OrderMaker.Web.DataHandler.Filter;
using Mtd.OrderMaker.Web.DataHandler.Stack;
using Mtd.OrderMaker.Web.Models.Index;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.Components.Index
{
    [ViewComponent(Name = "IndexRows")]
    public class Rows : ViewComponent
    {
        private readonly OrderMakerContext _context;
        private readonly UserManager<WebAppUser> _userManager;
        public int pageCount = 0;


        public Rows(OrderMakerContext orderMakerContext, UserManager<WebAppUser> userManager)
        {
            _context = orderMakerContext;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string idForm)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            FilterHandler handlerFilter = new FilterHandler(_context, idForm, user);
            Incomer incomer = await handlerFilter.GetIncomerDataAsync();
            TypeQuery typeQuery = await handlerFilter.GetTypeQueryAsync();
            OutFlow outFlow = await handlerFilter.GetStackFlowAsync(incomer, typeQuery);
            IList<MtdStore> mtdStore = outFlow.MtdStores;

            decimal count = outFlow.Count / incomer.PageSize;
            pageCount = Convert.ToInt32(Math.Round(count, MidpointRounding.AwayFromZero));
            pageCount = pageCount == 0 ? 1 : pageCount;

            IList<string> storeIds = mtdStore.Select(s => s.Id).ToList();
            IList<string> fieldIds =  fieldIds = incomer.FieldForColumn.Select(x => x.Id).ToList();
            

            StackHandler handlerStack = new StackHandler(_context);
            IList<MtdStoreStack> mtdStoreStack = await handlerStack.GetStackAsync(storeIds, fieldIds);

            RowsModelView rowsModel = new RowsModelView
            {
                IdForm = idForm,
                SearchNumber = incomer.SearchNumber,
                PageCount = pageCount,
                MtdFormPartFields = incomer.FieldForColumn,
                MtdStores = mtdStore,
                MtdStoreStack = mtdStoreStack
            };

            return View("Default", rowsModel);
        }

    }



}