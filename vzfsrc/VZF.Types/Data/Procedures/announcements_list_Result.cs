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
    
    public partial class announcements_list_Result
    {
        public int ForumID { get; set; }
        public int TopicID { get; set; }
        public System.DateTime Posted { get; set; }
        public int LinkTopicID { get; set; }
        public Nullable<int> TopicMovedID { get; set; }
        public Nullable<int> FavoriteCount { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Styles { get; set; }
        public int UserID { get; set; }
        public string Starter { get; set; }
        public string StarterDisplay { get; set; }
        public Nullable<int> Replies { get; set; }
        public Nullable<int> NumPostsDeleted { get; set; }
        public int Views { get; set; }
        public Nullable<System.DateTime> LastPosted { get; set; }
        public Nullable<int> LastUserID { get; set; }
        public string LastUserName { get; set; }
        public string LastUserDisplayName { get; set; }
        public Nullable<int> LastMessageID { get; set; }
        public int LastTopicID { get; set; }
        public int TopicFlags { get; set; }
        public short Priority { get; set; }
        public Nullable<int> PollID { get; set; }
        public int ForumFlags { get; set; }
        public string FirstMessage { get; set; }
        public string StarterStyle { get; set; }
        public string LastUserStyle { get; set; }
        public Nullable<System.DateTime> LastForumAccess { get; set; }
        public Nullable<System.DateTime> LastTopicAccess { get; set; }
        public Nullable<int> TotalRows { get; set; }
        public Nullable<int> PageIndex { get; set; }
    }
}
