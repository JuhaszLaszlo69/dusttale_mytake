# Undertale Clone - Godot Project

This is a working Undertale clone created by merging assets and code from multiple sources.

## Project Structure

### Core Systems
- **Battle System**: Complete battle system with enemies, bullets, waves, and soul mechanics (`battle/`, `enemy_data/`, `bullets/`, `waves/`, `soul/`)
- **Overworld**: Player movement and exploration (`overworld.tscn`, `player/`)
- **Title Screen**: Main menu with battle selection (`intermediate_scenes/title_screen.tscn`)

### Autoloads
- `Global`: Battle system global signals and utilities
- `Fade`: Screen fade transitions
- `signalManager`: Game-wide signal management
- `MusicController`: Background music control
- `SceneChanger`: Scene transition management
- `globalVariables`: Player stats, inventory, save data
- `itemLibrary`: Item definitions

### Key Features
- Full battle system with multiple enemy types (Cherry, Poseur, Present, Godot)
- Player movement with animations
- Battle transitions from overworld
- Title screen with enemy selection
- Save system
- Dialogue system (from Has_entire_map)
- Multiple maps and tilesets

## How to Play

1. Start the game - you'll see the title screen
2. Select "Go to Overworld" to explore, or select an enemy name to go directly to battle
3. In the overworld, walk into the "Battle Zone" area to trigger a battle
4. Use arrow keys/WASD to move
5. In battle, use arrow keys to dodge bullets and Z/X to interact with menus

## Controls
- **Movement**: Arrow Keys or WASD
- **Interact**: Z or Enter
- **Cancel**: X or Escape

## Notes
- Some path references in .import files may show old paths - these are metadata only and don't affect functionality
- The battle system uses UID references for scenes
- Player sprites and animations are in `player/Sprites/`
- Battle assets are in `sprites/`, `sounds/`, and `songs/`

## Merged Sources
- Battle system from `Battle_system/undertale_godot-enemies`
- Player and map from `Start_of_the_game_map/Godot-Tale-master`
- Dialogue and NPCs from `Has_entire_map/Godette-Tale-master`
- Additional assets from other folders

