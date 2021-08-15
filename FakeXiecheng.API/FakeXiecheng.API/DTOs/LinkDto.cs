using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.DTOs
{
    public class LinkDto
    {
        public LinkDto(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }

        /// <summary>
        /// 超链接
        /// </summary>
        public string Href { get; set; }
        /// <summary>
        /// 关系，URL 的简单描述
        /// </summary>
        public string Rel { get; set; }
        /// <summary>
        /// HTTP 的方法
        /// </summary>
        public string Method { get; set; }
    }
}
