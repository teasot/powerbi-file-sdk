using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PowerBIFileSDK
{
    public static class PageHelper
    {
        public static void PerformActionOnEachPage(string FileLocation, Action<JObject> ActionToPerform) 
        {
            FileHelper.PerformActionOnLayout(
                FileLocation, 
                (JObject Layout) =>
                {
                    JArray PagesArray = (JArray)Layout.SelectToken(JsonPaths.Pages);
                    foreach (JObject PageObject in PagesArray)
                        ActionToPerform(PageObject);
                }
            );
        }

        public static void RenamePage(JObject PageObject, string PageName)
        {
            PageObject.SelectToken(JsonPaths.PageDisplayName).Replace(PageName);
        }
    }
}
