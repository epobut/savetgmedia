@echo off
setlocal enabledelayedexpansion
title TelegramSaver Installer

echo [*] Shaking hands with Windows...

:: 1. Шукаємо шлях до компілятора (csc.exe)
set "csc="
for /r "C:\Windows\Microsoft.NET\Framework64" %%f in (csc.exe) do (
    if exist "%%f" set "csc=%%f"
)

if not defined csc (
    echo [!] Error: .NET Framework not found.
    pause
    exit /b
)

:: 2. Визначаємо назви та шляхи (все відносно поточної папки)
set "source=%~dp0TelegramSaver.cs"
set "exe_name=TelegramSaver.exe"
set "target_dir=%APPDATA%\TelegramSaver"
set "target_exe=%target_dir%\%exe_name%"

if not exist "%source%" (
    echo [!] Error: TelegramSaver.cs not found in this folder.
    pause
    exit /b
)

:: 3. Створюємо папку в AppData (там антивіруси менше сваряться)
if not exist "%target_dir%" mkdir "%target_dir%"

:: 4. Компіляція
echo [*] Compiling...
"%csc%" /target:winexe /out:"%target_exe%" "%source%"

if %errorlevel% neq 0 (
    echo [!] Compilation failed.
    pause
    exit /b
)
echo [+] Compiled successfully to %target_exe%

:: 5. Додавання в Планувальник завдань (Автозапуск)
echo [*] Setting up Autostart...
powershell -Command "$action = New-ScheduledTaskAction -Execute '%target_exe%'; $trigger = New-ScheduledTaskTrigger -AtLogOn; Register-ScheduledTask -Action $action -Trigger $trigger -TaskName 'TelegramSaver' -Description 'Auto-save media in Telegram' -Force" >nul 2>&1

if %errorlevel% neq 0 (
    echo [!] Could not create task. Try running as Administrator.
) else (
    echo [+] Scheduled task created.
)

:: 6. Запуск програми зараз
start "" "%target_exe%"

echo.
echo ==========================================
echo DONE! TelegramSaver is running and active.
echo It will start automatically with Windows.
echo ==========================================
pause