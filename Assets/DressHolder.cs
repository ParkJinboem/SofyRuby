using UnityEngine;
using UnityEngine.UI;

public class DressHolder : MonoBehaviour
{
    public MenuSelector ms;

    [SerializeField] Image imgDefault;

    private Effect ef;
    private Image img;
    private PoolSystem ps;

    private int idx;    

    public Effect Effect
    {
        get { return ef; }
    }

    public int Idx
    {
        get { return idx; }
    }

    public bool IsShow
    {
        get { return img.enabled; }
    }

    private void Awake()
    {
        img = gameObject.GetComponent<Image>();
        ef = gameObject.GetComponentInChildren<Effect>();
        ps = gameObject.GetComponentInChildren<PoolSystem>();
    }

    public void SetIdx(int idx)
    {
        this.idx = idx;

        if (ps != null)
        {
            ps.Clear();
            Transform trImg = ps.GetObjectFromPool(string.Format("{0:D2}", idx), true);
            if (trImg != null)
            {
                trImg.gameObject.SetActive(true);
            }
        }
        else
        {
            img.sprite = MakingMgr.GetInst().SpriteLoader(ms, idx);
        }
    }

    public void Show(bool isAllHide = false)
    {
        if (ps != null)
        {
            ps.gameObject.SetActive(true);
        }

        bool useDefault = isAllHide && idx == 1 && imgDefault != null;
        img.enabled = !useDefault;
        if (imgDefault != null) imgDefault.enabled = useDefault;
    }

    public void Hide()
    {
        if (ps != null)
        {
            ps.gameObject.SetActive(false);
        }

        img.enabled = false;
        if (imgDefault != null) imgDefault.enabled = false;
    }
}
