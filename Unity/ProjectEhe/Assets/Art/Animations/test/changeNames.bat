@ECHO OFF
setlocal enabledelayedexpansion
SET mypath=%~dp0

set missing=0
set arg1="null"
set arg2="null"
set arg1Prompted="null"
set /p arg1="old string: "
if %arg1%=="null" (
set missing=1
goto continue
)

:promptAgain
set /p arg2="new string: "
IF %arg2%=="" (
echo parameter cant be null, try again.
goto promptAgain
) else (
goto continue
)
:continue

del /Q extract

for /f "delims=" %%a in ('dir /b *.zip') do (
set currentFile=%%a

unzip !currentFile! -d extract/
cd %mypath%/extract
for /f "delims=" %%a in ('dir /b *.fbx') do (
if %%a==Beta.fbx (
del  Beta.fbx > nul
) else (
rename %%a !currentFile:~0,-4!.fbx > nul
mv !currentFile:~0,-4!.fbx ../
)

)

cd %mypath%
del %%a
)

echo perkele
echo. > temp.txt
for /f "delims=" %%a in ('dir /b *.fbx') do (
echo %%a >> temp.txt
)

IF !missing!==1 (
  for /f "tokens=*" %%a in (temp.txt) do (
    echo %%a | FINDSTR anim_common
    if errorlevel 1 if not errorlevel 2 rename %%a anim_common_%%a > nul
  )
) ELSE (
for /f "tokens=*" %%a in (temp.txt) do (
set str=%%a
set newName=!str:%arg1%=%arg2%!
rem echo %%a | FINDSTR anim_common >nul
rem if errorlevel 0 if not errorlevel 1 rename %%a !newName! > nul
rename %%a !newName! > nul
)
)
del temp.txt
