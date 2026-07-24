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

## Pending / next steps

- [ ] Re-run commit now that git identity is set
- [ ] Run the verification checklist from `agent-configuration.md`:
      - deny force-push (`git push --force`) is rejected outright
      - `dotnet test` runs without asking (allow)
      - `dotnet ef database drop` prompts for confirmation (ask)
      - editing a file under `Migrations/` is denied
      - asking agent to TRUNCATE a table is blocked by the PreToolUse hook
      - an Edit/Write produces a line in `.claude/hooks/edit-log.txt`
- [ ] Commit stays local only until the user explicitly asks to push
