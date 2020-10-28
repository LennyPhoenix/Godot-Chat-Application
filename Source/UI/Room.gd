extends Control
signal leave_lobby
signal send_message(text)


onready var chat_box: = $ChatBox
onready var member_list: = $MemberList


func _on_DiscordManager_ReceivedMessage(
	username: String, message: String
) -> void:
	chat_box.add_message(username, message)


func _on_DiscordManager_UpdateMemberListText(list) -> void:
	member_list.bbcode_text = list


func _on_Leave_button_up() -> void:
	emit_signal("leave_lobby")


func _on_ChatBox_send_message(text: String) -> void:
	emit_signal("send_message", text)
