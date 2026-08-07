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

1. [ ] 三個 bug 我都先在頁面上重現過，才開始找程式
   - 這次沒有照建議流程：agent 直接根據 `activity-guideline.md` 裡客訴的文字描述往
     Controller → Service → Repository 追根因，沒有先在頁面上手動重現。你把 app
     跑起來準備自己操作時，我把佔用 5299 port 的程序關掉讓你手動接手——**這步驟
     待你自己在頁面上跑一次 3 個重現提示，確認症狀確實消失**
2. [ ] 我給 agent 的資訊包含具體觀察（頁碼／金額數字／庫存數字），而不是只貼客訴原文
   - 同上，這次沒有補充具體觀察數字，是直接把三張客訴原文交給 agent 去追
3. [ ] 每個修復都回到頁面驗證過症狀消失
   - 尚未做（同第 1 點，待你手動驗證）
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

1. `/Products/LowStock` 不帶參數 → 門檻 10 的結果；帶 `?threshold=3` → 結果隨之改變
2. `?threshold=0`、`?threshold=-1` → 頁面顯示驗證錯誤，不是 500
3. 售出數量欄位排除了 Cancelled 訂單（可用一筆已取消的訂單驗證）
4. 停售（已停售 badge）商品不出現在列表
5. 程式分層與命名跟既有的 Products 功能一致（請 agent 自我 review 一次，並自己確認）
6. 至少 3 個新測試，`dotnet test` 全綠

練習 4

1. 重構後 `dotnet test` 全綠
2. 我能說出這次重構「改善了什麼、沒有改變什麼」
3. 我有在 code review 的角度看過 diff（不是 agent 說好就好）

---

## 附錄：值得留下的對話片段

（貼 1–2 段最有代表性的 prompt 與回應**摘要**——不用貼全文，重點是「我怎麼問」和「它怎麼答」。）
