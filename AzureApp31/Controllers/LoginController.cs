using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AzureApp31.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        //private readonly SignInManager<IdentityUser> signInManager;

        //public LoginController(
        //    SignInManager<IdentityUser> signInManager)
        //{
        //    this.signInManager = signInManager;
        //}

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(UserModel login)
        {
            IActionResult response = Unauthorized();

            var appUser = AuthenticateUser(login);

            if (appUser != null)
            {
                var tokenString = GenerateJSONWebToken(appUser);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        private string GenerateJSONWebToken(ApplicationUser appUser)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("AzureAppTokenKey"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, appUser.LoginName),
                new Claim("Email", appUser.EmailAddress),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                "AzureApp",
                "AzureApp",
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private ApplicationUser AuthenticateUser(UserModel login)
        {
            ApplicationUser user = null;

            if (login.LoginName == "VZ")
            {
                user = new ApplicationUser { LoginName = "Vasile Zavizion", EmailAddress = "vzavizion@gmail.com" };
            }

            return user;
        }


        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<IActionResult> LoginGoogle(UserModel login)
        //{
        //    var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", null);

        //    var externalLogins = await signInManager.GetExternalAuthenticationSchemesAsync();

        //    // Get the login information about the user from the external login provider
        //    var info = await signInManager.GetExternalLoginInfoAsync();

        //    if (info == null)
        //    {
                
        //    }

        //    // If the user already has a login (i.e if there is a record in AspNetUserLogins
        //    // table) then sign-in the user with this external login provider
        //    var signInResult = await signInManager.ExternalLoginSignInAsync(
        //        info.LoginProvider,
        //        info.ProviderKey,
        //        isPersistent: false,
        //        bypassTwoFactor: true);

        //    if (signInResult.Succeeded)
        //    {
                
        //    }

        //    return null;
        //}
    }
}
