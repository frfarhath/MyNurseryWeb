using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNursery.Utility
{
    public static class TempDataExtension
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object value;
            tempData.TryGetValue(key, out value);
            if (value != null)
            {
                return JsonConvert.DeserializeObject<T>((string)value);
            }
            return null;
        }

        public static void PutAll<T>(this ITempDataDictionary tempData, string key, IEnumerable<T> value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static IEnumerable<T> GetAll<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            IEnumerable<T> values;
            if (tempData.ContainsKey(key))
            {
                values = (IEnumerable<T>)JsonConvert.DeserializeObject(tempData[key].ToString());
                return values;
            }
            return null;
        }
    }
}
