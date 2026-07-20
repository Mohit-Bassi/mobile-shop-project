---
name: push
description: Commit and push all current changes to git
---

Stage all changes, write a clear and concise commit message summarizing what changed, commit, and push to the current branch. Run the build (dotnet build in /backend, npm run build in /frontend/public-site and /frontend/admin-panel) before committing to make sure nothing is broken. If the build fails, stop and report the error instead of committing.
