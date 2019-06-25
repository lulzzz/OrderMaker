using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mtd.OrderMaker.Web.Areas.Identity.Data;
using Mtd.OrderMaker.Web.Data;
using Mtd.OrderMaker.Web.DataHandler.Approval;
using Mtd.OrderMaker.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.Controllers.Store
{

    [Route("api/store/approval")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public class ApprovalController : ControllerBase
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandlerTrial _userHandler;


        public ApprovalController(OrderMakerContext context, UserHandlerTrial userHandler)
        {
            _context = context;
            _userHandler = userHandler;
        }

        [HttpPost("approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostApproveAsync()
        {
            string storeId = Request.Form["id-store"];
            WebAppUser webAppUser = await _userHandler.GetUserAsync(HttpContext.User);
            ApprovalHandler approvalHandler = new ApprovalHandler(_context, storeId);
            bool isApprover = await approvalHandler.IsApproverAsync(webAppUser);
            if (!isApprover) { return NotFound(); }

            await approvalHandler.ActionApprove(webAppUser);
            return Ok();
        }

        [HttpPost("reject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostRejectAsync()
        {
            string storeId = Request.Form["id-store"];
            bool completeOk = bool.TryParse(Request.Form["checkbox-complete"], out bool completeCheck);
            bool stageOk = int.TryParse(Request.Form["next-stage"], out int stageId);
            if (!stageOk || !completeOk) { return NotFound(); }

            WebAppUser webAppUser = await _userHandler.GetUserAsync(HttpContext.User);
            ApprovalHandler approvalHandler = new ApprovalHandler(_context, storeId);

            bool isFirstStage = await approvalHandler.IsFirstStageAsync();
            bool isApprover = await approvalHandler.IsApproverAsync(webAppUser);
            if (!isApprover || isFirstStage) { return NotFound(); }

            await approvalHandler.ActionReject(completeCheck, stageId, webAppUser);
            return Ok();
        }

    }
}
