extends Node2D

# Shop items configuration: [Item resource, EXP cost, Gold cost]
var shop_items: Array[Dictionary] = [
	{"item": preload("res://items/apple.tres"), "exp_cost": 10, "gold_cost": 5},
	{"item": preload("res://items/nice_cream.tres"), "exp_cost": 15, "gold_cost": 8},
	{"item": preload("res://items/pie.tres"), "exp_cost": 20, "gold_cost": 10}
]

var button := preload("uid://ptt71q0lsxgx")

var exp_label: Label
var gold_label: Label
var items_container: VBoxContainer
var text_box: TextBox

func _ready() -> void:
	# Find nodes manually to ensure they're found
	exp_label = get_node_or_null("%ExpLabel") as Label
	gold_label = get_node_or_null("%GoldLabel") as Label
	items_container = get_node_or_null("%ItemsContainer") as VBoxContainer
	text_box = get_node_or_null("%TextBox") as TextBox
	
	if not exp_label:
		exp_label = get_node_or_null("UILayer/UI/ShopContainer/ExpLabel") as Label
	if not gold_label:
		gold_label = get_node_or_null("UILayer/UI/ShopContainer/GoldLabel") as Label
	if not items_container:
		items_container = get_node_or_null("UILayer/UI/ShopContainer/ItemsContainer") as VBoxContainer
	if not text_box:
		text_box = get_node_or_null("UILayer/UI/TextBox") as TextBox
	
	# Clear text box default text and hide it initially
	if text_box:
		text_box.text = ""
		text_box.visible_ratio = 0.0
		text_box.visible = false
	
	# Wait a frame to ensure all nodes are ready
	await get_tree().process_frame
	Fade.fade_from_black()
	update_exp_display()
	update_gold_display()
	populate_shop()
	var exit_button = get_node_or_null("%ExitButton")
	if exit_button:
		exit_button.grab_focus()

func update_exp_display() -> void:
	if exp_label:
		exp_label.text = "EXP: %d" % Global.player_exp

func update_gold_display() -> void:
	if gold_label:
		gold_label.text = "Gold: %d" % Global.player_gold

func populate_shop() -> void:
	if not items_container:
		return
	# Clear existing items
	for child in items_container.get_children():
		child.queue_free()
	
	# Add shop items
	for shop_item in shop_items:
		var item: Item = shop_item.item
		var exp_cost: int = shop_item.exp_cost
		var gold_cost: int = shop_item.gold_cost
		
		var item_button: Button = button.instantiate()
		# Ensure button has proper minimum size - make it wider and taller to prevent overlap and wrapping
		item_button.custom_minimum_size = Vector2(600, 80)
		var item_text := "%s - %d EXP, %d Gold" % [item.item_name, exp_cost, gold_cost]
		var text_node = item_button.get_node_or_null("text")
		if text_node:
			# Ensure text node is properly sized to fit within button and prevent wrapping
			text_node.custom_minimum_size = Vector2(580, 70)
			# Disable text wrapping to keep text on one line
			text_node.autowrap_mode = TextServer.AUTOWRAP_OFF
			text_node.text = Util.shake(item_text)
		item_button.focus_exited.connect(func():
			item_button.modulate.a = 0.5)
		item_button.pressed.connect(buy_item.bind(item, exp_cost, gold_cost))
		items_container.add_child(item_button)

func buy_item(item: Item, exp_cost: int, gold_cost: int) -> void:
	if not text_box:
		return
	
	# Check if player has enough EXP and Gold
	var has_enough_exp = Global.player_exp >= exp_cost
	var has_enough_gold = Global.player_gold >= gold_cost
	
	if has_enough_exp and has_enough_gold:
		# Spend both currencies (atomic transaction)
		Global.spend_exp(exp_cost)
		Global.spend_gold(gold_cost)
		Global.add_item_to_inventory(item)
		update_exp_display()
		update_gold_display()
		text_box.visible = true
		# Use plain text directly without BBCode for shop messages
		var message = "* You bought %s for %d EXP and %d Gold!" % [item.item_name, exp_cost, gold_cost]
		text_box.text = message
		text_box.visible_ratio = 1.0
		await get_tree().create_timer(2.0).timeout
		text_box.clear_text()
		text_box.visible = false
	else:
		text_box.visible = true
		# Use plain text directly without BBCode for shop messages
		var error_message = ""
		if not has_enough_exp and not has_enough_gold:
			error_message = "* You don't have enough EXP and Gold!"
		elif not has_enough_exp:
			error_message = "* You don't have enough EXP!"
		else:
			error_message = "* You don't have enough Gold!"
		text_box.text = error_message
		text_box.visible_ratio = 1.0
		await get_tree().create_timer(2.0).timeout
		text_box.clear_text()
		text_box.visible = false

func _on_exit_button_pressed() -> void:
	Fade.fade_into_black()
	await Fade.fade_into_black()
	# Return to last scene (should be overworld_original where shop is located)
	if Global.last_scene_path != "":
		get_tree().change_scene_to_file(Global.last_scene_path)
	else:
		# Fallback to overworld_original if no last scene is saved
		get_tree().change_scene_to_file("res://maps/overworld_original.tscn")
