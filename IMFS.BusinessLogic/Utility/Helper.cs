using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace IMFS.BusinessLogic.Utility
{
    public static class Helper
    {
        // Deep clone
        public static T DeepClone<T>(this T a)
        {
            var serialized = JsonConvert.SerializeObject(a);
            return JsonConvert.DeserializeObject<T>(serialized);


            
        }

    }
}
