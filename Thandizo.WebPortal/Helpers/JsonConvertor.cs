using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thandizo.WebPortal.Helpers
{
    public static class JsonConvertor
    {
        public static StringBuilder ConvertObjectToMessages(string jsonData)
        {
            StringBuilder parsedMesage = new StringBuilder();
            try
            {
                var jObject = JsonConvert.DeserializeObject<JObject>(jsonData);

                var properties = jObject.Properties();
                foreach (var property in properties)
                {
                    var values = JsonConvert.DeserializeObject<List<string>>(property.Value.ToString());
                    foreach (var value in values)
                    {
                        parsedMesage.Append(value).Append("; ");
                    }
                }
            }
            catch (Exception)
            {
                //just return ERROR
                parsedMesage.Append("ERROR");
            }
            return parsedMesage;
        }
    }
}
