using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace FakeXiecheng.API.ResourceParameters
{
    public class TouristRouteResourceParamaters
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        public string OrderBy { get; set; } // order 加上 s 后就为订单，不加为排序
        public string Keyword { get; set; }
        public string RatingOperator { get; set; }
        public int? RatingValue { get; set; }
        private string _rating;
        public string Rating
        {
            get { return _rating; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    Regex regex = new Regex(@"([A-Za-z0-9\-]+)(\d+)");
                    Match match = regex.Match(value);
                    if (match.Success)
                    {
                        RatingOperator = match.Groups[1].Value;
                        RatingValue = Int32.Parse(match.Groups[2].Value);
                    }
                }
                _rating = value;
            }
        }
        public string Fields { get; set; }
    }
}
