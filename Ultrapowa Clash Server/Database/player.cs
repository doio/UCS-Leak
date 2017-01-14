namespace UCS.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class player
    {
        public long PlayerId { get; set; }
        public byte AccountStatus { get; set; }
        public byte AccountPrivileges { get; set; }
        public System.DateTime LastUpdateTime { get; set; }
        public string IPAddress { get; set; }
        public string Avatar { get; set; }
        public string GameObjects { get; set; }
    }
}
