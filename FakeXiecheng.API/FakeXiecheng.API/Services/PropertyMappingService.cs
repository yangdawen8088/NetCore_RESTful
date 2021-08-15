using FakeXiecheng.API.DTOs;
using FakeXiecheng.API.Moldes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _touristRoutePropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {   // 这里添加可排序字段映射 没有在这个里面注册的映射都属于非法的输入 应当处理为 400 级别错误
                {"Id",new PropertyMappingValue(new List<string>(){ "Id"}) },
                {"Title",new PropertyMappingValue(new List<string>(){ "Title"}) },
                {"Rating",new PropertyMappingValue(new List<string>(){ "Rating"}) },
                {"OriginalPrice",new PropertyMappingValue(new List<string>(){ "OriginalPrice"}) },
            };
        private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<TouristRouteDto, TouristRoute>(_touristRoutePropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDetination>()
        {
            // 获得匹配的映射对象
            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDetination>>();
            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }
            throw new Exception($"无法为 <{typeof(TSource)},{typeof(TDetination)}> 找到准确的属性映射实例");
        }
        public bool IsMappingExists<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }
            // 使用都好来分割字段字符串
            var fieldsAfterSplit = fields.Split(",");
            foreach (var field in fieldsAfterSplit)
            {
                // 去掉空格
                var trimmedField = field.Trim();
                // 获得属性名称字符串
                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ? trimmedField : trimmedField.Remove(indexOfFirstSpace);
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
