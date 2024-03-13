using System;
using UnityEngine;
using Random = System.Random;

namespace HCGames.Utilities
{
    public static class Math
    {
        public static float GetHorizontalScreenWorldSize(Camera cam, float cameraDistance)
        {
            var halfFOV = cam.fieldOfView * 0.5f;
            var aspectRatio = cam.aspect;
            return Mathf.Tan(Mathf.Deg2Rad * halfFOV) * cameraDistance * 2 * aspectRatio;
        }
        
        public static float GetVerticalScreenWorldSize(Camera cam, float cameraDistance)
        {
            var halfFOV = cam.fieldOfView * 0.5f;
            return Mathf.Tan(Mathf.Deg2Rad * halfFOV) * cameraDistance * 2;
        }

        public static TimeSpan RemainingTime(DateTime lastTime, DateTime now, float cooldown)
        {
            var targetTime = lastTime.AddSeconds(cooldown);
            return targetTime.Subtract(now);
        }
        
        public static int GetWeightedRandom(int[] weights){
            var currentWeight = 0;
            var totalWeight = 0;
            foreach (var weight in weights){
                totalWeight += weight;
            }
            if (totalWeight == 0) return -1;
            var rand = UnityEngine.Random.Range(0, totalWeight);
            for (var i = 0; i < weights.Length; i++)
            {
                var weight = weights[i];
                if(weight == 0) continue;
                currentWeight += weight;
                if (rand < currentWeight){
                    return i;
                }
            }
            return -1;
        }
        
        public static int GetWeightedRandom(double[] weights){
            var currentWeight = 0d;
            var totalWeight = 0d;
            foreach (var weight in weights){
                totalWeight += weight;
            }
            if (totalWeight == 0) return -1;
            var rand = new Random().NextDouble() * totalWeight;
            for (var i = 0; i < weights.Length; i++)
            {
                var weight = weights[i];
                if(weight == 0) continue;
                currentWeight += weight;
                if (rand < currentWeight){
                    return i;
                }
            }
            return -1;
        }
    }
}