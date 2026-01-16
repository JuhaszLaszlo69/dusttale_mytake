extends CanvasLayer

@export var enemies: Array[PackedScene]

func _ready() -> void:
	Fade.fade_from_black()
	%Title.text = Util.shake(%Title.text)
	
	# Add overworld button first
	var overworld_button := Button.new()
	overworld_button.add_theme_font_size_override("font_size", 50)
	overworld_button.text = "Go to Overworld"
	%BattlesContainer.add_child(overworld_button)
	overworld_button.pivot_offset = overworld_button.size / 2
	overworld_button.focus_entered.connect(_on_focus_entered.bind(overworld_button))
	overworld_button.focus_exited.connect(_on_focus_exited.bind(overworld_button))
	overworld_button.pressed.connect(go_to_overworld)
	
	# Add debug button to view cutscene
	var debug_button := Button.new()
	debug_button.add_theme_font_size_override("font_size", 50)
	debug_button.text = "[DEBUG] View Cutscene"
	%BattlesContainer.add_child(debug_button)
	debug_button.pivot_offset = debug_button.size / 2
	debug_button.focus_entered.connect(_on_focus_entered.bind(debug_button))
	debug_button.focus_exited.connect(_on_focus_exited.bind(debug_button))
	debug_button.pressed.connect(go_to_cutscene)
	
	for scene: PackedScene in enemies:
		var instance: Node = scene.instantiate()
		assert(instance is Enemy, "Only put Enemy scenes in the enemies array")
		var enemy := instance as Enemy
		var button := Button.new()
		button.add_theme_font_size_override("font_size", 50)
		button.text = "[DEBUG] - %s" % enemy.enemy_name
		%BattlesContainer.add_child(button)
		button.pivot_offset = button.size / 2
		button.focus_entered.connect(
			_on_focus_entered.bind(button)
		)
		button.focus_exited.connect(
			_on_focus_exited.bind(button)
		)
		button.pressed.connect(
			go_to_battle.bind(enemy)
			)
	
	%BattlesContainer.get_child(0).grab_focus()

func go_to_overworld() -> void:
	%Song.stop()
	await Fade.fade_into_black()
	
	# Try loading the scene first to check if it exists
	var scene = load("res://maps/overworld_original.tscn")
	if scene == null:
		print("ERROR: Could not load overworld_original.tscn")
		# Try alternative
		scene = load("res://overworld.tscn")
		if scene == null:
			print("ERROR: Could not load overworld.tscn either!")
			return
	
	var error = get_tree().change_scene_to_file("res://maps/overworld_original.tscn")
	if error != OK:
		print("Error changing scene: ", error)
		print("Error code: ", error)

func _on_focus_entered(button: Button) -> void:
	button.modulate.a = 1
	%MoveSound.play()
	var tween := get_tree().create_tween().set_ease(Tween.EASE_IN_OUT).set_trans(Tween.TRANS_BOUNCE)
	tween.tween_property(button, "scale", Vector2(1.5,1.5), 0.2)
	tween.tween_property(button, "scale", Vector2(1,1), 0.1)

func _on_focus_exited(button: Button) -> void:
	button.modulate.a = 0.5

func go_to_cutscene() -> void:
	%Song.stop()
	await Fade.fade_into_black()
	if is_inside_tree() and get_tree():
		get_tree().change_scene_to_file("res://cutscenes/all_bosses_defeated.tscn")

func go_to_battle(enemy: Enemy) -> void:
	%Song.stop()
	%Encounter1.play()
	var debug_text = "[DEBUG] - %s" % enemy.enemy_name
	for button: Button in %BattlesContainer.get_children():
		button.modulate.a = 1.0 if button.text == debug_text else 0.0
		button.release_focus()
	await get_tree().create_timer(0.25).timeout
	%Encounter2.play()
	await Fade.fade_into_black()
	Battle.enemy = enemy
	get_tree().change_scene_to_file("uid://45qmet5s5aix")
