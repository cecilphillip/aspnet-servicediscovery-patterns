using System.Collections.Generic;

namespace SchoolClient
{
    public class ServiceWrapper<T> where T : class
    {
        public T Service { get; set; }
        public IEnumerable<ServiceMeta> Meta { get; set; }
    }
}