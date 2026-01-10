extends Area2D

@export var enemy_scene: PackedScene
@export var boss_name: String = ""

func _ready():
	# Check if this boss is already killed and hide the zone if so
	if boss_name != "" and Global.is_boss_killed(boss_name):
		visible = false
		set_deferred("monitoring", false)
		set_deferred("monitorable", false)
		return
	
	body_entered.connect(_on_body_entered)
	
func _on_body_entered(body):
	if body.name == "Jugador" or body.name == "Player":
		start_battle()

func start_battle():
	if enemy_scene == null:
		# Default to cherry if none specified
		enemy_scene = preload("res://enemy_data/cherry.tscn")
	
	# Save current scene and player position before battle
	var current_scene = get_tree().current_scene.scene_file_path
	Global.last_scene_path = current_scene
	
	# Find player and save position
	var player: Node2D = null
	var scene_root = get_tree().current_scene
	
	# Try different ways to find the player
	var player_node = scene_root.get_node_or_null("Player")
	if player_node and player_node is Node2D:
		player = player_node
	else:
		player_node = scene_root.get_node_or_null("Jugador")
		if player_node and player_node is Node2D:
			player = player_node
		else:
			player_node = scene_root.get_node_or_null("Objetos/Jugador")
			if player_node and player_node is Node2D:
				player = player_node
	
	if not player:
		# Try finding by groups
		var players = get_tree().get_nodes_in_group("player")
		for p in players:
			if p is Node2D:
				player = p
				break
	
	if player and player is Node2D:
		Global.last_player_position = player.global_position
	else:
		Global.last_player_position = Vector2.ZERO
	
	var enemy_instance = enemy_scene.instantiate()
	if enemy_instance:
		Battle.enemy = enemy_instance
		Fade.fade_into_black()
		await Fade.fade_into_black()
		get_tree().change_scene_to_file("uid://45qmet5s5aix")
