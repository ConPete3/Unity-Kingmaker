# Kingmaker

A 2D strategy game built with Unity.

## Tech Stack

- **Engine:** Unity 2D
- **Language:** C#
- **Target Platforms:** TBD

## Project Structure

```
Assets/
├── Scripts/
│   ├── Managers/     # Game managers (GameManager, AudioManager, etc.)
│   ├── HexGrid/      # Hex grid system and related logic
│   ├── Data/         # Data classes, scriptable objects, configs
│   ├── Screens/      # Screen/scene controllers
│   └── UI/           # UI components and controllers
├── Prefabs/          # Reusable game object prefabs
└── Sprites/          # 2D sprite assets
```

## Getting Started

### Prerequisites

- Unity 2022.3 LTS or later (check `ProjectSettings/ProjectVersion.txt` for exact version)
- Git

### Opening the Project

1. Clone this repository
2. Open Unity Hub
3. Click "Add" and select the project folder
4. Open the project with the correct Unity version

## Development Workflow

1. **Create a feature branch** from `main` for new features
2. **Keep commits atomic** - one logical change per commit
3. **Test in Unity** before committing
4. **Pull latest changes** before starting new work

### Code Guidelines

- Follow C# naming conventions (PascalCase for public members, camelCase for private)
- Keep scripts organized in appropriate folders
- Use meaningful names for GameObjects and components

## License

TBD
