extends Node2D

@export var default_enemy: PackedScene

@onready var exp_label: Label = %ExpLabel
@onready var gold_label: Label = %GoldLabel

func _ready():
	# Fade in from black
	Fade.fade_from_black()
	update_exp_display()
	update_gold_display()
	
	# Restore player position if returning from battle
	if Global.last_player_position != Vector2.ZERO:
		await get_tree().process_frame
		restore_player_position()

func update_exp_display():
	exp_label.text = "EXP: %d" % Global.player_exp

func update_gold_display():
	gold_label.text = "Gold: %d" % Global.player_gold

func restore_player_position():
	var player: Node2D = null
	var player_node = get_node_or_null("Objetos/Jugador")
	if player_node and player_node is Node2D:
		player = player_node
	
	if player and player is Node2D and Global.last_player_position != Vector2.ZERO:
		player.global_position = Global.last_player_position
		Global.last_player_position = Vector2.ZERO
		Global.last_scene_path = ""
