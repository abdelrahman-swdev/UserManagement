using Microsoft.EntityFrameworkCore.Migrations;

namespace UserManagement.Data.Migrations
{
    public partial class SeedAdminUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "INSERT INTO [Security].[Users] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FullName], [ProfilePicture]) VALUES (N'459d17cd-6509-468f-a039-c698b3203d6d', N'admin', N'ADMIN', N'admin@admin.com', N'ADMIN@ADMIN.COM', 0, N'AQAAAAEAACcQAAAAED0VAlYXhO2RsNIZ15PHqvF+t7phQQGG10pYfSKo4eFfminLQZP/MVB0PWIpY7TUug==', N'FLAMM4SWL3RTCFQVVIDWWQR535BWTUQV', N'5c2dc898-dc6f-430b-b9af-89eb5ebdfffe', NULL, 0, 0, NULL, 1, 0, N'Admin User', null)"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [Security].[Users] WHERE Id = '459d17cd-6509-468f-a039-c698b3203d6d'");
        }
    }
}
