using System.Collections.Generic;

namespace ROTools.Yaml
{
    public class Root<T>
    {
        public Header Header { get; set; }
        public List<T> Body { get; set; }
    }
}