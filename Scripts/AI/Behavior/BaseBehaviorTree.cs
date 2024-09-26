using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using TheValley.Scripts.Models;
using TheValley.Scripts.Models.Metabolism;
using TheValley.Scripts.Models.Item;
using TheValley.Scripts.Models.Item.Consumable;

namespace TheValley.Scripts.AI.Behavior
{
    public abstract class BaseBehaviorTree
    {
        protected IBehaviorNode _root;
        private readonly Random _random = new Random();
        private float _wanderTimer = 0.0f;
        private const float _wanderInterval = 3.0f;

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
            _root.Execute(creature);
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

                // Generate a random direction on the XZ plane only
                Vector3 randomDirection = new Vector3(
                    (float)(_random.NextDouble() * 2 - 1), // X direction between -1 and 1
                    0, // Y direction is 0 to keep the creature on the ground
                    (float)(_random.NextDouble() * 2 - 1)  // Z direction between -1 and 1
                ).Normalized();

                // Set the velocity based on the random direction and creature's speed
                creature.Velocity = -(randomDirection * creature.Speed);

                // Calculate the target direction and update rotation on the XZ plane
                Vector3 targetDirection = randomDirection.Normalized();
                
                // Get the current basis of the creature's transform
                var currentTransform = creature.GlobalTransform;

                // Compute the new rotation basis using LookAt to generate the correct forward direction
                Vector3 lookAtTarget = currentTransform.Origin + targetDirection;
                lookAtTarget.Y = currentTransform.Origin.Y; // Ensure the creature stays level

                // Update the transform with the new rotation
                creature.LookAt(lookAtTarget, Vector3.Up);
            }
            else
            {
                _wanderTimer -= creature.Delta;
            }
        }

        /*
        ** For the moment, this method only get the closest node available. More complexity will come about it with the priority system.
        ** Or probably in another method. Is this the place for it ? idk
        */
        public Node3D FindClosestNodeInGroup(List<Node3D> nodes, string groupName, Creature creature)
        {
            Node3D closestNode = null;
            float closestDistance = float.MaxValue;

            if (nodes is null) {
                return null;
            }
            
            foreach (Node3D node in nodes)
            {
                if (node.IsInGroup(groupName))
                {
                    float distance = node.GlobalPosition.DistanceTo(creature.GlobalPosition);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNode = node;
                    }
                }
            }
            return closestNode;
        }

        /*
        ** For now it's quite similar to find the closest element in node3D list, but it will differ too with priority and 
        ** time handling
        */
        public Node3D RememberClosestElementInGroup(List<MemoryEntry> nodes, string groupName, Creature creature)
        {
            Node3D closestNode = null;
            float closestDistance = float.MaxValue;

            if (nodes is null) {
                return null;
            }
            
            foreach (MemoryEntry node in nodes)
            {
                if (node.item.IsInGroup(groupName))
                {
                    float distance = node.item.GlobalPosition.DistanceTo(creature.GlobalPosition);
                    if (distance < closestDistance)
                    {

                        closestDistance = distance;
                        closestNode = node.item as Node3D;
                    }
                }
            }
            return closestNode;
        }

        public void MoveToTarget<T>(Creature creature, T target) where T : GeneralItem
        {
            if (target != null)
            {
                creature.NavigationAgent.TargetPosition = target.GlobalTransform.Origin;
                Vector3 nextPosition = creature.NavigationAgent.GetNextPathPosition();
                Vector3 direction = (nextPosition - creature.GlobalPosition).Normalized();
                creature.Velocity = direction * creature.Speed;
                GD.Print($"{creature.Name} Start moving to {typeof(T).Name} at {creature.Velocity}");
            }
        }

        public void ConsumeResource(Creature creature, GeneralItem resource, 
        Func<Creature, bool> hasConsumedEnough,
        Action<Creature> stopDrain,
        Action<Creature> restoreDrain,
        Action<Creature> incrementResource,
        Action<Creature> decrementResourceValue, 
        CreatureStatus status)
        {
            if (creature.NavigationAgent.IsTargetReached())
            {
                GD.Print($"{creature.Name} starts consuming {resource.Name}");

                // Set the creature to the appropriate state (e.g., Eating, Drinking)
                creature.SetState(CreatureState.Eating);  // or Drinking, depending on the resource

                // Disable the drain while consuming
                stopDrain(creature);

                // Simulate consuming the resource
                while (!hasConsumedEnough(creature))
                {
                    incrementResource(creature);  // Increment the hunger/thirst
                    decrementResourceValue(creature);  // Reduce the resource's value
                    resource.Update();  // Update the resource state (FULL, CONSUMED, DEPLETED)
                }

                // Remove the status once enough has been consumed
                creature.CreatureStatuses.Remove(status);

                // Set the creature back to its normal behavior (e.g., Wandering)
                creature.SetState(CreatureState.Wandering);
                GD.Print($"{creature.Name} has consumed enough {resource.Name} and will resume normal behavior.");

                // Restore the normal drain rate for the resource
                restoreDrain(creature);
            }
        }


        public bool CheckAndSetStatus(Creature creature, Func<Creature, bool> isBelowThreshold, CreatureStatus status)
        {
            bool isConditionMet = isBelowThreshold(creature);
            if (isConditionMet && !creature.CreatureStatuses.Contains(status))
            {
                creature.CreatureStatuses.Add(status);
            }
            return isConditionMet;
        }

        public T FindClosestResource<T>(Creature creature, string resourceGroup) where T : GeneralItem
        {
            var resource = FindClosestNodeInGroup(creature.Vision.GetVisibleObjects(), resourceGroup, creature) as T;
            if (resource != null)
                return resource;

            resource = FindClosestNodeInGroup(creature.Smell.GetSmeltItems(), resourceGroup, creature) as T;
            if (resource != null)
                return resource;

            resource = RememberClosestElementInGroup(creature.Memory, resourceGroup, creature) as T;
            return resource;
        }
    }
}
