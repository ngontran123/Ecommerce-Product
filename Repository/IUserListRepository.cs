using System;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
namespace Ecommerce_Product.Repository;

public interface IUserListRepository
{
public Task<IEnumerable<ApplicationUser>> filterUserList(FilterUser user);

public Task<IEnumerable<ApplicationUser>> getAllUserList();

public Task<PageList<ApplicationUser>> pagingUser(int page_size,int page);

// public Task<IEnumerable<ApplicationUser>> getUserListByRole(string role);

// public Task<bool> checkUserRole(string email,string role);
// public Task<bool> checkUserExist(string email);
// public Task<bool> addUser(ApplicationUser user);
// public Task<bool> updateUser(ApplicationUser user);
// public Task<ApplicationUser> getUser(string email);
// public Task<bool> deleteUser(string email);

// public Task<bool> sendEmail(string email,string receiver,string subject);

}