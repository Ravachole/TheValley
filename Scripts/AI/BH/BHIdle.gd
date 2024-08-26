class_name Idle extends ConditionLeaf

func tick(actor:Node, _blackboard:Blackboard) -> int:
	var _v : Vector3 =  Vector3(randf_range(-10, 10),0,randf_range(-10,10))
	actor.Move(_v)
	actor.MoveState(25, 6)
	return SUCCESS
