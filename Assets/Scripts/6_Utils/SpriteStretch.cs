using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteStretch : MonoBehaviour
{
    private bool isApply = false;
    private int lastWidth, lastHeight;
    private float lastSpriteWidth, lastSpriteHeight;
    private Sprite lastSprite;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        Refresh();
    }

    private void Update()
    {
        if (isApply)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        Sprite sprite = sr.sprite;

        float spriteWidth = 1f;
        float spriteHeight = 1f;
        if (sprite != null)
        {
            spriteWidth = sprite.bounds.size.x;
            spriteHeight = sprite.bounds.size.y;
        }

        if (lastWidth != Screen.width ||
            lastHeight != Screen.height ||
            lastSprite != sprite)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            lastSprite = sprite;
            lastSpriteWidth = spriteWidth;
            lastSpriteHeight = spriteHeight;

            Apply();
        }
    }

    private void Apply()
    {
        float cameraHeight = Camera.main.orthographicSize * 2;
        Vector2 cameraSize = new Vector2(Camera.main.aspect * cameraHeight, cameraHeight);
        Vector2 spriteSize = new Vector2(lastSpriteWidth, lastSpriteHeight);
        Vector3 scale = Vector3.one;

        float ratioX = cameraSize.x / spriteSize.x;
        float ratioY = cameraSize.y / spriteSize.y;
        if (ratioX > ratioY)
        {
            scale.x = scale.y = ratioX;
        }
        else
        {
            scale.x = scale.y = ratioY;
        }
        transform.localScale = scale;

        isApply = true;
    }
}
