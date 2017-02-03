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
        public static string CustomRender(this JToken token)
        {
            var serializer = new JsonSerializer();

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            serializer.Converters.Add(new BytesToHexConverter());
            serializer.Serialize(sw, token);
            return sb.ToString();
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
    }
}
