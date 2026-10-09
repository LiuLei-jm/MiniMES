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
        public const string Create = "roles:create";
        public const string Update = "roles:update";
        public const string Delete = "roles:delete";
        public const string Assign = "roles:assign";
    }
    public static class System
    {
        public const string Admin = "system:admin";
    }
    public static class PermissionManagement
    {
        public const string View = "permissions:view";
        public const string Create = "permissions:create";
        public const string Update = "permissions:update";
        public const string Delete = "permissions:delete";
        public const string Assign = "permissions:assign";
    }
    public static class Audit
    {
        public const string View = "audit:view";
    }

}
