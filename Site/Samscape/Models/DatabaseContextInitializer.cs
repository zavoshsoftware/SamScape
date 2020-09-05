using System;

namespace Models
{
    internal static class DatabaseContextInitializer
    {
        static DatabaseContextInitializer()
        {

        }

        internal static void Seed(DatabaseContext databaseContext)
        {
            InitialRoles(databaseContext);
        }

        public static void InitialRoles(DatabaseContext databaseContext)
        {
            InsertRole("69423eb7-d852-4294-aa59-1168a0f72d93", "superadministrator", "راهبر ویژه", databaseContext);
            InsertRole("499c14c7-e89d-4053-b499-7b8c6c90ef0b", "administrator", "راهبر", databaseContext);
            InsertRole("7fbf2f0b-9df5-4c37-a004-9c98173a26dd", "customer", "مشتری", databaseContext);
        }

        public static void InsertRole(string roleId, string roleName, string roleTitle, DatabaseContext databaseContext)
        {
            Guid id = new Guid(roleId);
            Role role = new Role();
            role.Id = id;
            role.Title = roleTitle;
            role.Name = roleName;
            role.CreationDate = DateTime.Now;
            role.IsActive = true;
            role.IsDeleted = false;

            databaseContext.Roles.Add(role);
            databaseContext.SaveChanges();
        }

    }
}
