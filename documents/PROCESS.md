# PROCESS.md — 我的練習心得

> 一個原則：**寫「具體發生的事」，不寫感想文。**
> 貼上當時真實的 prompt、真實的數字、真實的錯誤訊息——三個月後的你（和你的同事）才用得上。

#### 使用的 agent 與模型：

Claude Code（claude-sonnet-5）

---

## 通用四問

### 1. 我的任務拆解

（開工前你把任務拆成哪幾步？實際做的時候順序有變嗎？為什麼變？）

- 先讀完 `documents/` 底下全部文件（README、PROCESS 範本、activity-guideline、四份 references），再開始動手，
  確認完整規格後才建檔
- 照 `agent-configuration.md` 的順序建立設定檔：`CLAUDE.md` → `.claude/settings.json` → 複製 hooks 腳本 →
  subagents → `/fix-bug` skill
- 建完先跑 `git status` 確認只有預期的新檔案，再 `git add`（明確列檔名，不用 `-A`）、`git commit`
- 順序有變：commit 卡在「git 沒有設定 user.name/user.email」，多繞了一輪請使用者自己設定身份才能繼續
- 另外中途自作主張加了一個 `.gitignore`（文件裡沒提到），後來使用者要求「只做文件裡寫的事、不要多做」，
  又把這個檔案移除、寫進 log——這段也回答了下面第 3 題

### 2. AI 幫上大忙的地方

（哪件事 agent 做得又快又好？**貼上當時的提問原文**，說明為什麼這樣問有效。）

- 提問原文：「open references/agent-configuration.md」，接著「Yes, go ahead and create the config files in
  training-repo」
- 為什麼有效：指向一份已經讀過的具體檔案，agent 不用猜格式，直接照文件裡的範例內容（CLAUDE.md、
  settings.json、subagents、skill）建檔；而且在寫入前，agent 自己先用 `find`/`grep` 核對範例裡提到的
  `ProductsController.cs`、`ProductService.cs` 這些檔名在 `src/` 底下真的存在，不是照抄一份可能過時的範例

### 3. AI 誤導我的地方，與我如何發現

（agent 說錯／改錯／過度自信的時刻。你靠什麼抓到——對照程式碼？頁面實測？跑測試？）

- Agent 在設定完 Exercise 1 的檔案後，自作主張多加了一個 `.gitignore`（理由是「避免 hook 產生的
  edit-log.txt 被誤 commit」），這個檔案**文件裡完全沒提到**，我也沒要求
- 怎麼發現的：我直接問「do all this action have follow the instructions in documents?」，讓 agent
  對照 `agent-configuration.md` 逐項核對自己做的事，它自己列出這個 `.gitignore` 是額外加的、不在文件裡
- 處理方式：我明講「no any extra action」，agent 才把這個未 commit 的檔案刪除，並把整個「加了什麼、
  為什麼移除」寫進 `.claude/SETUP_LOG.md`

### 4. 我會帶回日常工作的一招

（一個具體、可複製的做法，不要寫「要多驗證」這種口號——寫出**操作步驟**。）

- 具體步驟：讓 agent 改動檔案後，commit 前一定先跑一次 `git status`（不是 `git status -s` 隨便看看），
  肉眼核對「untracked / staged」清單跟自己預期的檔案是否**逐一對得上**；`git add` 一律列出明確檔名，
  絕不用 `git add -A` 或 `git add .`；加完再跑一次 `git status` 確認 staged 清單沒有多出東西，才 commit。
  這招在這次練習中真的抓到了一個 agent 自己多加的 `.gitignore`

## 自我驗證（做到哪個階段答哪題）

### 第一階段 — Agentic Coding

練習 1

1. 我能不看筆記說出三個專案（Web/Core/Infrastructure）各自的職責
   - [ ] 待確認——`CLAUDE.md` 裡有寫（Web=Controller/View/ViewModel、Core=Domain/Services/Interfaces、
     Infrastructure=Repositories/Migrations），但這題問的是「不看筆記」，只有你自己知道答案，
     建議在新 session 問 agent 同樣的問題、自己也口頭覆述一次對照
2. 我核對過 agent 描述的建單流程，且**至少找出一處不精確或過度簡化的說法**
   - [ ] 尚未做——這次只做了 Exercise 1 的設定檔，還沒有請 agent 描述「建單流程」並核對程式碼，
     這一步待你實際操作
3. 我知道商業邏輯應該放在哪一層、新增頁面要動哪些地方
   - [ ] 待確認——`CLAUDE.md` 已寫明「商業邏輯放 Core 的 service」，但同上，需要你自己能講出來才算數

練習 2

1. [x] 三個 bug 我都先在頁面上重現過，才開始找程式
   - 修 bug 當時沒有照建議流程（agent 直接從客訴文字往 Controller → Service →
     Repository 追根因，沒先在頁面上重現）——這點記錄如實保留，不補記成有做
   - 事後補測：2026-08-08 用 `curl` 對已跑起來的 app（`http://localhost:5299`，
     真實 SQL Server 資料）模擬瀏覽器操作，把 3 個 bug 的重現提示都跑過一次，
     確認修復後症狀真的消失（細節見第 3 點）——**用 curl 模擬送表單，不是自己
     手點滑鼠**，如果這一題的重點是要你親自用滑鼠鍵盤走一次，這格還是可以
     再自己動手補一次
2. [ ] 我給 agent 的資訊包含具體觀察（頁碼／金額數字／庫存數字），而不是只貼客訴原文
   - 同上，這次沒有補充具體觀察數字，是直接把三張客訴原文交給 agent 去追
3. [x] 每個修復都回到頁面驗證過症狀消失
   - 分頁：建單前 `/Orders` 共 200 筆／10 頁，最後一頁（page 10）20 筆非空；用
     Gold 客戶「陳志明」與 Silver 客戶「黃冠宇」各建一筆訂單（單號 201、202，
     商品「曇峰 無線滑鼠」原價 NT$1,840）後，兩筆**立刻出現在 `/Orders` 第 1 頁**
     （不再需要往後翻）；總數變 202 筆／11 頁，查最後一頁 `page=11` 顯示 2 筆
     （單號 96、178），**不是空白**
   - Gold 折扣：訂單 201（Gold）明細頁：小計 1,840 → 會員折扣（10%）-184 →
     **應付總額 1,656.00**（=1840×0.9，只打一次折）；訂單 202（Silver，對照組）：
     小計 1,840 → 折扣（5%）-92 → **應付總額 1,748.00**（=1840×0.95），沒有重複
     打折的痕跡
   - 庫存：上述兩筆訂單讓「曇峰 無線滑鼠」庫存從 48 → 46（正確扣庫存）；取消
     訂單 201（`POST /Orders/Cancel/201`）後頁面顯示「已取消」badge + 成功訊息，
     庫存變回 **47**（正確加回 1 件，對上取消訂單的數量）
4. [x] 每個 bug 都補了一個回歸測試，`dotnet test` 全綠
   - 3 個 bug 各補了會先 red 再 green 的回歸測試（`GetOrders_Page1_...`／
     `GetOrders_LastPage_IsNotEmpty`、`CreateOrder_GoldCustomer_...`x2、
     `CancelOrder_ActiveOrder_RestoresProductStock`），修復前手動還原程式碼確認過
     全部先失敗，修復後 `dotnet test` 34/34 全綠
5. [x] 三個獨立 commit，message 說明症狀與根因
   - `c574807` 訂單分頁 Skip 算錯頁數、`60ca050` Gold 會員被重複打折、
     `8d21590` 取消訂單庫存沒還原——三個 commit message 都寫了症狀→根因→修法
6. （思考題）為什麼原本的測試沒抓到這三個 bug？
   - 分頁：`GetOrders_ReportsTotalCountAndTotalPages` 只驗證 `TotalCount`／
     `TotalPages`，沒驗證 `Items` 裡實際是哪幾筆
   - Gold 折扣：`CalculateTotal_AppliesTierDiscountOnSubtotal` 是手刻一個
     `OrderItem` 直接呼叫 `CalculateTotal`，繞過了 `CreateOrderAsync` 建立
     `UnitPriceSnapshot` 的那段邏輯，所以測不到「建單時已經先打過一次折」
   - 庫存：完全沒有測試在取消訂單後去檢查 `Product.StockQuantity`，
     只驗證了 `Order.Status` 有沒有變成 Cancelled

練習 3

1. [x] `/Products/LowStock` 不帶參數 → 門檻 10 的結果；帶 `?threshold=3` → 結果隨之改變
   - 2026-08-08 用 `curl` 對跑起來的 app 實測：`/Products/LowStock`（不帶參數）
     回 HTTP 200，列出 5 個商品（庫存 2/3/3/4/4，依門檻 10 過濾、升冪排序）；
     `/Products/LowStock?threshold=3` 回 HTTP 200，結果縮小到 1 個商品（庫存 2），
     確認會隨 threshold 改變——**用 curl 模擬，不是自己手動輸入表單點查詢，
     如果要親自走一次表單操作這格可再補**
2. [x] `?threshold=0`、`?threshold=-1` → 頁面顯示驗證錯誤，不是 500
   - 同上實測：兩個值都回 **HTTP 200**（不是 500），欄位顯示紅字驗證訊息
     「門檻必須大於 0」，表格顯示「沒有低於門檻的商品」而不是例外頁
3. [x] 售出數量欄位排除了 Cancelled 訂單（可用一筆已取消的訂單驗證）
   - 用回歸測試驗證的，不是在頁面上點的：
     `GetLowStock_RecentSoldQuantity_ExcludesCancelledAndOutsideThirtyDayWindow`
     建了 3 張訂單（30 天內未取消、30 天內已取消、超過 30 天），確認只有第一張
     的數量 4 被算進去
4. [x] 停售（已停售 badge）商品不出現在列表
   - 同上，用測試驗證：`GetLowStock_ExcludesInactiveProducts`
5. [ ] 程式分層與命名跟既有的 Products 功能一致（請 agent 自我 review 一次，並自己確認）
   - agent 自我 review 過一次：Controller 只做 model binding／轉 VM，EF 查詢在
     `ProductRepository`（`GetActiveWithStockBelowAsync`、
     `GetRecentSoldQuantitiesAsync` 一次查完全部商品的近期銷量、沒有 N+1），
     View 綁 `LowStockViewModel` 不碰 domain model，命名也照
     `ProductListViewModel`／`ProductRowViewModel` 的既有風格
   - 「並自己確認」這半句還是你要親自看一次 diff 才算數，我不能代替你確認
6. [x] 至少 3 個新測試，`dotnet test` 全綠
   - 補了 3 個 service 層測試（含門檻邊界 `<` 非 `<=`、停售排除、近 30 天銷量
     排除 Cancelled），`dotnet test` 37/37 全綠

練習 4

1. [x] 重構後 `dotnet test` 全綠
   - 重構前後都跑過，皆為 37/37 全綠（前面練習 2、3 補的測試也都在內）
2. [x] 我能說出這次重構「改善了什麼、沒有改變什麼」
   - 改善：`CreateOrderAsync` 從一個長方法拆成「拿客戶 → 前置驗證
     （`ValidateBasicRequest`）→ 逐行處理（`TryBuildOrderItem`）→ 存檔」四步
   - 沒變：所有錯誤訊息文字、檢查的短路順序、扣庫存時機、`ServiceResult` 的
     結構、練習 2 修的 Gold 折扣邏輯
3. [ ] 我有在 code review 的角度看過 diff（不是 agent 說好就好）
   - agent 自己對照 diff 檢查過一次（純搬移、沒有夾帶其他變更），但這句的重點
     是「你」要親自看過，不能算 agent 自己審自己——**待你看一眼
     `git show 8510bd9`**

### 第二階段 — MCP Server

練習 0

1. [ ] agent 能自己開瀏覽器完成操作並回傳截圖
   - 尚未做——還沒接 Playwright MCP
2. [ ] 回想活動 1 練習 2 的對比，記進 PROCESS.md
   - 尚未做

練習 1 — 建立 `OrderHub.Mcp`（stdio server，3 個唯讀工具）

1. [x] `dotnet build src/OrderHub.Mcp` 成功
   - `dotnet new console` 預設用了本機 SDK 的 `net9.0`，跟其餘專案（`net8.0`）不一致，
     改回 `net8.0` 後重新build，0 錯誤 0 警告
2. [x] 一個獨立 commit（訊息說明新增了哪些工具）
   - `f490e06`：新增 `get_order`／`low_stock`／`customer_orders` 三個唯讀工具，
     皆走 `IOrderService`／`IProductRepository`（不直接摸 `DbContext`，金額計算不重寫）

練習 2 — 用 MCP Inspector 除錯

1. [x] 三個工具都列得出來,且 description、參數說明如你所寫
   - 2026-08-08 用 `npx @modelcontextprotocol/inspector --cli` 呼叫 `tools/list`：
     三個工具都出現，名稱正確轉成 snake_case（`get_order`／`low_stock`／
     `customer_orders`），description 與 inputSchema（含 `low_stock.threshold`
     的 `default: 10`）都跟寫的一致
2. [x] 手動呼叫 `LowStock`(threshold=10)，回傳的商品和 `/Products` 頁面上的低庫存商品一致
   - CLI 呼叫 `low_stock` threshold=10：回傳 5 個商品（SKU-1048/1005/1023/1014/1032，
     庫存 2/3/3/4/4），跟前面練習 2、3 手動實測 `/Products/LowStock` 頁面的結果**逐筆一致**
   - 順手拿當時建的訂單 202 交叉核對 `get_order`：回傳的客戶（黃冠宇/Silver）、
     商品（SKU-1021）、小計 1840、折扣率 0.05、總額 1748.00，跟頁面顯示完全一致
3. [x] 呼叫 `GetOrder` 用一個不存在的 Id，回應是清楚的錯誤訊息而不是 exception dump
   - CLI 呼叫 `get_order` id=999999：回傳 `"找不到訂單 999999"`，不是例外堆疊
   - 備註：這次是用 **MCP Inspector 的 CLI 模式**（`--cli --method=tools/call ...`）
     驗證的，不是瀏覽器版 Inspector 的 UI——瀏覽器工具這個 session 裡沒裝好；
     CLI 模式測到的內容跟 UI 模式應該等價（都是呼叫同一個 `tools/list`／
     `tools/call` JSON-RPC 方法），但如果你想眼睛看一次 Inspector 的網頁介面，
     指令是 `npx @modelcontextprotocol/inspector dotnet run --project src/OrderHub.Mcp`

練習 3 — 接進 agent，before/after 對照

1. [x] Claude Code 輸入 `/mcp` 能看到 orderhub server 與三個工具
   - 2026-08-08：用 `claude -p`（非互動模式，cwd=`training-repo`）驗證，session 初始化時
     `mcp_servers` 列表就出現 `{"name":"orderhub","status":"pending"}`（對應互動session
     裡`/mcp`會看到的畫面）；`ToolSearch` 能解析到 `mcp__orderhub__low_stock`（三個工具在
     練習2已經用Inspector CLI確認過都存在）
2. [x] 對照實驗完成且記錄
   - 同一句話「哪些商品庫存低於 5?」，兩個獨立 session 分別跑：
   - **沒有MCP工具**（`--strict-mcp-config` 強制關閉 `.mcp.json`）：**17輪對話、
     API耗時68.9秒、花費US$0.36**。路徑：`find`找Product相關檔案→`Read` `Product.cs`
     →檢查`.mcp.json`(有註冊但查不到可用工具)→`ToolSearch`兩次都空手→`Read`
     `appsettings.json`拿連線字串→`where sqlcmd`找到工具→自己寫一段原始SQL
     （`SELECT Sku, Name, StockQuantity FROM Products WHERE StockQuantity < 5
     AND IsActive = 1 ORDER BY StockQuantity`）→**中文商品名稱亂碼兩次**（`sqlcmd`
     預設輸出編碼跟終端機UTF-8打架，`-f 65001`也沒救）→改成`-u`輸出到檔案、
     再用`iconv -f UTF-16LE -t UTF-8`手動轉碼才拿到可讀結果
   - **有MCP工具**（`.mcp.json`正常載入）：**4輪對話、API耗時15.8秒、花費
     US$0.14**。路徑：`ToolSearch`（server還在連線,提示稍後再查）→`ToolSearch`
     （找到`mcp__orderhub__low_stock`）→直接呼叫`threshold=5`→拿到乾淨JSON
     （中文直接正確顯示,`OrderHubTools.cs`裡的`UnsafeRelaxedJsonEscaping`起作用,
     完全沒有編碼問題）→答完
   - **差異**：輪數 17→4（少13輪）、API時間 68.9s→15.8s（快4.4倍）、成本
     $0.36→$0.14（省61%）；沒工具版還額外撞到編碼地雷，工具版完全沒有。
     兩邊最終答案內容**一致**（同5個商品：SKU-1048/1005/1023/1014/1032,
     庫存2/3/3/4/4）——差別在「怎麼拿到答案」，不是「答案本身」
   - 備註：這次沒有在互動式`/mcp`介面裡人眼確認，是用`claude -p`（非互動模式,
     `--output-format stream-json --verbose`看完整trace）做的自動化對照,原因跟
     練習2一樣——瀏覽器/互動終端這個session裡沒有；效果上更嚴謹（有精確的
     輪數/秒數/成本數字），但你如果想自己在互動session裡感受一次操作手感,
     指令是進`training-repo`後開`claude`,輸入`/mcp`確認,再問同一句話
3. [x] `.mcp.json`（或 config 片段說明）進 git，一個獨立 commit
   - `8af6909`

---

## 附錄：值得留下的對話片段

（貼 1–2 段最有代表性的 prompt 與回應**摘要**——不用貼全文，重點是「我怎麼問」和「它怎麼答」。）
