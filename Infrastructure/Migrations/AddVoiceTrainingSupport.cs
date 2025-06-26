using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVoiceTrainingSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add voice training fields if they don't exist
            // Note: These may already exist from previous migrations
            // Check your User entity to confirm these fields are properly defined

            // Ensure IsTrainedVoice column exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = 'IsTrainedVoice')
                BEGIN
                    ALTER TABLE [AspNetUsers] ADD [IsTrainedVoice] bit NOT NULL DEFAULT 0
                END
            ");

            // Ensure VoiceModel support exists 
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserVoiceModels')
                BEGIN
                    CREATE TABLE [UserVoiceModels] (
                        [Id] nvarchar(450) NOT NULL,
                        [Name] nvarchar(max) NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_UserVoiceModels] PRIMARY KEY ([Id])
                    )
                END
            ");

            // Add foreign key if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = 'VoiceModelId')
                BEGIN
                    ALTER TABLE [AspNetUsers] ADD [VoiceModelId] nvarchar(450) NULL
                    ALTER TABLE [AspNetUsers] ADD CONSTRAINT [FK_AspNetUsers_UserVoiceModels_VoiceModelId] 
                        FOREIGN KEY ([VoiceModelId]) REFERENCES [UserVoiceModels] ([Id])
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove voice training fields
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AspNetUsers_UserVoiceModels_VoiceModelId')
                BEGIN
                    ALTER TABLE [AspNetUsers] DROP CONSTRAINT [FK_AspNetUsers_UserVoiceModels_VoiceModelId]
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = 'VoiceModelId')
                BEGIN
                    ALTER TABLE [AspNetUsers] DROP COLUMN [VoiceModelId]
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.tables WHERE name = 'UserVoiceModels')
                BEGIN
                    DROP TABLE [UserVoiceModels]
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = 'IsTrainedVoice')
                BEGIN
                    ALTER TABLE [AspNetUsers] DROP COLUMN [IsTrainedVoice]
                END
            ");
        }
    }
}
