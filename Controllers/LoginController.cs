using JWT_Role.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWT_Role.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private JwtroleContext _context;
        private IConfiguration _config;
        public LoginController(JwtroleContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        private UserInfo AuthenticateUser(UserInfo userInfo)
        {
            UserInfo user_ = null;
            var dbUser = _context.UserInfos.SingleOrDefault(u => u.Username == userInfo.Username && u.Password == userInfo.Password);
            if (dbUser != null)
            {
                user_ = new UserInfo { Username = dbUser.Username, Password = dbUser.Password, Role = dbUser.Role };
            }
            return user_;
        }
        private string GenerateToken(UserInfo userinfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userinfo.Username),
                new Claim(ClaimTypes.Role, userinfo.Role)
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(UserInfo userInfo)
        {
            IActionResult response = Unauthorized();
            var user_ = AuthenticateUser(userInfo);
            if (user_ != null)
            {
                var Token = GenerateToken(user_);
                response = Ok(new { Token = Token });
            }
            return response;
        }
    }
}
