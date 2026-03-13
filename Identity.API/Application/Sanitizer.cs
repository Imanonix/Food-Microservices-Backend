using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Application
{
    public static class Sanitizer
    {
        //private static readonly HtmlSanitizer _sanitizer;
        //static Sanitizer()
        //{
        //    _sanitizer = new HtmlSanitizer();
        //    _sanitizer.AllowedTags.Clear(); // هیچ تگی مجاز نیست
        //    _sanitizer.AllowedAttributes.Clear(); // هیچ attribute مجاز نیست  
        //}

        //public static string Sanitize(string input)
        //{
        //    if(string.IsNullOrEmpty(input)) return input;
        //    var sanitizedText = input.Trim();
        //    sanitizedText = _sanitizer.Sanitize(sanitizedText);
        //    return WebUtility.HtmlEncode(sanitizedText);
        //}
    }
}
