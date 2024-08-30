using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public abstract class BaseBehaviorTree<TCreature> where TCreature : Creature
{
    protected BehaviorNode<TCreature> _root;
    private Random _random = new Random();
    private float _wanderTimer = 0.0f;
    private const float _wanderInterval = 2.0f;

    public BaseBehaviorTree()
        {
            _root = new Selector<TCreature>(new List<BehaviorNode<TCreature>>
            {
                CreateHungerSequence(),
                CreateThirstSequence(),
                CreateTiredSequence(),
                CreateIdleSequence(),
                CreateWanderSequence()
            });
        }

    protected abstract BehaviorNode<TCreature> CreateHungerSequence();
    protected abstract BehaviorNode<TCreature> CreateThirstSequence();
    protected abstract BehaviorNode<TCreature> CreateTiredSequence();
    protected abstract BehaviorNode<TCreature> CreateIdleSequence();
    protected abstract BehaviorNode<TCreature> CreateWanderSequence();

    // Update method now takes a TCreature parameter
    public void Update(TCreature creature)
    {
        bool result = _root.Execute(creature); // Passes the specific type of creature
        GD.Print($"Behavior Tree Update Result: {result}");
    }

    protected void Wander(TCreature creature)
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
    protected bool IsNearTarget(TCreature creature, string targetAreaName)
    {
        var targetArea = creature.GetNode<Area3D>(targetAreaName);
        var target = targetArea.GetOverlappingBodies().FirstOrDefault();

        if (target != null)
        {
            float distance = creature.GlobalTransform.Origin.DistanceTo(target.GlobalTransform.Origin);
            // TODO : Make Distance custom. For now it's default value.
            return distance < 2.0f; 
        }
        return false;
    }
}
