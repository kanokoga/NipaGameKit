using UnityEngine;
using UnityEngine.Serialization;

namespace NipaGameKit
{
    public class CoreComp : CompMonoBase
    {
        public virtual Faction Faction => this.faction;
        public virtual float MaxHitpoint => this.maxHitpoint;
        public virtual float Hitpoint => this.hitpoint;
        public virtual Vector3 Position => this._transform.position;
        public virtual Quaternion Rotation => this._transform.rotation;
        public override int InitOrder { get; } = -100;

        [SerializeField] private Faction faction;
        [SerializeField] private float maxHitpoint;
        [SerializeField] private float hitpoint;


        public override void Init(int monoId)
        {
            base.Init(monoId);
            if(this.hitpoint <= 0)
            {
                this.hitpoint = this.maxHitpoint;
            }
            CompGroup<CoreComp>.Add(this);
        }

        public virtual void AddHitpoint(float add)
            => this.hitpoint = Mathf.Clamp(this.hitpoint + add, 0, this.maxHitpoint);

        public override void Dispose()
        {
            CompGroup<CoreComp>.Remove(this);
        }
    }

    public enum Faction
    {
        Neutral,
        Blue,
        Red,
        Green,
        Yellow,
        Purple
    }
}
