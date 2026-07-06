namespace MiniMES.Application.Authentication.Constrants;

public static class Permissions
{
    public static class Users
    {
        public const string View = "users:view";
        public const string Create = "users:create";
        public const string Update = "users:update";
        public const string Delete = "users:delete";
    }
    public static class Roles
    {
        public const string View = "roles:view";
        public const string Assign = "roles:assign";
    }
    public static class System
    {
        public const string Admin = "system:admin";
    }

}
