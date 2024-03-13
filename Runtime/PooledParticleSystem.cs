using HCGames.Utilities;
using UnityEngine;

public class PooledParticleSystem : MonoBehaviour
{
   public void OnParticleSystemStopped()
   {
      PoolManager.instance.Release(gameObject);
   }
}
