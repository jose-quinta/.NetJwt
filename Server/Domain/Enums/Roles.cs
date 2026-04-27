namespace Server.Domain.Enums {
    public enum Roles {
        Admin,
        User,
        Moderator
    }

    public enum Permissions {
        ReadUsers,
        WriteUsers,
        DeleteUsers,
        ReadProducts,
        WriteProducts,
        DeleteProducts,
        ManageRoles
    }
}