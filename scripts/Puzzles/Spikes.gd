extends StaticBody2D
@export var aryActivationPhrases: Array[String] = [] #Strings that'll be accepted one of the 'activation(s)' # (Array, String)
@export var intMaxActivationCount : int #Amount of signals required before opening
var intActivationCount : int = 0 #Amount of signals recieved

func _ready():
	signalManager.connect("object_activated", Callable(self, "_on_object_activated"))
	if globalVariables.activationList.has(str(get_tree().current_scene.scene_file_path)): #Check if there are any objects already activated in this scene
		for item in globalVariables.activationList[str(get_tree().current_scene.scene_file_path)].split(", "): #Loop through all saved activation phrases
			_on_object_activated(str(item)) #Send saved phrases through function

func _on_object_activated(s):
	if str(s) in aryActivationPhrases: #Check if received signal is acceptable
		intActivationCount += 1 #Count the activation
		if intMaxActivationCount == intActivationCount:
			$Sprite2D.set_frame(5) #Visually retract spikes
			$CollisionShape2D.set_deferred("disabled", true) #Turn off the collider
