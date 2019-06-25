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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.Data;
using Mtd.OrderMaker.Web.DataHandler.Approval;
using Mtd.OrderMaker.Web.Models.LogDocument;
using Mtd.OrderMaker.Web.Models.Store;
using Mtd.OrderMaker.Web.Services;

namespace Mtd.OrderMaker.Web.Areas.Workplace.Pages.Store
{
    public class DetailsModel : PageModel
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandlerTrial _userHandler;

        public DetailsModel(OrderMakerContext context, UserHandlerTrial userHandler)
        {
            _context = context;
            _userHandler = userHandler;
        }

        public MtdStore MtdStore { get; set; }
        public MtdForm MtdForm { get; set; }
        public ChangesHistory ChangesHistory { get; set; }
        public MtdStoreOwner StoreOwner { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsEditor { get; set; }
        public bool IsApprover { get; set; }
        public bool IsFirstStage { get; set; }
        public int ApprovalStatus { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MtdStore = await _context.MtdStore.FirstOrDefaultAsync(m => m.Id == id);

            if (MtdStore == null)
            {
                return NotFound();
            }

            var user = await _userHandler.GetUserAsync(HttpContext.User);
            bool isViewer = await _userHandler.IsViewer(user, MtdStore.MtdForm, MtdStore.Id);
            IsEditor = await _userHandler.IsEditor(user, MtdStore.MtdForm, MtdStore.Id);
            IsAdmin = await _userHandler.IsAdmin(user);

            if (!isViewer)
            {
                return Forbid();
            }


            MtdForm = await _context.MtdForm.Include(m => m.InverseParentNavigation).FirstOrDefaultAsync(x => x.Id == MtdStore.MtdForm);
            MtdLogDocument edited = await _context.MtdLogDocument.Where(x => x.MtdStore == MtdStore.Id).OrderByDescending(x => x.TimeCh).FirstOrDefaultAsync();
            MtdLogDocument created = await _context.MtdLogDocument.Where(x => x.MtdStore == MtdStore.Id).OrderBy(x => x.TimeCh).FirstOrDefaultAsync();

            StoreOwner = await _context.MtdStoreOwner.Where(x => x.Id == MtdStore.Id).FirstOrDefaultAsync();

            ChangesHistory = new ChangesHistory
            {
                CreateByTime = MtdStore.Timecr.ToString()
            };

            if (edited != null)
            {
                ChangesHistory.LastEditedUser = edited.UserName;
                ChangesHistory.LastEditedTime = edited.TimeCh.ToString();
            }

            if (created != null)
            {
                if (MtdStore.Timecr.Date == created.TimeCh.Date)
                {
                    ChangesHistory.CreateByUser = created.UserName;
                }
            }

            ViewData["UsersList"] = new SelectList(_userHandler.Users, "Id", "Title");

            ApprovalHandler approvalHandler = new ApprovalHandler(_context, MtdStore.Id);
            IsApprover = await approvalHandler.IsApproverAsync(user);
            IsFirstStage = await approvalHandler.IsFirstStageAsync();
            IList<MtdApprovalStage> stages = await approvalHandler.GetStagesDownAsync();
            ViewData["Stages"] = new SelectList(stages.OrderByDescending(x => x.Stage), "Id", "Name");

            ApprovalStatus = 0;
            if (!IsFirstStage) { ApprovalStatus = 1; }
            if (await approvalHandler.IsComplete())
            {
                var allStages = await approvalHandler.GetStagesAsync();
                List<int> stageIds = allStages.Select(x => x.Id).ToList();
                IList<MtdLogApproval> logs =  await approvalHandler.GetHistory();

                ApprovalStatus = logs.OrderByDescending(x=>x.Timech).FirstOrDefault().Result == 0 ? 2 : 3; 
            }
            

            return Page();
        }


    }
}
