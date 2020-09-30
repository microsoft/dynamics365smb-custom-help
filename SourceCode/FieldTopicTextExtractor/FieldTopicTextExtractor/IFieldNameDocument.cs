using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldTopicTextExtractor
{
    public interface IFieldNameDocument
    {
        List<FieldTopicInfo> FieldTopics { get; set; }
        void Save(string file);
    }
}
