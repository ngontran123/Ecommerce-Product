using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using System.IO;
using System.Text;
using iText.Commons.Utils;
using Org.BouncyCastle.Math.EC.Rfc8032;
using System.ComponentModel;

namespace Ecommerce_Product.Controllers;
[Route("admin")]
public class ProductListController : Controller
{
    private readonly ILogger<ProductListController> _logger;

    // private readonly ICategoryRepository _categoryList;

    // public CategoryListController(ILogger<CategoryListController> logger,ICategoryRepository categoryList)
    // {
    //     _logger = logger;
    //    this._categoryList=categoryList; 
    // }

   private readonly IProductRepository _product;

   private readonly ICategoryListRepository _category;
  private readonly IWebHostEnvironment _webHostEnv;

   
   public ProductListController(IProductRepository product,ICategoryListRepository category,ILogger<ProductListController> logger,IWebHostEnvironment webHostEnv)
   {
    this._product=product;
    this._category=category;
    this._webHostEnv=webHostEnv;
    this._logger=logger; 
   }
  //[Authorize(Roles ="Admin")]
  [Route("product_list")]
  [HttpGet]
  public async Task<IActionResult> ProductList()
  {       string select_size="7";
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
          FilterProduct prod_filter=new FilterProduct("","","","","","");
          ViewBag.filter_obj=prod_filter;
          var cats=await this._category.getAllCategory();
          var brands=await this._category.getAllBrandList();
          ViewBag.CatList=cats;
          ViewBag.BrandList = brands;
          ViewBag.StatusList = new List<string>{"Hết hàng","Còn hàng"};
    try
    {  
        var prods=await this._product.pagingProduct(7,1);
        return View(prods);
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Get Product List Exception:"+er.Message);
    }
    return View();
  }


//[Authorize(Roles ="Admin")]
  [Route("product_list/paging")]
   [HttpGet]
  public async Task<IActionResult> ProductListPaging([FromQuery]int page_size,[FromQuery] int page=1,string productname="",string brand="",string category="",string start_date="",string end_date="",string status="")
  {
    try{
         var prods=await this._product.pagingProduct(page_size,page);
         if(!string.IsNullOrEmpty(productname)||!string.IsNullOrEmpty(brand) || !string.IsNullOrEmpty(start_date) || !string.IsNullOrEmpty(end_date) || !string.IsNullOrEmpty(category) || !string.IsNullOrEmpty(status))
         {
            FilterProduct prod=new FilterProduct(productname,start_date,end_date,category,brand,status);
            var filter_prods=await this._product.filterProduct(prod);
            var filter_prods_paging=PageList<Product>.CreateItem(filter_prods.AsQueryable(),page,page_size);
            ViewBag.filter_obj=filter_prods_paging;
         }
          List<string> options=new List<string>(){"7","10","20","50"};
          
          ViewBag.options=options;

          var cats=await this._category.getAllCategory();
          var brands=await this._category.getAllBrandList();
          ViewBag.CatList=cats;
          ViewBag.BrandList = brands;
          ViewBag.StatusList = new List<string>{"Hết hàng","Còn hàng"};
        
          
          string select_size=page_size.ToString();
          
          ViewBag.select_size=select_size;
          
          return View("~/Views/ProductList/ProductList.cshtml",prods);
        }
     
        catch(Exception er)
        {
            this._logger.LogTrace("Paging Product List Exception:"+er.Message);
        }
    return View();
  }

//[Authorize(Roles ="Admin")]

   [Route("product_list")]
   [HttpPost]
   public async Task<IActionResult> ProductList(FilterProduct products)
   {
    try
    {   
    string startdate=products.StartDate;
    string enddate = products.EndDate;


 if(!string.IsNullOrEmpty(startdate))
 {
   string[] reformatted=startdate.Trim().Split('-');

   startdate=reformatted[1]+"/"+reformatted[2]+"/"+reformatted[0];
 }
     if(!string.IsNullOrEmpty(enddate))
{ 
   string[] reformatted=enddate.Trim().Split('-');

   enddate=reformatted[1]+"/"+reformatted[2]+"/"+reformatted[0];
 }      string select_size="7";
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
         var cats=await this._category.getAllCategory();
          var brands=await this._category.getAllBrandList();
          ViewBag.CatList=cats;
          ViewBag.BrandList = brands;
          ViewBag.StatusList = new List<string>{"Hết hàng","Còn hàng"};
       var product_list=await this._product.filterProduct(products);
       var product_paging=PageList<Product>.CreateItem(product_list.AsQueryable(),1,7);
       ViewBag.filter_obj=product_list;  
    return View("~/Views/ProductList/ProductList.cshtml",product_paging);
    }
    catch(Exception er)
    {
    this._logger.LogTrace("Filter Product List Exception:"+er.Message); 
    }
    return View();
   }
  
  [Route("product_list/delete")]
  [HttpGet]
  public async Task<IActionResult> DeleteProduct(int id)
  {
    try
    {
      int res=await this._product.deleteProduct(id);
      if(res==0)
      {
        TempData["Status_Delete"]=0;
        TempData["Message_Delete"]=$"Xóa sản phẩm mã {id} thất bại";
      }
      else{
         TempData["Status_Delete"]=1;
        TempData["Message_Delete"]=$"Xóa sản phẩm mã {id} thành công"; 
      }
    }
    catch(Exception er)
    {
       this._logger.LogTrace("Remove Product Exception:"+er.Message); 
     
    }
    return RedirectToAction("ProductList","ProductList");
  }
 [Route("product_list/export")]
 [HttpGet]
  public async Task<IActionResult> ExportToExcel()
  {
    try
    {
     var content= await this._product.exportToExcelProduct();
  return File(content,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","Products.xlsx");
    }
    catch(Exception er)
    {
    this._logger.LogTrace("Export Product Excel Exception:"+er.Message); 
    }
    return RedirectToAction("ProductList","ProductList");
  }

  [Route("product_list/add")]
  [HttpGet]
  public async Task<IActionResult> AddProductList()
  { 
    var category_list=await this._category.getAllCategory();
    
    var brand_list = await this._category.getAllBrandList();

    List<SubCategory> sub_cat_list=new List<SubCategory>();

    foreach(var cat in category_list)
    {
      foreach(var sub_cat in cat.SubCategories)
      {
        sub_cat_list.Add(sub_cat);
      }
    }
    ViewBag.CategoryList=category_list;
    ViewBag.BrandList=brand_list;
    ViewBag.SubCatList=sub_cat_list;
    return View();
  }

  [Route("product_list/add")]
  [HttpPost]
  public async Task<IActionResult> AddProductList(AddProductModel model)
  {
  try
  {

 int created_res=await this._product.addNewProduct(model);

 if(created_res==0)
 {  
  ViewBag.Status=0;
  ViewBag.Created_Product="Thêm sản phẩm thất bại";
 }
 else if(created_res==-1)
 {
  ViewBag.Status=-1;
  TempData["Status"]="-1";
  ViewBag.Created_Product="Sản phẩm đã tồn tại trong hệ thống";
 }
 else
 {
 ViewBag.Status=1;
 ViewBag.Created_Product="Thêm sản phẩm thành công";
 }
 }
  catch(Exception er)
  {
    this._logger.LogTrace("Add Product Exception:"+er.Message);
    Console.WriteLine("Add Product List Exception:"+er.Message);
  }
var category_list=await this._category.getAllCategory(); 
    
    var brand_list = await this._category.getAllBrandList();

    List<SubCategory> sub_cat_list=new List<SubCategory>();

    foreach(var cat in category_list)
    {
      foreach(var sub_cat in cat.SubCategories)
      {
        sub_cat_list.Add(sub_cat);
      }
    }
    ViewBag.CategoryList=category_list;
    ViewBag.BrandList=brand_list;
    ViewBag.SubCatList=sub_cat_list;
  return View();
  }

  [Route("product_list/{id}/variant")]
  [HttpGet]
  public async Task<IActionResult> VariantList(int id)
  {
      string select_size="7";
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
         var variant_list=await this._product.pagingVariant(id,7,1);
         return View(variant_list);
  }

  [Route("product_list/{id}/variant/paging")]
  
  [HttpGet]
  public async Task<IActionResult> VariantListPaging(int id,int page_size,int page=1)
  { 
    
    Console.WriteLine("In Paging variant");
     var variant_list=await this._product.pagingVariant(id,page_size,page);
     List<string> options=new List<string>(){"7","10","20","50"};
          
     ViewBag.options=options;
        
          
     string select_size=page_size.ToString();
          
      ViewBag.select_size=select_size;
          
      return View("~/Views/ProductList/VariantList.cshtml",variant_list);
  }

  [Route("product_list/{id}/product_info")]
  
  [HttpGet]

  public async Task<IActionResult> ProductInfo(int id)
  {
    var category_list=await this._category.getAllCategory(); 
    
    var brand_list = await this._category.getAllBrandList();

    List<SubCategory> sub_cat_list=new List<SubCategory>();

    foreach(var cat in category_list)
    {
      foreach(var sub_cat in cat.SubCategories)
      {
        sub_cat_list.Add(sub_cat);
      }
    }
    ViewBag.CategoryList=category_list;
    ViewBag.BrandList=brand_list;
    ViewBag.SubCatList=sub_cat_list;
    var product=await this._product.findProductById(id);
    return View("~/Views/ProductList/ProductInfo.cshtml",product);
  }
 
 [Route("product_list/{id}/product_info")]
 [HttpPost]
 public async Task<IActionResult> ProductInfo(int id,AddProductModel product)
{
try
{
    int update_res=await this._product.updateProduct(id,product);
    if(update_res==0)
 {  
  ViewBag.Status=0;
  ViewBag.Updated_Product="Cập nhật sản phẩm thất bại";
 }
 else
 {
 ViewBag.Status=1;
 ViewBag.Updated_Product="Cập nhật sản phẩm thành công";
 }
 var product_ob=await this._product.findProductById(id);
 return View("~/Views/ProductList/ProductInfo.cshtml",product_ob);
}
catch(Exception er)
{
  this._logger.LogTrace("Update Product Info Exception:"+er.Message);
}
  return View();
 }
}