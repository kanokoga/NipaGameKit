using UnityEngine;

namespace NipaGameKit.Statuses
{
    public class Status
    {
        public string Id { get; private set; }
        public float Value { get; private set; }
        public float BaseValue{ get; private set; }
        public string ModifyInfo{ get; private set; }


        public Status(string id, float baseValue)
        {
            this.Id = id;
            this.BaseValue = baseValue;
            this.Value = baseValue;
        }

        public void Reset()
        {
            this.Value = this.BaseValue;
            this.ModifyInfo = string.Empty;
        }

        public void AddModifyInfo(string info)
        {
            if (string.IsNullOrEmpty(this.ModifyInfo))
            {
                this.ModifyInfo = info;
            }
            else
            {
                this.ModifyInfo += ", " + info;
            }
        }

        public void SetValue(float newValue)
        {
            this.Value = newValue;
        }
    }
}
