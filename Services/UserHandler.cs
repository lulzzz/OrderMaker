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

using Microsoft.AspNetCore.Identity;
using Mtd.OrderMaker.Web.Areas.Identity.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.Services
{
    public enum RightsType
    {
        View, Create, Edit, Delete
    };

    public class UserHandler
    {
        public readonly UserManager<WebAppUser> _userManager;

        public string RightView => "-view";
        public string RightCreate => "-create";
        public string RightEdit => "-edit";
        public string RightDelete => "-delete";

        private List<string> rights;
        public ReadOnlyCollection<string> Rights => rights.AsReadOnly();

        public UserHandler(UserManager<WebAppUser> userManager)
        {
            _userManager = userManager;
            rights = new List<string>()
            {RightView,RightCreate,RightDelete,RightEdit };
        }

        private string RightTypeToString(RightsType rightsType)
        {
            return $"-{rightsType.ToString().ToLower()}";
        }


        public async Task<List<string>> GetFormIdsAsync(WebAppUser user, RightsType rightsType)
        {
            string right = RightTypeToString(rightsType);
            IList<Claim> claims = await _userManager.GetClaimsAsync(user);
            List<string> formIds = claims.Where(x => x.Value == right).Select(x => x.Type).ToList();

            return formIds;
        }

        

        public async Task<bool> IsRight(WebAppUser user, RightsType rightsType, string idForm)
        {
            bool result;

            string right = RightTypeToString(rightsType);
            IList<Claim> claims = await _userManager.GetClaimsAsync(user);
            result = claims.Where(x => x.Type == idForm && x.Value == right).Any();

            return result;
        }

    }
}
