using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Extensions;

namespace TrustStampCore.Service
{
    public class BytesToHexConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(byte[]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            byte[] arr = (byte[])value;
            writer.WriteValue(arr.ConvertToHex());
        }

        //public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        //{
        //    if (value == null)
        //    {
        //        writer.WriteNull();
        //        return;
        //    }

        //    byte[] data = (byte[])value;

        //    // Compose an array.
        //    writer.WriteStartArray();

        //    for (var i = 0; i < data.Length; i++)
        //    {
        //        writer.WriteValue(data[i]);
        //    }

        //    writer.WriteEndArray();
        //}
    }
}
