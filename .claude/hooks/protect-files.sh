#!/usr/bin/env bash
# Blocks Edit/Write on secrets, production config, and .git internals.
# Reads the PreToolUse hook payload (JSON) from stdin.

input=$(cat)
file=$(echo "$input" | node -e '
let d="";
process.stdin.on("data",c=>d+=c);
process.stdin.on("end",()=>{
  try {
    const j = JSON.parse(d);
    process.stdout.write((j.tool_input && j.tool_input.file_path) || "");
  } catch (e) {}
});
')

if [ -z "$file" ]; then
  exit 0
fi

base=$(basename "$file")
blocked=""

case "$base" in
  .env|appsettings.json|appsettings.Production.json)
    blocked="1"
    ;;
esac

case "$file" in
  */.git/*|*/.git|.git/*|.git)
    blocked="1"
    ;;
esac

if [ -n "$blocked" ]; then
  echo "Blocked: '$file' is a protected file (secrets, production config, or .git internals) and cannot be edited by Claude Code." >&2
  exit 2
fi

exit 0
