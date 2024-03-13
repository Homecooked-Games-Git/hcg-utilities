using HCGames.Utilities;
using UnityEngine;

public class PooledParticleSystem : MonoBehaviour
{
   public void OnParticleSystemStopped()
   {
      PoolManager.Instance.Release(gameObject);
   }
}
