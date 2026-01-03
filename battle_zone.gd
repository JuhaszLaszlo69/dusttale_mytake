extends Area2D

@export var enemy_scene: PackedScene

func _ready():
	body_entered.connect(_on_body_entered)
	
func _on_body_entered(body):
	if body.name == "Jugador" or body.name == "Player":
		start_battle()

func start_battle():
	if enemy_scene == null:
		# Default to cherry if none specified
		enemy_scene = preload("res://enemy_data/cherry.tscn")
	
	var enemy_instance = enemy_scene.instantiate()
	if enemy_instance:
		Battle.enemy = enemy_instance
		Fade.fade_into_black()
		await Fade.fade_into_black()
		get_tree().change_scene_to_file("uid://45qmet5s5aix")

