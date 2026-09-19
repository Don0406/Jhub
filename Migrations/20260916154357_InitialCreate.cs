using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace _UPDATED__FrontEnd_Capstone.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Users",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sex",
                table: "Users",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Users",
                type: "longtext",
                nullable: false);

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ReferenceNumber = table.Column<string>(type: "longtext", nullable: false),
                    TourId = table.Column<int>(type: "int", nullable: false),
                    LeadName = table.Column<string>(type: "longtext", nullable: false),
                    LeadAge = table.Column<int>(type: "int", nullable: false),
                    LeadEmail = table.Column<string>(type: "longtext", nullable: false),
                    LeadPhone = table.Column<string>(type: "longtext", nullable: false),
                    SpecialCategory = table.Column<string>(type: "longtext", nullable: false),
                    CompanionNames = table.Column<string>(type: "longtext", nullable: true),
                    Slots = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "longtext", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ProofOfPaymentUrl = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    TourName = table.Column<string>(type: "longtext", nullable: false),
                    Destination = table.Column<string>(type: "longtext", nullable: false),
                    DepartureDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OperationalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCapacity = table.Column<int>(type: "int", nullable: false),
                    SlotsFilled = table.Column<int>(type: "int", nullable: false),
                    MinParticipants = table.Column<int>(type: "int", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    VehicleAssignment = table.Column<string>(type: "longtext", nullable: true),
                    AccommodationAssignment = table.Column<string>(type: "longtext", nullable: true),
                    ImageUrl = table.Column<string>(type: "longtext", nullable: true),
                    ItineraryHighlights = table.Column<string>(type: "longtext", nullable: true),
                    Status = table.Column<string>(type: "longtext", nullable: false),
                    IsBoosted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    BoostedUntil = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    VendorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    VendorName = table.Column<string>(type: "longtext", nullable: false),
                    ServiceType = table.Column<string>(type: "longtext", nullable: false),
                    ContactInfo = table.Column<string>(type: "longtext", nullable: false),
                    PaymentTerms = table.Column<string>(type: "LONGTEXT", nullable: false),
                    Payables = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "longtext", nullable: false),
                    TourId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vendors_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TourId",
                table: "Bookings",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_VendorId",
                table: "Tours",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_TourId",
                table: "Vendors",
                column: "TourId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Tours_TourId",
                table: "Bookings",
                column: "TourId",
                principalTable: "Tours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tours_Vendors_VendorId",
                table: "Tours",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Tours_TourId",
                table: "Vendors");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Tours");

            migrationBuilder.DropTable(
                name: "Vendors");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Sex",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");
        }
    }
}
