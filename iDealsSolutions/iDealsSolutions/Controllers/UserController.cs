using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using iDealsSolutions.Models;
using iDealsSolutions.DbData;
using System.Text;

namespace iDealsSolutions.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public UserController()
        {
        }

        public UserController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [Authorize]
        public ActionResult Get(string userName)
        {
            if (DBContext.ConnectionCheck())
            {
                var user = DBContext.Create().User.Where(u => u.UserName == userName).FirstOrDefault();
                LogWriter("Get", "Get", string.Format("User by Email {0}", userName));                
                return View(user);
            }
            return RedirectToAction("Error", "Home", new { error = "Connection to SQL broken, Please check connection string" });  
        }

        [Authorize]
        [HttpGet]
        public ActionResult Update(string userName)
        {  
            if (DBContext.ConnectionCheck())
            {
                var user = DBContext.Create().User.Where(u => u.UserName == userName).FirstOrDefault();
                UpdateViewModel updateModel = new UpdateViewModel(user);               
                return View(updateModel);
            }
            return RedirectToAction("Error", "Home", new { error = "Connection to SQL broken, Please check connection string" });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Update(UpdateViewModel model)
        {           
            if (!ModelState.IsValid)
            {
                return View(model);
            }       

            if(DBContext.ConnectionCheck())
            {
                try
                {
                    var dbContext = DBContext.Create();
                    var user = dbContext.User.Where(u => u.Id == model.Id).FirstOrDefault();
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.Name = model.Name;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Surname = model.Surname;                    
                    dbContext.SaveChanges();
                    LogWriter("Update", "Update", string.Format("Successful Updating User {0}", model.Id));
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    return RedirectToAction("Get", "User", new { userName = model.Email });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    LogWriter("Error", "Update", string.Format("Error Updating User {0}. Exeption: {1}", model.Id, ex.Message));
                    return View(model);
                }                
            }
            return RedirectToAction("Error", "Home", new { error = "Connection to SQL broken, Please check connection string" });
        }        
      
        [Authorize]
        public ActionResult Delete()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);            
            return RedirectToAction("Login", "User");
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {           
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            if(DBContext.ConnectionCheck())
            {
                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        LogWriter("Login", "Login",string.Format("Successful Login User {0}", model.Email));
                        return RedirectToAction("Get", "User", new { userName = model.Email });                    
                    default:
                        LogWriter("Error", "Login", string.Format("Invalid login attempt {0}", model.Email));
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                }
            }
            return RedirectToAction("Error", "Home", new { error = "Connection to SQL broken, Please check connection string or try register new user first." });
        }      

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Surname = model.Surname, Name = model.Name, PhoneNumber = model.Phone };
                try
                {
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        LogWriter("Register", "Register", string.Format("Successful Register User {0}", model.Email));
                        return RedirectToAction("Get", "User", new { userName = model.Email });
                    }
                    AddErrors(result);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home", new { error = ex.Message });
                }                                
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        } 
      
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff(string userName)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            LogWriter("LogOff", "LogOff", string.Format("Successful LogOff User {0}", userName));
            return RedirectToAction("Index", "Home");
        }       

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private void LogWriter(string eventType, string eventAction, string eventBody)
        {
            var dbContext = DBContext.Create();
            dbContext.Logs.Add(new Logs()
            {
                LogId = Guid.NewGuid().ToString(),
                EventType = eventType,
                EventAction = eventAction,
                EventBody = eventBody,
                EventCreationDate = DateTime.UtcNow                
            });
            dbContext.SaveChanges();
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var error in result.Errors)
            {
                if (!(error.EndsWith("is already taken.") && error.StartsWith("Name")))
                {
                    ModelState.AddModelError("", error);
                    sb.Append(error + "/n");
                }
            }

            LogWriter("Error", "Register", string.Format("Error: {0}", sb.ToString()));
        }        

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}