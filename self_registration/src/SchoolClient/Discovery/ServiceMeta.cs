using System.Collections.Generic;

namespace SchoolClient
{
    public class ServiceMeta
    {
        public ServiceMeta(string path, IEnumerable<string> verbs)
        {
            Path = path;
            Verbs = verbs;
        }
        public string Path { get; }
        public IEnumerable<string> Verbs { get; }
    }
}