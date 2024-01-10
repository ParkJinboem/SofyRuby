using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance = null;

    [SerializeField] PoolSystem ps;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Show(string effectName, Vector3 pos, bool isLocal = false)
    {
        Transform trEffect = ps.GetObjectFromPool(effectName, pos, isLocal);
        trEffect.gameObject.SetActive(true);
    }

    public void Show(string effectName, Vector3 pos, float scale,bool isLocal = false)
    {
        Transform trEffect = ps.GetObjectFromPool(effectName, pos, isLocal);
        trEffect.localScale = new Vector3(scale, scale, scale);
        trEffect.gameObject.SetActive(true);
    }

    public void Return(Transform tr)
    {
        ps.Return(tr);
    }
}
