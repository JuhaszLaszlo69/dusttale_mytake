extends Control

# Shop items configuration: [Item resource, EXP cost]
var shop_items: Array[Dictionary] = [
	{"item": preload("res://items/apple.tres"), "cost": 10},
	{"item": preload("res://items/nice_cream.tres"), "cost": 15},
	{"item": preload("res://items/pie.tres"), "cost": 20}
]

var button := preload("uid://ptt71q0lsxgx")

@onready var exp_label: Label = %ExpLabel
@onready var items_container: VBoxContainer = %ItemsContainer
@onready var text_box: TextBox = %TextBox

var is_visible := false

func _ready() -> void:
	visible = false
	update_exp_display()
	populate_shop()

func show_shop() -> void:
	if is_visible:
		return
	is_visible = true
	visible = true
	update_exp_display()
	%ExitButton.grab_focus()

func hide_shop() -> void:
	if not is_visible:
		return
	is_visible = false
	visible = false
	text_box.clear_text()

func update_exp_display() -> void:
	exp_label.text = "EXP: %d" % Global.player_exp

func populate_shop() -> void:
	# Clear existing items
	for child in items_container.get_children():
		child.queue_free()
	
	# Add shop items
	for shop_item in shop_items:
		var item: Item = shop_item.item
		var cost: int = shop_item.cost
		
		var item_button: Button = button.instantiate()
		var item_text := "%s - %d EXP" % [item.item_name, cost]
		item_button.get_node("text").text = Util.shake(item_text)
		item_button.focus_exited.connect(func():
			item_button.modulate.a = 0.5)
		item_button.pressed.connect(buy_item.bind(item, cost))
		items_container.add_child(item_button)

func buy_item(item: Item, cost: int) -> void:
	if Global.spend_exp(cost):
		Global.add_item_to_inventory(item)
		update_exp_display()
		text_box.scroll("* You bought %s for %d EXP!" % [item.item_name, cost])
		await text_box.finished_scrolling
		text_box.clear_text()
	else:
		text_box.scroll("* You don't have enough EXP!")
		await text_box.finished_scrolling
		text_box.clear_text()

func _on_exit_button_pressed() -> void:
	hide_shop()

