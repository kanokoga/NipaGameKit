using System;
using UnityEngine;

namespace NipaGameKit.Statuses
{
    [Serializable]
    public class Status
    {
        public string Type { get => type; private set => type = value; }
        public int ID { get => id; set => id = value; }
        public float BaseValue { get => baseValue; private set => baseValue = value; }
        public float Value { get => value; private set => this.value = value; }
        public string ModifyInfo { get => modifyInfo; private set => modifyInfo = value; }

        [SerializeField] private string type;
        [SerializeField] private int id;
        [SerializeField] private float baseValue;
        [SerializeField,Tooltip("debug")] private float value;
        [SerializeField,Tooltip("debug")] private string modifyInfo;


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

        public void SetType(string newType)
        {
            this.Type = newType;
        }
    }
}
