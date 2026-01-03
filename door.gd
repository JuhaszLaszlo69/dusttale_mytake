extends Area2D

@export var target_scene: String = ""
@export var target_position: Vector2 = Vector2.ZERO

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
		# Double check player is still overlapping
		var bodies = get_overlapping_bodies()
		for body in bodies:
			if body.name == "Jugador" or body.name == "Player":
				enter_door()
				return

func show_interaction_prompt():
	# Could add a visual prompt here
	pass

func enter_door():
	# Get reference to the node with open_door()
	var main_node = get_node("DoorSFX")
	print("Main node: ", main_node)
	# Call the function
	if main_node != null:
		await main_node.open_door()
	else:
		print("ERROR: could not find node with open_door()!")

	# Change scene after sound + fade
	if target_scene != "":
		get_tree().change_scene_to_file(target_scene)
	else:
		get_tree().change_scene_to_file("res://main_map.tscn")
