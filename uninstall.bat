@echo off
title TelegramSaver Uninstaller
echo [*] Stopping TelegramSaver...

:: 1. Закриваємо програму, якщо вона працює
taskkill /f /im TelegramSaver.exe >nul 2>&1

:: 2. Видаляємо задачу з Планувальника
echo [*] Removing Autostart task...
powershell -Command "Unregister-ScheduledTask -TaskName 'TelegramSaver' -Confirm:$false" >nul 2>&1

:: 3. Видаляємо файли
echo [*] Deleting files from AppData...
if exist "%APPDATA%\TelegramSaver" (
    rmdir /s /q "%APPDATA%\TelegramSaver"
)

echo.
echo ==========================================
echo DONE! TelegramSaver has been removed.
echo ==========================================
pause