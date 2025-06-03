﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HabitsApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHabitEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Habits",
                newName: "Color");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Color",
                table: "Habits",
                newName: "Title");
        }
    }
}
