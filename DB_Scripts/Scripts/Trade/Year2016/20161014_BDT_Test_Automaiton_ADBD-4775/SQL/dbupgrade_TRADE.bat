@echo off
REM Test parameters
IF %1NOTHING==NOTHING GOTO WARNING
IF %2NOTHING==NOTHING GOTO WARNING
IF %3NOTHING==NOTHING GOTO WARNING
IF %4NOTHING==NOTHING GOTO WARNING
REM
set ISQL="sqlcmd.exe"
set SQLCMDLOGINTIMEOUT=0
REM
echo Upgrading ICTS trade database (MS SQL Server) for issue #ADSO-4774 ...
REM ****************************************************************************
echo ****************************************************************************
echo Updating 'associated_cpty' column in inventory_build_draw table
%ISQL% -S %1 -U %2 -P %3 -d %4 -w 200 -i UUU_transdata_chgs\upd_inventory_build_draw_data.sql
echo ****************************************************************************
REM ****************************************************************************
echo Done
goto end
:WARNING
Echo Usage: dbupgrade_TRADE [server] [login] [password] [database]
:end
