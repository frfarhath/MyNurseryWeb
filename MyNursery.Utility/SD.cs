namespace MyNursery.Utility
{
    public static class SD
    {
        public const string Role_Admin = "NuAD";
        public const string Role_OtherUser = "NuUS"; //Admin Added
        public const string Role_User = "NuOUS"; //Registered User

        public const string Success_Msg = "success";  
        public const string Info_Msg = "info";        
        public const string Warning_Msg = "warning";  
        public const string Error_Msg = "danger";     

        public const string Status_Active = "Y";
        public const string Status_Deactive = "N";
        public const string Status_Approved = "APP";
        public const string Status_Pending = "PEN";
        public const string Status_Inprocess = "INP";
        public const string Status_Suspended = "SUS";
        public const string Status_Disapproved = "DIS";
        public const string Status_Yes = "Y";
        public const string Status_No = "N";
        public const string Status_Paid = "PAID";

        //Blogs
        public const string Status_Published = "Published";
        public const string Status_Draft = "Draft";
        public const string Status_Archived = "Archived";

        //Users
        public const string Role_SuperAdmin = "NuSAD";
        public const string Role_AdminCSAD = "CsAD";

        // User Types
        public const string UserType_Predefined = "Predefined";
        public const string UserType_Registered = "Registered";
        public const string UserType_AdminAdded = "AdminAdded";
    }
}