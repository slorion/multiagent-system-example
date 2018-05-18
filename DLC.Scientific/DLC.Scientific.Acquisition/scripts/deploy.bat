cls

set root=%~dp0

echo %root%

IF [%package%] EQU [] set package=%1
IF [%platform%] EQU [] set platform=%2
IF [%build%] EQU [] set build=%3
IF [%custom%] EQU [] set custom=%4
IF [%removeAgentConfig%] EQU [] set removeAgentConfig=%5

IF [%platform%] EQU [AnyCpu] set platformBinFolder=
IF [%platform%] NEQ [AnyCpu] set platformBinFolder=%platform%

REM ===============================================================================
REM Create package
REM ===============================================================================

set output_path=.\%package%

del "%root%\tmp\*.*" /q /s
xcopy "%output_path%\*.lnk" "%root%\tmp\" /Y

rmdir /s /q "%output_path%"
md "%output_path%"
md "%output_path%\Agents"
md "%output_path%\Modules"

xcopy "%root%\tmp" "%output_path%"

REM ===============================================================================
REM Copy librairies (must be done first)
REM ===============================================================================

xcopy "%root%\..\BuildMeToDeploy\bin\%platformBinFolder%\%build%" "%output_path%\libs\" /S /Y
del "%output_path%\libs\BuildMeToDeploy.*" /q /f

REM ===============================================================================
REM Copy modules
REM ===============================================================================

md "%output_path%\Modules"
move /Y "%output_path%\libs\*Module.*" "%output_path%\Modules"

REM ===============================================================================
REM Copy agents
REM ===============================================================================

md "%output_path%\Agents"
move /Y "%output_path%\libs\*Agent.*" "%output_path%\Agents"
del "%output_path%\Agents\*Multiagent.*" /q /f

REM ===============================================================================
REM Config
REM ===============================================================================

move /Y "%output_path%\libs\*.conf" "%output_path%\Agents"

xcopy "%root%\..\config" "%output_path%\config\" /E /Y

IF [%config%] NEQ [] (
	del "%output_path%\config\*.*" /q
	xcopy "%output_path%\config\%config%\*.*" "%output_path%\config\"
	xcopy "%root%\..\config\nlog.conf" "%output_path%\config\" /E /Y
	FOR /D %%p IN ("%output_path%\config\*.*") DO rmdir "%%p" /s /q
)

REM ===============================================================================
REM Copy multiagent
REM ===============================================================================

powershell -Command "dir %root%\..\packages\dlc.multiagent.desktopapp-%platform%* | Sort-Object { [Version] $(if ($_.BaseName -match '(\d+\.){3}\d+') { $matches[0] } else { '0.0.0.0' }) } | select -last 1 -ExpandProperty Name" > ma_dir.txt
set /p ma_dir=<ma_dir.txt
xcopy "%root%\..\packages\%ma_dir%\exe" "%output_path%\" /E /Y
del ma_dir.txt /q/f

REM ===============================================================================
REM Copy custom files
REM ===============================================================================
xcopy ".\%custom%" "%output_path%\" /E /Y

REM ===============================================================================
REM Delete config
REM ===============================================================================
IF [%removeAgentConfig%] EQU [true] del /s /q %output_path%\*.conf

REM ===============================================================================
REM Copy UI files
REM ===============================================================================
robocopy "%output_path%\libs\UI" "%output_path%\UI" /E /IS /MOVE /NFL /NDL /NJH /NJS /NC /NS /NP

pause

REM force return code 0 (without leaving batch)
ver > nul