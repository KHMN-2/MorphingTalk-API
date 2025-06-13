# PowerShell script to delete user from SQL Server database
# Usage: .\delete-user.ps1

# Database connection parameters
$ServerName = "db15713.public.databaseasp.net"
$DatabaseName = "db15713"
$Username = "db15713"
$Password = "2n=N?Zh9j_8P"
$EmailToDelete = "hossamaf15@gmail.com"

# SQL commands to delete the user and ALL related data
$SqlCommands = @"
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

-- Check if user exists
DECLARE @UserExists INT;
SELECT @UserExists = COUNT(*) FROM AspNetUsers WHERE Email = '$EmailToDelete';

IF @UserExists > 0
BEGIN
    PRINT 'User found. Proceeding with complete deletion...';
    
    -- Get the user ID
    DECLARE @UserId NVARCHAR(450);
    SELECT @UserId = Id FROM AspNetUsers WHERE Email = '$EmailToDelete';
    PRINT 'User ID: ' + @UserId;
    
    -- ========================================
    -- DELETE MESSAGES AND CONVERSATIONS
    -- ========================================
    
    -- Get ConversationUser IDs for this user
    DECLARE @ConversationUserIds TABLE (ConversationUserId UNIQUEIDENTIFIER);
    INSERT INTO @ConversationUserIds 
    SELECT Id FROM ConversationUsers WHERE UserId = @UserId;
    
    -- Delete all messages sent by this user
    DELETE FROM TextMessages 
    WHERE Id IN (
        SELECT Id FROM Messages 
        WHERE ConversationUserId IN (SELECT ConversationUserId FROM @ConversationUserIds)
    );
    
    DELETE FROM VoiceMessages 
    WHERE Id IN (
        SELECT Id FROM Messages 
        WHERE ConversationUserId IN (SELECT ConversationUserId FROM @ConversationUserIds)
    );
    
    -- Delete base messages
    DELETE FROM Messages 
    WHERE ConversationUserId IN (SELECT ConversationUserId FROM @ConversationUserIds);
    
    -- Get conversations where this user is the only participant
    DECLARE @ConversationsToDelete TABLE (ConversationId UNIQUEIDENTIFIER);
    INSERT INTO @ConversationsToDelete
    SELECT ConversationId 
    FROM ConversationUsers 
    WHERE ConversationId IN (
        SELECT ConversationId FROM ConversationUsers WHERE UserId = @UserId
    )
    GROUP BY ConversationId
    HAVING COUNT(*) = 1; -- Only delete if user is alone
    
    -- Delete ConversationUsers records for this user
    DELETE FROM ConversationUsers WHERE UserId = @UserId;
    
    -- Delete empty conversations (where this user was the only participant)
    DELETE FROM Conversations 
    WHERE Id IN (SELECT ConversationId FROM @ConversationsToDelete);
    
    -- ========================================
    -- DELETE FRIENDSHIPS
    -- ========================================
    
    -- Delete friendships where user is either party
    DELETE FROM Friendships WHERE UserId1 = @UserId OR UserId2 = @UserId;
    
    -- ========================================
    -- DELETE ASP.NET IDENTITY DATA
    -- ========================================
    
    -- Delete Identity related records
    DELETE FROM AspNetUserRoles WHERE UserId = @UserId;
    DELETE FROM AspNetUserClaims WHERE UserId = @UserId;
    DELETE FROM AspNetUserLogins WHERE UserId = @UserId;
    DELETE FROM AspNetUserTokens WHERE UserId = @UserId;
    
    -- ========================================
    -- DELETE USER PROFILE DATA
    -- ========================================
    
    -- Delete from Users table (if different from AspNetUsers)
    DELETE FROM Users WHERE Id = @UserId;
    
    -- ========================================
    -- FINALLY DELETE THE MAIN USER RECORD
    -- ========================================
    
    -- Delete the main user record
    DELETE FROM AspNetUsers WHERE Email = '$EmailToDelete';
    
    PRINT 'User and all related data deleted successfully.';
    
    -- Show deletion summary
    PRINT '========================================';
    PRINT 'DELETION SUMMARY:';
    PRINT '- User account: DELETED';
    PRINT '- All messages: DELETED';
    PRINT '- All conversations (where user was alone): DELETED';
    PRINT '- All friendships: DELETED';
    PRINT '- All identity data: DELETED';
    PRINT '========================================';
END
ELSE
BEGIN
    PRINT 'User with email $EmailToDelete not found.';
END

-- Verify complete deletion
PRINT 'VERIFICATION:';
SELECT COUNT(*) as RemainingUsers FROM AspNetUsers WHERE Email = '$EmailToDelete';
SELECT COUNT(*) as RemainingFriendships FROM Friendships WHERE UserId1 = (SELECT Id FROM AspNetUsers WHERE Email = '$EmailToDelete') OR UserId2 = (SELECT Id FROM AspNetUsers WHERE Email = '$EmailToDelete');
SELECT COUNT(*) as RemainingConversationUsers FROM ConversationUsers WHERE UserId = (SELECT Id FROM AspNetUsers WHERE Email = '$EmailToDelete');
"@

try {
    Write-Host "============================================" -ForegroundColor Magenta
    Write-Host "      COMPLETE USER DELETION SCRIPT        " -ForegroundColor Magenta
    Write-Host "============================================" -ForegroundColor Magenta
    Write-Host ""
    Write-Host "WARNING: This will delete ALL data related to the user!" -ForegroundColor Red
    Write-Host "Including:" -ForegroundColor Yellow
    Write-Host "  - User account" -ForegroundColor Yellow
    Write-Host "  - All messages sent by user" -ForegroundColor Yellow
    Write-Host "  - All conversations (if user was alone)" -ForegroundColor Yellow
    Write-Host "  - All friendships involving user" -ForegroundColor Yellow
    Write-Host "  - All identity/auth data" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Server: $ServerName" -ForegroundColor Cyan
    Write-Host "Database: $DatabaseName" -ForegroundColor Cyan
    Write-Host "Email to delete: $EmailToDelete" -ForegroundColor Red
    Write-Host ""
    
    # Ask for confirmation
    $confirmation = Read-Host "Are you sure you want to proceed? Type 'YES' to confirm"
    if ($confirmation -ne "YES") {
        Write-Host "Operation cancelled by user." -ForegroundColor Yellow
        exit
    }
    
    Write-Host ""
    Write-Host "Connecting to database and executing deletion..." -ForegroundColor Yellow
    
    # Execute the SQL commands using sqlcmd
    $SqlCommands | sqlcmd -S $ServerName -d $DatabaseName -U $Username -P $Password -N -C
      if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "============================================" -ForegroundColor Green
        Write-Host "    DELETION COMPLETED SUCCESSFULLY!       " -ForegroundColor Green
        Write-Host "============================================" -ForegroundColor Green
    } else {
        Write-Host ""
        Write-Host "Script execution failed with exit code: $LASTEXITCODE" -ForegroundColor Red
    }
}
catch {
    Write-Host "Error occurred: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
