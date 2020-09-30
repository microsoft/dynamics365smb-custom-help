using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldTopicTextExtractor
{
    class DomParser
    {
        internal static FieldTopicInfo getFieldTopicSummary(FileInfo file)
        {

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.Load(file.FullName);

            FieldTopicInfo fieldTopicInfo = new FieldTopicInfo()
            {
                Id = Path.GetFileNameWithoutExtension(file.Name),
                Text = htmlDocument.DocumentNode.SelectNodes(".//p").First().InnerHtml.Trim()
            };
            return fieldTopicInfo;
        }
    }
}
