using Ecommerce_Product.Repository;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ecommerce_Product.Support_Serive;
using Microsoft.VisualBasic;
namespace Ecommerce_Product.Service;
public class LoginService:ILoginRepository
{

    private readonly UserManager<ApplicationUser> _userManager;

    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly Support_Serive.Service _support_service;

    private readonly SmtpService _smtpService;

    private readonly ILogger<LoginService> _logger;

    public LoginService(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager,Support_Serive.Service service,SmtpService smtpService,ILogger<LoginService> logger)
    {
        this._userManager=userManager;
        this._roleManager=roleManager;
        this._support_service=service;
        this._smtpService=smtpService;
        this._logger=logger;
    }

    public async Task<IEnumerable<ApplicationUser>> getUserList()
    {
        var list_users= await this._userManager.Users.ToListAsync();
        return list_users;
    }

    public async Task<IEnumerable<ApplicationUser>>getUserListByRole(string role)
    {
      if(await this._roleManager.RoleExistsAsync(role))
      {
        var list_user=await this._userManager.GetUsersInRoleAsync(role);
        return list_user;
      }
      else
      {
       return new List<ApplicationUser>();
      }
    }
  
  public async Task<bool> checkUserRole(string email,string role)
  { 
    var is_role=false;
    var user=await this._userManager.FindByEmailAsync(email);
    if(user!=null)
    {
      bool role_user=await this._userManager.IsInRoleAsync(user,role);
      if(role_user)
      {
        is_role=true;
        return is_role;
      }
    }
    return is_role;
  }

    public async Task<ApplicationUser> getUser(string email)
    {
        var user=await this._userManager.FindByEmailAsync(email);
        return user;
    }
    public async Task<bool> checkUserExist(string email)
    { 
      bool exist=false;
      var user=await this._userManager.FindByEmailAsync(email);
      if(user!=null)
      {
         exist=true;
         return exist;
      }
      return exist;
    } 

    public async Task<bool> addUser(ApplicationUser user)
    {   bool is_created=false;
        string email=user.Email;
        var user_created=await this._userManager.FindByEmailAsync(email);
        if(user_created==null)
        {
            await this._userManager.CreateAsync(user);
            is_created=true;
            return is_created;
        }
        return is_created;
    }

    public async Task<bool> updateUser(ApplicationUser user)
    {
        bool is_update=false;

        string email = user.Email;
        
        var user_created=await this._userManager.FindByEmailAsync(email);
        
        if(user_created!=null)
        {
      user_created.Email=user.Email;
      user_created.PhoneNumber=user.PhoneNumber;
      user_created.Address1=user.Address1;
      user_created.Address2=user.Address2;
      user_created.Gender=user.Gender;
      user_created.UserName=user.UserName;
      var is_updated=await this._userManager.UpdateAsync(user_created);
     if(is_updated.Succeeded)
     {
      is_update=true;
      return is_update;
     }
        }
      return is_update;
    }

    public async Task<bool> deleteUser(string email)
    {
        bool is_delete=false;
        var user_created=await this._userManager.FindByEmailAsync(email);
        if(user_created!=null)
        {
            var is_deleted=await this._userManager.DeleteAsync(user_created);
            if(is_deleted.Succeeded)
            {
                is_delete=true;
                return is_delete;
            }
        }
        return is_delete;
    }

    public async Task<bool> sendEmail(string email,string receiver,string subject)
    { 
        bool is_send= false;
    try{
        var user = await this._userManager.FindByEmailAsync(email);
        if(user!=null)
        {
        string new_password=this._support_service.generateRandomPassword();        
        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        Console.WriteLine("new-password"+new_password);
        Console.WriteLine("token:"+code);
        Console.WriteLine(user.Email);

       var is_reset= await this._userManager.ResetPasswordAsync(user,code,new_password);
       
       if(is_reset.Succeeded)
       {
         await this._smtpService.sendEmail(new_password,receiver,subject);
        is_send=true;
        return is_send;
       }
       else{
        foreach(var er in is_reset.Errors)
        {   this._logger.LogTrace("Reset Password Exception:"+er.Description);
            Console.WriteLine("reset password error:"+er.Description);
        }
       }
       
        }
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Send Email Exception:"+er.Message);
    }
        return is_send;
    }
 }