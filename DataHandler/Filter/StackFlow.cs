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

using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.DataHandler.Approval;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.DataHandler.Filter
{
    public partial class FilterHandler
    {
        public async Task<OutFlow> GetStackFlowAsync(Incomer incomer, TypeQuery typeQuery) {

            OutFlow outFlow = new OutFlow();

            if (incomer.WaitList == 1)
            {
                List<string> storesForUser = await ApprovalHandler.GetWaitStoreIds(_context, _user);
                queryMtdStore = queryMtdStore.Where(x => storesForUser.Contains(x.Id));
            }
            
            bool ownOnly = _userRights.Where(x => x.Value.Contains("view-own")).Any();
            if (ownOnly)
            {
                IList<string> storeIds = await _context.MtdStoreOwner.Where(x => x.UserId == _user.Id).Select(x => x.Id).ToListAsync();
                queryMtdStore = queryMtdStore.Where(x => storeIds.Contains(x.Id));
            }



            switch (typeQuery)
            {
                case TypeQuery.number:
                    {
                        outFlow = await GetDataForNumberAsync(incomer);
                        break;
                    }
                case TypeQuery.text:
                    {
                        outFlow = await GetDataForTextAsync(incomer);
                        break;
                    }
                case TypeQuery.field:
                case TypeQuery.textField:
                    {
                        outFlow = await GetDataForFieldAsync(incomer);
                        break;
                    }
                default:
                    {
                        outFlow = await GetDataForEmptyAsync(incomer);
                        break;
                    }
            }

            return outFlow;
        }
    }
}
