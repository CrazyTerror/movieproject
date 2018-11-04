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
                name: "Genres",
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
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    ReleaseDate = table.Column<DateTime>(nullable: true),
                    Runtime = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Budget = table.Column<int>(nullable: true),
                    Revenue = table.Column<int>(nullable: true),
                    VoteCount = table.Column<int>(nullable: true),
                    VoteAverage = table.Column<float>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Slug = table.Column<string>(nullable: true),
                    OfficialSite = table.Column<string>(nullable: true),
                    IMDB = table.Column<string>(nullable: true),
                    TMDB = table.Column<string>(nullable: true),
                    Wikipedia = table.Column<string>(nullable: true),
                    Twitter = table.Column<string>(nullable: true),
                    Facebook = table.Column<string>(nullable: true),
                    Instagram = table.Column<string>(nullable: true),
                    Poster = table.Column<string>(nullable: true),
                    Banner = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
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
                    Homepage = table.Column<string>(nullable: true),
                    IMDB = table.Column<string>(nullable: true),
                    TMDB = table.Column<string>(nullable: true),
                    Wikipedia = table.Column<string>(nullable: true),
                    Twitter = table.Column<string>(nullable: true),
                    Facebook = table.Column<string>(nullable: true),
                    Instagram = table.Column<string>(nullable: true),
                    Slug = table.Column<string>(nullable: true),
                    Poster = table.Column<string>(nullable: true),
                    Banner = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Series",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    NumberOfSeasons = table.Column<int>(nullable: true),
                    NumberOfEpisodes = table.Column<int>(nullable: true),
                    FirstAirDate = table.Column<DateTime>(nullable: true),
                    LastAirDate = table.Column<DateTime>(nullable: true),
                    EpisodeRunTime = table.Column<int>(nullable: true),
                    OriginalLanguage = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    VoteCount = table.Column<int>(nullable: true),
                    VoteAverage = table.Column<float>(nullable: true),
                    OfficialSite = table.Column<string>(nullable: true),
                    IMDB = table.Column<string>(nullable: true),
                    TMDB = table.Column<string>(nullable: true),
                    Wikipedia = table.Column<string>(nullable: true),
                    Twitter = table.Column<string>(nullable: true),
                    Facebook = table.Column<string>(nullable: true),
                    Instagram = table.Column<string>(nullable: true),
                    Slug = table.Column<string>(nullable: true),
                    Poster = table.Column<string>(nullable: true),
                    Banner = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MovieGenre",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MovieId = table.Column<int>(nullable: false),
                    GenreId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieGenre", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieGenre_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieGenre_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieCredits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MovieId = table.Column<int>(nullable: false),
                    PersonId = table.Column<int>(nullable: false),
                    Character = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieCredits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieCredits_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieCredits_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SeasonNumber = table.Column<int>(nullable: true),
                    EpisodeCount = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    AirDate = table.Column<DateTime>(nullable: true),
                    IMDB = table.Column<string>(nullable: true),
                    TMDB = table.Column<string>(nullable: true),
                    Wikipedia = table.Column<string>(nullable: true),
                    Poster = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    SeriesId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seasons_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeriesGenre",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SeriesId = table.Column<int>(nullable: false),
                    GenreId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriesGenre", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeriesGenre_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeriesGenre_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Episodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    AirDate = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    EpisodeNumber = table.Column<int>(nullable: true),
                    SeasonNumber = table.Column<int>(nullable: true),
                    Runtime = table.Column<int>(nullable: true),
                    VoteCount = table.Column<int>(nullable: true),
                    VoteAverage = table.Column<float>(nullable: true),
                    IMDB = table.Column<string>(nullable: true),
                    TMDB = table.Column<string>(nullable: true),
                    Wikipedia = table.Column<string>(nullable: true),
                    Banner = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    SeasonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Episodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Episodes_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EpisodeCredits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EpisodeId = table.Column<int>(nullable: false),
                    PersonId = table.Column<int>(nullable: false),
                    Character = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpisodeCredits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EpisodeCredits_Episodes_EpisodeId",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EpisodeCredits_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeCredits_EpisodeId",
                table: "EpisodeCredits",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeCredits_PersonId",
                table: "EpisodeCredits",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_SeasonId",
                table: "Episodes",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieCredits_MovieId",
                table: "MovieCredits",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieCredits_PersonId",
                table: "MovieCredits",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenre_GenreId",
                table: "MovieGenre",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenre_MovieId",
                table: "MovieGenre",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_SeriesId",
                table: "Seasons",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_SeriesGenre_GenreId",
                table: "SeriesGenre",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_SeriesGenre_SeriesId",
                table: "SeriesGenre",
                column: "SeriesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "EpisodeCredits");

            migrationBuilder.DropTable(
                name: "MovieCredits");

            migrationBuilder.DropTable(
                name: "MovieGenre");

            migrationBuilder.DropTable(
                name: "SeriesGenre");

            migrationBuilder.DropTable(
                name: "Episodes");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "Series");
        }
    }
}
