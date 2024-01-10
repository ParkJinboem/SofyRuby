using UnityEngine;
using UnityEngine.UI;

public class UISpriteSheetAnimation : MonoBehaviour
{
    public float speed = 1f;
    public int frameRate = 30;
    public bool isAutoStart = true;
    public bool isLoop = true;    
    public Sprite[] sprites;

    private Image image;
    private float timePerFrame = 0f;
    private float elapsedTime = 0f;
    private int currentFrame = 0;

    void Start()
    {
        enabled = false;
        image = GetComponent<Image>();
        image.enabled = false;

        LoadSpriteSheet();

        if (isAutoStart)
        {
            Play();
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime * speed;
        if (elapsedTime >= timePerFrame)
        {
            elapsedTime = 0f;
            SetSprite();
            currentFrame++;
            if (currentFrame >= sprites.Length)
            {
                if (isLoop)
                {
                    currentFrame = 0;
                }
                else
                {
                    enabled = false;
                }
            }
        }
    }

    private void LoadSpriteSheet()
    {
        timePerFrame = 1f / frameRate;
    }

    private void SetSprite()
    {
        if (currentFrame < sprites.Length)
        {
            image.sprite = sprites[currentFrame];
        }
        if (!image.enabled)
        {
            image.enabled = true;
        }
    }

    public void Play()
    {
        enabled = true;
    }
}
