using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Helper
{
    public class PaginationList<T> : List<T>
    {
        public PaginationList(int totalCount, int currentPage, int pageSize, List<T> items)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            AddRange(items);
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);// 取最小整数
        }
        public static async Task<PaginationList<T>> CreateAsync(int currentPage, int pageSize, IQueryable<T> result)
        {
            var totalCount = await result.CountAsync();
            // 以下为给数据分页的操作
            // pagination
            // skip     获取从多少条数据开始数，第一条数据序号为 0
            var skip = (currentPage - 1) * pageSize;
            result = result.Skip(skip); // 从 skip 条数据及之后的数据筛选出来
            // 以 pageSize 为标准显示一定量的数据
            result = result.Take(pageSize); // 将筛选出来的数据截取 pageSize 条数据出来
            // 以上几个步骤即可实现每页 pageSize 条数据，第 pageNumber 页数据的分页效果
            //include vs join
            var items = await result.ToListAsync();
            return new PaginationList<T>(totalCount, currentPage, pageSize, items);
        }
        /// <summary>
        /// 页面总量，当前页数据总量
        /// </summary>
        public int TotalPages { get; private set; }
        /// <summary>
        /// 数据库数据量
        /// </summary>
        public int TotalCount { get; private set; }
        /// <summary>
        /// 判断是否存在上一页
        /// </summary>
        public bool HasPreviors => CurrentPage > 1;
        /// <summary>
        /// 判断是否存在下一页
        /// </summary>
        public bool HasNext => CurrentPage < TotalPages;
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// 每页数据量
        /// </summary>
        public int PageSize { get; set; }
    }
}
