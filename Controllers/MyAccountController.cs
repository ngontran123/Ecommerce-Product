using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ecommerce_Product.Service;
using Ecommerce_Product.Repository;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using reCAPTCHA.AspNetCore;
using Microsoft.Extensions.Options;

namespace Ecommerce_Product.Controllers
{ 
    public class MyAccountController : BaseController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ITrackDataRepository _trackData;

        private readonly ILoginRepository _loginRepos;

        private readonly IUserListRepository _userList;

        private readonly IRecaptchaService _recaptcha;

        private readonly RecaptchaResponse _recaptcha_response;

        private readonly ICategoryListRepository _category;

        private readonly ISettingRepository _setting;

        private readonly ILogger<MyAccountController> _logger;

        public MyAccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,IUserListRepository userList,ICategoryListRepository category,ILogger<MyAccountController> logger,IRecaptchaService recaptcha,IOptions<RecaptchaResponse> recaptcha_response,ISettingRepository setting,ITrackDataRepository trackData,ILoginRepository loginRepos):base(category)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger=logger;
            _loginRepos= loginRepos;
            _recaptcha=recaptcha;
            _setting=setting;
            _userList=userList;
            _category=category;
            _recaptcha_response=recaptcha_response.Value;
            _trackData=trackData;
        }

        [Route("login")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> MyAccount()
        {   
        //     if(User.Identity.IsAuthenticated)
        // {
        //     return RedirectToAction("Dashboard","Dashboard");            
        // } 

        if(Request.Cookies["VisitorCounted"]==null)
        {
       int total_visitor=await this._trackData.getCurrentVisitedCount();
       total_visitor+=1;
        int updated_res= await this._trackData.updateCurrentVisitedCount(total_visitor);
        CookieOptions options = new CookieOptions{
        Expires=DateTime.Now.AddYears(1),
        IsEssential=true,
        HttpOnly=true
        };
        Response.Cookies.Append("VisitorCounted","true",options);
        }

       int setting_status=await this._setting.getStatusByName("recaptcha");
       if(setting_status==1)
       {
        ViewBag.SiteKey=this._recaptcha_response.SiteKey;
       }
    return View("~/Views/ClientSide/MyAccount/MyAccount.cshtml");
        }

        [Route("change_password")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ChangePassword()
        {  Console.WriteLine("just come here");
            return View("~/Views/ClientSide/MyAccount/ChangePassword.cshtml");
        }
        
        [Route("change_password/{username}")]
        [HttpGet]
        public IActionResult ChangePassword([FromQuery]string username,[FromQuery]string email,[FromQuery]string password)
        {
            ViewBag.Email=email;
            ViewBag.Password= password;
            Console.WriteLine("Email did pass here:"+email);
            return View("~/Views/ClientSide/MyAccount/ChangePassword.cshtml");
        }

    
  [HttpPost]
  public async Task<JsonResult> LoginHandle(LoginModel model)
  {      
    StatusResponse response=new StatusResponse();

    try
    {
    _logger.LogInformation("Running in Login Action"); 
      
      string username=model.UserName.Trim();
      
      string password = model.Password.Trim();
      
      bool is_remember_me= model.RememberMe;

      Console.WriteLine("is remember me:"+is_remember_me);

      
      var admin_user=await this._loginRepos.getUserByUsername(username);
      int setting_status=await this._setting.getStatusByName("recaptcha");
    if(setting_status==1)
       {
        ViewBag.SiteKey=this._recaptcha_response.SiteKey;
       }

            if(admin_user!=null)
            {   string email=admin_user.Email;
             
                bool check_is_user=await this._loginRepos.checkUserRole(email,"User");
                Console.WriteLine("check is admin:"+check_is_user);
                Console.WriteLine("password here is:"+password);
            if(check_is_user)
            {  
                var result = await _signInManager.PasswordSignInAsync(username,password,is_remember_me,lockoutOnFailure: false);

                if(!result.Succeeded)
                {   Console.WriteLine("result here is:"+result.ToString());
                   response=new StatusResponse{
                          Status=0,
                          Title="Đăng nhập",
                          Message="Mật khẩu không chính xác",
                        SiteKey=this._recaptcha_response.SiteKey

                   };
            
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            else
            {
             if(setting_status==1)
             {
             var recapchaResult = await this._recaptcha.Validate(Request);
             if(!recapchaResult.success)
             {
            response=new StatusResponse{
                            Status=0,
                            Title="Đăng nhập",
                            Message="Hãy chọn captcha để chứng minh bạn ko phải robot.",
                            SiteKey=this._recaptcha_response.SiteKey
                    };
     
         return Json(response);
             }
             }
              HttpContext.Session.SetString("UserId",admin_user.Id);
              HttpContext.Session.SetString("Username",admin_user.UserName);
              HttpContext.Session.SetString("Email",admin_user.Email);
              HttpContext.Session.SetString("Password",password);
              HttpContext.Session.SetString("UserSession", "Active");
              HttpContext.Session.SetString("Avatar",admin_user.Avatar);
                response=new StatusResponse{
                            Status=1,
                            Title="Đăng nhập",
                            Message="Đăng nhập thành công"
                     };
            }
            }
            }
            else
            {   Console.WriteLine("User is null");
                    response=new StatusResponse{
                            Status=0,
                            Title="Đăng nhập",
                            Message="Username không chính xác",
                            SiteKey=this._recaptcha_response.SiteKey
                    };
                //   TempData["LoginFailed"]="True";
                //   TempData["ErrorContent"]="Username không chính xác";
                  ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
        
        
        }
        catch(Exception er)
        {
            _logger.LogTrace("Login Exception:"+er.Message);
            Console.WriteLine("Login Exception:"+er.Message);
        }
        return Json(response);
  }

  [HttpPost]
  public async Task<JsonResult> RegisterHandle(Register model)
  { 
    StatusResponse response = new StatusResponse();
    Console.WriteLine("did come to register here");
    try 
    {   int setting_status=await this._setting.getStatusByName("recaptcha");
      
        Console.WriteLine("Setting status:"+setting_status);   
        if(setting_status==1)
             {
             var recapchaResult = await this._recaptcha.Validate(Request);
             if(!recapchaResult.success)
             {Console.WriteLine("Captcha is not valid");
            response=new StatusResponse{
                            Status=0,
                            Title="Đăng ký",
                            Message="Hãy chọn captcha để chứng minh bạn ko phải robot.",
                    };
            Console.WriteLine("Captcha is not valid here");
         return Json(response);
             }
             }
        //     string email=model.Email.Trim();
        //     string username=model.UserName.Trim();
        //     string password=model.Password.Trim();
        //     string phone=model.PhoneNumber.Trim();
        //     string address1=model.Address1.Trim();
        //     string address2=model.Address2.Trim();
        //     string gender=model.Gender;
        //     Console.WriteLine("gender:"+gender);
        //     string avatar="https://cdn-icons-png.flaticon.com/128/3135/3135715.png";
        //  var newNormalUser = new ApplicationUser{UserName = username,Email=email,Address1=address1,Address2=address2,Gender=gender,PhoneNumber=phone,Avatar=avatar};
            int createUser = await this._userList.createUser(model);
            if(createUser==1)
            { 
               
                response=new StatusResponse{
                    Status=1,
                    Title="Đăng ký",
                    Message="Đăng ký thành công",
                SiteKey=this._recaptcha_response.SiteKey

                };
            }
            else{
                   response=new StatusResponse{
                    Status=0,
                    Title="Đăng ký",
                    Message="Đăng ký thất bại",
                SiteKey=this._recaptcha_response.SiteKey

                };
            }

    }
    catch(Exception er)
    {
        Console.WriteLine("Register Exception:"+er.Message);
        this._logger.LogTrace("Register Exception:"+er.Message);
    }
    return Json(response);
  }
     
    //     [Route("login")]
    //     [HttpPost]
    //    [AllowAnonymous]
    //     public async Task<IActionResult> Index(LoginModel model)
    //     {  
    //     try
    //     {
   
    // _logger.LogInformation("Running in Login Action"); 
      
    //   string username=model.UserName;
      
    //   string password = model.Password;
      
    //   bool is_remember_me= model.RememberMe;

    //   Console.WriteLine("is remember me:"+is_remember_me);
      
    //   var admin_user=await this._loginRepos.getUserByUsername(username);
    //   int setting_status=await this._setting.getStatusByName("recaptcha");
    // if(setting_status==1)
    //    {
    //     ViewBag.SiteKey=this._recaptcha_response.SiteKey;
    //    }


    // // if(normalUser==null)
    // // {   Console.WriteLine("normal user here");
    //     //var newNormalUser = new ApplicationUser{UserName = normalEmail,Email=normalEmail,Address1="here",Address2="there",Gender="Male"};
    // //     var createUser = await _userManager.CreateAsync(newNormalUser,normalPassword);
    // //     if(createUser.Succeeded)
    // //     { Console.WriteLine("It used to be in here");
    // //         await _userManager.AddToRoleAsync(newNormalUser,"User");
    // //     }
    // //     else{
    // //         foreach (var error in createUser.Errors)
    // //         {
    // //             ModelState.AddModelError(string.Empty, error.Description);
    // //             Console.WriteLine(error.Description);
    // //             this._logger.LogDebug($"Created User:{error.Code}.{error.Description}");
    // //         }
    // //     }
    // // }  
    //         if(admin_user!=null)
    //         {   string email=admin_user.Email;
             
    //             bool check_is_admin=await this._loginRepos.checkUserRole(email,"Admin");
    //             Console.WriteLine("check is admin:"+check_is_admin);
    //             Console.WriteLine("password here is:"+password);
    //         if(check_is_admin)
    //         {  
    //             var result = await _signInManager.PasswordSignInAsync(username,password,is_remember_me,lockoutOnFailure: false);

    //             if(!result.Succeeded)
    //             {   Console.WriteLine("result here is:"+result.ToString());
    //                 TempData["LoginFailed"]="True";
    //                 TempData["ErrorContent"]="Mật khẩu không chính xác";
             
    //                 ModelState.AddModelError(string.Empty, "Invalid login attempt.");
    //             }
    //         else
    //         {
    //          if(setting_status==1)
    //          {
    //          var recapchaResult = await this._recaptcha.Validate(Request);
    //          if(!recapchaResult.success)
    //          {
    //      TempData["LoginFailed"]="True";
    //      TempData["ErrorContent"]="Hãy chọn captcha để chứng minh bạn ko phải robot.";   
    //      ViewBag.SiteKey=this._recaptcha_response.SiteKey;
    //      return View(model);
    //          }
    //          }
    //           HttpContext.Session.SetString("UserId",admin_user.Id);
    //           HttpContext.Session.SetString("Username",admin_user.UserName);
    //           HttpContext.Session.SetString("Email",admin_user.Email);
    //           HttpContext.Session.SetString("Password",password);
    //           HttpContext.Session.SetString("UserSession", "Active");
    //           HttpContext.Session.SetString("Avatar",admin_user.Avatar);
    //           return RedirectToAction("Dashboard","Dashboard");
    //         }
    //         }
    //     else
    //     {          Console.WriteLine("Not admin");
    //                TempData["LoginFailed"]="True";
    //                TempData["ErrorContent"]="Tài khoản này không có quyền admin"; 
    //                 ModelState.AddModelError(string.Empty, "Invalid login attempt.");
    //     }
    //         }
    //         else
    //         {   Console.WriteLine("User is null");
    //               TempData["LoginFailed"]="True";
    //               TempData["ErrorContent"]="Username không chính xác";
    //               ModelState.AddModelError(string.Empty, "Invalid login attempt.");
    //         }
        
        
    //     }
    //     catch(Exception er)
    //     {
    //         _logger.LogTrace("Login Exception:"+er.Message);
    //         Console.WriteLine("Login Exception:"+er.Message);
    //     }
    //         return View(model);
    //     }
    

    [Route("forgot_password")]
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
     return View("~/Views/ClientSide/MyAccount/ForgotPassword.cshtml");
    }

    //[Route("change_password_handle")]
[HttpPost]
    public async Task<JsonResult> ChangePasswordHandle(string email,string password,string new_password)
    {  

        StatusResponse response=new StatusResponse();
        try{
         var user=await this._userManager.FindByEmailAsync(email);
         if(user!=null)
         {
            var change_password=await this._userManager.ChangePasswordAsync(user,password ,new_password);
            if(change_password.Succeeded)
            {
                response=new StatusResponse{
                    Status=1,
                    Title="Đổi mật khẩu",
                    Message="Đã đổi mật khẩu thành công"
                };
            }
            else
            { 
                response=new StatusResponse{
                    Status=0,
                    Title="Đổi mật khẩu",
                    Message="Mật khẩu hiện tại của bạn không đúng"
                };
            }
         }
         else
         {
            response=new StatusResponse{
                Status=0,
                Title="Đổi mật khẩu",
                Message="Email của bạn không đúng"
            };
         }           
        
        }
        catch(Exception er)
        {
            this._logger.LogTrace("Change Password Exception:"+er.Message);
            Console.WriteLine("Change Password Exception:",er.Message);
        }
        return Json(response);
    }
        // [Route("change_password")]
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> ChangePassword(ChangePassword model)
        // {
        //  if(ModelState.IsValid)
        //  {
        //   string email=model.Email;
        //   string curr_password= model.Password;
        //   string new_password = model.New_Password;
       
        //   var user=await this._userManager.FindByEmailAsync(email);
        //   if(user!=null)
        //   {
        //      var change_password=await this._userManager.ChangePasswordAsync(user,curr_password,new_password);
        //      if(change_password.Succeeded)
        //      {
        //         TempData["ChangePassword"]="True";
        //         TempData["ChangePasswordContent"] ="Đã đổi mật khẩu thành công"; 
        //       return RedirectToAction("Index");
        //      }
        //      else
        //      { 
        //         ViewBag.ChangePassword="False";
        //         ViewBag.ErrorContent="Mật khẩu hiện tại của bạn không đúng";
        //      foreach(var error in change_password.Errors)
        //      {
        //         this._logger.LogTrace("Change Password Errors:"+error.Description);
        //      }
        //      }
        //   }
        //   else
        //   {
        //      ViewBag.ChangePassword="False";
        //      ViewBag.ErrorContent="Email của bạn không đúng";
        //      this._logger.LogTrace("Change Password Errors:Email is incorrect");
        //   }           
        //  }
        //  return View(model);
        // }

[HttpPost]
public async Task<JsonResult> ForgotPasswordHandle(string email)
{
    StatusResponse response=new StatusResponse();
    try{
   
        
            Console.WriteLine("Received email:"+email);
            string subject="Nhận mật khẩu mới";               
           bool is_send= await this._loginRepos.sendEmail(email,email,subject,0);
           if(is_send)
           {
            response=new StatusResponse{
                Status=1,
                Title="Quên mật khẩu",
                Message="Tin nhắn khôi phục mật khẩu đã được gửi đến email của bạn"
            };
           }
           else{
            response=new StatusResponse{
                Status=0,
                Title="Quên mật khẩu",
                Message="Có lỗi xảy ra trong quá trình gửi tin nhắn"
            };
           }
        
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Forgot Password:"+er.Message);
        Console.WriteLine("Forgot password:",er.Message);
    }
    return Json(response);
}

    [Route("{username}/info")]
    [HttpGet]
    public async Task<IActionResult> AccountInfo(string username)
    {   Console.WriteLine("Username here is:"+username);
        var user = await this._userList.findUserByName(username);
        
        return View("~/Views/ClientSide/MyAccount/AccountInfo.cshtml",user);
    }

[HttpPost]
[Route("user/update_info")]
public async Task<IActionResult> AccountInfoUpdate(UserInfo user)
{ int res_update=0;
Console.WriteLine("Update user info did come to this place");
  try
  {

    res_update=await this._userList.updateUser(user);

    Console.WriteLine("Update User info:"+res_update);
    
    if(res_update==1)
    { 
      ViewBag.Status=1;
      ViewBag.Update_Message="Cập nhật User thành công";
    }
    else
    {
      ViewBag.Status=0;
      ViewBag.Update_Message="Cập nhật User thất bại";
    }
   
    var user_after=await this._userList.findUserById(user.Id);
  if(!string.IsNullOrEmpty(user_after.Avatar))
  {
    this.HttpContext.Session.SetString("Avatar",user_after.Avatar);
  }


        return View("~/Views/ClientSide/MyAccount/AccountInfo.cshtml",user_after);

  }
  catch(Exception er)
  {
     Console.WriteLine("Update User Info Exception:"+er.InnerException?.Message??er.Message);
     this._logger.LogTrace("Update User Info Exception:"+er.InnerException?.Message??er.Message); 
  }
  return RedirectToAction("UserList","UserList");
} 

        // POST: /Account/Logout
        [Route("logout")]
        [HttpGet]  
        public async Task<IActionResult> Logout()
        {   
            await _signInManager.SignOutAsync();
            this.HttpContext.Session.Clear();
            return RedirectToAction("HomePage","HomePage");
        }
         
 
    }
}