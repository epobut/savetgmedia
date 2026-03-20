[UA] Проста утиліта для швидкого збереження медіафайлів у Telegram Desktop одним кліком.
[EN] A simple utility for quick media saving in Telegram Desktop with a single click.

---

## 🇺🇦 Українською

### Що це таке?
Це маленька програма, яка автоматизує процес збереження фото та відео в Telegram Desktop. Вам більше не потрібно натискати "Зберегти як", обирати папку та закривати перегляд вручну.

### Як це працює?
Програма працює у фоні та реагує на **натискання коліщатка миші (Middle Click)**, коли ви переглядаєте медіа в Telegram:
1. Ви натискаєте коліщатко на фото/відео.
2. Програма сама натискає `Ctrl + S`.
3. Автоматично підтверджує збереження (натискає `Enter`).
4. Чекає завершення завантаження та сама закриває вікно перегляду (клікає на "хрестик").

### Особливості:
- **Розумне очікування:** Програма чекає, поки зникне діалогове вікно, перш ніж закрити медіаплеєр.
- **Точність:** Вираховує координати кнопки закриття відносно розміру вашого вікна.
- **Безпека:** Працює тільки тоді, коли вікно Telegram активне.

---

## 🇺🇸 English

### What is it?
A lightweight utility designed to automate media saving in Telegram Desktop. It eliminates the need to manually click "Save As," confirm dialogues, and close the media viewer.

### How it works?
The app runs in the background and triggers a macro when you **click the mouse wheel (Middle Click)** while using Telegram:
1. You middle-click on a photo or video.
2. The app triggers the `Ctrl + S` shortcut.
3. It automatically confirms the save dialogue (hits `Enter`).
4. It waits for the process to finish and then closes the media viewer by clicking the "X" button.

### Key Features:
- **Smart Waiting:** The script monitors the save dialogue and only proceeds once the file is ready.
- **Dynamic Positioning:** Automatically calculates the "Close" button position based on your current window size.
- **Context Aware:** Only intercepts clicks when the Telegram window is active.

---

## 🛠 Tech Stack
- **Language:** C#
- **API:** Win32 API (User32.dll, Kernel32.dll) for mouse/keyboard hooks and window management.
