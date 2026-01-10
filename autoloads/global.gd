extends Node

signal wave_done(wave_scene: Node2D, soul: Soul)
signal add_bullet(bullet: Node2D, transform: Transform2D)
signal change_mercy(amount: int)
signal heal_player(amount: int)
signal bullet_destroyed(pos: Vector2)
signal monster_visible(new_val: bool)
signal play_shoot_sound

# Boss tracking and progression
var killed_bosses: Array[String] = []
var player_exp: int = 0
var player_gold: int = 0
var battle_inventory: Array[Item] = []
var cutscene_played: bool = false

# Position tracking for battle return
var last_scene_path: String = ""
var last_player_position: Vector2 = Vector2.ZERO

const BOSS_NAMES: Array[String] = ["Cherry", "Poseur", "Present", "Godot"]

func mark_boss_killed(boss_name: String) -> void:
	if not is_boss_killed(boss_name):
		killed_bosses.append(boss_name)

func is_boss_killed(boss_name: String) -> bool:
	return boss_name in killed_bosses

func add_exp(amount: int) -> void:
	player_exp += amount

func spend_exp(amount: int) -> bool:
	if player_exp >= amount:
		player_exp -= amount
		return true
	return false

func spend_gold(amount: int) -> bool:
	if player_gold >= amount:
		player_gold -= amount
		return true
	return false

func add_item_to_inventory(item: Item) -> void:
	battle_inventory.append(item)

func all_bosses_killed() -> bool:
	for boss_name in BOSS_NAMES:
		if not is_boss_killed(boss_name):
			return false
	return true
