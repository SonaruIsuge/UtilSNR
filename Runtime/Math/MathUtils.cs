using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UtilSNR.Math
{
    public static class MathUtils
    {
        public static float ClampedRemap(float input_min, float input_max, float output_min, float output_max, float value)
        {
            if (value < input_min)
            {
                return output_min;
            }
            else if (value > input_max)
            {
                return output_max;
            }
            else
            {
                return (value - input_min) / (input_max - input_min) * (output_max - output_min) + output_min;
            }
        }

        public static void Shuffle<T>(IList<T> list)
        {
            System.Random random = new();
            var n = list.Count;

            while (n > 1)
            {
                n--;
                var k = random.Next(n + 1);
               (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}
