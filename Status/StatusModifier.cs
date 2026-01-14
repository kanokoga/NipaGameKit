using System.Collections.Generic;
using UnityEngine;

namespace NipaGameKit.Statuses
{
    public class StatusModifier
    {
        private Dictionary<string, List<IModifier>> modifiersByStatusType = new Dictionary<string, List<IModifier>>();

        public void Add(string statusType, IModifier modifier)
        {
            if(!this.modifiersByStatusType.ContainsKey(statusType))
            {
                this.modifiersByStatusType[statusType] = new List<IModifier>();
            }

            this.modifiersByStatusType[statusType].Add(modifier);
        }

        public void UpdateStatus(Status status, Context context)
        {
            if(this.modifiersByStatusType.TryGetValue(status.Type, out var modifiers))
            {
                status.Reset();
                var addValue = 0f;
                var mulValue = 1f;

                foreach(var modifier in modifiers)
                {
                    if(modifier.IsApplicable(context))
                    {
                        switch(modifier.Type)
                        {
                            case ModifierType.Additive:
                                addValue += modifier.Value;
                                break;
                            case ModifierType.Multiplicative:
                                mulValue += modifier.Value;
                                break;
                        }

                        status.AddModifyInfo(modifier is Modifier<Context> mod ? mod.modifyInfo : "Modifier applied");
                    }
                }

                var newValue = status.BaseValue;
                newValue = (newValue + addValue) * mulValue;
                status.SetValue(newValue);
            }
        }
    }
}
