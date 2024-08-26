
class_name GetAreas extends ConditionLeaf

func tick(actor:Node, _blackboard:Blackboard) -> int:
	
	var _e = actor.senses.getAreas()
	if _e == null:
		return FAILURE
	else:
		actor.resource = _e
		return SUCCESS
