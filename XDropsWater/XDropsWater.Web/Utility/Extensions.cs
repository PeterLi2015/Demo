using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace XDropsWater.Web.Utility
{
    public static class Extensions
    {
        public static RouteValueDictionary ToRouteValues(this NameValueCollection col, Object obj)
        {
            var values = new RouteValueDictionary(obj);
            if (col != null)
            {
                foreach (string key in col)
                {
                    //values passed in object override those already in collection
                    if (!values.ContainsKey(key) && !key.StartsWith("_"))
                        values[key] = col[key];
                }
            }
            return values;
        }
        public static string GetErrors(this ModelStateDictionary states)
        {
            int iErrorNumber = 1;
            StringBuilder sbErrorMessages = new StringBuilder();
            foreach (ModelState modelState in states.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    sbErrorMessages.AppendFormat("{0}. {1}{2}", iErrorNumber++, error.ErrorMessage, Environment.NewLine);
                }

            }
            return sbErrorMessages.ToString();
        }

        public static string DivideByLength(this string oriStr, int charNo)
        {
            StringBuilder sbd = new StringBuilder();
            if (oriStr.Length <= charNo) return oriStr;
            string newStr = oriStr;
            var index = oriStr.Length / charNo;
            for (int i = 0; i <= index; i++)
            {

                if (i == index)
                {
                    sbd.Append(newStr);
                }
                else
                {
                    sbd.Append(newStr.Substring(0, charNo) + "<br/>");
                    newStr = oriStr.Substring(charNo * (i + 1));
                }

            }
            return sbd.ToString();
        }
    }
}