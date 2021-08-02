using AuthenticationMicroservice.Database;
using AuthenticationMicroservice.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        DatabaseContext _db;
        IConfiguration _config;
        public AuthenticationController(DatabaseContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }
        private string GenerateJSONWebToken(UserModel userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //For JWT Cliams help : https://tools.ietf.org/html/rfc7519#section-5
            var claims = new Claim[] {
                             new Claim(JwtRegisteredClaimNames.Sub, userInfo.Name),
                             new Claim(JwtRegisteredClaimNames.Email, userInfo.Username),
                             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                             new Claim("Roles", userInfo.Roles.First()), //claim for authorization
                             new Claim("CreatedDate", DateTime.Now.ToString()),
                             };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                                            _config["Jwt:Audience"],
                                            claims,
                                            expires: DateTime.Now.AddMinutes(120),
                                            signingCredentials: credentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);
            return encodedJwt;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Return Users List", OperationId = "GetUsers")]
        public IActionResult GetUsers()
        {
            var data = _db.Users.Select(u => new { u.Name, u.UserId, u.Username }).ToList();
            return Ok(data);
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation(Summary = "Validate User", OperationId = "ValidateUser")]
        public IActionResult ValidateUser(LoginModel model)
        {

            UserModel data = (from user in _db.Users
                              join userrole in _db.UserRoles
                              on user.UserId equals userrole.UserId
                              where user.Username == model.Username && user.Password == model.Password
                              select new UserModel
                              {
                                  UserId = user.UserId,
                                  Username = user.Username,
                                  Name = user.Name,
                                  Roles = _db.Roles.Where(r => r.RoleId == userrole.RoleId).Select(r => r.Name).ToArray()
                              }).FirstOrDefault();

            if (data != null)
            {
                data.Token = GenerateJSONWebToken(data);
                return Ok(data);
            }
            else
            {
                return NoContent();
            }
        }
    }
}
