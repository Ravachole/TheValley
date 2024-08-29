using System;
using System.Collections.Generic;

public abstract class BehaviorNode
{
    public abstract bool Execute(Herbivore herbivore);
}

public class Selector : BehaviorNode
{
    private List<BehaviorNode> _children;

    public Selector(List<BehaviorNode> children)
    {
        _children = children;
    }

    public override bool Execute(Herbivore herbivore)
    {
        foreach (var child in _children)
        {
            if (child.Execute(herbivore))
            {
                return true;
            }
        }
        return false;
    }
}

public class Sequence : BehaviorNode
{
    private List<BehaviorNode> _children;

    public Sequence(List<BehaviorNode> children)
    {
        _children = children;
    }

    public override bool Execute(Herbivore herbivore)
    {
        foreach (var child in _children)
        {
            if (!child.Execute(herbivore))
            {
                return false;
            }
        }
        return true;
    }
}

public class ConditionNode : BehaviorNode
{
    private readonly Func<Herbivore, bool> _condition;
    private readonly bool _expectedResult;

    public ConditionNode(Func<Herbivore, bool> condition, bool expectedResult = true)
    {
        _condition = condition;
        _expectedResult = expectedResult;
    }

    public override bool Execute(Herbivore herbivore)
    {
        return _condition(herbivore) == _expectedResult;
    }
}

public class ActionNode : BehaviorNode
{
    private Action<Herbivore> _action;

    public ActionNode(Action<Herbivore> action)
    {
        _action = action;
    }

    public override bool Execute(Herbivore herbivore)
    {
        _action(herbivore);
        return true; // Suppose action is always successful
    }
}
