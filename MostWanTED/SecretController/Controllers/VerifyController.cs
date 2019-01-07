using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace SecretController.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerifyController : ControllerBase
    {
        private IConfiguration _config;

        public VerifyController(IConfiguration config)
        {
            _config = config;
        }

        // GET api/verify/status
        [HttpGet("status")]
        public IActionResult Status() => Ok();

        // GET api/values
        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Verifizierung erfolgreich! Hier sehen Sie Ihre privaten Daten." };
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] JObject login)
        {
            IActionResult response = Unauthorized();
            var user = AuthenticateUser(login);

            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        private string GenerateJSONWebToken(JObject userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private JObject AuthenticateUser(JObject login)
        {
            JObject user = new JObject(); ;

            //Validate the User Credentials  
            //Demo Purpose, I have Passed HardCoded User Information  
            if ((String)login["username"] == "IEGUser")
            {
                user.Add("username", "IEG TestUser");
            }
            return user;
        }
    }
}
