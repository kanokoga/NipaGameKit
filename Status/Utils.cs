using UnityEngine;


namespace NipaGameKit.Statuses
{
    public static class StatHelper
    {
        public static string GetPercentage(float percentage)
        {
            var signPrefix = percentage >= 0 ? "+" : "-";
            return $"{signPrefix}{Mathf.Abs(percentage) * 100f:0.#}%";
        }
    }
}
