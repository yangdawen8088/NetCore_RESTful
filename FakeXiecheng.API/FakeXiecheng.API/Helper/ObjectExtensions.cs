using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Helper
{
    public static class ObjectExtensions
    {
        public static ExpandoObject ShapeData<TSource>(
            this TSource source,
            string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var dataShapedObject = new ExpandoObject();
            if (string.IsNullOrWhiteSpace(fields))
            {
                // 希望返回动态类型对象 ExpandoObject 所有的属性
                var propertyInfos = typeof(TSource).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                foreach (var propertyInfo in propertyInfos)
                {
                    var propertyValue = propertyInfo.GetValue(source);
                    ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
                }
                return dataShapedObject;
            }
            // 使用逗号来分隔字段字符串
            var fieldsAfterSplit = fields.Split(',');
            foreach (var field in fieldsAfterSplit)
            {
                // 去掉首尾多余的空格，获得属性名称
                var propertyName = field.Trim();
                var propertyInfo = typeof(TSource).GetProperty(
                    propertyName,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null)
                {
                    throw new Exception($"属性 {propertyName} 找不到" + $" {typeof(TSource)}");
                }
                var propertyValue = propertyInfo.GetValue(source);
                ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
            }
            return dataShapedObject;
        }
    }
}
