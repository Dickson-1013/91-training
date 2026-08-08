# Agent Configuration Setup Log

Purpose: record every decision/action taken while setting up Claude Code config for OrderHub
(Training Exercise 1), so each step can be traced back or rolled back individually.
Source guide: `documents/references/agent-configuration.md`.

## Actions (2026-07-24)

1. **Created `CLAUDE.md`** (repo root)
   - What: project memory (intro, stack, layering conventions, common commands, dangerous files, don'ts)
   - Why: required by Exercise 1; content taken from the guide's OrderHub example
   - Verified: `ProductsController.cs` / `ProductService.cs` referenced in the file actually exist under `src/`
   - Rollback: `git rm CLAUDE.md` (pre-commit) or `git revert <commit-hash>` (post-commit)

2. **Created `.claude/settings.json`**
   - What: permission rules — `deny` (force-push, hard-reset, reading secrets, editing migrations),
     `ask` (db drop, git push), `allow` (build/test/run/git status/diff/log/add/commit) — plus
     hooks wiring (PreToolUse -> block-destructive-sql.ps1, PostToolUse -> log-edits.ps1)
   - Why: block destructive actions outright, avoid prompt fatigue on safe repeated commands
   - Rollback: delete file / `git revert <commit-hash>`

3. **Copied hook scripts into `.claude/hooks/`**
   - What: `block-destructive-sql.ps1`, `log-edits.ps1` copied verbatim from `documents/activities/scripts/`
   - Why: referenced by `.claude/settings.json` hooks section
   - Rollback: delete `.claude/hooks/`

4. **Created `.claude/agents/code-reviewer.md` and `test-runner.md`**
   - What: subagents scoped to read-only-ish tools (reviewer: Read/Grep/Glob/Bash; test-runner: Bash/Read/Grep)
   - Why: isolate review/test noise from main context; reviewer enforces layering/ViewModel/decimal/test checks
   - Rollback: delete files

5. **Created `.claude/skills/fix-bug/SKILL.md`**
   - What: `/fix-bug` slash command encoding the standard bug-fix workflow (reproduce -> locate -> confirm ->
     fix -> code-reviewer -> regression test via test-runner -> commit)
   - Why: Exercise 2 repeats this flow 3x; avoids re-explaining it each time
   - Rollback: delete file

## Git actions

- `git status` (before staging) — confirmed only `CLAUDE.md` and `.claude/` were untracked, nothing else touched
- `git add CLAUDE.md .claude/` — staged the 7 new files **by explicit name**, not `-A`
- `git commit ...` — **failed**: git identity (`user.name`/`user.email`) not configured in this environment
- Did not run `git config` myself (kept out of scope) — asked user to set identity, which they did:
  `git config user.name "dm91"` / `git config user.email "IM0091@oberps.com"`
- Status as of writing: **staged, not yet committed, not pushed**

## Commit 1 (2026-07-24, `840be8d`)

- Committed after user set repo-local git identity (`user.name "dm91"`, `user.email "IM0091@oberps.com"`)
- Staged files verified via `git status` immediately before commit — no unexpected files included
- Local only, not pushed

## Verification (2026-07-24)

Ran what could be checked directly (outside a live Claude Code session rooted at this repo):

- [x] `.claude/settings.json` parses as valid JSON; rule counts match the guide (6 deny / 2 ask / 8 allow, 1 PreToolUse, 1 PostToolUse)
- [x] `block-destructive-sql.ps1` — fed a `TRUNCATE TABLE OrderItems` command via stdin: printed "Action denied", exit code 2 (blocks)
- [x] `block-destructive-sql.ps1` — fed a safe `dotnet test` command via stdin: exit code 0 (allows)
- [x] `log-edits.ps1` — fed a simulated Write-tool payload via stdin: appended a correctly formatted line to `.claude/hooks/edit-log.txt` and returned the expected `systemMessage` JSON
  - This created a test-only entry in `edit-log.txt`. Removed it afterward (not real usage data) and added
    `.gitignore` (`.claude/hooks/edit-log.txt`, `.claude/settings.local.json`, `CLAUDE.local.md`) so this
    runtime artifact is never accidentally committed going forward.

**Not yet verifiable from here** — these require actually opening a Claude Code session with this repo as
the project root and issuing the prompts, since they depend on the harness's own permission-prompt and
subagent-dispatch behavior, not just the config files' contents:

- [ ] `git push --force` is rejected outright (deny) with no prompt
- [ ] `dotnet test` runs without asking (allow)
- [ ] `dotnet ef database drop` prompts for confirmation first (ask)
- [ ] Editing a file under `Migrations/` is denied
- [ ] Asking the agent to run `sqlcmd` with `TRUNCATE` is blocked by the PreToolUse hook end-to-end
- [ ] A real Edit/Write from the agent produces a line in `.claude/hooks/edit-log.txt`
- [ ] Asking "what are the layering conventions?" in a fresh session — agent answers from `CLAUDE.md` without reading files
- [ ] Asking the agent to install a new NuGet package — it asks first instead of installing directly
- [ ] `code-reviewer` and `test-runner` subagents can be invoked (explicitly or via auto-delegation)
- [ ] `/fix-bug <symptom>` triggers the skill workflow

## Rollback (2026-07-29): removed `.gitignore`

- What: deleted `.gitignore` (contained `.claude/hooks/edit-log.txt`, `.claude/settings.local.json`, `CLAUDE.local.md`)
- Why: it was never committed (confirmed via `git status --short` showing `?? .gitignore` beforehand) and was
  **not requested by the user or specified anywhere in `documents/`** — it was my own addition. Per explicit
  user instruction ("follow only the documented instructions, no extra action"), rolled it back.
- How: plain `rm` — safe, since the file was untracked and had never been part of a commit
- Verified after: `training-repo` now contains only the files documented in `agent-configuration.md`
  (`CLAUDE.md`, `.claude/settings.json`, `.claude/hooks/*.ps1`, `.claude/agents/*.md`, `.claude/skills/fix-bug/SKILL.md`)
  plus this log, which the user separately and explicitly asked for.

## Going-forward policy (2026-07-29)

- Only take actions explicitly specified in `documents/` (README.md, PROCESS.md, activities/activity-guideline.md,
  references/*.md) or explicitly requested by the user — no unrequested/undocumented extras.
- `SETUP_LOG.md` (this file) continues, since the user explicitly asked for a traceable action log — every
  action from here on gets an entry with what/why/rollback-method before or immediately after it happens.

## Branch + commit (2026-07-29): PROCESS.md draft

- What: created branch `docs/process-exercise1` off `main` (which was 1 commit ahead of `origin/main`),
  then staged + committed exactly 2 modified files: `documents/PROCESS.md` and this log
- Why: user asked to "branch out from 91-training and commit the change" — keeps `main` untouched while
  the PROCESS.md draft (reviewed by user before this commit) is recorded on its own branch
- Rollback: `git checkout main` (branch stays behind untouched); to remove entirely, `git branch -D
  docs/process-exercise1` (only if the branch is not needed — destructive, would ask before doing this)
- Local only, not pushed

## Activity 2, Exercise 1 (2026-08-08): `OrderHub.Mcp` scaffold

- What: new console project `src/OrderHub.Mcp` — stdio MCP server exposing 3 read-only tools
  (`get_order`, `low_stock`, `customer_orders`), reusing `OrderHub.Core`/`Infrastructure` service
  and repository layers (no logic duplicated, no direct `DbContext` access from the tool class)
  - `Program.cs`: generic host, EF Core `OrderHubDbContext` wired to the same connection string as
    `OrderHub.Web`, DI registrations for `ICustomerRepository`/`IProductRepository`/`IOrderRepository`/
    `IOrderService`, logging forced to stderr (stdout is the MCP protocol channel), stdio transport
  - `OrderHubTools.cs`: `[McpServerToolType]` class, constructor-injects `IOrderService` +
    `IProductRepository`; entities are projected to anonymous objects before serializing (avoids the
    `Order` <-> `Customer` circular-reference crash the guide calls out); amounts come from
    `IOrderService.CalculateSubtotal/CalculateTotal/GetDiscountRate`, not reimplemented
  - `OrderHub.Mcp.csproj`: packages `ModelContextProtocol` 2.1.0 (`--prerelease`) and
    `Microsoft.Extensions.Hosting`, project references to `OrderHub.Core`/`OrderHub.Infrastructure`
  - `OrderHub.sln`: modified to add the new project
- Why: `documents/activities/activity-2-custom-mcp.md`, Exercise 1
- Deviation from the guide: `dotnet new console` defaulted to `net9.0` (this machine's installed SDK),
  but every other project in the solution (`Core`/`Web`) targets `net8.0` per `README.md`'s stated
  stack — changed `<TargetFramework>` to `net8.0` for consistency; rebuilt clean afterward (0
  errors/warnings) so this wasn't just a cosmetic edit
- Verified: method/property names the guide's code assumes (`GetOrderAsync`, `CalculateSubtotal`,
  `CalculateTotal`, `GetDiscountRate`, `GetCustomerOrdersAsync`, `IProductRepository.GetActiveAsync`,
  and the `Order`/`Customer`/`Product`/`OrderItem` domain properties) checked against the actual
  source before writing the tool class — all matched exactly, no adaptation needed; `dotnet build
  src/OrderHub.Mcp` succeeds (0 errors, 0 warnings)
- Not yet done: MCP Inspector testing (Exercise 2), `dotnet build`-only checked — the tools have not
  been invoked live yet
- Rollback: `git rm -r src/OrderHub.Mcp`, revert `OrderHub.sln`'s diff, `git revert <commit-hash>`
  (post-commit); no other files touched

## Activity 2, Exercise 2 (2026-08-08): MCP Inspector verification (CLI mode)

- What: verified all 3 tools using `npx @modelcontextprotocol/inspector --cli` against the built
  `src/OrderHub.Mcp/bin/Debug/net8.0/OrderHub.Mcp.dll` (avoided `dotnet run --project` as the
  target — its own `--project` flag collided with the inspector CLI's argument parser; running the
  built DLL directly with no flags sidesteps that)
  - `--method=tools/list` (note: `=` syntax required — `--method tools/list` as two separate argv
    tokens gets swallowed into the variadic `target` capture instead of parsed as inspector's own
    option, in every order tried) — all 3 tools listed, names/descriptions/inputSchema as written
  - `tools/call low_stock threshold=10` — 5 products (SKU-1048/1005/1023/1014/1032, stock
    2/3/3/4/4), matching the `/Products/LowStock` page results from the earlier Exercise 2/3
    manual verification exactly
  - `tools/call get_order id=202` — cross-checked against order 202 (created during that same
    earlier verification): customer 黃冠宇/Silver, SKU-1021, subtotal 1840, discount 0.05,
    total 1748.00, all matching the page
  - `tools/call get_order id=999999` — returned `"找不到訂單 999999"`, not an exception dump
  - `tools/call customer_orders customerId=3` — sanity check, returned a plausible order list
    including order 202
- Why: `documents/activities/activity-2-custom-mcp.md`, Exercise 2
- Deviation from the guide: used Inspector's **CLI mode**, not the web UI it describes — the
  claude-in-chrome browser tool wasn't installed in this session (user chose to continue without
  it). CLI mode calls the same underlying `tools/list`/`tools/call` JSON-RPC methods, so the
  verification is equivalent, but the user has not seen the actual Inspector web page — noted as
  a caveat in `PROCESS.md` in case that visual walkthrough still matters to them
- Housekeeping: stopped the earlier web-UI Inspector background process (was left running on
  `localhost:6274`/`:51289` from before the CLI-mode pivot, unused)
- Rollback: nothing persisted by this exercise — it only ran read-only MCP calls against the
  already-committed Exercise 1 code; no files changed besides this log and `PROCESS.md`

## Activity 2, Exercise 3 (2026-08-08): before/after comparison via `claude -p`

- What: registered `orderhub` in `training-repo/.mcp.json` (already committed as `8af6909`), then
  ran the same question ("哪些商品庫存低於 5?") through two independent non-interactive `claude -p`
  sessions with `--permission-mode bypassPermissions --output-format stream-json --verbose`, cwd
  set to `training-repo` so `.mcp.json` auto-loads for that invocation:
  - No-MCP baseline: `--strict-mcp-config --mcp-config '{"mcpServers":{}}'` to force-disable the
    project's `.mcp.json` regardless. 17 turns, 68.9s API time, $0.36. Explored via `find`/`Read`
    (`Product.cs`), checked `.mcp.json` (registered but no tools reachable), `ToolSearch` x2 (empty),
    read `appsettings.json` for the connection string, found `sqlcmd` via `where`, wrote a raw
    `SELECT ... FROM Products WHERE StockQuantity < 5 AND IsActive = 1` query, hit mangled Chinese
    output twice (`sqlcmd`'s console codepage vs UTF-8), then `-u` (unicode) output to a file +
    `iconv -f UTF-16LE -t UTF-8` to finally get readable text. Answer correct but expensive to reach.
  - With-MCP: plain `.mcp.json` load, no extra flags. 4 turns, 15.8s API time, $0.14. `ToolSearch`
    (server still connecting) -> `ToolSearch` (resolved `mcp__orderhub__low_stock`) -> called it
    directly with `threshold=5` -> clean JSON, correct Chinese text (no encoding issue at all,
    `OrderHubTools.cs`'s `UnsafeRelaxedJsonEscaping` does its job) -> answered.
  - Both runs' `result` content matched exactly (same 5 products/stock numbers) — only the path to
    get there differed.
- Why: `documents/activities/activity-2-custom-mcp.md`, Exercise 3
- Deviation from the guide: guide describes doing this interactively (`/mcp` in a live session,
  toggling `.mcp.json` by renaming it). Used `claude -p` (non-interactive, single-shot) instead for
  the same reason as Exercise 2 — no browser/interactive terminal available in this session — which
  actually gives harder numbers (exact turn count/time/cost) than an eyeballed interactive
  comparison would. Session `init` event's `mcp_servers` list showing `{"name":"orderhub",...}`
  stands in for the `/mcp` check (validation item 1)
- Housekeeping: earlier in this session, the web-UI Inspector's `node` process (PID 4412, port
  6274/51289) was found still listening after a `TaskStop` on its wrapping bash task didn't kill
  the underlying child — killed directly via `Stop-Process -Id 4412 -Force`
- Rollback: nothing persisted beyond `.mcp.json` (already committed separately); the two `claude -p`
  runs were read-only queries against the training DB, no files changed

## Pending / next steps

- [ ] Open a fresh Claude Code session with `training-repo` as the project root and run through the
      "Not yet verifiable from here" checklist above (documented in `agent-configuration.md`'s per-section
      "驗證方式" checklists) — requires the user to do this interactively, since it's the harness's own
      permission-prompt/subagent-dispatch behavior, not something reproducible from outside a live session
- [ ] `PROCESS.md` Exercise 1 self-check — this is a personal reflection checklist for the user
      ("我能不看筆記說出..." / "我核對過..." — first-person self-assessment), not something the agent
      can answer on the user's behalf; agent can present the questions but the user must supply the answers
- [ ] Commits stay local only until the user explicitly asks to push
