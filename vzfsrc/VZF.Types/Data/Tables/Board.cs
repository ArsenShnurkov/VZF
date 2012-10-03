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
    
    public partial class yaf_Board
    {
        public yaf_Board()
        {
            this.yaf_AccessMask = new HashSet<yaf_AccessMask>();
            this.yaf_Active = new HashSet<yaf_Active>();
            this.yaf_BannedIP = new HashSet<yaf_BannedIP>();
            this.yaf_BBCode = new HashSet<yaf_BBCode>();
            this.yaf_Category = new HashSet<yaf_Category>();
            this.yaf_Extension = new HashSet<yaf_Extension>();
            this.yaf_Group = new HashSet<yaf_Group>();
            this.yaf_NntpServer = new HashSet<yaf_NntpServer>();
            this.yaf_Rank = new HashSet<yaf_Rank>();
            this.yaf_Registry = new HashSet<yaf_Registry>();
            this.yaf_Smiley = new HashSet<yaf_Smiley>();
            this.yaf_User = new HashSet<yaf_User>();
        }
    
        public int BoardID { get; set; }
        public string Name { get; set; }
        public bool AllowThreaded { get; set; }
        public string MembershipAppName { get; set; }
        public string RolesAppName { get; set; }
    
        public virtual ICollection<yaf_AccessMask> yaf_AccessMask { get; set; }
        public virtual ICollection<yaf_Active> yaf_Active { get; set; }
        public virtual ICollection<yaf_BannedIP> yaf_BannedIP { get; set; }
        public virtual ICollection<yaf_BBCode> yaf_BBCode { get; set; }
        public virtual ICollection<yaf_Category> yaf_Category { get; set; }
        public virtual ICollection<yaf_Extension> yaf_Extension { get; set; }
        public virtual ICollection<yaf_Group> yaf_Group { get; set; }
        public virtual ICollection<yaf_NntpServer> yaf_NntpServer { get; set; }
        public virtual ICollection<yaf_Rank> yaf_Rank { get; set; }
        public virtual ICollection<yaf_Registry> yaf_Registry { get; set; }
        public virtual ICollection<yaf_Smiley> yaf_Smiley { get; set; }
        public virtual ICollection<yaf_User> yaf_User { get; set; }
    }
}
