extends ActionLeaf

func tick(actor, _blackboard):
	var tween =  get_tree().create_tween()
	var lake = get_node("AreaLake")
	tween.tween_property(actor, "position", lake.position, 5)
	tween.play()
	return SUCCESS
	pass
