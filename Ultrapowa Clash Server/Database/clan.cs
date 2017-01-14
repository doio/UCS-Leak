namespace UCS.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class clan
    {
        public long ClanId { get; set; }
        public System.DateTime LastUpdateTime { get; set; }
        public string Data { get; set; }
    }
}
