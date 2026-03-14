# Unity Editor Baseline

Status: execution note v1

Chosen baseline:
- Unity **2022.3 LTS**
- Repo bootstrap pin: **2022.3.70f1** in `ProjectSettings/ProjectVersion.txt`
- Validation host policy: if `2022.3.70f1` is not locally available, use the newest available **2022.3 LTS** patch on the validation host rather than jumping to Unity 6+

Why this is the conservative compatible choice:
- the current `Assets/Binho/Core` code already uses **file-scoped namespaces**, which requires **C# 10** syntax support
- that makes older LTS lines such as **2021.3** a poor fit for first import/compile validation
- `2022.3 LTS` is the narrowest stable LTS family that matches the current code style without opting into a newer major Unity line than necessary
- this tranche only needs import/test enablement, so a conservative pre-Unity-6 LTS baseline is preferable to a newer feature line

Notes:
- this choice is about first import/compile/test compatibility for the existing pre-geometry contracts/sync scaffold
- it is **not** a commitment yet to shipping on a newer Unity family, package stack, render pipeline, or runtime feature set
- exact package resolution still needs a real Unity import on a host with an installed editor
