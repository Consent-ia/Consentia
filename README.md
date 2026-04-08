# Consentia

**Consentia** is a 2D top-down narrative RPG built in Unity 6. The game uses an immersive, sci-fi city setting as a metaphor for **digital consent and data privacy**. Players take on the role of the "Chief," working to rebuild a city's shattered identity system, one choice at a time.

## Overview

Consentia is an interactive narrative game that takes players through different acts, each with unique environments and characters. The game features a save/load system, allowing players to continue their progress across sessions.

## Features

- **Multi-Act Story**: Progress through five distinct acts (Act 1–5) plus an introduction
- **Interactive NPCs**: Engage with various characters including Ada, Aster, Brink, Carta, Curio, Dweller, Ledger, Lexi, Merchant, Mira, Mirr, Nox, Pulse, Quill, Trace, and Velor
- **Save/Load System**: Continue your journey from where you left off
- **Landing Screen**: Start a new game or load previous progress
- **In-Game Menu**: Resume, restart, or exit with ease
- **Custom Animations**: Smooth player, NPC, portal, and UI animations
- **Audio System**: Immersive music and sound effects with audio mixing

## Technical Details

### Built With
- **Unity Version**: 6000.3.9f1
- **Unity 2D Packages**:
  - 2D Animation (v13.0.4)
  - 2D Tilemap
  - 2D Sprite Shape
  - 2D Aseprite Importer
  - Universal Render Pipeline (URP)
- **TextMesh Pro**: For advanced text rendering
- **Unity Input System**: Modern input handling

### Project Structure

```
Assets/
├── Animation/         # Character and UI animations
├── Art/               # Sprites, UI elements, maps, and visual assets
├── NPC/               # NPC dialog assets
├── Prefabs/           # Reusable game objects
├── Scenes/            # Game scenes (IntroAct, Act1-4)
├── Scripts/           # C# game logic
│   ├── GameManagerScripts/
│   ├── MapScripts/
│   ├── NPCScripts/
│   ├── PlayerScripts/
│   ├── SaveSystem/
│   ├── SoundScripts/
│   └── UIScripts/
├── Sounds/            # Music and SFX
└── Tiles/             # Tilemap assets for environments
```

## Getting Started

### Prerequisites
- Unity Editor (version 6000.3.9f1 or compatible)
- Git (for version control)

### Installation

1. Clone the repository:
    ```bash
    git clone https://github.com/Consent-ia/Consentia.git
    cd Consentia/src
    ```

2. Open the project in Unity:
   - Launch Unity Hub
   - Click "Add" and select the \`src\` folder
   - Open the project with Unity 6000.3.9f1

3. Open the IntroAct scene to start:
   - Navigate to \`Assets/Scenes/IntroAct.unity\`
   - Press Play in the Unity Editor

## How to Play

1. **Start Screen**: Choose to start a new game or load a saved game
2. **Movement**: Use the configured input controls to move your character
3. **NPC Interaction**: Approach NPCs to trigger dialogue
4. **Menu**: Access the in-game menu to resume, restart, or exit
5. **Progress**: Your progress is automatically saved and can be loaded later

## Development

### Key Systems

- **SceneTransitionManager**: Handles scene transitions with fade effects
- **ProgressManager**: Tracks player progress and game state
- **Save System**: Manages saving and loading game states
- **Dialog System**: Powers NPC conversations and interactions
- **Audio System**: Controls music and sound effects with mixing

## Acknowledgments

- Unity Technologies for the game engine and 2D tools
- Asset creators and contributors
