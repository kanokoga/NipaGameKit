using System.Collections.Generic;
using UnityEngine;

namespace NipaGameKit.Statuses
{
    public class StatusUpdater
    {
        private Dictionary<string, List<IModifier>> modifiersByStatusType = new Dictionary<string, List<IModifier>>();

        public void AddModifier(string statusType, IModifier modifier)
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
                var addMulValue = 1f;
                var mulValue = 1f;

                foreach(var modifier in modifiers)
                {
                    var weight = modifier.Evaluate(context);
                    if(CEE.IsValidWeight(weight))
                    {
                        var v = modifier.Value * weight;
                        switch(modifier.Type)
                        {
                            case ModifierType.Additive:
                                addValue += v;
                                break;
                            case ModifierType.AddictiveMultiplication:
                                addMulValue += v;
                                break;
                            case ModifierType.Multiplicative:
                                mulValue *= v + 1f;
                                break;
                        }

                        status.AddModifyInfo(modifier.GetModifyInfo());
                    }
                }

                var newValue = status.BaseValue;
                newValue = (newValue + addValue) * addMulValue * mulValue;
                status.SetValue(newValue);
            }
        }
    }
}
