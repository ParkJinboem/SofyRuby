using UnityEngine;

public class EffectSystem : MonoBehaviour
{
    public void OnParticleSystemStopped()
    {
        EffectManager.Instance.Return(transform);
    }
}
