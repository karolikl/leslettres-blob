using ImageResizeWebApp.Helpers;
using ImageResizeWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace ImageResizeWebApp.Controllers
{
    public class LetterController : Controller
    {
        // make sure that appsettings.json is filled with the necessary details of the azure storage
        private readonly AzureStorageConfig storageConfig = null;

        public LetterController(IOptions<AzureStorageConfig> config)
        {
            storageConfig = config.Value;
        }

        // GET: LetterController
        public ActionResult Index()
        {
            return View();
        }

        // GET: LetterController/Index/name
        [HttpGet("Letter/{name}")]
        public async Task<IActionResult> Index(string name)
        {
            try
            {
                if (storageConfig.AccountKey == string.Empty || storageConfig.AccountName == string.Empty)
                    return BadRequest("Sorry, can't retrieve your Azure storage details from appsettings.js, make sure that you add Azure storage details there.");

                if (storageConfig.ImageContainer == string.Empty)
                    return BadRequest("Please provide a name for your image container in Azure blob storage.");

                if (storageConfig.TranslatedTextContainer == string.Empty)
                    return BadRequest("Please provide a name for your translatedText container in Azure blob storage.");

                //List<string> imagelUrls = await StorageHelper.GetImageUrls(storageConfig);
                TranslatedLetter letter = await StorageHelper.GetLetter(storageConfig, name);
                return View(letter);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
