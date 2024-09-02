using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using TheValley.Scripts.Models;

namespace TheValley.Scripts.AI.Behavior
{
    public abstract class BaseBehaviorTree
    {
        protected IBehaviorNode _root;
        private readonly Random _random = new Random();
        private float _wanderTimer = 0.0f;
        private const float _wanderInterval = 2.0f;

        protected BaseBehaviorTree()
        {
        }

        protected abstract IBehaviorNode CreateHungerSequence();
        protected abstract IBehaviorNode CreateThirstSequence();
        protected abstract IBehaviorNode CreateTiredSequence();
        protected abstract IBehaviorNode CreateIdleSequence();
        protected abstract IBehaviorNode CreateWanderSequence();

    public void Initialize()
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
        public void Update(Creature creature)
        {
            bool result = _root.Execute(creature);
        }

        protected void Wander(Creature creature)
        {
            if (creature.CurrentState != CreatureState.Wandering)
            {
                return;
            }
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
            }
            else
            {
                _wanderTimer -= creature.Delta;
            }
        }

        protected bool IsNearTarget(Creature creature, string targetAreaName)
        {
            var targetArea = creature.GetNode<Area3D>(targetAreaName);
            var target = targetArea.GetOverlappingBodies().FirstOrDefault();

            if (target != null)
            {
                float distance = creature.GlobalTransform.Origin.DistanceTo(target.GlobalTransform.Origin);
                return distance < 2.0f; 
            }
            return false;
        }
    }
}
