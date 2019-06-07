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

using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.DataHandler.Filter
{
    public partial class FilterHandler
    {
        public async Task<OutFlow> GetStackFlowAsync(Incomer incomer, TypeQuery typeQuery) {

            OutFlow outFlow = new OutFlow();
            
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