extends Control
signal create_lobby


func _on_Create_button_up() -> void:
	emit_signal("create_lobby")
