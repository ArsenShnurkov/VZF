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
    
    public partial class yaf_BannedIP
    {
        public int ID { get; set; }
        public int BoardID { get; set; }
        public string Mask { get; set; }
        public System.DateTime Since { get; set; }
        public string Reason { get; set; }
        public Nullable<int> UserID { get; set; }
    
        public virtual yaf_Board yaf_Board { get; set; }
    }
}
