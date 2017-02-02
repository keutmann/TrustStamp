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
    }
}
