using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public abstract class BaseBehaviorTree
{
    protected IBehaviorNode _root;
    private Random _random = new Random();
    private float _wanderTimer = 0.0f;
    private const float _wanderInterval = 2.0f;

    public BaseBehaviorTree()
    {
        _root = new Selector(new List<IBehaviorNode>
        {
            CreateHungerSequence(),
            CreateThirstSequence(),
            CreateTiredSequence(),
            CreateIdleSequence(),
            CreateWanderSequence()
        });
    }

    protected abstract IBehaviorNode CreateHungerSequence();
    protected abstract IBehaviorNode CreateThirstSequence();
    protected abstract IBehaviorNode CreateTiredSequence();
    protected abstract IBehaviorNode CreateIdleSequence();
    protected abstract IBehaviorNode CreateWanderSequence();

    // Update method now takes a Creature parameter
    public void Update(Creature creature)
    {
        bool result = _root.Execute(creature); // Passes the specific type of creature
        GD.Print($"Behavior Tree Update Result: {result}");
    }

    protected void Wander(Creature creature)
    {
        if (creature.CurrentState != CreatureState.Wandering)
        {
            return;
        }
        GD.Print($"{creature.Name} Start wandering");
        _wanderTimer -= creature.Delta;
        if (_wanderTimer <= 0.0f)
        {
            _wanderTimer = _wanderInterval;

            Vector3 randomDirection = new Vector3(
                (float)(_random.NextDouble() * 2 - 1),
                0,
                (float)(_random.NextDouble() * 2 - 1)
            );

            randomDirection = randomDirection.Normalized() * creature.Speed;
            creature.Velocity = randomDirection;
            GD.Print($"{creature.Name} New Velocity: {creature.Velocity}");
        }
        else
        {
            _wanderTimer -= (float)creature.Delta;
        }
    }

    // Common method for checking proximity
    protected bool IsNearTarget(Creature creature, string targetAreaName)
    {
        var targetArea = creature.GetNode<Area3D>(targetAreaName);
        var target = targetArea.GetOverlappingBodies().FirstOrDefault();

        if (target != null)
        {
            float distance = creature.GlobalTransform.Origin.DistanceTo(target.GlobalTransform.Origin);
            // TODO: Make Distance custom. For now it's default value.
            return distance < 2.0f; 
        }
        return false;
    }
}
