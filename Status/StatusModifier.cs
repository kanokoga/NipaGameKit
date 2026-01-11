using System.Collections.Generic;
using UnityEngine;

namespace NipaGameKit.Statuses
{
    public class StatusModifier : MonoBehaviour
    {
        private Dictionary<string, List<IModifier>> modifiersByStatusId = new Dictionary<string, List<IModifier>>();


        public void Add(string statusId, IModifier modifier)
        {
            if(!this.modifiersByStatusId.ContainsKey(statusId))
            {
                this.modifiersByStatusId[statusId] = new List<IModifier>();
            }

            this.modifiersByStatusId[statusId].Add(modifier);
        }

        private void UpdateStatus(Status status, Context context)
        {
            if(this.modifiersByStatusId.TryGetValue(status.Id, out var modifiers))
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
