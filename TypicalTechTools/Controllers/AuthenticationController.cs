using Microsoft.AspNetCore.Mvc;
using TypicalTechTools.Models.DTOs;
using TypicalTechTools.Models.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Ganss.Xss;

namespace TypicalTechTools.Controllers
{

    /// <summary>
    /// Authentication Controller handles the Login system of the application. When a user creates an
    /// account, their Role is set to "GUEST" by default. This cannot be changed in the application, if
    /// needed to modify privilege it should be updated in the database. When a user logs in, a new claim
    /// is set using their Role, which and will be used to verify their privileges when accessing endpoints. Additional Authentication properties
    /// and Cookie settings are also applied when the user logs in.
    /// </summary>
    public class AuthenticationController : Controller
    {
        //Private field to hold reference to repository
        private readonly IAuthenticationRepository _repository;

        //Create variable for sanitiser class
        private readonly HtmlSanitizer _sanitizer;
        //Requesting the repository from the dependency injection by naming it in our constructor.
        public AuthenticationController(IAuthenticationRepository repository, HtmlSanitizer sanitizer)
        {
            _repository = repository;
            _sanitizer = sanitizer;
        }

        public IActionResult Login([FromQuery] string ReturnUrl)
        {
            LoginDTO loginDTO = new LoginDTO()
            {
                ReturnURL = string.IsNullOrWhiteSpace(ReturnUrl) ? "/Product" : ReturnUrl
            };
            return View(loginDTO);
        }


        /// <summary>
        /// User login method. The username will be sanitized prior to being sent to the repository for processing. 
        /// </summary>
        /// <remarks>Sanitization of the username in this method is probably not necessary as it is sanitized when they create an account, but it can't hurt.</remarks>
        /// <param name="loginDto">Login DTO</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            //Check the provided data in the DTO meets the validation rules.
            if (ModelState.IsValid == false)
            {
                return View(loginDto);
            }

            //Pass the DTO to the repository to see if the login details were correct.
            var user = _repository.Authenticate(loginDto);

            //If the login failed
            if (user == null)
            {
                //Pass an error message to the viewbag then redirect the user back to the login page.
                ViewBag.LoginMessage = "Username or Password is invalid.";
                return View();
            }

            //If the login was successful, add an entry to the session data to track the user being logged in. 
            var claims = new List<Claim>
            {
                //Set the user's role in a claim that we can check for their access permissions in our [Authorize] tags.
                new Claim(ClaimTypes.Role, user.Role),
                //Next two claims are just examples and not used in our system.
                new Claim(ClaimTypes.Email, user.Email),
                //Store the user's ID in their claims so we can reference it later.
                new Claim("ID",user.Id.ToString())
            };

            //Holds any identities associated with the user as well as their claims.
            //This forms the data container that is 
            //held when they are logged into the system
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, 
                CookieAuthenticationDefaults.AuthenticationScheme));

            //Lets you set additional settings associated with the user's login and can even allow overriding some of
            //the default cookie settings
            var authProperties = new AuthenticationProperties
            {
                //Allows you to override the sliding expiry in the cookie settings.
                AllowRefresh = true,
                //Makes the cookie stay active even after the browser closes, until the timeout lapses
                IsPersistent = true,

                //Sets the redirection for the user back to their intended address
                RedirectUri = loginDto.ReturnURL
            };

            //Signs the user in using the values we have set.
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

            //Redirect the user back to where they were trying to go.
            return Redirect(loginDto.ReturnURL);
        }

        public ActionResult Logoff()
        {
            HttpContext.SignOutAsync();
            //Redirect the user to the home page.
            return RedirectToAction("Index", "Product");
        }


        [HttpGet]
        public ActionResult Create()
        {

            return View();
        }

        public ActionResult AccessDenied()
        {
            return View();
        }
        /// <summary>
        /// Create user method. Username is sanitized prior to being processed.
        /// </summary>
        /// <param name="userDto">The DTO for the user being created</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateUserDTO userDto)
        {

            //Check that provided details match the rules in the data model
            if (ModelState.IsValid == false)
            {
                return View(userDto);
            }
            //Sanitizing username to avoid potentially dangerous scripting being injected
            userDto.UserName = _sanitizer.Sanitize(userDto.UserName);
            //Attempt to save the user to the database
            var user = _repository.CreateUser(userDto);
            //If the repository returns null to indicate an error
            if (user == null)
            {
                //Generate an error message and go back to the form view
                ViewBag.CreateUserError = "Username already exists. Please choose a different username.";
                return View(userDto);
            }
            //If successful, generate a success message and clear the model in the form
            ViewBag.CreatedUserConfirmation = "User Account Created";
            ModelState.Clear();
            //Return to the main page
            return View();
        }
    }
}
