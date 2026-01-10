extends Area2D

var player_nearby = false
var shop_ui: Control = null

func _ready():
	body_entered.connect(_on_body_entered)
	body_exited.connect(_on_body_exited)
	# Find shop UI in the scene
	shop_ui = get_tree().current_scene.get_node_or_null("ShopUI")

func _on_body_entered(body):
	if body.name == "Jugador" or body.name == "Player":
		player_nearby = true
		show_interaction_prompt()

func _on_body_exited(body):
	if body.name == "Jugador" or body.name == "Player":
		player_nearby = false

func _process(_delta):
	if Input.is_action_just_pressed("ui_accept") and player_nearby:
		var bodies = get_overlapping_bodies()
		for body in bodies:
			if body.name == "Jugador" or body.name == "Player":
				open_shop()
				return

func show_interaction_prompt():
	# Could add a visual prompt here
	pass

func open_shop():
	# Save current scene and player position before entering shop
	var current_scene = get_tree().current_scene.scene_file_path
	Global.last_scene_path = current_scene
	
	var player: Node2D = null
	var scene_root = get_tree().current_scene
	player = scene_root.get_node_or_null("Objetos/Jugador") as Node2D
	
	if not player:
		player = scene_root.get_node_or_null("Jugador") as Node2D
	
	if not player:
		player = scene_root.get_node_or_null("Player") as Node2D
	
	if not player:
		var players = get_tree().get_nodes_in_group("player")
		if players.size() > 0 and players[0] is Node2D:
			player = players[0] as Node2D
	
	if player:
		Global.last_player_position = player.global_position
	else:
		Global.last_player_position = Vector2.ZERO
	
	# Change to shop scene with fade
	Fade.fade_into_black()
	await Fade.fade_into_black()
	get_tree().change_scene_to_file("res://shop/shop.tscn")
