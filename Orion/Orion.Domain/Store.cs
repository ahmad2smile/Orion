using System.Collections.Generic;

namespace Orion.Domain
{
    public class Store
    {
        public IList<Channel> Channels { get; set; }
        public User User { get; set; }
    }
}
