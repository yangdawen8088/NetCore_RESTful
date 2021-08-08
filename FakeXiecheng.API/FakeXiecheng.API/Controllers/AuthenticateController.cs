using FakeXiecheng.API.DTOs;
using FakeXiecheng.API.Moldes;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITouristRouteRepository _touristRouteRepository;
        public AuthenticateController(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITouristRouteRepository touristRouteRepository
            )
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _touristRouteRepository = touristRouteRepository;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // 1 验证用户名 密码
            var loginResult = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, false);
            if (!loginResult.Succeeded) // 判断用户信息验证是否正确
            {
                return BadRequest("登陆信息验证不正确！");
            }
            var user = await _userManager.FindByNameAsync(loginDto.Email);
            // 2 创建 JWT
            //  header
            var signingAlgorithm = SecurityAlgorithms.HmacSha256;
            //  payload
            var claims = new List<Claim>
            {
                // sub
                new Claim(JwtRegisteredClaimNames.Sub,user.Id),
                // 添加网站管理员角色    
                //new Claim(ClaimTypes.Role,"Admin")
            };
            var roleNames = await _userManager.GetRolesAsync(user);
            foreach (var roleName in roleNames)
            {
                var roleClaim = new Claim(ClaimTypes.Role, roleName);
                claims.Add(roleClaim);
            }
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

        [AllowAnonymous]
        [HttpPost("register")] // 用户注册 API
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // 这里登陆密码要求必须有一个大写字母和一个小写字母，这是框架系统规定的密码最低强度
            // 1. 使用用户名创建用户对象
            var user = new ApplicationUser()
            {
                UserName = registerDto.Email,
                Email = registerDto.Email
            };
            // 2. hash 密码，保存用户，即将用户登陆密码加密散列
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            // 3. 初始化购物车
            var shoppingCart = new ShoppingCart()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id
            };
            await _touristRouteRepository.CreateShoppingCart(shoppingCart);
            await _touristRouteRepository.SaveAsync();
            // 4. return
            return Ok();
        }
    }
}
