using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Service;

namespace TrustStampCore.Extensions
{
    public static class JsonExtensions
    {
        public static void SetProperty(this JToken token, string name, object val)
        {
            if (!token.HasValues)
                token.Replace(new JObject());
            token[name].Replace(new JProperty(name, val));
        }

        public static JProperty EnsureProperty(this JToken token, string name, object val)
        {
            if (!token.HasValues)
                token.Replace(new JObject(new JProperty(name, val)));
            return (JProperty)token[name];
        }

        public static JObject EnsureObject(this JToken token)
        {
            if (!token.HasValues)
                token.Replace(new JObject());
            return (JObject)token;
        }


        public static string CustomRender(this JToken token)
        {
            var serializer = new JsonSerializer();

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            serializer.Converters.Add(new BytesToHexConverter());
            serializer.Serialize(sw, token);
            return sb.ToString();
        }

        public static string ToStringValue(this JToken token, string defaultValue = "")
        {
            if (token == null)
                return defaultValue;

            if (token.Type == JTokenType.Null)
                return defaultValue;

            if (token.Type == JTokenType.String)
                return (string)token;

            return defaultValue;
        }

        public static bool ToBoolean(this JToken token, bool defaultValue = false)
        {
            if (token == null)
                return defaultValue;

            if (token.Type == JTokenType.Null)
                return defaultValue;

            if (token.Type == JTokenType.Boolean)
                return (bool)token;

            return defaultValue;
        }

        public static int ToInteger(this JToken token, int defaultValue = 0)
        {
            if (token == null)
                return defaultValue;

            if (token.Type == JTokenType.Null)
                return defaultValue;

            if (token.Type == JTokenType.Integer)
                return (int)token;

            return defaultValue;
        }

        public static DateTime ToDateTime(this JToken token, DateTime defaultValue)
        {
            if (token == null)
                return defaultValue;

            if (token.Type == JTokenType.Null)
                return defaultValue;

            if (token.Type == JTokenType.Date)
                return (DateTime)token;

            return defaultValue;
        }

    }
}
