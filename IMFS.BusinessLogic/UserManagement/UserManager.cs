using IMFS.DataAccess.Repository;
using IMFS.Services.Services;
using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.Misc;
using IMFS.Web.Models.User;
using Microsoft.Extensions.Configuration;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;

namespace IMFS.BusinessLogic.UserManagement
{
    public class UserManager : IUserManager
    {

        private readonly IIMFSEmailService _emailService;

        private readonly IRepository<AspNetUsers> _aspNetUsersRepository;
        private readonly IRepository<AspNetRoles> _aspNetRolesRepository;
        private readonly IRepository<AspNetUserRoles> _aspNetUserRolesRepository;
        private readonly IRepository<QuoteDuration> _quoteDurationRepository;
        private readonly IRepository<Web.Models.DBModel.FinanceType> _financeTypeRepository;
        public IConfiguration Configuration { get; }


        public UserManager(

            IConfiguration configuration,
            IIMFSEmailService emailService,
            IRepository<AspNetUsers> aspNetUsersRepository,
            IRepository<AspNetRoles> aspNetRolesRepository,
            IRepository<AspNetUserRoles> aspNetUserRolesRepository,
             IRepository<QuoteDuration> quoteDurationRepository,
             IRepository<Web.Models.DBModel.FinanceType> financeTypeRepository
            )
        {
            _aspNetUsersRepository = aspNetUsersRepository;
            _aspNetRolesRepository = aspNetRolesRepository;
            _aspNetUserRolesRepository = aspNetUserRolesRepository;
            _quoteDurationRepository = quoteDurationRepository;
            _financeTypeRepository = financeTypeRepository;
            _emailService = emailService;
            Configuration = configuration;
        }

        public List<UserModel> GetUsers()
        {

            var users = (from user in _aspNetUsersRepository.Table
                         join asprole in _aspNetUserRolesRepository.Table on user.Id equals asprole.UserId into ur
                         from asprole in ur.DefaultIfEmpty()
                         join rolename in _aspNetRolesRepository.Table on asprole.RoleId equals rolename.Id into ro
                         from rolename in ro.DefaultIfEmpty()
                         join quoteDuration in _quoteDurationRepository.Table on user.DefaultFinanceDuration equals quoteDuration.Id into qd
                         from quoteDuration in qd.DefaultIfEmpty()
                         join financeType in _financeTypeRepository.Table on user.DefaultFinanceType equals financeType.QuoteDurationType into ft
                         from financeType in ft.DefaultIfEmpty()
                         select new UserModel()
                         {
                             Id = user.Id,
                             FirstName = user.FirstName,
                             LastName = user.LastName,
                             FullName = user.FirstName + " " + user.LastName,
                             Email = user.Email,
                             RoleName = rolename.Name,
                             RoleId = rolename.Id,
                             Active = user.Active,
                             CustomerName = user.CustomerName,
                             CustomerNumber = user.CustomerNumber,
                             DefaultFinanceDuration = user.DefaultFinanceDuration,
                             DefaultFinanceDurationName = quoteDuration.Value,
                             DefaultFinanceFrequency = user.DefaultFinanceFrequency,
                             DefaultFinanceType = user.DefaultFinanceType,
                             DefaultFinanceTypeName = financeType.Description,
                             ExcludeGST = user.ExcludeGST,
                             JobTitle = user.JobTitle,
                             PhoneNumber = user.PhoneNumber,
                             Title = user.Title,
                             UserName = user.UserName
                         }).OrderBy(x => x.FirstName).ToList();

            return users;

        }

        public List<UserModel> SearchUser(UserSearchModel userSearchModel)
        {
            //var query = from user in _aspNetUsersRepository.Table
            //            join asprole in _aspNetUserRolesRepository.Table on user.Id equals asprole.UserId into ur
            //            from asprole in ur.DefaultIfEmpty()
            //            join rolename in _aspNetRolesRepository.Table on asprole.RoleId equals rolename.Id into ro
            //            from rolename in ro.DefaultIfEmpty()
            //            join quoteDuration in _quoteDurationRepository.Table on user.DefaultFinanceDuration equals quoteDuration.Id into qd
            //            from quoteDuration in qd.DefaultIfEmpty()
            //            join financeType in _financeTypeRepository.Table on user.DefaultFinanceType equals financeType.QuoteDurationType into ft
            //            from financeType in ft.DefaultIfEmpty()
            //            select user;

            //if (!string.IsNullOrEmpty(userSearchModel.FirstName))
            //{               
            //    query = query.Where(q => q.FirstName.Contains(userSearchModel.FirstName));
            //}
            //if (!string.IsNullOrEmpty(userSearchModel.LastName))
            //{
            //    query = query.Where(q => q.LastName.Contains(userSearchModel.LastName));
            //}
            //if (!string.IsNullOrEmpty(userSearchModel.Email))
            //{
            //    query = query.Where(q => q.Email.Contains(userSearchModel.Email));
            //}
            //if (!string.IsNullOrEmpty(userSearchModel.CustomerNumber))
            //{
            //    query = query.Where(q => q.CustomerNumber.Contains(userSearchModel.CustomerNumber));
            //}
            //if (!string.IsNullOrEmpty(userSearchModel.CustomerName))
            //{
            //    query = query.Where(q => q.CustomerName.Contains(userSearchModel.CustomerName));
            //}

            //var results = query.Select(user => new UserModel()
            //{
            //    Id = user.Id,
            //    FirstName = user.FirstName,
            //    LastName = user.LastName,
            //    FullName = user.FirstName + " " + user.LastName,
            //    Email = user.Email,
            //    RoleName = rolename.Name,
            //    RoleId = rolename.Id,
            //    Active = user.Active,
            //    CustomerName = user.CustomerName,
            //    CustomerNumber = user.CustomerNumber,
            //    DefaultFinanceDuration = user.DefaultFinanceDuration,
            //    DefaultFinanceDurationName = quoteDuration.Value,
            //    DefaultFinanceFrequency = user.DefaultFinanceFrequency,
            //    DefaultFinanceType = user.DefaultFinanceType,
            //    DefaultFinanceTypeName = financeType.Description,
            //    ExcludeGST = user.ExcludeGST,
            //    JobTitle = user.JobTitle,
            //    PhoneNumber = user.PhoneNumber,
            //    Title = user.Title,
            //    UserName = user.UserName
            //}).OrderBy(x => x.FirstName).ToList();


            var users = (from user in _aspNetUsersRepository.Table
                         join asprole in _aspNetUserRolesRepository.Table on user.Id equals asprole.UserId into ur
                         from asprole in ur.DefaultIfEmpty()
                         join rolename in _aspNetRolesRepository.Table on asprole.RoleId equals rolename.Id into ro
                         from rolename in ro.DefaultIfEmpty()
                         join quoteDuration in _quoteDurationRepository.Table on user.DefaultFinanceDuration equals quoteDuration.Id into qd
                         from quoteDuration in qd.DefaultIfEmpty()
                         join financeType in _financeTypeRepository.Table on user.DefaultFinanceType equals financeType.QuoteDurationType into ft
                         from financeType in ft.DefaultIfEmpty()
                         select new UserModel()
                         {
                             Id = user.Id,
                             FirstName = user.FirstName,
                             LastName = user.LastName,
                             FullName = user.FirstName + " " + user.LastName,
                             Email = user.Email,
                             RoleName = rolename.Name,
                             RoleId = rolename.Id,
                             Active = user.Active,
                             CustomerName = user.CustomerName,
                             CustomerNumber = user.CustomerNumber,
                             DefaultFinanceDuration = user.DefaultFinanceDuration,
                             DefaultFinanceDurationName = quoteDuration.Value,
                             DefaultFinanceFrequency = user.DefaultFinanceFrequency,
                             DefaultFinanceType = user.DefaultFinanceType,
                             DefaultFinanceTypeName = financeType.Description,
                             ExcludeGST = user.ExcludeGST,
                             JobTitle = user.JobTitle,
                             PhoneNumber = user.PhoneNumber,
                             Title = user.Title,
                             UserName = user.UserName
                         }).OrderBy(x => x.FirstName).ToList();

            if (!string.IsNullOrEmpty(userSearchModel.FirstName))
            {
                users = users.Where(u => u.FirstName.ToLower().Contains(userSearchModel.FirstName.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(userSearchModel.LastName))
            {
                users = users.Where(u => u.LastName.ToLower().Contains(userSearchModel.LastName.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(userSearchModel.Email))
            {
                users = users.Where(u => u.Email.ToLower().Contains(userSearchModel.Email.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(userSearchModel.CustomerNumber))
            {
                users = users.Where(u => u.CustomerNumber.Contains(userSearchModel.CustomerNumber)).ToList();
            }
            if (!string.IsNullOrEmpty(userSearchModel.CustomerName))
            {
                users = users.Where(u => u.CustomerName.ToLower().Contains(userSearchModel.CustomerName.ToLower())).ToList();
            }

            return users;

        }

        public ErrorModel CreateNewUser(IMFSIdentityUser identityUser)
        {
            var response = new ErrorModel();
            try
            {
                var newUser = new AspNetUsers();
                newUser.Id = identityUser.Id;
                newUser.FirstName = identityUser.FirstName;
                newUser.LastName = identityUser.LastName;
                newUser.UserName = identityUser.UserName;
                newUser.Email = identityUser.Email;
                newUser.PhoneNumber = identityUser.PhoneNumber;
                newUser.CustomerNumber = identityUser.CustomerNumber;
                newUser.CustomerName = identityUser.CustomerName;
                newUser.Active = identityUser.Active;
                newUser.DefaultFinanceType = identityUser.DefaultFinanceType;
                newUser.DefaultFinanceDuration = identityUser.DefaultFinanceDuration;
                newUser.DefaultFinanceFrequency = identityUser.DefaultFinanceFrequency;
                newUser.ExcludeGST = identityUser.ExcludeGST;
                _aspNetUsersRepository.Insert(newUser);
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public ErrorModel SendNewUserEmail(string templateFolderPath, IMFSIdentityUser newUser)
        {
            var response = new ErrorModel();
            try
            {
                var newUserTemplateModel = new AutoCreateUserTemplateModel();
                newUserTemplateModel.FirstName = newUser.FirstName;
                newUserTemplateModel.LastName = newUser.LastName;
                newUserTemplateModel.Email = newUser.Email;
                newUserTemplateModel.IMFSHostLink = Configuration.GetValue<string>("IMFSClientAppHost");
                newUserTemplateModel.IMFSUserLink = Configuration.GetValue<string>("IMFSClientAppHost") + "/user-edit/" + newUser.Id;
                newUserTemplateModel.CustomerNumber = newUser.CustomerNumber;
                newUserTemplateModel.CustomerName = newUser.CustomerName;

                string ingramLogoHTML = string.Empty;

                List<LinkedResource> linkedResources = new List<LinkedResource>();
                LinkedResource ingramResource = _emailService.CreateLinkedResourceFromImage("ingram.jpg", 1, templateFolderPath, out ingramLogoHTML);
                newUserTemplateModel.IngramLogoHTML = ingramLogoHTML + "<br/><br/>";
                linkedResources.Add(ingramResource);

                string body = generateAutoCreateUserEmailBody(templateFolderPath, newUserTemplateModel);

                AlternateView altView = null;
                if (linkedResources != null && linkedResources.Count > 0)
                {
                    altView = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);
                    foreach (LinkedResource lr in linkedResources)
                    {
                        altView.LinkedResources.Add(lr);
                    }
                }

                _emailService.Send(Configuration.GetValue<string>("NoReplyEmail"), "sun.aung@ingrammicro.com", string.Empty, string.Empty, "New user created in IMFS", body, null, altView, null);
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.ToString();
            }
            return response;
        }

        public AspNetUsers GetUserDetails(string userId)
        {
            var user = _aspNetUsersRepository.GetById(userId);

            return user;
        }

        public AspNetUsers GetUserDetailsByEmail(string email)
        {
            var user = _aspNetUsersRepository.Table.Where(x => x.Email.ToLower() == email.ToLower()).FirstOrDefault();

            return user;
        }

        public AspNetUsers GetUserDetailsByUserName(string username)
        {
            var user = _aspNetUsersRepository.Table.Where(x => x.UserName == username).FirstOrDefault();

            return user;
        }

        public ErrorModel UpdateUserStatus(UserModel userModel)
        {
            var response = new ErrorModel();
            try
            {
                var user = _aspNetUsersRepository.GetById(userModel.Id);
                if (user != null)
                {
                    //user.FirstName = userModel.FirstName;
                    //user.LastName = userModel.LastName;
                    user.PhoneNumber = userModel.PhoneNumber;
                    user.JobTitle = userModel.JobTitle;

                    user.DefaultFinanceType = userModel.DefaultFinanceType;
                    user.DefaultFinanceDuration = userModel.DefaultFinanceDuration;
                    user.DefaultFinanceFrequency = userModel.DefaultFinanceFrequency;

                    //Editable by IMStaff
                    //role
                    //active

                    _aspNetUsersRepository.Update(user);
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }

        public ErrorModel DeactivateUserStatus(string userId)
        {
            var response = new ErrorModel();
            try
            {
                var user = _aspNetUsersRepository.GetById(userId);

                if (user != null)
                {
                    user.Active = false;
                    _aspNetUsersRepository.Update(user);
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }

        public ErrorModel ActivateUserStatus(string userId)
        {
            var response = new ErrorModel();
            try
            {
                var user = _aspNetUsersRepository.GetById(userId);

                if (user != null)
                {
                    user.Active = true;
                    _aspNetUsersRepository.Update(user);
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }


        #region private methods
        private string generateAutoCreateUserEmailBody(string templatePath, AutoCreateUserTemplateModel newUserTemplateModel)
        {
            var emailBodyHTMLPath = templatePath + "\\NewUserAutoCreate.cshtml";
            //var emailHtmlBody = templateService.RunCompile(File.ReadAllText(emailBodyHTMLPath), "key", typeof(AutoCreateUserTemplateModel), newUserTemplateModel, null);
            var emailHtmlBody = Engine.Razor.RunCompile(File.ReadAllText(emailBodyHTMLPath), "templateKey", typeof(AutoCreateUserTemplateModel), newUserTemplateModel, null);
            return emailHtmlBody;
        }
        #endregion
    }
}
