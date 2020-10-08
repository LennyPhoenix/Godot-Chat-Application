extends Control
#class_name ChatBox
signal send_message(text)

onready var chat_log: = $VBoxContainer/ChatLog
onready var input_label: = $VBoxContainer/HBoxContainer/Label
onready var input_field: = $VBoxContainer/HBoxContainer/InputBox

var user_name: = "Lenny"
var last_user: = ""

func _on_InputBox_text_entered(new_text: String) -> void:
	if new_text != "":
		input_field.text = ""
		add_message(user_name, new_text)
		emit_signal("send_message", new_text)

func _input(event: InputEvent) -> void:
	if event is InputEventKey:
		if event.pressed and event.scancode == KEY_ENTER:
			input_field.grab_focus()
		elif event.pressed and event.scancode == KEY_ESCAPE:
			input_field.release_focus()

func add_message(username: String, text: String) -> void:
	chat_log.bbcode_text += "\n"

	#if username != last_user:
	#	chat_log.bbcode_text += "\n"

	chat_log.bbcode_text += "{username} [color=#aaaaaa]:[/color]  {text}".format({"username": username, "text": text})
	last_user = username

func reset() -> void:
	input_field.text = ""
	chat_log.bbcode_text = ""
