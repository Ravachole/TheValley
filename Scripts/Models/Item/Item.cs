using System;
using Godot;

namespace TheValley.Scripts.Models.Item
{
    public enum ItemState {
        FULL,
        CONSUMED,
        DEPLETED
    }
    public abstract partial class Item : Node3D
    {
        private float _value;
        public float Value 
        {
            get => _value;
            set => _value = Mathf.Clamp(value, 0.0f, Maximum);
        }
        public float Maximum {get;set;}
        public Timer RegenerationTimer {get;set;}
        public float Regeneration {get;set;}
        public ItemState State {get;set;}
        public MeshInstance3D MeshInstance { get; set; }

        protected Item(string _name, float _maximum = 100.0f, float _regeneration = 0.0f, float _regenerationInterval = 10.0f, ItemState _state = ItemState.FULL)
        {
            Name = _name;
            RegenerationTimer = new Timer();
            RegenerationTimer.WaitTime = _regenerationInterval;
            RegenerationTimer.OneShot = false;
            RegenerationTimer.Timeout += OnRegenerate;
            AddChild(RegenerationTimer);
        }
        public virtual void ChangeState(ItemState itemState)
        {
            State = itemState;
            GD.Print(nameof(Item) + " changes status for : " + itemState);
        }
        public virtual void Update()
        {
            var material = MeshInstance.GetActiveMaterial(0).Duplicate() as StandardMaterial3D;
            GD.Print(nameof(Item) + " current value is :  " + Value);
            if (Value == Maximum) {
                ChangeState(ItemState.FULL);
                material.AlbedoColor = new Color(0, 1, 0); // Green for FULL
            }
   
            else if (Value <= 0.0f) {
                ChangeState(ItemState.DEPLETED);
                material.AlbedoColor = new Color(1, 1, 0); // Yellow for CONSUMED
            }
                
            else {
                ChangeState(ItemState.CONSUMED);
                material.AlbedoColor = new Color(1, 0, 0); // Red for DEPLETED
            }      
        }
        private void OnRegenerate()
        {
            if (Value < Maximum)
            {
                Value += Regeneration;
                Update(); // Call the Update method to handle state changes
            }
        }
    }
}