using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.Data;

namespace Mtd.OrderMaker.Web.Areas.Config.Pages.Interface
{

    public class IndexModel : PageModel
    {

        private readonly OrderMakerContext _context;


        public IndexModel(OrderMakerContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["ImgMenu"] = await GetImageFromConfig(1);
            ViewData["ImgAppBar"] = await GetImageFromConfig(2);
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {

            await SaveImg(1);
            await SaveImg(2);
            await _context.SaveChangesAsync();

            return Page();
        }


        private async Task SaveImg(int code)
        {
            string idCheckBox = $"{code}-delete";
            if (Request.Form[idCheckBox].FirstOrDefault() == null || Request.Form[idCheckBox].FirstOrDefault() == "false")
            {
                string idInput = $"{code}-file-upload-input";
                IFormFile file = Request.Form.Files.FirstOrDefault(x => x.Name == idInput);
                if (file != null)
                {
                    byte[] streamArray = new byte[file.Length];
                    await file.OpenReadStream().ReadAsync(streamArray, 0, streamArray.Length);
                    MtdConfigFile imgConfig = new MtdConfigFile()
                    {
                        Id = code,
                        Name = code == 1 ? "Image for menu" : "Image for AppBar",
                        FileData = streamArray,
                        FileSize = streamArray.Length,
                        FileType = file.ContentType
                    };

                    bool exists = await _context.MtdConfigFiles.Where(x => x.Id == code).AnyAsync();
                    if (exists)
                        _context.Attach(imgConfig).State = EntityState.Modified;
                    else
                        _context.Attach(imgConfig).State = EntityState.Added;
                }
            }
            else
            {
                MtdConfigFile header = new MtdConfigFile() { Id = code };
                _context.Attach(header).State = EntityState.Deleted;
            }

        }


        private async Task<string> GetImageFromConfig(int code)
        {
            string result = string.Empty;
            MtdConfigFile configFile = await _context.MtdConfigFiles.FindAsync(code);
            if (configFile != null && configFile.FileData != null)
            {
                string base64 = Convert.ToBase64String(configFile.FileData);
                result = string.Format("data:{0};base64,{1}", configFile.FileType, base64);
            }

            return result;
        }

    }
}