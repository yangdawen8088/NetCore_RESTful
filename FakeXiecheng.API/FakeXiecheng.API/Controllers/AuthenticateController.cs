using FakeXiecheng.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthenticateController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthenticateController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            // 1 验证用户名 密码

            // 2 创建 JWT
            //  header
            var signingAlgorithm = SecurityAlgorithms.HmacSha256;
            //  payload
            var claims = new[]
            {
                // sub
                new Claim(JwtRegisteredClaimNames.Sub,"fake_user_id")
            };
            //  signiture
            // 此处密钥位数必须超过 16 位
            var secretByte = Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]);
            var signiingKey = new SymmetricSecurityKey(secretByte);
            var signingCredentials = new SigningCredentials(signiingKey, signingAlgorithm);
            var token = new JwtSecurityToken(// 创建 Token 
                issuer: _configuration["Authentication:Issuer"], // 谁发布的这个 Token
                audience: _configuration["Authentication:Audience"], // 这个 Token 将会发布给谁
                claims,
                notBefore: DateTime.UtcNow, // 发布时间
                expires: DateTime.UtcNow.AddDays(1), // 有效期多久，这里为 1 天
                signingCredentials
                );
            // 以字符串的形式输出 Token
            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            // 3 return 200 OK + JWT
            return Ok(tokenStr);
        }
    }
}
