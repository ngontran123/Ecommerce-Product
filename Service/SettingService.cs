using Ecommerce_Product.Repository;
using Ecommerce_Product.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Npgsql.Replication;
using System.Drawing;

namespace Ecommerce_Product.Service;

public class SettingService:ISettingRepository
{
    private readonly EcommerceShopContext _context;

    private readonly Support_Serive.Service _sp_services;
  public SettingService(EcommerceShopContext context,Support_Serive.Service sp_services)
  {
    this._context=context;
    this._sp_services=sp_services;
  }



public async Task<IEnumerable<Setting>> getAllSetting()
{
    var settings=this._context.Settings.ToList();

    return settings;
}


public async Task<int> updateSetting(SettingModel setting)
{  int updated_res=0;
    try
    {  
   
   string signup=setting.SignUp;
   string change_password=setting.ChangePassword;
   string recaptcha=setting.Recaptcha;
   string purchased = setting.Purchased;
   string cancelled = setting.Cancelled;
   string refund = setting.Refund;
   
   Console.WriteLine("SIGNUP:"+signup);
   Console.WriteLine("CHANGE PASSWORD:"+change_password);
   Console.WriteLine("PURCHASED:"+purchased);
   Console.WriteLine("CANCELLED:"+cancelled);
   Console.WriteLine("REFUND:"+refund);   

  

   int signup_val=string.IsNullOrEmpty(signup)?0:1;

   int change_password_val=string.IsNullOrEmpty(change_password)?0:1;

   int recaptcha_val=string.IsNullOrEmpty(recaptcha)?0:1;

   int purchased_val=string.IsNullOrEmpty(purchased)?0:1;

   int cancelled_val=string.IsNullOrEmpty(cancelled)?0:1;

   int refund_val=string.IsNullOrEmpty(refund)?0:1;


//    Console.WriteLine("singup_val:"+signup_val);
//    Console.WriteLine("change_password_val:"+change_password_val);
//    Console.WriteLine("two_fa_val:"+two_fa_val);
//    Console.WriteLine("purchased_val:"+purchased_val);
//    Console.WriteLine("cancelled_val:"+cancelled_val);
//    Console.WriteLine("refund_val:"+refund_val);

   Dictionary<string,int> setting_name=new Dictionary<string,int>{{"signup",signup_val},{"changepassword",change_password_val},{"recaptcha",recaptcha_val},{"purchased",purchased_val},{"cancelled",cancelled_val},{"refund",refund_val}};
  
  for(int i=0;i<setting_name.Count;i++)
  { //Console.WriteLine("Settingname:"+setting_name.Keys.ElementAt(i));    
     var setting_obs=await this._context.Settings.FirstOrDefaultAsync(s=>s.Settingname==setting_name.Keys.ElementAt(i));
         
         if(setting_obs!=null)
         {   //Console.WriteLine(setting_obs.Settingname);
            if(setting_name.Values.ElementAt(i)==1)
            {
            string updatetime=DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss");
            setting_obs.Status=1;
            setting_obs.Updateddate=updatetime;
            this._context.Settings.Update(setting_obs);
            }
            else
            {
            string updatetime=DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss");
            setting_obs.Status=0;
            setting_obs.Updateddate=updatetime;
            this._context.Settings.Update(setting_obs);
            }
         }
  
  }
  updated_res=1;
  await this.saveChanges();
    }
    catch(Exception e)
    {   updated_res=0;
        Console.WriteLine("Update Setting Exception:"+e.Message);
    }
    return updated_res;
}

   public async Task<int> getStatusByName(string name)
   {
     int status=-1;
     var setting=await this._context.Settings.FirstOrDefaultAsync(s=>s.Settingname==name);
     if(setting!=null)
     {
        status=setting.Status;
     }
     return status;

   }
   

   
  public async Task saveChanges()
  {
    await this._context.SaveChangesAsync();
  }



}