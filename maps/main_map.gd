extends Node2D

@onready var music_player: AudioStreamPlayer = $MusicPlayer


func _ready():
	Fade.fade_from_black()
	Fade.fade_finished.connect(_on_fade_finished)

func _on_fade_finished():
	music_player.stream = load("res://songs/005. Ruins (UNDERTALE Soundtrack) - Toby Fox.mp3")
	music_player.play()
