
class_name SetTargetWater extends ActionLeaf

func tick(actor:Node, _blackboard:Blackboard) -> int:
	actor.set_target(actor.resource.global_position)
	return SUCCESS