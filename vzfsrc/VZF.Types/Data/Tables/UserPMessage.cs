//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VZF.Types.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class yaf_UserPMessage
    {
        public int UserPMessageID { get; set; }
        public int UserID { get; set; }
        public int PMessageID { get; set; }
        public int Flags { get; set; }
        public Nullable<bool> IsRead { get; set; }
        public Nullable<bool> IsInOutbox { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public bool IsReply { get; set; }
    
        public virtual yaf_PMessage yaf_PMessage { get; set; }
        public virtual yaf_User yaf_User { get; set; }
    }
}
