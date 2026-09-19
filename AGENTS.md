## Rules

- Consult first before troubleshooting, debugging, refactoring, or any code change. When consulting, reply in <=3 lines: the blocker/what happened and the plan. Do not run builds/tests, git commands, or edit files until the user approves.

- Approvals are scoped to exactly what was approved. An approved change does not authorize: fixing analyzer/code errors, editing additional files, running builds/tests to "verify", or other follow-ups. If the work needs anything beyond the approved scope — including fixes for errors that the change surfaces — stop and re-consult, then wait for approval.

- If an approved build/test was run and it fails, do not fix anything. Report the failures and a proposed fix plan, then wait for approval.

## Build

- `dotnet build HadesWeb/HadesWeb.csproj`
- `dotnet watch run --project HadesWeb/HadesWeb.csproj` (or F5 via `.vscode/launch.json` -> `bin/Debug/net10.0/HadesWeb.dll`).

## Gotchas

- `App_Data/users.json`, `App_Data/emails.json`: UTF-16LE + CRLF; read via `StreamReader` (BOM auto-detect). Rewriting as UTF-8 breaks parsing. Passwords are bcrypt; `Role` shipped as `ClaimTypes.Role` at login.
- Paths resolve from `IWebHostEnvironment.ContentRootPath` (`App_Data/...`, lowercase; case-sensitive off-Windows). Controllers use primary constructors.
- `Models/Home.cs` uses `IFormFile`; `Controller.User` is the `ClaimsPrincipal` from cookie auth.