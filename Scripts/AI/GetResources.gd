
class_name GetResources extends ConditionLeaf

func tick(actor:Node, _blackboard:Blackboard) -> int:
	
	var _e = actor.senses.getResource()
	if _e == null:
		return FAILURE
	else:
		actor.resource = _e
		return SUCCESS
