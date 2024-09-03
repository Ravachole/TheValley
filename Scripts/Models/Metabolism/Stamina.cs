namespace TheValley.Scripts.Models.Metabolism
{
    public class Stamina : Metabolism 
    {
        public Stamina() : base(0.3f, 0.0f) {}
        
        public override void Update(double delta)
        {
            // Apply thirst-specific update logic here
            DrainOverTime(delta);
        }
    }
}