extends Node2D

@onready var text_label: Label = %TextLabel
@onready var particles: CPUParticles2D = %Particles

var messages: Array[String] = [
	"Game over you killed every boss!",
	"You have achieved the ultimate power...",
	"But at what cost?",
	"The world is now empty...",
	"Only you remain.",
    "Determined."
]

var current_message_index := 0

func _ready() -> void:
	# Clear text immediately and hide it
	text_label.text = ""
	text_label.modulate.a = 0.0
	text_label.visible = true
	
	# Center particles based on viewport
	var viewport_size = get_viewport().get_visible_rect().size
	particles.position = viewport_size / 2.0
	
	# Wait for fade to complete before starting
	await Fade.fade_from_black()
	await get_tree().process_frame  # Ensure everything is ready
	start_cutscene()

func start_cutscene() -> void:
	# Start particle effects
	particles.emitting = true
	
	# Show first message
	await show_message(messages[0])
	await get_tree().create_timer(1.0).timeout
	
	# Show remaining messages
	for i in range(1, messages.size()):
		text_label.text = ""
		await get_tree().create_timer(0.5).timeout
		await show_message(messages[i])
		await get_tree().create_timer(1.5).timeout
	
	# Final fade out
	await get_tree().create_timer(2.0).timeout
	
	# Fade out text
	var fade_tween := create_tween()
	fade_tween.tween_property(text_label, "modulate:a", 0.0, 0.5)
	await fade_tween.finished
	
	text_label.text = ""
	particles.emitting = false
	
	# Fade to black and transition
	await Fade.fade_into_black()
	if is_inside_tree() and get_tree():
		get_tree().change_scene_to_file("res://maps/main_map.tscn")

func show_message(message: String) -> void:
	# Clear and hide text first
	text_label.text = ""
	text_label.modulate.a = 0.0
	text_label.visible = true
	
	# Fade in text label first (quick fade)
	var fade_tween := create_tween()
	fade_tween.tween_property(text_label, "modulate:a", 1.0, 0.2)
	await fade_tween.finished
	
	# Now type out message letter by letter (while visible)
	var full_text := ""
	for i in range(message.length()):
		full_text += message[i]
		text_label.text = full_text
		await get_tree().create_timer(0.05).timeout
	
	# Wait a bit after typing completes
	await get_tree().create_timer(0.5).timeout

