extends CanvasLayer

signal fade_finished

@onready var anim: AnimationPlayer = $AnimationPlayer
@onready var door_sfx: AudioStreamPlayer = $DoorSFX

func fade_into_black(duration: float = 1) -> void:
	anim.play("to_black")
	anim.speed_scale = anim.current_animation_length / duration
	await anim.animation_finished

func fade_from_black() -> void:
	anim.speed_scale = 1.0
	anim.play("from_black")
	await anim.animation_finished
	emit_signal("fade_finished")
