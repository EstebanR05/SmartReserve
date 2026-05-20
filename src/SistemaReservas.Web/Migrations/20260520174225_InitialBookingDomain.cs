using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaReservas.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialBookingDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccommodationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccommodationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsHighSeason = table.Column<bool>(type: "bit", nullable: false),
                    IsSpecialRate = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TouristSites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    City = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    SiteType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MaxCapacity = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TouristSites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccommodationUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TouristSiteId = table.Column<int>(type: "int", nullable: false),
                    AccommodationTypeId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    MaxCapacity = table.Column<int>(type: "int", nullable: false),
                    BedroomCount = table.Column<int>(type: "int", nullable: false),
                    BathroomCount = table.Column<int>(type: "int", nullable: false),
                    HasKitchen = table.Column<bool>(type: "bit", nullable: false),
                    HasParking = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccommodationUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccommodationUnits_AccommodationTypes_AccommodationTypeId",
                        column: x => x.AccommodationTypeId,
                        principalTable: "AccommodationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccommodationUnits_TouristSites_TouristSiteId",
                        column: x => x.TouristSiteId,
                        principalTable: "TouristSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TouristSiteId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContactFullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CheckInDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CheckOutDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Adults = table.Column<int>(type: "int", nullable: false),
                    Children = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_TouristSites_TouristSiteId",
                        column: x => x.TouristSiteId,
                        principalTable: "TouristSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RatePlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TouristSiteId = table.Column<int>(type: "int", nullable: false),
                    AccommodationTypeId = table.Column<int>(type: "int", nullable: false),
                    AccommodationUnitId = table.Column<int>(type: "int", nullable: true),
                    SeasonId = table.Column<int>(type: "int", nullable: false),
                    MinPeople = table.Column<int>(type: "int", nullable: false),
                    MaxPeople = table.Column<int>(type: "int", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdditionalPersonPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    RateType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatePlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RatePlans_AccommodationTypes_AccommodationTypeId",
                        column: x => x.AccommodationTypeId,
                        principalTable: "AccommodationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RatePlans_AccommodationUnits_AccommodationUnitId",
                        column: x => x.AccommodationUnitId,
                        principalTable: "AccommodationUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RatePlans_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RatePlans_TouristSites_TouristSiteId",
                        column: x => x.TouristSiteId,
                        principalTable: "TouristSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReservationUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    AccommodationUnitId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PeopleCount = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservationUnits_AccommodationUnits_AccommodationUnitId",
                        column: x => x.AccommodationUnitId,
                        principalTable: "AccommodationUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationUnits_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccommodationTypes_Name",
                table: "AccommodationTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccommodationUnits_AccommodationTypeId",
                table: "AccommodationUnits",
                column: "AccommodationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AccommodationUnits_TouristSiteId_Code",
                table: "AccommodationUnits",
                columns: new[] { "TouristSiteId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RatePlans_AccommodationTypeId",
                table: "RatePlans",
                column: "AccommodationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RatePlans_AccommodationUnitId",
                table: "RatePlans",
                column: "AccommodationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_RatePlans_SeasonId",
                table: "RatePlans",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_RatePlans_TouristSiteId_AccommodationTypeId_AccommodationUnitId_SeasonId_MinPeople_MaxPeople",
                table: "RatePlans",
                columns: new[] { "TouristSiteId", "AccommodationTypeId", "AccommodationUnitId", "SeasonId", "MinPeople", "MaxPeople" });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_CheckInDate_CheckOutDate",
                table: "Reservations",
                columns: new[] { "CheckInDate", "CheckOutDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TouristSiteId",
                table: "Reservations",
                column: "TouristSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_UserId",
                table: "Reservations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationUnits_AccommodationUnitId",
                table: "ReservationUnits",
                column: "AccommodationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationUnits_ReservationId_AccommodationUnitId",
                table: "ReservationUnits",
                columns: new[] { "ReservationId", "AccommodationUnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_StartDate_EndDate",
                table: "Seasons",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TouristSites_Name",
                table: "TouristSites",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RatePlans");

            migrationBuilder.DropTable(
                name: "ReservationUnits");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "AccommodationUnits");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "AccommodationTypes");

            migrationBuilder.DropTable(
                name: "TouristSites");
        }
    }
}
