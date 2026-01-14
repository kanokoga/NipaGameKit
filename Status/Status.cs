using UnityEngine;

namespace NipaGameKit.Statuses
{
    public class Status
    {
        public string Type { get; private set; }
        public int ID { get; set; }
        public float Value { get; private set; }
        public float BaseValue { get; private set; }
        public string ModifyInfo { get; private set; }


        public Status(string type, int id, float baseValue)
        {
            this.Type = type;
            this.ID = id;
            this.BaseValue = baseValue;
            this.Value = baseValue;
            this.ModifyInfo = string.Empty;
        }

        public void Reset()
        {
            this.Value = this.BaseValue;
            this.ModifyInfo = string.Empty;
        }

        public void AddModifyInfo(string info)
        {
            if(string.IsNullOrEmpty(this.ModifyInfo))
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
