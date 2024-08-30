using System;
using System.Collections.Generic;
using System.Linq;
using TheValley.Scripts.Models;

namespace TheValley.Scripts.AI.Behavior
{
    
    public interface IBehaviorNode
    {
        bool Execute(Creature creature);
    }

    // Selector class implementing IBehaviorNode
    public class Selector : IBehaviorNode
    {
        private readonly List<IBehaviorNode> _children;

        public Selector(List<IBehaviorNode> children)
        {
            _children = children;
        }

        public bool Execute(Creature creature)
        {
           return _children.Exists(child => child.Execute(creature));
        }
    }

    // Sequence class implementing IBehaviorNode
    public class Sequence : IBehaviorNode
    {
        private readonly List<IBehaviorNode> _children;

        public Sequence(List<IBehaviorNode> children)
        {
            _children = children;
        }

        public bool Execute(Creature creature)
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

    // ConditionNode class implementing IBehaviorNode
    public class ConditionNode : IBehaviorNode
    {
        private readonly Func<Creature, bool> _condition;
        private readonly bool _expectedResult;

        public ConditionNode(Func<Creature, bool> condition, bool expectedResult = true)
        {
            _condition = condition;
            _expectedResult = expectedResult;
        }

        public bool Execute(Creature creature)
        {
            return _condition(creature) == _expectedResult;
        }
    }

    // ActionNode class implementing IBehaviorNode
    public class ActionNode : IBehaviorNode
    {
        private readonly Action<Creature> _action;

        public ActionNode(Action<Creature> action)
        {
            _action = action;
        }

        public bool Execute(Creature creature)
        {
            _action(creature);
            return true; // Assume action is always successful
        }
    }
}
