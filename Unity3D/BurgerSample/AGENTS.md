# Repository Guidelines

## Project Structure & Module Organization
This is a Unity project targeting Unity `6000.3.8f1`. Gameplay code lives in `Assets/Scripts`, with subfolders such as `Player`, `HoldItem`, `GameConfig`, and `UI_Panel`. Reusable interaction code is in `Assets/ActionSpot`. Scenes are in `Assets/Scenes` (`GameScene.unity`, `SampleScene.unity`), prefabs are in `Assets/Prefabs`, and art assets are grouped under `Assets/Art`. Package dependencies are declared in `Packages/manifest.json`; project-level Unity settings are in `ProjectSettings`.

Do not commit generated local state from `Library`, `Temp`, `Logs`, `.vs`, or user-specific settings unless explicitly required.

## Build, Test, and Development Commands
Open the project with Unity Hub or the matching Unity Editor version:

```powershell
Unity.exe -projectPath .
```

Run tests in batch mode when test assemblies exist:

```powershell
Unity.exe -batchmode -projectPath . -runTests -testPlatform EditMode -quit
Unity.exe -batchmode -projectPath . -runTests -testPlatform PlayMode -quit
```

For local iteration, open `Assets/Scenes/GameScene.unity` in the Editor and use Play Mode. Build targets should be configured through Unity Build Settings unless a project build script is added later.

## Coding Style & Naming Conventions
Use C# with 4-space indentation and Unity-style `MonoBehaviour` lifecycle methods (`Start`, `Update`, etc.). Keep public types and methods in `PascalCase`; private fields and locals use `camelCase`. Serialized private fields should use `[SerializeField] private`, as seen in `PlayerController`. Interface names use an `I` prefix, such as `IInteractable` and `IHoldable`.

Keep runtime scripts in feature folders under `Assets/Scripts`. Put editor-only code in an `Editor` folder so Unity excludes it from player builds.

## Testing Guidelines
The Unity Test Framework is included, but this repository currently has no dedicated test folder. Add Edit Mode tests under `Assets/Tests/EditMode` for pure logic and Play Mode tests under `Assets/Tests/PlayMode` for scene or physics behavior. Name test files after the behavior under test, for example `BurgerPoolTests.cs`.

Before opening a pull request, run relevant Edit Mode or Play Mode tests and manually smoke-test affected scenes.

## Commit & Pull Request Guidelines
Recent history uses short, descriptive commit subjects. Keep commits focused and write summaries in either Korean or English, for example `README update` or `Add burger pool interaction`. Avoid mixing unrelated asset, scene, and code changes in one commit.

Pull requests should include a concise description, affected scenes or prefabs, test results, and screenshots or short clips for visible gameplay/UI changes. Link related issues when available and call out any required Unity version or package changes.
