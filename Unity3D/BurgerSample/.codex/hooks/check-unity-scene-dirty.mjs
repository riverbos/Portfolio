import fs from "node:fs";
import path from "node:path";

const input = await readStdin();
const cwd = input.cwd || process.cwd();
const statePath = path.join(cwd, ".codex", "state", "unity-scene-state.json");

if (!fs.existsSync(statePath)) {
  process.exit(0);
}

try {
  const state = JSON.parse(fs.readFileSync(statePath, "utf8"));
  const dirtyScenes = Array.isArray(state.dirtyScenes) ? state.dirtyScenes : [];

  if (dirtyScenes.length > 0) {
    const sceneList = dirtyScenes.map(scene => scene.path || scene.name || "Untitled Scene").join(", ");
    const reason = `Unity Scene이 Dirty 상태입니다: ${sceneList}. Unity Editor에서 Scene을 저장한 뒤 다시 시도해 주세요.`;

    process.stdout.write(JSON.stringify({
      hookSpecificOutput: {
        hookEventName: "PreToolUse",
        permissionDecision: "deny",
        permissionDecisionReason: reason
      },
      systemMessage: reason
    }));
  }
} catch (error) {
  process.stderr.write(`Unity Scene 상태 파일을 읽지 못했습니다: ${error.message}\n`);
}

async function readStdin() {
  let data = "";
  for await (const chunk of process.stdin) {
    data += chunk;
  }

  if (!data.trim()) {
    return {};
  }

  try {
    return JSON.parse(data);
  } catch {
    return {};
  }
}
