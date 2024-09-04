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
        public float Regeneration {get;set;}
        public ItemState State {get;set;}

        protected Item(string _name, float _maximum = 100.0f, float _regeneration = 0.0f, ItemState _state = ItemState.FULL)
        {
            Name = _name;
        }
        public virtual void ChangeState(ItemState itemState)
        {
            State = itemState;
            GD.Print(nameof(Item) + " changes status for : " + itemState);
        }
        public virtual void Update()
        {
            GD.Print(nameof(Item) + " current value is :  " + Value);
            if (Value == Maximum)
                ChangeState(ItemState.FULL);
            else if (Value <= 0.0f)
                ChangeState(ItemState.DEPLETED);
            else
                ChangeState(ItemState.CONSUMED);
        }
    }
}