extends Node2D

@onready var music_player: AudioStreamPlayer = $MusicPlayer
@onready var door_sfx: AudioStreamPlayer = $DoorSFX


func _ready():
	Fade.fade_from_black()
	Fade.fade_finished.connect(_on_fade_finished)

func open_door():
	# Load door sound
	door_sfx.stream = load("res://addons/SMRT/Minecraft Door Sound Effect (Open and Close Sound Effects)..mp3")
	door_sfx.play()
	print("Door opened")

	# Get sound length
	var door_length := door_sfx.stream.get_length()

	# Start fade synced to sound
	await Fade.fade_into_black(door_length)

func _on_fade_finished():
	music_player.stream = load("res://addons/SMRT/005. Ruins (UNDERTALE Soundtrack) - Toby Fox.mp3")
	music_player.play()
