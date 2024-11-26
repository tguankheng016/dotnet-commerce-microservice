using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CommerceMicro.ProductService.Application.Data.Migrations
{
    /// <inheritdoc />
    public partial class Added_Product : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    stock_quantity = table.Column<int>(type: "integer", nullable: false),
                    category_id = table.Column<int>(type: "integer", nullable: true),
                    version = table.Column<long>(type: "bigint", nullable: false),
                    creator_user_id = table.Column<long>(type: "bigint", nullable: true),
                    creation_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modifier_user_id = table.Column<long>(type: "bigint", nullable: true),
                    last_modification_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleter_user_id = table.Column<long>(type: "bigint", nullable: true),
                    deletion_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                    table.ForeignKey(
                        name: "fk_products_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_products_category_id",
                table: "products",
                column: "category_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "products");
        }
    }
}
