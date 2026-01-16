extends Node2D

# Shop items configuration: [Item resource, EXP cost, Gold cost]
var shop_items: Array[Dictionary] = [
	{"item": preload("res://items/apple.tres"), "exp_cost": 10, "gold_cost": 5},
	{"item": preload("res://items/nice_cream.tres"), "exp_cost": 15, "gold_cost": 8},
	{"item": preload("res://items/pie.tres"), "exp_cost": 20, "gold_cost": 10}
]

# Menu navigation variables
var disable_menu: bool = false
var choice: int = 1
var choice_rtn: int = 0
var choice_size: int = 2  # Buy, Exit
var choice_extend: String = ""  # Current menu path
var choice_extend_copy: String = ""

# Node references
@onready var shopkeeper: Sprite2D = %Shopkeeper
@onready var shopkeeper_anim: AnimationPlayer = %Shopkeeper.get_node("AnimationPlayer")
@onready var buy_panel: Panel = %BuyPanel
@onready var inter_panel_text: Label = %InterPanelText
@onready var buy_panel_text: Label = %BuyPanelText
@onready var info_panel_text: Label = %InfoPanelText
@onready var exp_label: Label = %ExpLabel
@onready var gold_label: Label = %GoldLabel
@onready var buy_options: VBoxContainer = %buyOptions
@onready var confirm_prompt: Label = %ConfirmPrompt
@onready var option_1_cursor: TextureRect = get_node("BuyPanel/Option 1/Cursor")
@onready var option_2_cursor: TextureRect = get_node("BuyPanel/Option 2/Cursor")

var current_choice_cursor: TextureRect = null
var current_buy_choice: int = 0
var current_item_data: Dictionary = {}

func _ready() -> void:
	# Initialize shopkeeper animation
	shopkeeper_anim.play("Idle")
	
	# Set initial cursor
	current_choice_cursor = option_1_cursor
	current_choice_cursor.visible = true
	option_2_cursor.visible = false
	
	# Initialize displays
	update_exp_display()
	update_gold_display()
	
	# Show greeting
	show_greeting()
	
	# Wait for fade
	await get_tree().process_frame
	Fade.fade_from_black()

func update_exp_display() -> void:
	if exp_label:
		exp_label.text = "EXP: %d" % Global.player_exp

func update_gold_display() -> void:
	if gold_label:
		gold_label.text = "Gold: %d" % Global.player_gold

func show_greeting() -> void:
	inter_panel_text.text = "* Welcome to the shop!"
	buy_panel_text.text = ""
	buy_panel_text.visible = false

func _input(ev: InputEvent) -> void:
	if disable_menu:
		return
	
	if ev is InputEventKey and ev.pressed:
		if ev.keycode == KEY_UP:
			if choice_extend == "":
				update_choice("up", choice_size)
			elif choice_extend == "buyOptions/":
				update_buy_choice("up")
			elif choice_extend == "ConfirmPrompt/":
				update_confirm_choice("up")
		elif ev.keycode == KEY_DOWN:
			if choice_extend == "":
				update_choice("down", choice_size)
			elif choice_extend == "buyOptions/":
				update_buy_choice("down")
			elif choice_extend == "ConfirmPrompt/":
				update_confirm_choice("down")
		elif ev.keycode == KEY_ENTER or ev.keycode == KEY_KP_ENTER:
			handle_select()
		elif ev.keycode == KEY_C or ev.keycode == KEY_X or ev.keycode == KEY_BACKSPACE:
			handle_back()

func update_choice(direction: String, max_options: int) -> void:
	if current_choice_cursor:
		current_choice_cursor.visible = false
	
	if direction == "up":
		if choice == 1:
			choice = max_options
		else:
			choice -= 1
	elif direction == "down":
		if choice == max_options:
			choice = 1
		else:
			choice += 1
	
	# Update cursor
	if choice == 1:
		current_choice_cursor = option_1_cursor
		option_2_cursor.visible = false
	else:
		current_choice_cursor = option_2_cursor
		option_1_cursor.visible = false
	
	if current_choice_cursor:
		current_choice_cursor.visible = true

func update_buy_choice(direction: String) -> void:
	var options = buy_options.get_children()
	if options.size() == 0:
		return
	
	# Hide current cursor
	if current_buy_choice >= 0 and current_buy_choice < options.size():
		var current_option = buy_options.get_child(current_buy_choice)
		var current_cursor = current_option.get_node_or_null("Cursor")
		if current_cursor:
			current_cursor.visible = false
	
	if direction == "up":
		if current_buy_choice == 0:
			current_buy_choice = options.size() - 1
		else:
			current_buy_choice -= 1
	elif direction == "down":
		if current_buy_choice == options.size() - 1:
			current_buy_choice = 0
		else:
			current_buy_choice += 1
	else:
		# Initial call - just show cursor
		if current_buy_choice >= options.size():
			current_buy_choice = 0
	
	# Show new cursor
	if current_buy_choice >= 0 and current_buy_choice < options.size():
		var new_option = buy_options.get_child(current_buy_choice)
		var new_cursor = new_option.get_node_or_null("Cursor")
		if new_cursor:
			new_cursor.visible = true
		
		# Update info panel
		update_info_panel()

func handle_select() -> void:
	if choice_extend == "":
		# Main menu
		if choice == 1:  # Buy
			enter_buy_menu()
		elif choice == 2:  # Exit
			exit_shop()
	elif choice_extend == "buyOptions/":
		# Buy menu - show confirm prompt
		show_confirm_prompt()
	elif choice_extend == "ConfirmPrompt/":
		# Confirm prompt
		handle_confirm()

func handle_back() -> void:
	if choice_extend == "buyOptions/":
		# Exit buy menu
		exit_buy_menu()
	elif choice_extend == "ConfirmPrompt/":
		# Exit confirm prompt
		exit_confirm_prompt()

func enter_buy_menu() -> void:
	choice_extend = "buyOptions/"
	choice_extend_copy = "buyOptions/"
	
	# Hide main menu options
	get_node("BuyPanel/Option 1").visible = false
	get_node("BuyPanel/Option 2").visible = false
	
	# Hide InterPanelText to prevent overlap with buy options
	inter_panel_text.visible = false
	
	# Show "What would you like?" text when viewing products
	buy_panel_text.visible = true
	buy_panel_text.text = "What would\nyou like?"
	
	# Show buy options
	buy_options.visible = true
	populate_buy_options()
	
	# Show info panel
	var info_panel = get_node("BuyPanel/InfoPanel")
	if info_panel:
		info_panel.visible = true
	
	# Set first item as selected
	current_buy_choice = 0
	update_buy_choice("")

func exit_buy_menu() -> void:
	choice_extend = ""
	choice_extend_copy = ""
	
	# Hide buy options
	buy_options.visible = false
	var info_panel = get_node("BuyPanel/InfoPanel")
	if info_panel:
		info_panel.visible = false
	
	# Show main menu options
	get_node("BuyPanel/Option 1").visible = true
	get_node("BuyPanel/Option 2").visible = true
	
	# Show InterPanelText again
	inter_panel_text.visible = true
	
	# Hide "What would you like?" when back at main menu
	buy_panel_text.visible = false
	
	# Reset choice
	choice = 1
	update_choice("", choice_size)
	
	# Update text
	inter_panel_text.text = "* Go on"

func populate_buy_options() -> void:
	# Clear existing options
	for child in buy_options.get_children():
		child.queue_free()
	
	# Create menu entries for each shop item
	for i in range(shop_items.size()):
		var shop_item = shop_items[i]
		var item: Item = shop_item.item
		var exp_cost: int = shop_item.exp_cost
		var gold_cost: int = shop_item.gold_cost
		
		# Create option label
		var option_label = Label.new()
		option_label.name = "Option %d" % (i + 1)
		option_label.text = "%d EXP, %d Gold - %s" % [exp_cost, gold_cost, item.item_name]
		option_label.custom_minimum_size = Vector2(0, 60)
		option_label.add_theme_font_size_override("font_size", 27)
		option_label.vertical_alignment = VERTICAL_ALIGNMENT_CENTER
		
		# Create cursor
		var cursor = TextureRect.new()
		cursor.name = "Cursor"
		cursor.texture = preload("res://sprites/red_soul.svg.png")
		cursor.scale = Vector2(0.18868, 0.18868)
		cursor.expand = true
		cursor.visible = (i == 0)  # Only first one visible
		cursor.offset_left = -50.0
		cursor.offset_top = 20.0
		cursor.offset_right = 144.0
		cursor.offset_bottom = 214.0
		
		option_label.add_child(cursor)
		buy_options.add_child(option_label)

func update_info_panel() -> void:
	if current_buy_choice >= 0 and current_buy_choice < shop_items.size():
		# Clear info panel - all info is already in the buy options list
		info_panel_text.text = ""

func show_confirm_prompt() -> void:
	if current_buy_choice >= shop_items.size():
		return
	
	var shop_item = shop_items[current_buy_choice]
	var item: Item = shop_item.item
	var exp_cost: int = shop_item.exp_cost
	var gold_cost: int = shop_item.gold_cost
	
	current_item_data = shop_item
	choice_rtn = current_buy_choice + 1
	
	# Hide "What would you like?" text
	buy_panel_text.visible = false
	
	# Update confirm prompt text
	confirm_prompt.text = "Buy it for\n%d EXP, %d Gold?" % [exp_cost, gold_cost]
	confirm_prompt.visible = true
	
	# Hide buy options
	buy_options.visible = false
	
	# Set choice to confirm menu
	choice_extend = "ConfirmPrompt/"
	choice = 1
	
	# Show cursor on Yes
	var yes_cursor = get_node("BuyPanel/ConfirmPrompt/Option 1/Cursor")
	var no_cursor = get_node("BuyPanel/ConfirmPrompt/Option 2/Cursor")
	yes_cursor.visible = true
	no_cursor.visible = false

func update_confirm_choice(direction: String) -> void:
	var yes_cursor = get_node("BuyPanel/ConfirmPrompt/Option 1/Cursor")
	var no_cursor = get_node("BuyPanel/ConfirmPrompt/Option 2/Cursor")
	
	if direction == "up":
		if choice == 1:
			choice = 2
		else:
			choice = 1
	elif direction == "down":
		if choice == 2:
			choice = 1
		else:
			choice = 2
	
	if choice == 1:
		yes_cursor.visible = true
		no_cursor.visible = false
	else:
		yes_cursor.visible = false
		no_cursor.visible = true

func exit_confirm_prompt() -> void:
	confirm_prompt.visible = false
	buy_options.visible = true
	# Show "What would you like?" when returning to buy menu
	buy_panel_text.visible = true
	buy_panel_text.text = "What would\nyou like?"
	choice_extend = "buyOptions/"
	current_buy_choice = choice_rtn - 1
	choice_rtn = 0
	update_buy_choice("")

func handle_confirm() -> void:
	if choice == 1:  # Yes
		# Attempt purchase
		var item: Item = current_item_data.item
		var exp_cost: int = current_item_data.exp_cost
		var gold_cost: int = current_item_data.gold_cost
		
		var has_enough_exp = Global.player_exp >= exp_cost
		var has_enough_gold = Global.player_gold >= gold_cost
		
		if has_enough_exp and has_enough_gold:
			# Purchase successful
			Global.spend_exp(exp_cost)
			Global.spend_gold(gold_cost)
			Global.add_item_to_inventory(item)
			update_exp_display()
			update_gold_display()
			
			# Play shopkeeper animation
			shopkeeper_anim.play("Speaking")
			await get_tree().create_timer(0.4).timeout
			shopkeeper_anim.play("Idle")
			
			# Show success message - hide buy options temporarily to show message
			buy_options.visible = false
			inter_panel_text.visible = true
			inter_panel_text.text = "* You bought %s!" % item.item_name
			buy_panel_text.text = "Come\nagain!"
		else:
			# Not enough currency - hide buy options temporarily to show error
			shopkeeper_anim.play("Speaking")
			await get_tree().create_timer(0.4).timeout
			shopkeeper_anim.play("Idle")
			buy_options.visible = false
			inter_panel_text.visible = true
			if not has_enough_exp and not has_enough_gold:
				inter_panel_text.text = "* You don't have enough EXP and Gold!"
			elif not has_enough_exp:
				inter_panel_text.text = "* You don't have enough EXP!"
			else:
				inter_panel_text.text = "* You don't have enough Gold!"
			buy_panel_text.text = "That's not\nenough\nmoney."
	
	# Return to buy menu after showing message
	await get_tree().create_timer(1.5).timeout
	inter_panel_text.visible = false
	buy_options.visible = true
	exit_confirm_prompt()

func exit_shop() -> void:
	Fade.fade_into_black()
	await Fade.fade_into_black()
	if is_inside_tree() and get_tree():
		if Global.last_scene_path != "":
			get_tree().change_scene_to_file(Global.last_scene_path)
		else:
			get_tree().change_scene_to_file("res://maps/overworld_original.tscn")
