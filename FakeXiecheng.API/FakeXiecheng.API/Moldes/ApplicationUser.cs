using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Moldes
{
    public class ApplicationUser : IdentityUser
    {
        public string Address { get; set; }
        // shoppingCart
        public ShoppingCart ShoppingCart { get; set; }
        // Orders
        /// <summary>
        /// 用户角色
        /// </summary>
        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; }
        /// <summary>
        /// 用户权限的申明
        /// </summary>
        //public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }
        /// <summary>
        /// 用户的第三方登陆信息
        /// </summary>
        //public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
        /// <summary>
        /// 用户登陆的 Sesssion Token 
        /// </summary>
        //public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; }
    }
}
