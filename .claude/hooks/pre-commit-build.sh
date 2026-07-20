#!/usr/bin/env bash
# Blocks `git commit` if the backend or either frontend fails to build.
# Registered on PreToolUse with an "if": "Bash(git commit *)" filter as a fast
# path, but that filter has a known edge case (a raw carriage-return byte
# anywhere in the command can make it match unrelated commands), so this
# script independently re-checks the actual command from stdin and exits
# immediately unless it is really a `git commit`.

set -o pipefail

input=$(cat)
command=$(echo "$input" | node -e '
let d="";
process.stdin.on("data",c=>d+=c);
process.stdin.on("end",()=>{
  try {
    const j = JSON.parse(d);
    process.stdout.write((j.tool_input && j.tool_input.command) || "");
  } catch (e) {}
});
')

case "$command" in
  git\ commit*) ;;
  *) exit 0 ;;
esac

echo "Running pre-commit build checks (backend, public-site, admin-panel)..." >&2

if ! (cd backend && dotnet build 2>&1); then
  echo "Blocked commit: 'dotnet build' failed in /backend. Fix the build errors above before committing." >&2
  exit 2
fi

if ! (cd frontend/public-site && npm run build 2>&1); then
  echo "Blocked commit: 'npm run build' failed in /frontend/public-site. Fix the build errors above before committing." >&2
  exit 2
fi

if ! (cd frontend/admin-panel && npm run build 2>&1); then
  echo "Blocked commit: 'npm run build' failed in /frontend/admin-panel. Fix the build errors above before committing." >&2
  exit 2
fi

exit 0
