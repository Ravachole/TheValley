using System;
using System.Collections.Generic;

public abstract class BehaviorNode<TCreature> where TCreature : Creature
{
    public abstract bool Execute(TCreature creature);
}

public class Selector<TCreature> : BehaviorNode<TCreature> where TCreature : Creature
{
    private List<BehaviorNode<TCreature>> _children;

    public Selector(List<BehaviorNode<TCreature>> children)
    {
        _children = children;
    }

    public override bool Execute(TCreature creature)
    {
        foreach (var child in _children)
        {
            if (child.Execute(creature))
            {
                return true;
            }
        }
        return false;
    }
}

public class Sequence<TCreature> : BehaviorNode<TCreature> where TCreature : Creature
{
    private List<BehaviorNode<TCreature>> _children;

    public Sequence(List<BehaviorNode<TCreature>> children)
    {
        _children = children;
    }

    public override bool Execute(TCreature creature)
    {
        foreach (var child in _children)
        {
            if (!child.Execute(creature))
            {
                return false;
            }
        }
        return true;
    }
}

public class ConditionNode<TCreature> : BehaviorNode<TCreature> where TCreature : Creature
{
    private readonly Func<TCreature, bool> _condition;
    private readonly bool _expectedResult;

    public ConditionNode(Func<TCreature, bool> condition, bool expectedResult = true)
    {
        _condition = condition;
        _expectedResult = expectedResult;
    }

    public override bool Execute(TCreature creature)
    {
        return _condition(creature) == _expectedResult;
    }
}

public class ActionNode<TCreature> : BehaviorNode<TCreature> where TCreature : Creature
{
    private Action<TCreature> _action;

    public ActionNode(Action<TCreature> action)
    {
        _action = action;
    }

    public override bool Execute(TCreature creature)
    {
        _action(creature);
        return true; // Assume action is always successful
    }
}
