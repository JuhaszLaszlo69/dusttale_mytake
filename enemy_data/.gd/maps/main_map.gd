extends Node2D

@onready var music_player: AudioStreamPlayer = $MusicPlayer
@onready var inventory_container: VBoxContainer = %InventoryContainer
@onready var inventory_label: Label = %InventoryLabel
@onready var exp_label: Label = %ExpLabel
@onready var gold_label: Label = %GoldLabel

func _ready():
	Fade.fade_from_black()
	Fade.fade_finished.connect(_on_fade_finished)
	setup_inventory_ui()
	update_inventory_display()
	update_exp_display()
	update_gold_display()
	
	# Hide boss sprites if they are already defeated
	hide_defeated_bosses()
	
	# Restore player position if returning from battle
	if Global.last_player_position != Vector2.ZERO:
		await get_tree().process_frame
		restore_player_position()
	
	# Check if all bosses are killed and trigger cutscene
	if Global.all_bosses_killed() and not Global.cutscene_played:
		await get_tree().create_timer(0.5).timeout
		trigger_cutscene()

func _on_fade_finished():
	music_player.stream = load("res://songs/005. Ruins (UNDERTALE Soundtrack) - Toby Fox.mp3")
	music_player.play()

func setup_inventory_ui():
	# Inventory UI is set up in the scene, just update display
	pass

func update_inventory_display():
	if Global.battle_inventory.size() == 0:
		inventory_label.text = "Inventory:\n(Empty)"
	else:
		var item_names: Array[String] = []
		for item in Global.battle_inventory:
			item_names.append(item.item_name)
		inventory_label.text = "Inventory:\n" + "\n".join(item_names)

func update_exp_display():
	exp_label.text = "EXP: %d" % Global.player_exp

func update_gold_display():
	gold_label.text = "Gold: %d" % Global.player_gold

func restore_player_position():
	var player: Node2D = null
	var player_node = get_node_or_null("Player")
	if player_node and player_node is Node2D:
		player = player_node
	else:
		player_node = get_node_or_null("Jugador")
		if player_node and player_node is Node2D:
			player = player_node
	
	if player and player is Node2D and Global.last_player_position != Vector2.ZERO:
		player.global_position = Global.last_player_position
		Global.last_player_position = Vector2.ZERO
		Global.last_scene_path = ""

func hide_defeated_bosses():
	# Hide boss sprites if they are already defeated
	var cherry_sprite = get_node_or_null("%CherrySprite")
	if cherry_sprite and Global.is_boss_killed("Cherry"):
		cherry_sprite.visible = false
	
	var poseur_sprite = get_node_or_null("%PoseurSprite")
	if poseur_sprite and Global.is_boss_killed("Poseur"):
		poseur_sprite.visible = false
	
	var present_sprite = get_node_or_null("%PresentSprite")
	if present_sprite and Global.is_boss_killed("Present"):
		present_sprite.visible = false
	
	var godot_sprite = get_node_or_null("%GodotSprite")
	if godot_sprite and Global.is_boss_killed("Godot"):
		godot_sprite.visible = false

func trigger_cutscene():
	Global.cutscene_played = true
	Fade.fade_into_black()
	await Fade.fade_into_black()
	get_tree().change_scene_to_file("res://cutscenes/all_bosses_defeated.tscn")
