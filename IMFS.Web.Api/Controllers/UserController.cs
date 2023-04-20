using IMFS.BusinessLogic.RoleManagement;
using IMFS.BusinessLogic.UserManagement;
using IMFS.Web.Models.User;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace IMFS.Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : BaseController
    {
        private readonly IUserManager _userManager;
        private readonly IRoleManager _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(IUserManager userManager,
             IRoleManager roleManager,
                  IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("CheckUserStatus")]
        [HttpGet]
        public IActionResult CheckUserStatus()
        {
            try
            {
                var claims = HttpContext.User.Claims;
                var currentUserEmail = claims.FirstOrDefault(x => x.Type == "email")?.Value.ToLower();
                var currentUserName = claims.FirstOrDefault(x => x.Type == "preferred_username")?.Value.ToLower();
                var existingUser = _userManager.GetUserDetailsByUserName(currentUserName);
                if (existingUser == null)
                {
                    var firstName = claims.FirstOrDefault(x => x.Type == "firstName")?.Value;
                    var lastName = claims.FirstOrDefault(x => x.Type == "lastName")?.Value;
                    var customerNumber = claims.FirstOrDefault(x => x.Type == "imResellerId")?.Value;
                    var companyName = claims.FirstOrDefault(x => x.Type == "companyName")?.Value;
                    var phone = claims.FirstOrDefault(x => x.Type == "phone")?.Value;

                    IMFSIdentityUser newUser = new IMFSIdentityUser
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        UserName = currentUserName,
                        Email = currentUserEmail,
                        PhoneNumber = phone,
                        CustomerNumber = customerNumber,
                        CustomerName = companyName,
                        Active = true,
                        DefaultFinanceType = 1,
                        DefaultFinanceDuration = 3,
                        DefaultFinanceFrequency = "Monthly",
                        ExcludeGST = true
                    };
                    var userResult = _userManager.CreateNewUser(newUser);

                    if (!userResult.HasError)
                    {
                        var standardRole = "ResellerStandard";
                        if (currentUserEmail.Contains("@ingrammicro.com"))
                        {
                            standardRole = "IMStaff";
                        }

                        var roleResult = _roleManager.AddUserToRole(newUser.Id, standardRole);
                        if (!roleResult.HasError)
                        {
                            string webRootPath = _webHostEnvironment.ContentRootPath;
                            var templateFolderPath = Path.Combine(webRootPath, "Templates\\Emails");
                            _userManager.SendNewUserEmail(templateFolderPath, newUser);
                        }
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }

        }

        [Route("GetCurrentUserInfo")]
        [HttpGet]
        public IActionResult GetCurrentUserInfo()
        {
            try
            {
                var claims = HttpContext.User.Claims;
                var userName = claims.FirstOrDefault(x => x.Type == "preferred_username")?.Value.ToLower();
                var userId = claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
                var firstName = claims.FirstOrDefault(x => x.Type == "firstName")?.Value;
                var lastName = claims.FirstOrDefault(x => x.Type == "lastName")?.Value;
                var email = claims.FirstOrDefault(x => x.Type == "email")?.Value.ToLower();
                var customerNumber = claims.FirstOrDefault(x => x.Type == "imResellerId")?.Value;
                var companyName = claims.FirstOrDefault(x => x.Type == "companyName")?.Value;
                var phone = claims.FirstOrDefault(x => x.Type == "phone")?.Value;
                var role = claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;

                var currentUserInfo = new CurrentUserInfo()
                {
                    UserId = userId,
                    UserName = userName,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    CustomerNumber = customerNumber,
                    CompanyName = companyName,
                    Phone = phone,
                    Role = role
                };

                return Ok(currentUserInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }

        }

        [Route("GetAllUsers")]
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _userManager.GetUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }

        }


        [Route("GetUserDetails")]
        [HttpGet]
        public IActionResult GetUserDetails(string userId)
        {
            try
            {
                var users = _userManager.GetUserDetails(userId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }

        }


        [Route("ActivateUser")]
        [HttpGet]
        public IActionResult ActivateUser(string userId)
        {
            try
            {
                var response = _userManager.ActivateUserStatus(userId);
                if (response.HasError)
                {
                    return BadRequest(new { status = "Error", message = response.ErrorMessage });
                }
                else
                {
                    return Ok(new { status = "Success", message = "User information updated successfully" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }

        }

        [Route("DeactivateUser")]
        [HttpGet]
        public IActionResult DeactivateUser(string userId)
        {
            try
            {
                var response = _userManager.DeactivateUserStatus(userId);
                if (response.HasError)
                {
                    return BadRequest(new { status = "Error", message = response.ErrorMessage });
                }
                else
                {
                    return Ok(new { status = "Success", message = "User information updated successfully" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }

        [Route("SaveUser")]
        [HttpPost]
        public IActionResult SaveUser(UserModel userModel)
        {
            try
            {
                var response = _userManager.UpdateUserStatus(userModel);
                if (response.HasError)
                {
                    return Ok(new { status = "Error", message = response.ErrorMessage });
                }
                else
                {
                    return Ok(new { status = "Success", message = "User information updated successfully" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }

        [Route("SearchUser")]
        [HttpPost]
        public IActionResult SearchUser(UserSearchModel userSearchModel)
        {
            try
            {
                var response = _userManager.SearchUser(userSearchModel);
                return Ok(response);

                //if (response.HasError)
                //{
                //    return Ok(new { status = "Error", message = response.ErrorMessage });
                //}
                //else
                //{
                //    return Ok(new { status = "Success", message = "User information updated successfully" });
                //}
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }


        [Route("GetUsers")]
        [HttpGet]
        public IActionResult GetUsers(int skip, int take, string sortField, int sortOrder, string nameFilter,
            string emailFilter, string statusFilter, string roleFilter)
        {
            try
            {
                var users = _userManager.GetUsers();

                if (!string.IsNullOrEmpty(nameFilter) && nameFilter != "null")
                {
                    users = users.Where(u => u.FullName.ToLower().Contains(nameFilter.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(emailFilter) && emailFilter != "null")
                {
                    //var test = users.Where(u => u.Email.ToLower().StartsWith(emailFilter.ToLower())).ToList();

                    users = users.Where(u => u.Email.ToLower().StartsWith(emailFilter.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "null")
                {
                    string active = "active";
                    string terminated = "terminated";
                    if (terminated.StartsWith(statusFilter.ToLower()))
                    {
                        users = users.Where(u => u.Active == true).ToList();
                    }
                    else if (active.StartsWith(statusFilter.ToLower()))
                    {
                        users = users.Where(u => (u.Active == false)).ToList();
                    }

                }
                if (!string.IsNullOrEmpty(roleFilter) && roleFilter != "null")
                {
                    users = users.Where(u => !string.IsNullOrEmpty(u.RoleName) && u.RoleName.ToLower().Contains(roleFilter.ToLower())).ToList();
                }


                if (!string.IsNullOrEmpty(sortField) && sortField != "undefined")
                {
                    switch (sortField)
                    {
                        case "FullName":
                            users = sortOrder == 1 ? users.OrderByDescending(u => u.FirstName + " " + u.LastName).ToList() : users.OrderBy(u => (u.FirstName + " " + u.LastName)).ToList();
                            break;

                        case "Email":
                            users = sortOrder == 1 ? users.OrderByDescending(u => u.Email).ToList() : users.OrderBy(u => u.Email).ToList();
                            break;
                    }
                }
                var paginatedUsers = users.Skip(skip).Take(take).ToList();
                return Ok(new { users = paginatedUsers, totalCount = users.Count() });
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "UserController.GetUsers :: Unhandled exception");
                return BadRequest(new { status = "Failed" });
            }
        }
    }
}
