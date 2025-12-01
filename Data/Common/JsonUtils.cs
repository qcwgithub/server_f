using System;
using System.Collections.Generic;

namespace Data
{
    public class JsonUtils
    {
        public static string stringify(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
        public static string stringifyIndent(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
        }

        public static string flattenNestedClassConverter(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, new FlattenNestedClassConverter());
        }

        public static T parse<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        // 尝试解析请求的JSON，并检查它是否能被序列化为T类型
        public static bool TryParseJsonFromStream<T>(string json, out T result)
        {
            Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings
            {
                MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error
            };

            try
            {
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json, settings);
            }
            catch (Newtonsoft.Json.JsonException)
            {
                result = default(T);
                return false;
            }
            catch (Exception)
            {
                result = default(T);
                return false;
            }

            return true;
        }

        public static object parse(string json, Type type)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(json, type);
        }
    }
}