using UnityEngine;

public class Effect : MonoBehaviour
{
    public Transform trEffect;
    public float effScale;
    private Vector3 pos;

    public string EffectName
    {
        get { return trEffect.name; }
    }

    public Vector3 Position
    {
        get
        {
            pos = transform.position;
            pos.z = 0f;
            return pos;
        }
    }
}
