using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Helper
{
    public class ArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext modelBindingContext)
        {
            // 我们的活页夹仅适用于可枚举类型
            if (!modelBindingContext.ModelMetadata.IsEnumerableType)
            {
                modelBindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }
            // 通过值提供者通过值获取输入的值
            var value = modelBindingContext.ValueProvider.GetValue(modelBindingContext.ModelName).ToString();
            // 如果该值为 null 或空格，我们返回 null
            if (string.IsNullOrWhiteSpace(value))
            {
                modelBindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }
            // 该值不是空值或空格，并且模型的类型是可枚举的。
            // 获取枚举类型和转换器
            var elementType = modelBindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];
            var converter = TypeDescriptor.GetConverter(elementType);
            // 将值列表中的每一项转换为可枚举类型
            var values = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => converter.ConvertFromString(x.Trim())).ToArray();
            // 创建该类型的数组。并将其设置为模型值
            var typedValues = Array.CreateInstance(elementType, values.Length);
            values.CopyTo(typedValues, 0);
            modelBindingContext.Model = typedValues;
            // 返回一个成功的结果，传入模型
            modelBindingContext.Result = ModelBindingResult.Success(modelBindingContext.Model);
            return Task.CompletedTask;
        }
    }
}
