# MultiTimer 多功能計時器

一個輕量的 Windows 桌面計時器工具，支援多個計時器同時運行。

## 功能特色

- **正數 / 倒數模式** — 每個計時器可獨立選擇正數計時或倒數計時
- **時間到提醒** — 音效 + 彈窗通知，顯示計時器名稱
- **多計時器管理** — 動態新增 / 移除計時器，數量不限
- **自訂名稱** — 為每個計時器設定名稱，方便辨識
- **全域快捷鍵** — 為計時器設定快捷鍵，按下後自動重新開始倒數（即使程式不在前景）
- **視窗置頂** — 勾選後視窗保持在所有視窗最上層
- **自動儲存** — 關閉程式時自動儲存所有設定，下次開啟自動還原
- **自動調整視窗大小** — 視窗高度隨計時器數量自動伸縮

## 系統需求

- Windows 10 / 11
- .NET 8.0 Runtime（或以上）

## 建置與執行

```bash
cd MultiTimer
dotnet build
dotnet run
```

或建置後直接執行：

```
MultiTimer\bin\Debug\net8.0-windows\MultiTimer.exe
```

## 使用說明

1. **新增計時器** — 點擊「＋ 新增計時器」按鈕
2. **設定名稱** — 在左側文字框輸入計時器名稱
3. **選擇模式** — 下拉選單選擇「正數」或「倒數」
4. **設定時間** — 輸入時、分、秒
   - 正數模式：設定目標時間，到達後提醒（設為 0 則持續計時不提醒）
   - 倒數模式：設定倒數時間，歸零後提醒
5. **設定快捷鍵** — 點擊「快捷鍵」輸入框，按下想要的組合鍵（如 Ctrl+F1）
   - 設定後，在任何畫面按下該快捷鍵，計時器會自動重置並重新開始
6. **開始 / 暫停** — 點擊「開始」按鈕，計時中可點「暫停」
7. **重置** — 點擊「重置」回到初始狀態
8. **移除** — 點擊紅色「✕」移除該計時器
9. **視窗置頂** — 勾選工具列的「視窗置頂」

## 設定檔

設定自動儲存於執行檔同目錄下的 `settings.json`，包含：

- 視窗置頂狀態
- 所有計時器的名稱、模式、時間、快捷鍵

## 專案結構（MVP 架構）

```
MultiTimer/
├── Program.cs                  # 程式進入點
├── Form1.cs                    # 主視窗（實作 IMainView）
├── Form1.Designer.cs           # 設計器檔案
│
├── Models/                     # Model 層 — 商業邏輯與資料
│   ├── TimerModel.cs           # 計時器核心邏輯（正數/倒數/Tick）
│   ├── TimerData.cs            # 資料模型（序列化用）
│   ├── SettingsService.cs      # 設定檔存取服務
│   ├── SoundService.cs         # 音效播放服務
│   └── GlobalHotkey.cs         # Windows 全域熱鍵管理
│
├── Views/                      # View 層 — UI 呈現與事件轉發
│   ├── IMainView.cs            # 主視窗介面
│   ├── ITimerView.cs           # 計時器面板介面
│   ├── TimerPanelView.cs       # 計時器面板 UI 實作
│   └── HotkeyTextBox.cs        # 快捷鍵輸入框控制項
│
├── Presenters/                 # Presenter 層 — 協調 Model 與 View
│   ├── MainPresenter.cs        # 主視窗 Presenter（管理計時器集合、熱鍵、設定）
│   └── TimerPresenter.cs       # 單一計時器 Presenter
│
└── MultiTimer.csproj           # 專案檔
```

---

## 版本紀錄

### v1.3.0 (2025-05-19)

- 重構：整體架構調整為 MVP（Model-View-Presenter）模式
  - Model：TimerModel 負責計時邏輯，SettingsService/SoundService 負責基礎服務
  - View：ITimerView/IMainView 介面定義，TimerPanelView 實作 UI
  - Presenter：TimerPresenter/MainPresenter 協調邏輯與介面
- 音效改為 Ring02.wav
- 移除時間到彈窗通知，改為僅播放音效

### v1.2.0 (2025-05-19)

- 新增：全域快捷鍵功能，按下快捷鍵可重新開始倒數
- 新增：自動儲存/載入設定（settings.json）
- 新增：快捷鍵輸入框控制項

### v1.1.0 (2025-05-19)

- 修正：標籤文字遮擋輸入框的問題
- 修正：Label 背景透明
- 新增：視窗高度隨計時器數量自動調整

### v1.0.0 (2025-05-19)

- 初始版本
- 支援正數/倒數計時
- 多計時器管理（新增/移除）
- 自訂計時器名稱
- 時間到音效 + 彈窗提醒
- 視窗置頂選項
