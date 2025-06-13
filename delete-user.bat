#!/bin/bash
# Alternative batch version for Windows
@echo off
echo Deleting user hossamaf15@gmail.com from database...
echo.

sqlcmd -S db15713.public.databaseasp.net -d db15713 -U db15713 -P "2n=N?Zh9j_8P" -N -C -Q "SET QUOTED_IDENTIFIER ON; SET ANSI_NULLS ON; DECLARE @UserId NVARCHAR(450); SELECT @UserId = Id FROM AspNetUsers WHERE Email = 'hossamaf15@gmail.com'; IF @UserId IS NOT NULL BEGIN DELETE FROM AspNetUserRoles WHERE UserId = @UserId; DELETE FROM AspNetUserClaims WHERE UserId = @UserId; DELETE FROM AspNetUserLogins WHERE UserId = @UserId; DELETE FROM AspNetUserTokens WHERE UserId = @UserId; DELETE FROM AspNetUsers WHERE Email = 'hossamaf15@gmail.com'; PRINT 'User deleted successfully.'; END ELSE PRINT 'User not found.'; SELECT COUNT(*) as RemainingUsers FROM AspNetUsers WHERE Email = 'hossamaf15@gmail.com';"

echo.
echo Done!
pause
