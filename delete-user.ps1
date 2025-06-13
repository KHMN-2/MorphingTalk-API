# PowerShell script to delete user from SQL Server database
# Usage: .\delete-user.ps1

# Database connection parameters
$ServerName = "db15713.public.databaseasp.net"
$DatabaseName = "db15713"
$Username = "db15713"
$Password = "2n=N?Zh9j_8P"
$EmailToDelete = "hossamaf15@gmail.com"

# SQL commands to delete the user
$SqlCommands = @"
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

-- Check if user exists
DECLARE @UserExists INT;
SELECT @UserExists = COUNT(*) FROM AspNetUsers WHERE Email = '$EmailToDelete';

IF @UserExists > 0
BEGIN
    PRINT 'User found. Proceeding with deletion...';
    
    -- Get the user ID
    DECLARE @UserId NVARCHAR(450);
    SELECT @UserId = Id FROM AspNetUsers WHERE Email = '$EmailToDelete';
    
    -- Delete related records first (to avoid foreign key constraints)
    DELETE FROM AspNetUserRoles WHERE UserId = @UserId;
    DELETE FROM AspNetUserClaims WHERE UserId = @UserId;
    DELETE FROM AspNetUserLogins WHERE UserId = @UserId;
    DELETE FROM AspNetUserTokens WHERE UserId = @UserId;
    
    -- Delete any custom related data (add more if needed)
    -- DELETE FROM YourCustomTable WHERE UserId = @UserId;
    
    -- Finally delete the user
    DELETE FROM AspNetUsers WHERE Email = '$EmailToDelete';
    
    PRINT 'User deleted successfully.';
END
ELSE
BEGIN
    PRINT 'User with email $EmailToDelete not found.';
END

-- Verify deletion
SELECT COUNT(*) as RemainingUsers FROM AspNetUsers WHERE Email = '$EmailToDelete';
"@

try {
    Write-Host "Connecting to database..." -ForegroundColor Yellow
    Write-Host "Server: $ServerName" -ForegroundColor Cyan
    Write-Host "Database: $DatabaseName" -ForegroundColor Cyan
    Write-Host "Email to delete: $EmailToDelete" -ForegroundColor Red
    Write-Host ""
    
    # Execute the SQL commands using sqlcmd
    $SqlCommands | sqlcmd -S $ServerName -d $DatabaseName -U $Username -P $Password -N -C
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "Script executed successfully!" -ForegroundColor Green
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
