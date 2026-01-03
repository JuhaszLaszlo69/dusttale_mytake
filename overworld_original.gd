extends Node2D

@export var default_enemy: PackedScene

func _ready():
	# Fade in from black
	Fade.fade_from_black()
	
	# Connect battle trigger
	var battle_trigger = $BattleTrigger
	if battle_trigger:
		# Create collision shape if it doesn't exist
		var collision = battle_trigger.get_node_or_null("CollisionShape2D")
		if not collision:
			collision = CollisionShape2D.new()
			var shape = RectangleShape2D.new()
			shape.size = Vector2(100, 100)
			collision.shape = shape
			battle_trigger.add_child(collision)
		
		battle_trigger.body_entered.connect(_on_battle_trigger_entered)

func _on_battle_trigger_entered(body):
	if body.name == "Jugador" or body.name == "Player":
		start_battle()

func start_battle(enemy_scene: PackedScene = null):
	if enemy_scene == null:
		enemy_scene = default_enemy
	if enemy_scene == null:
		# Default to first enemy if none specified
		enemy_scene = preload("res://enemy_data/cherry.tscn")
	
	# Load enemy and start battle
	var enemy_instance = enemy_scene.instantiate()
	if enemy_instance and enemy_instance.has_method("get") and enemy_instance.get("enemy_name"):
		Battle.enemy = enemy_instance
		Fade.fade_into_black()
		await Fade.fade_into_black()
		# Use the UID from the battle scene
		get_tree().change_scene_to_file("uid://45qmet5s5aix")
