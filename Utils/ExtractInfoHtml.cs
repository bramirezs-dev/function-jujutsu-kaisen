using HtmlAgilityPack;
using System;
using System.Linq;

namespace FunctionJujutsu.Utils
{
    internal static class ExtractInfoHtml
    {
        public static HtmlNodeCollection ExtractSpecifyPartHtml(string html,string xpath)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var sectionPart = htmlDoc.DocumentNode
                .SelectNodes(xpath);
            return sectionPart;
        }
    }
}
