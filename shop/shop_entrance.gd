extends Area2D

var player_nearby = false

func _ready():
	body_entered.connect(_on_body_entered)
	body_exited.connect(_on_body_exited)

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
				enter_shop()
				return

func show_interaction_prompt():
	# Could add a visual prompt here
	pass

func enter_shop():
	Fade.fade_into_black()
	await Fade.fade_into_black()
	get_tree().change_scene_to_file("res://shop/shop.tscn")
