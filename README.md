# MultiTimer 多功能計時器

一個輕量的 Windows 桌面計時器工具，支援多個計時器同時運行。

## 功能特色

- **正數 / 倒數模式** — 每個計時器可獨立選擇正數計時或倒數計時
- **時間到提醒** — 音效 + 彈窗通知，顯示計時器名稱
- **多計時器管理** — 動態新增 / 移除計時器，數量不限
- **自訂名稱** — 為每個計時器設定名稱，方便辨識
- **全域快捷鍵** — 為計時器設定快捷鍵，時間到時自動模擬按下該按鍵
- **視窗置頂** — 勾選後視窗保持在所有視窗最上層
- **自動儲存** — 關閉程式時自動儲存所有設定，下次開啟自動還原
- **自動調整視窗大小** — 視窗高度隨計時器數量自動伸縮
- **表格化介面** — 清晰的表格佈局，一目了然

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
   - 勾選「自動」欄位後，時間到會自動模擬按下該快捷鍵
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

詳見 [VersionLog.md](VersionLog.md)
