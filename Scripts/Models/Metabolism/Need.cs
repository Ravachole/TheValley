using Godot;
using System;

/**
** This Class is used to Prioritize the needs of any creature. Depending of their status (hungry / thirsty...)
** But also depending of their distance of the differents resources. The point being that they can 
** Strategize about managing their resources, and introduce, kind of, the notion of "choice" for the AI.
**/
namespace TheValley.Scripts.Models
{
    public class Need
    {
        public string Name { get; set; }
        public float Priority { get; set; }
        public float Value { get; set; }
        public float DistanceToResource { get; set; }
        public float MaxDistance { get; set; }

        public Need(string name, float initialValue = 50f)
        {
            Name = name;
            Value = initialValue;
            Priority = initialValue;
        }
        /**
        ** This Algorithm is far from perfect, to refine with further complexity (field, obstacles...)
        **/
        public int CalculatePriority()
        {
            // Base priority is directly tied to the need's value
            float basePriority = Value;

            // Calculate distance factor (higher priority if the resource is closer)
            float distanceFactor = (MaxDistance - DistanceToResource) / MaxDistance;

            // Final priority is adjusted by the distance factor
            return (int)(basePriority * distanceFactor);
        }
        // Example method to increase need value over time
        public void IncreaseNeed(float amount)
        {
            Value = Math.Min(100f, Value + amount); // Ensure the value doesn't exceed 100
            CalculatePriority();
        }
    }
}