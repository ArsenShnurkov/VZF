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
    
    public partial class yaf_Choice
    {
        public int ChoiceID { get; set; }
        public int PollID { get; set; }
        public string Choice { get; set; }
        public int Votes { get; set; }
        public string ObjectPath { get; set; }
        public string MimeType { get; set; }
    
        public virtual yaf_Poll yaf_Poll { get; set; }
    }
}