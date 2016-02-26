@echo off

SET config=Debug

IF NOT "%1"=="" (
    SET config=%1
)

rem %windir%\microsoft.net\framework\v4.0.30319\msbuild /m PsiBB.sln /p:Configuration=%config%
"%ProgramFiles(X86)%\MSBuild\14.0\Bin\MSBuild.exe" /m PsiBB.sln /p:Configuration=%config%

@IF %ERRORLEVEL% NEQ 0 GOTO err

@exit /B 0

:err
@PAUSE
@exit /B 1
