using System.Runtime.InteropServices.ObjectiveC;
using System.Transactions;
using Godot;
using Godot.NativeInterop;

namespace TheValley.Scripts.Models.Metabolism
{
    public abstract class Metabolism
    {
        // TODO : Move default values to global stuff. Specific stats come with specific obj.
        /**
        ** Privates or public here ? I dont want these to be readable, but is it pertinent in an abstract ? Research
        **/
        public readonly float _minimum = 0.0f;
        public readonly float _maximum = 100.0f;
        private float _current;
        public float Drain { get;  set; }
        public float CurrentDrain{ get; set; }
        public float Regeneration { get;  set; }
        private readonly float _threshold = 20.0f;
        public float Priority {get;set;}
        public float Current
        {
            get => _current;
            set => _current = Mathf.Clamp(value, _minimum, _maximum);
        }

        public float Minimum
        {
            get => _minimum;
        }

        public float Maximum
        {
            get => _maximum;
        }

        public float Threshold
        {
            get => _threshold;
        }

        protected Metabolism(float drain, float regeneration)
        {
            Drain = CurrentDrain = drain;
            Priority = Current = Maximum;
            Regeneration = regeneration; 
        }
        // Abstract method to be implemented by subclasses
        public abstract void Update(double delta);

        // Common logic for draining the resource over time
        protected void DrainOverTime(double delta)
        {
            Current -= CurrentDrain * (float)delta;
            Current = Mathf.Clamp(Current, Minimum, Maximum); // Ensure it stays within bounds
        }

        // Common logic for regenerating the resource
        protected void RegenerateOverTime(double delta)
        {
            Current += Regeneration * (float)delta;
            Current = Mathf.Clamp(Current, Minimum, Maximum); // Ensure it stays within bounds
        }

        // Check if the current value is below the threshold
        public bool IsBelowThreshold()
        {
            return _current <= _threshold;
        }

        // TEST METHOD TO MAKE DYNAMIC
        public bool IsFull()
        {
            return _current >= 50.0f;
        }
    }
}
