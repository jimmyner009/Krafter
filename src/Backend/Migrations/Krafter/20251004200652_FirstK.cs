using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend.Migrations.Krafter
{
    /// <inheritdoc />
    public partial class FirstK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KrafterUser",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedById = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<string>(type: "character varying(36)", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteReason = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    IsOwner = table.Column<bool>(type: "boolean", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KrafterUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KrafterUser_KrafterUser_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "KrafterUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KrafterUser_KrafterUser_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "KrafterUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRefreshTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRefreshTokens", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "KrafterRole",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedById = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<string>(type: "character varying(36)", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteReason = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KrafterRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KrafterRole_KrafterUser_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "KrafterUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KrafterRole_KrafterUser_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "KrafterUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KrafterUserClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", maxLength: 36, nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedById = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<string>(type: "character varying(36)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteReason = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    UserId = table.Column<string>(type: "character varying(36)", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KrafterUserClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KrafterUserClaim_KrafterUser_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "KrafterUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KrafterUserClaim_KrafterUser_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "KrafterUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KrafterUserClaim_KrafterUser_UserId",
                        column: x => x.UserId,
                        principalTable: "KrafterUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KrafterUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "character varying(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KrafterUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_KrafterUserLogins_KrafterUser_UserId",
                        column: x => x.UserId,
                        principalTable: "KrafterUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KrafterUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "character varying(36)", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KrafterUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_KrafterUserTokens_KrafterUser_UserId",
                        column: x => x.UserId,
                        principalTable: "KrafterUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KrafterRoleClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", maxLength: 36, nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedById = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<string>(type: "character varying(36)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteReason = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    RoleId = table.Column<string>(type: "character varying(36)", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KrafterRoleClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KrafterRoleClaim_KrafterRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "KrafterRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KrafterRoleClaim_KrafterUser_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "KrafterUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KrafterRoleClaim_KrafterUser_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "KrafterUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KrafterUserRole",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "character varying(36)", nullable: false),
                    RoleId = table.Column<string>(type: "character varying(36)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedById = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<string>(type: "character varying(36)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteReason = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KrafterUserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_KrafterUserRole_KrafterRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "KrafterRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KrafterUserRole_KrafterUser_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "KrafterUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KrafterUserRole_KrafterUser_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "KrafterUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KrafterUserRole_KrafterUser_UserId",
                        column: x => x.UserId,
                        principalTable: "KrafterUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KrafterRole_CreatedById",
                table: "KrafterRole",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_KrafterRole_NormalizedName_TenantId",
                table: "KrafterRole",
                columns: new[] { "NormalizedName", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KrafterRole_UpdatedById",
                table: "KrafterRole",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "KrafterRole",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KrafterRoleClaim_CreatedById",
                table: "KrafterRoleClaim",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_KrafterRoleClaim_RoleId",
                table: "KrafterRoleClaim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_KrafterRoleClaim_UpdatedById",
                table: "KrafterRoleClaim",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "KrafterUser",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_KrafterUser_CreatedById",
                table: "KrafterUser",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_KrafterUser_NormalizedEmail_TenantId",
                table: "KrafterUser",
                columns: new[] { "NormalizedEmail", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KrafterUser_NormalizedUserName_TenantId",
                table: "KrafterUser",
                columns: new[] { "NormalizedUserName", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KrafterUser_UpdatedById",
                table: "KrafterUser",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "KrafterUser",
                column: "NormalizedUserName");

            migrationBuilder.CreateIndex(
                name: "IX_KrafterUserClaim_CreatedById",
                table: "KrafterUserClaim",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_KrafterUserClaim_UpdatedById",
                table: "KrafterUserClaim",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_KrafterUserClaim_UserId",
                table: "KrafterUserClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KrafterUserLogins_UserId",
                table: "KrafterUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KrafterUserRole_CreatedById",
                table: "KrafterUserRole",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_KrafterUserRole_RoleId",
                table: "KrafterUserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_KrafterUserRole_UpdatedById",
                table: "KrafterUserRole",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KrafterRoleClaim");

            migrationBuilder.DropTable(
                name: "KrafterUserClaim");

            migrationBuilder.DropTable(
                name: "KrafterUserLogins");

            migrationBuilder.DropTable(
                name: "KrafterUserRole");

            migrationBuilder.DropTable(
                name: "KrafterUserTokens");

            migrationBuilder.DropTable(
                name: "UserRefreshTokens");

            migrationBuilder.DropTable(
                name: "KrafterRole");

            migrationBuilder.DropTable(
                name: "KrafterUser");
        }
    }
}
