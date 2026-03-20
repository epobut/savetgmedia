using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

// TelegramSaver (Golden Standard Version)
// Логика: Автосохранение + Ожидание + Гарантированное закрытие через крестик окна.

namespace TelegramSaver
{
    class Program
    {
        private static LowLevelMouseProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        private static bool _isActionRunning = false;

        static void Main()
        {
            try { SetProcessDPIAware(); } catch { }

            _hookID = SetHook(_proc);
            if (_hookID == IntPtr.Zero) return;

            MSG msg;
            while (GetMessage(out msg, IntPtr.Zero, 0, 0))
            {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }
            UnhookWindowsHookEx(_hookID);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                if (wParam == (IntPtr)0x0207) // Middle Click
                {
                    if (IsTelegramActive())
                    {
                        if (!_isActionRunning)
                        {
                            _isActionRunning = true;
                            Task.Run(() => RunMacro());
                        }
                        return (IntPtr)1; 
                    }
                }
                if (wParam == (IntPtr)0x020A && _isActionRunning) return (IntPtr)1;
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static void RunMacro()
        {
            try
            {
                DoMouseClick();
                Thread.Sleep(400);

                SendCtrlS();

                IntPtr dialog = IntPtr.Zero;
                for (int i = 0; i < 300; i++)
                {
                    dialog = FindWindow("#32770", null);
                    if (dialog != IntPtr.Zero && IsWindowVisible(dialog)) break;
                    Thread.Sleep(10);
                }

                if (dialog == IntPtr.Zero) return; 

                Thread.Sleep(200);
                SetForegroundWindow(dialog);
                SendEnter(); 

                Thread.Sleep(200); 

                // УМНОЕ ОЖИДАНИЕ
                while (true)
                {
                    IntPtr hWindow = FindWindow("#32770", null);
                    if (hWindow == IntPtr.Zero) break; 
                    if (!IsWindowVisible(hWindow)) break;
                    Thread.Sleep(50);
                }

                Thread.Sleep(200);
                
                // Вызываем новую функцию закрытия
                CloseMediaViewer();
            }
            catch { }
            finally
            {
                _isActionRunning = false;
            }
        }

        // === ЗОЛОТОЙ СТАНДАРТ ЗАКРЫТИЯ ===
        private static void CloseMediaViewer()
        {
            POINT oldPos;
            GetCursorPos(out oldPos);

            // 1. Получаем активное окно (сейчас это Телеграм)
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd != IntPtr.Zero)
            {
                RECT rect;
                // 2. Узнаем физические границы этого окна
                if (GetWindowRect(hwnd, out rect))
                {
                    // 3. Вычисляем координаты кнопки "Закрыть" (X) в правом верхнем углу.
                    // rect.Right - это правый край окна. Отступаем от него 30 пикселей влево.
                    int targetX = rect.Right - 30;
                    
                    // rect.Top - это верхний край окна. Отступаем 25 пикселей вниз.
                    int targetY = rect.Top + 25;

                    // Кликаем точно в крестик!
                    SetCursorPos(targetX, targetY);
                    Thread.Sleep(30); 
                    DoMouseClick();
                }
            }

            // Возвращаем курсор, где он был
            SetCursorPos(oldPos.X, oldPos.Y);
        }

        // === ФУНКЦИИ ===
        private static void DoMouseClick() {
            mouse_event(0x0002, 0, 0, 0, 0); mouse_event(0x0004, 0, 0, 0, 0);
        }
        private static void SendCtrlS() {
            keybd_event(0x11, 0, 0, 0); keybd_event(0x53, 0, 0, 0);
            Thread.Sleep(10);
            keybd_event(0x53, 0, 0x0002, 0); keybd_event(0x11, 0, 0x0002, 0);
        }
        private static void SendEnter() {
            keybd_event(0x0D, 0, 0, 0); Thread.Sleep(10); keybd_event(0x0D, 0, 0x0002, 0);
        }
        private static bool IsTelegramActive() {
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd == IntPtr.Zero) return false;
            uint pid; GetWindowThreadProcessId(hwnd, out pid);
            try { return Process.GetProcessById((int)pid).ProcessName.ToLower().Contains("telegram"); } catch { return false; }
        }
        private static IntPtr SetHook(LowLevelMouseProc proc) {
            return SetWindowsHookEx(14, proc, LoadLibrary("user32.dll"), 0);
        }

        // === WINAPI ===
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        [StructLayout(LayoutKind.Sequential)] public struct POINT { public int X; public int Y; }
        [StructLayout(LayoutKind.Sequential)] public struct MSG { public IntPtr hwnd; public uint message; public IntPtr wParam; public IntPtr lParam; public uint time; public POINT pt; }
        
        // НОВАЯ СТРУКТУРА ДЛЯ ГРАНИЦ ОКНА
        [StructLayout(LayoutKind.Sequential)] public struct RECT { public int Left; public int Top; public int Right; public int Bottom; }[DllImport("user32.dll")] static extern IntPtr GetForegroundWindow();[DllImport("user32.dll")] static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);[DllImport("user32.dll", SetLastError = true)] static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")] static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("user32.dll")] static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);[DllImport("user32.dll")] static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
        [DllImport("user32.dll")] static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("user32.dll")] static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")] static extern int GetSystemMetrics(int nIndex);
        [DllImport("user32.dll")] static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")] static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);[DllImport("user32.dll")] static extern bool TranslateMessage([In] ref MSG lpMsg);[DllImport("user32.dll")] static extern IntPtr DispatchMessage([In] ref MSG lpMsg);
        [DllImport("user32.dll", SetLastError = true)] static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);[DllImport("user32.dll")] static extern bool UnhookWindowsHookEx(IntPtr hhk);[DllImport("user32.dll")] static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll")] static extern IntPtr LoadLibrary(string lpFileName);
        [DllImport("user32.dll")] static extern bool SetProcessDPIAware();
        
        // НОВАЯ ФУНКЦИЯ ЧТЕНИЯ ГРАНИЦ ОКНА
        [DllImport("user32.dll", SetLastError = true)] static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
    }
}