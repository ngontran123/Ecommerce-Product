using Ecommerce_Product.Repository;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ecommerce_Product.Support_Serive;
using OfficeOpenXml;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.Text;
namespace Ecommerce_Product.Service;
public class AdminListService:IAdminRepository
{

    private readonly UserManager<ApplicationUser> _userManager;

    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly Support_Serive.Service _support_service;

    private readonly SmtpService _smtpService;

    private readonly ILogger<LoginService> _logger;

    public AdminListService(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager,Support_Serive.Service service,SmtpService smtpService,ILogger<LoginService> logger)
    {
        this._userManager=userManager;
        this._roleManager=roleManager;
        this._support_service=service;
        this._smtpService=smtpService;
        this._logger=logger;
    }

    public async Task<IEnumerable<ApplicationUser>> filterUserList(FilterUser user)
    {
    string username=user.UserName;
    string email=user.Email;
    string phonenumber=user.PhoneNumber;
    string datetime=user.DateTime;
    var users=this._userManager.Users.AsQueryable();
    if(!string.IsNullOrEmpty(username))
    {
        users=users.Where(u=>u.UserName==username);
    }
    if(!string.IsNullOrEmpty(email))
    {
        users=users.Where(u=>u.Email==email);
    }
    if(!string.IsNullOrEmpty(phonenumber))
    {
        users=users.Where(u=>u.PhoneNumber==phonenumber);
    }
    if(!string.IsNullOrEmpty(datetime))
    {
        users=users.Where(u=>u.Created_Date==datetime);
    }
    return await users.ToListAsync();
    }

    public async Task<IEnumerable<ApplicationUser>> getAllUserList()
    {
      string role="Admin";
      var users=this._userManager.Users.ToList();
      List<ApplicationUser> userList=new List<ApplicationUser>();
      foreach(var user in users)
      {
        if(await this._userManager.IsInRoleAsync(user,role))
        {
            userList.Add(user);
        }
      }
      return userList;
    }

   public async Task<PageList<ApplicationUser>> pagingUser(int page_size,int page)
   { 

   IEnumerable<ApplicationUser> all_user= await this.getAllUserList();

   List<ApplicationUser> users=all_user.OrderByDescending(u=>u.Seq).ToList(); 

   //var users=this._userManager.Users;
   
   var user_list=PageList<ApplicationUser>.CreateItem(users.AsQueryable(),page,page_size);
   
   return user_list;
   }

   
public async Task<bool> checkUserExist(string email,string username)
{
    bool res=false;
    var check_user_email_exist=await this._userManager.FindByEmailAsync(email);
    var check_user_name_exist = await this._userManager.FindByNameAsync(username);
    if(check_user_email_exist!=null || check_user_name_exist!=null)
    {
        res=true;
    }
    return res;
}

   public async Task<int> createUser(Register user)
   { 
     int res_created=0;

    bool is_existed=await checkUserExist(user.Email,user.UserName);

    if(is_existed)
    {   res_created=-1;
        return res_created;
    }
     var users=this._userManager.Users;
     int seq=1;
      var latestUser = await users
            .OrderByDescending(u => u.Seq)  
            .FirstOrDefaultAsync();
    if(latestUser!=null)
    {
      seq=(latestUser.Seq??0)+1;
    }
     string role = "Admin";
     string created_date=DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
     var new_user=new ApplicationUser{UserName = user.UserName,Email=user.Email,Address1=user.Address1,Address2=user.Address2,Gender=user.Gender,PhoneNumber=user.PhoneNumber,Created_Date=created_date,Seq=seq,Avatar="https://cdn-icons-png.flaticon.com/128/3135/3135715.png"};
     var res=await this._userManager.CreateAsync(new_user,user.Password);
     if(res.Succeeded)
     {  
        await this._userManager.AddToRoleAsync(new_user,role);
        res_created=1;
     }
      foreach (var error in res.Errors)
    {
        Console.WriteLine(error.Description);
    }
    //  else{
    //     foreach(var err in res.Errors)
    //     {
    //         Console.WriteLine(err.Description);
    //     }
    //  }
    

     return res_created;
   }
  
  public async Task<ApplicationUser> findUserByEmail(string email)
  {
    var user=await this._userManager.FindByEmailAsync(email);
    return user;
  }

  public async Task<ApplicationUser> findUserById(string id)
  {
     var user=await this._userManager.FindByIdAsync(id);
    return user;
  }

public async Task<int> updateUser(UserInfo user_info)
{
 int res=0;
 string user_id=user_info.Id;
 if(string.IsNullOrEmpty(user_id))
 {
    return res;
 }
 var user=await this.findUserById(user_id);
 if(user!=null)
 {
   user.UserName=user_info.UserName;
   user.Email=user_info.Email;
   user.PhoneNumber=user_info.PhoneNumber;
   user.Address1=user_info.Address1;
   user.Address2=user_info.Address2;
   user.Gender=user_info.Gender;
   Console.WriteLine("Phone number for this user:"+user.PhoneNumber);
   var res_update= await this._userManager.UpdateAsync(user);
   if(res_update.Succeeded)
   {
    res=1;
   }
   else{
    foreach(var err in res_update.Errors)
    {
        Console.WriteLine("Update User error:"+err.Description);
    }
   }
 }
 return res;
}

public async Task<int> deleteUser(string email)
{
  int res=0;
  if(string.IsNullOrEmpty(email))
  {
    return res;
  }
  var user=await this.findUserByEmail(email);
  if(user!=null)
  {
    var delete_user=await this._userManager.DeleteAsync(user);
    if(delete_user.Succeeded)
    {
        res=1;
    }
    else{

    Console.WriteLine("delete user failed");
        foreach(var err in delete_user.Errors)
        {
            Console.WriteLine("Delete User error:"+err.Description);
        }
    }
  }
  return res;
}


public async Task<int> changeUserPassword(string email)
{
    int res=0;
    if(string.IsNullOrEmpty(email))
    {
        return res;
    }
    var user=await this.findUserByEmail(email);    
    if(user!=null)
    {
        string token = await this._userManager.GeneratePasswordResetTokenAsync(user);
        string new_password = "Ecommerce123@";
        var reset_password=await this._userManager.ResetPasswordAsync(user,token,new_password);
        if(reset_password.Succeeded)
        {
            res=1;
        }
        else
        {
            foreach(var err in reset_password.Errors)
            {
                Console.WriteLine("Change Password Exception:"+err.Description);                
            }
        }
    }
    return res;
}

 public async Task<MemoryStream> exportToExcel()
 {
  using(ExcelPackage excel = new ExcelPackage())
  {
    var worksheet=excel.Workbook.Worksheets.Add("User");
    worksheet.Cells[1,1].Value="STT";
    worksheet.Cells[1,2].Value="Tên User";
    worksheet.Cells[1,3].Value = "Email";
    worksheet.Cells[1,4].Value="Giới tính";
    worksheet.Cells[1,5].Value="Số điện thoại";
    worksheet.Cells[1,6].Value="Địa chỉ 1";
    worksheet.Cells[1,7].Value="Địa chỉ 2";
    worksheet.Cells[1,8].Value="Ngày tạo";
    var user=await this.getAllUserList();
    if(user!=null)
    {
Console.WriteLine("this user list is not null");
    List<ApplicationUser> list_user=user.ToList();
    Console.WriteLine("Length of user list here is:"+list_user.Count);
    for(int i=0;i<list_user.Count;i++)
    {
    worksheet.Cells[i+2,1].Value=(i+1).ToString();
    
    worksheet.Cells[i+2,2].Value=list_user[i].UserName;
    
    worksheet.Cells[i+2,3].Value=list_user[i].Email;
    
    worksheet.Cells[i+2,4].Value=list_user[i].Gender;

     worksheet.Cells[i+2,5].Value=list_user[i].PhoneNumber;

     worksheet.Cells[i+2,6].Value=list_user[i].Address1;

     worksheet.Cells[i+2,7].Value=list_user[i].Address2;

     worksheet.Cells[i+2,8].Value=list_user[i].Created_Date;
    Console.WriteLine("UserName:"+list_user[i].UserName);
        Console.WriteLine("UserName:"+list_user[i].Email);

    Console.WriteLine("UserName:"+list_user[i].PhoneNumber);

    Console.WriteLine("UserName:"+list_user[i].Gender);
    }    
   }
  var stream = new MemoryStream();
  excel.SaveAs(stream);
  stream.Position=0;
  Console.WriteLine("content here is:"+stream);
  return stream;
  }
 }

 public async Task<byte[]> exportToPDF()
 { 

 MemoryStream ms=new MemoryStream();
 try{
  using(PdfWriter writer=new PdfWriter(ms))
  {
    PdfDocument pdfDoc=new PdfDocument(writer);
    Document dc=new Document(pdfDoc);
    dc.Add(new Paragraph("User List").SetFontSize(20).SetBold());
    Table table = new Table(8);
    table.AddCell("STT");
    table.AddCell("Tên User");
    table.AddCell("Email");
    table.AddCell("Giới tính");
    table.AddCell("Số điện thoại");
    table.AddCell("Địa chỉ 1");
    table.AddCell("Địa chỉ 2");
    table.AddCell("Ngày tạo");
    var users=await this.getAllUserList();
  if(users!=null)
  {
    List<ApplicationUser> list_user = users.ToList();
    Console.WriteLine("User count:"+list_user.Count);
    int count_user=0;
    foreach(var user in list_user)
    {   count_user+=1;
         table.AddCell(count_user.ToString());
         table.AddCell(user.UserName);
         table.AddCell(user.Email);
         table.AddCell(user.Gender);
         table.AddCell(user.PhoneNumber);
         table.AddCell(user.Address1);
         table.AddCell(user.Address2);
         table.AddCell(user.Created_Date);
    }
  }
   dc.Add(table);
   dc.Close();
  }
 }
 catch(Exception er)
 {
    Console.WriteLine("PDF Exception:"+er.Message);
 }
   ms.Position=0;
  byte[] content = ms.ToArray();
  return content;
 }
  
public async Task<byte[]> exportToCSV()
{
  StringBuilder csv=new StringBuilder();

  

  csv.AppendLine("STT,Tên User,Email,Giới tính,Số điện thoại,Địa chỉ 1,Địa chỉ 2,Ngày tạo");
  var users=await this.getAllUserList();
  if(users!=null)
  {
    int count_user=0;
    List<ApplicationUser> list_user= users.ToList();
    foreach(var user in list_user)
    {
     count_user+=1;
    
     csv.AppendLine($"{count_user},{user.UserName},{user.Email},{user.Gender},{user.PhoneNumber},{user.Address1},{user.Address2},{user.Created_Date}");
    }
  }
   byte[] bytes=Encoding.UTF8.GetBytes(csv.ToString());

    var bom = Encoding.UTF8.GetPreamble();
    var fileBytes = new byte[bom.Length + bytes.Length];
    System.Buffer.BlockCopy(bom, 0, fileBytes, 0, bom.Length);
    System.Buffer.BlockCopy(bytes, 0, fileBytes, bom.Length, bytes.Length);
   return bytes;
}
 }