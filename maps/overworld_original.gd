extends Node2D

@export var default_enemy: PackedScene

func _ready():
	# Fade in from black
	Fade.fade_from_black()
