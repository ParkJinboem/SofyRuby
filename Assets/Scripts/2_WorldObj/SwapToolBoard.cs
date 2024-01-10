using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class SwapToolBoard : MonoBehaviour
{
    [SerializeField] private float boardMoveX = 1500f;
    [SerializeField] private int startMakeupIdx = 3;
    [SerializeField] private int startHairIdx = 5;

    [SerializeField] SpriteRenderer imgBg;
    [SerializeField] Image imgToolDefaultBg;
    [SerializeField] Image imgToolHairBg;
    [SerializeField] Image imgWashDress;
    [SerializeField] Image imgInnerWear;
    [SerializeField] Image imgHair;
    [SerializeField] Button btnSlide;

    [Header("Item Attacher")]
    [SerializeField] ItemAttacher nosePack;
    [SerializeField] ItemAttacher maskPack;

    [Header("Paint")]
    [SerializeField] Texture2D clearTexture_1024;
    [SerializeField] Texture2D clearTexture_512;
    [SerializeField] Texture2D dirtyTexture;
    [SerializeField] AdvancedMobilePaint.AdvancedMobilePaint paintDirty;
    [SerializeField] AdvancedMobilePaint.AdvancedMobilePaint paintCream;
    [SerializeField] AdvancedMobilePaint.AdvancedMobilePaint paintCheek;
    [SerializeField] AdvancedMobilePaint.AdvancedMobilePaint paintEyebrow;
    [SerializeField] AdvancedMobilePaint.AdvancedMobilePaint paintLip;
    [SerializeField] AdvancedMobilePaint.AdvancedMobilePaint paintShadow;

    private SwapBoardChecker[] swapBoards;
    private Sprite[] spBgs;
    private HolderIdx holderIdx;
    private GirlHolder girlHolder;
    private Pimples pimples;
    private CucumbleChecker cucumbles;
    private SideViewObj sideBar;
    private MakingStairBtn makeStair;

    private int toolIdx = 0;
    public int ToolIdx
    {
        get { return toolIdx; }
    }

    private void Awake()
    {
        paintDirty.gameObject.gameObject.GetComponent<MeshCollider>().enabled = true;
        paintCream.gameObject.GetComponent<MeshCollider>().enabled = false;
    }

    private void Start()
    {
        swapBoards = GetComponentsInChildren<SwapBoardChecker>();
        spBgs = MakingMgr.GetInst().SpriteLoader(SpriteSelectorIDX.BACKGROUND);
        holderIdx = FindObjectOfType<HolderIdx>();
        girlHolder = FindObjectOfType<GirlHolder>();
        pimples = FindObjectOfType<Pimples>();
        cucumbles = FindObjectOfType<CucumbleChecker>();
        sideBar = FindObjectOfType<SideViewObj>();
        makeStair = FindObjectOfType<IntroCanvas>().m_MakingStairPannel.GetComponent<MakingStairBtn>();
        makeStair.StairBtn[0].gameObject.SetActive(false);

        WorldMgr.GetInst().SetMaxToolIdx(swapBoards.Length - 1);
    }

    public void SwapNext()
    {
        if (toolIdx == swapBoards.Length - 1)
        {
            WorldMgr.GetInst().OnOffLoadPannel(true);
            WorldMgr.GetInst().LoadjujuSkin_OnClothes(girlHolder._Mask_Cheek.texture,
                girlHolder._Mask_EyeBrow.texture, girlHolder._Mask_Shadow.texture,
                girlHolder._Mask_Lipstick.texture, girlHolder._Hair.sprite, girlHolder._Eye.sprite);

            Invoke("GoClothScene", .5f);
            return;
        }

        makeStair.StairBtn[0].gameObject.SetActive(true);
        sideBar.GetComponent<Animator>().SetTrigger("NPBtn");
        btnSlide.gameObject.SetActive(false);

        switch (toolIdx)
        {
            case 1:
                paintDirty.SetDrawingTexture(clearTexture_1024);
                paintDirty.gameObject.GetComponent<RawImage>().enabled = false;
                paintDirty.gameObject.GetComponent<MeshCollider>().enabled = false;

                if (cucumbles != null) cucumbles.Show();
                if (nosePack != null) nosePack.Show();
                if (maskPack != null) maskPack.Show();
                paintCream.SetDrawingTexture(clearTexture_1024);
                paintCream.gameObject.GetComponent<RawImage>().enabled = true;
                paintCream.gameObject.GetComponent<MeshCollider>().enabled = true;
                break;
            case 2:
                if (pimples != null) pimples.Hide();
                if (cucumbles != null) cucumbles.Hide();
                if (nosePack != null) nosePack.Hide();
                if (maskPack != null) maskPack.Hide();
                paintCream.SetDrawingTexture(clearTexture_1024);
                paintCream.gameObject.GetComponent<RawImage>().enabled = false;
                paintCream.gameObject.GetComponent<MeshCollider>().enabled = false;
                break;
            default:
                break;
        }

        if (toolIdx == startMakeupIdx - 1)
        {
            imgBg.sprite = spBgs[1];

            imgWashDress.gameObject.SetActive(false);
            imgHair.gameObject.SetActive(true);
            imgInnerWear.gameObject.SetActive(true);
            paintCheek.SetDrawingTexture(clearTexture_512);
            paintEyebrow.SetDrawingTexture(clearTexture_512);
            paintLip.SetDrawingTexture(clearTexture_512);
            paintShadow.SetDrawingTexture(clearTexture_512);
            paintCheek.gameObject.GetComponent<RawImage>().enabled = true;
            paintEyebrow.gameObject.GetComponent<RawImage>().enabled = true;
            paintLip.gameObject.GetComponent<RawImage>().enabled = true;
            paintShadow.gameObject.GetComponent<RawImage>().enabled = true;
            paintCheek.gameObject.GetComponent<MeshCollider>().enabled = true;
            paintEyebrow.gameObject.GetComponent<MeshCollider>().enabled = true;
            paintLip.gameObject.GetComponent<MeshCollider>().enabled = true;
            paintShadow.gameObject.GetComponent<MeshCollider>().enabled = true;
        }

        if (toolIdx == startHairIdx - 1)
        {
            imgToolDefaultBg.enabled = false;
            imgToolHairBg.enabled = true;
        }
        else
        {
            imgToolDefaultBg.enabled = true;
            imgToolHairBg.enabled = false;
        }

        DotweenMgr.DoLocalMoveX(-boardMoveX, .5f, swapBoards[toolIdx].transform);
        DotweenMgr.DoLocalMoveX(0, .5f, swapBoards[toolIdx + 1].transform);
        toolIdx++;
        holderIdx.SetFlowerIdx(toolIdx);
    }

    public void SwapPrev()
    {
        if (toolIdx == 0)
        {
            return;
        }

        sideBar.GetComponent<Animator>().SetTrigger("NPBtn");
        btnSlide.gameObject.SetActive(false);

        switch (toolIdx)
        {
            case 1:
                break;
            case 2:
                btnSlide.gameObject.SetActive(false);
                paintDirty.SetDrawingTexture(dirtyTexture);
                paintDirty.gameObject.GetComponent<RawImage>().enabled = true;
                paintDirty.gameObject.GetComponent<MeshCollider>().enabled = true;

                if (cucumbles != null) cucumbles.Hide();
                if (nosePack != null) nosePack.Hide();
                if (maskPack != null) maskPack.Hide();
                paintCream.SetDrawingTexture(clearTexture_1024);
                paintCream.gameObject.GetComponent<RawImage>().enabled = false;
                paintCream.gameObject.GetComponent<MeshCollider>().enabled = false;
                break;
            case 3:
                if (pimples != null) pimples.Show();
                if (cucumbles != null) cucumbles.Show();
                if (nosePack != null) nosePack.Show();
                if (maskPack != null) maskPack.Show();
                paintCream.SetDrawingTexture(clearTexture_1024);
                paintCream.gameObject.GetComponent<RawImage>().enabled = true;
                paintCream.gameObject.GetComponent<MeshCollider>().enabled = true;
                break;
            default:
                break;
        }

        if (toolIdx == startMakeupIdx)
        {
            imgBg.sprite = spBgs[0];

            imgWashDress.gameObject.SetActive(true);
            imgHair.gameObject.SetActive(false);
            imgInnerWear.gameObject.SetActive(false);
            paintCheek.SetDrawingTexture(clearTexture_1024);
            paintEyebrow.SetDrawingTexture(clearTexture_1024);
            paintLip.SetDrawingTexture(clearTexture_1024);
            paintShadow.SetDrawingTexture(clearTexture_1024);
            paintCheek.gameObject.GetComponent<RawImage>().enabled = false;
            paintEyebrow.gameObject.GetComponent<RawImage>().enabled = false;
            paintLip.gameObject.GetComponent<RawImage>().enabled = false;
            paintShadow.gameObject.GetComponent<RawImage>().enabled = false;
            paintCheek.gameObject.GetComponent<MeshCollider>().enabled = false;
            paintEyebrow.gameObject.GetComponent<MeshCollider>().enabled = false;
            paintLip.gameObject.GetComponent<MeshCollider>().enabled = false;
            paintShadow.gameObject.GetComponent<MeshCollider>().enabled = false;
        }

        if (toolIdx == startHairIdx)
        {
            imgToolDefaultBg.enabled = false;
            imgToolHairBg.enabled = true;
        }
        else
        {
            imgToolDefaultBg.enabled = true;
            imgToolHairBg.enabled = false;
        }

        DotweenMgr.DoLocalMoveX(boardMoveX, .5f, swapBoards[toolIdx].transform);
        DotweenMgr.DoLocalMoveX(0, .5f, swapBoards[toolIdx - 1].transform);
        toolIdx--;
        holderIdx.SetFlowerIdx(toolIdx);
        if (toolIdx == 0)
        {
            makeStair.StairBtn[0].gameObject.SetActive(false);
        }
    }

    void GoClothScene() => SceneManager.LoadScene(GameManager.ClothSceneName);

    private void OnDisable()
    {
        try
        {
            makeStair.StairBtn[0].gameObject.SetActive(true);
        }
        catch (Exception e) { }
    }
}
