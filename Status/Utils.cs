using UnityEngine;


namespace NipaGameKit.Statuses
{
    public static class StatHelper
    {
        public static string GetPercentageWithPrefix(float percentage)
        {
            var signPrefix = percentage >= 0 ? "+" : "-";
            return $"{signPrefix}{Mathf.Abs(percentage) * 100f:0.#}%";
        }

        public static string GetPercentage(float percentage)
        {
            return $"{percentage * 100f:0.#}%";
        }


    }
}
