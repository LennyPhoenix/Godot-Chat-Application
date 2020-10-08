extends Control

onready var discord_manager: = $DiscordManager
onready var main_menu: = $MainMenu
onready var chat_room: = $Room


func _on_DiscordManager_JoinedLobby() -> void:
	var box = chat_room.get_node("ChatBox")
	main_menu.visible = false
	box.reset()
	chat_room.visible = true
	chat_room.get_node("ChatBox").add_message("System", "Welcome to the lobby!")


func _on_DiscordManager_LeftLobby() -> void:
	var box = chat_room.get_node("ChatBox")
	chat_room.visible = false
	box.reset()
	main_menu.visible = true
