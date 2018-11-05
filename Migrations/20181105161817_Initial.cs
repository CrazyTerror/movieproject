using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MovieProject.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FilmItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    ReleaseDate = table.Column<DateTime>(nullable: true),
                    Runtime = table.Column<int>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    OriginalLanguage = table.Column<string>(nullable: true),
                    VoteCount = table.Column<int>(nullable: true),
                    VoteAverage = table.Column<float>(nullable: true),
                    Slug = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    Episode_SeasonNumber = table.Column<int>(nullable: true),
                    Episode_EpisodeNumber = table.Column<int>(nullable: true),
                    SeasonId = table.Column<int>(nullable: true),
                    Budget = table.Column<int>(nullable: true),
                    Revenue = table.Column<int>(nullable: true),
                    Season_SeasonNumber = table.Column<int>(nullable: true),
                    Season_EpisodeCount = table.Column<int>(nullable: true),
                    SeriesId = table.Column<int>(nullable: true),
                    Series_SeasonCount = table.Column<int>(nullable: true),
                    Series_EpisodeCount = table.Column<int>(nullable: true),
                    FirstAirDate = table.Column<DateTime>(nullable: true),
                    LastAirDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilmItem_FilmItem_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "FilmItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilmItem_FilmItem_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "FilmItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Slug = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    BirthDate = table.Column<DateTime>(nullable: true),
                    DeathDate = table.Column<DateTime>(nullable: true),
                    BirthPlace = table.Column<string>(nullable: true),
                    Biography = table.Column<string>(nullable: true),
                    Slug = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FilmItemId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_FilmItem_FilmItemId",
                        column: x => x.FilmItemId,
                        principalTable: "FilmItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trivia",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FilmItemId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trivia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trivia_FilmItem_FilmItemId",
                        column: x => x.FilmItemId,
                        principalTable: "FilmItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FilmItemId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Videos_FilmItem_FilmItemId",
                        column: x => x.FilmItemId,
                        principalTable: "FilmItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FilmItemGenres",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FilmItemId = table.Column<int>(nullable: false),
                    GenreId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmItemGenres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilmItemGenres_FilmItem_FilmItemId",
                        column: x => x.FilmItemId,
                        principalTable: "FilmItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilmItemGenres_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FilmItemCredits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FilmItemId = table.Column<int>(nullable: false),
                    PersonId = table.Column<int>(nullable: false),
                    PartType = table.Column<string>(nullable: true),
                    Character = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmItemCredits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilmItemCredits_FilmItem_FilmItemId",
                        column: x => x.FilmItemId,
                        principalTable: "FilmItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilmItemCredits_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FilmItemId = table.Column<int>(nullable: false),
                    PersonId = table.Column<int>(nullable: false),
                    IMDB = table.Column<string>(nullable: true),
                    TMDB = table.Column<string>(nullable: true),
                    Trakt = table.Column<string>(nullable: true),
                    OfficialSite = table.Column<string>(nullable: true),
                    Wikipedia = table.Column<string>(nullable: true),
                    Twitter = table.Column<string>(nullable: true),
                    Facebook = table.Column<string>(nullable: true),
                    Instagram = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Media_FilmItem_FilmItemId",
                        column: x => x.FilmItemId,
                        principalTable: "FilmItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Media_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilmItem_SeasonId",
                table: "FilmItem",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_FilmItem_SeriesId",
                table: "FilmItem",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_FilmItemCredits_FilmItemId",
                table: "FilmItemCredits",
                column: "FilmItemId");

            migrationBuilder.CreateIndex(
                name: "IX_FilmItemCredits_PersonId",
                table: "FilmItemCredits",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_FilmItemGenres_FilmItemId",
                table: "FilmItemGenres",
                column: "FilmItemId");

            migrationBuilder.CreateIndex(
                name: "IX_FilmItemGenres_GenreId",
                table: "FilmItemGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Media_FilmItemId",
                table: "Media",
                column: "FilmItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Media_PersonId",
                table: "Media",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_FilmItemId",
                table: "Photos",
                column: "FilmItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Trivia_FilmItemId",
                table: "Trivia",
                column: "FilmItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_FilmItemId",
                table: "Videos",
                column: "FilmItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "FilmItemCredits");

            migrationBuilder.DropTable(
                name: "FilmItemGenres");

            migrationBuilder.DropTable(
                name: "Media");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "Trivia");

            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "FilmItem");
        }
    }
}
