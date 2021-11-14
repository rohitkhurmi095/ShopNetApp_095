using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShopNetApp.Utilities
{
    //===================
    //Session Extensions (for complex Types)
    //===================
    //By Default = int/string
    //To strore Obj/List in session - use extension methods via Serializing+Desrializing
    //T = userdefined Type (Eg: List<ShoppingCart>)
    public static class SessionExtensions
    {
        //SET Session
        //------------
        public static void Set<T>(this ISession session,string key,T value) 
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        //GET Session
        //------------
        public static T Get<T>(this ISession session,string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }

    }
}
