
using System.Text.RegularExpressions;
using System.Web;
using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc;
namespace Ecommerce_Product.Controllers;
public class StaticPageController:BaseController
{


 private readonly ICategoryListRepository _category;

 private readonly IStaticFilesRepository _static_files;

 private readonly ILogger<StaticPageController> _logger;

public StaticPageController(ICategoryListRepository category,IStaticFilesRepository static_files,ILogger<StaticPageController> logger):base(category)
{
   
    this._category=category;
    this._static_files=static_files;
    this._logger=logger;
}

// public IActionResult HomePage()
// {
//   return View();
// }



[HttpGet]
[Route("{page_name}")]

public async Task<IActionResult> StaticPage(string page_name)
{   
    
   var static_file=await this._static_files.findStaticFileByName(page_name);
   string content=HttpUtility.HtmlDecode(static_file.Content); 
   Console.WriteLine("Content iss:"+content);
   Regex reg= new Regex(@"\s*(<[^>]+>)\s*");
    content=reg.Replace(content,"$1");
    ViewBag.content=content;    
    return View("~/Views/ClientSide/StaticPage/StaticPage.cshtml");
}

}