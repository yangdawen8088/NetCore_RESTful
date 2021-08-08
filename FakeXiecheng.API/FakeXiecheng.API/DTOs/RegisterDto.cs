using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password), ErrorMessage = "密码输入不一致")] // 这个可以自动比较两个字段的值是否相同 
        public string ConfirmPassword { get; set; }
    }
}
