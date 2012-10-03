//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VZF.Types.Data
{
    using System;
    
    public partial class prov_getuser_Result
    {
        public string UserID { get; set; }
        public System.Guid ApplicationID { get; set; }
        public string Username { get; set; }
        public string UsernameLwd { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordFormat { get; set; }
        public string Email { get; set; }
        public string EmailLwd { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public Nullable<bool> IsLockedOut { get; set; }
        public Nullable<System.DateTime> LastLogin { get; set; }
        public Nullable<System.DateTime> LastActivity { get; set; }
        public Nullable<System.DateTime> LastPasswordChange { get; set; }
        public Nullable<System.DateTime> LastLockOut { get; set; }
        public Nullable<int> FailedPasswordAttempts { get; set; }
        public Nullable<int> FailedAnswerAttempts { get; set; }
        public Nullable<System.DateTime> FailedPasswordWindow { get; set; }
        public Nullable<System.DateTime> FailedAnswerWindow { get; set; }
        public Nullable<System.DateTime> Joined { get; set; }
        public string Comment { get; set; }
    }
}
