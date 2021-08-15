using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.ResourceParameters
{
    public class PaginationResourceParamaters
    {
        private int _pageNumber = 1;
        /// <summary>
        /// 表示当前请求数据的第几页
        /// </summary>
        public int PageNumber
        {
            get
            {
                return _pageNumber;
            }
            set
            {
                if (value >= 1)
                {
                    _pageNumber = value;
                }
            }
        }
        private int _pageSize = 10;
        const int maxPageSize = 50;
        /// <summary>
        /// 表示当前请求数据的每页大小，多少条数据
        /// </summary>
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                if (value >= 1)
                {
                    _pageSize = value > maxPageSize ? maxPageSize : value;
                }
            }
        }
    }
}
