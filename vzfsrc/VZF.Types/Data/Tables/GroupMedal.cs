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
    
    public partial class yaf_GroupMedal
    {
        public int GroupID { get; set; }
        public int MedalID { get; set; }
        public string Message { get; set; }
        public bool Hide { get; set; }
        public bool OnlyRibbon { get; set; }
        public byte SortOrder { get; set; }
    
        public virtual yaf_Group yaf_Group { get; set; }
        public virtual yaf_Medal yaf_Medal { get; set; }
    }
}