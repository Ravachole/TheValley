using System.Runtime.InteropServices.ObjectiveC;
using Godot;
using Godot.NativeInterop;
public abstract class Metabolism
{
    // TODO : Move default values to global stuff. Specific stats come with specific obj.
    private float _minimum = 0.0f;
    private float _maximum = 100.0f;
    private float _current = 100.0f;
    private float _drain = 5.0f;
    private float _currentDrain = 5.0f;
    private float _regeneration = 0.0f;
    private float _threshold = 20.0f;

    // Properties to get and set these values

    public float CurrentDrain
    {
        get => _currentDrain;
        set => _currentDrain = value;
    }
    public float Minimum
    {
        get => _minimum;
        set => _minimum = value;
    }

    public float Maximum
    {
        get => _maximum;
        set => _maximum = value;
    }

    public float Current
    {
        get => _current;
        set => _current = Mathf.Clamp(value, _minimum, _maximum);
    }

    public float Drain
    {
        get => _drain;
        set => _drain = value;
    }

    public float Regeneration
    {
        get => _regeneration;
        set => _regeneration = value;
    }

    public float Threshold
    {
        get => _threshold;
        set => _threshold = value;
    }

    // Abstract method to be implemented by subclasses
    public abstract void Update(double delta);

    // Common logic for draining the resource over time
    protected void DrainOverTime(double delta)
    {
        Current -= _currentDrain * (float)delta;
        Current = Mathf.Clamp(Current, _minimum, _maximum); // Ensure it stays within bounds
    }

    // Common logic for regenerating the resource
    protected void RegenerateOverTime(double delta)
    {
        Current += _regeneration * (float)delta;
        Current = Mathf.Clamp(Current, _minimum, _maximum); // Ensure it stays within bounds
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

public class Hunger : Metabolism {

    public Hunger() {
        Drain = 0.5f;
        CurrentDrain = Drain;
    }
    public override void Update(double delta)
    {
        // Apply hunger-specific update logic here
        DrainOverTime(delta);
        GD.Print("Hunger updated : " + Current);
    }
}
public class Thirst : Metabolism {

    public Thirst() {
        Drain = 1.5f;
        CurrentDrain = Drain;
    }
    public override void Update(double delta)
    {
        // Apply thirst-specific update logic here
        DrainOverTime(delta);
        GD.Print("Thirst updated : " + Current);
    }
}
// class Stress : Metabolism {}
// class Health : Metabolism {}
public class Stamina : Metabolism {
    public Stamina() {
        Drain = 0.3f;
        CurrentDrain = Drain;
    }
    
       public override void Update(double delta)
    {
        // Apply thirst-specific update logic here
        DrainOverTime(delta);
        GD.Print("Stamina updated : " + Current);
    }
}
// class Growth : Metabolism {}

