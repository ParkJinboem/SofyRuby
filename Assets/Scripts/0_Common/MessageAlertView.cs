using UnityEngine;
using DG.Tweening;
using TMPro;

public class MessageAlertView : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI text;

    public void Show(string message)
    {
        canvasGroup.DOKill();
        canvasGroup.alpha = 0;
        text.text = message;
        gameObject.SetActive(true);
        canvasGroup.DOFade(1, 0.2f).OnComplete(() =>
        {
            canvasGroup.alpha = 1;
            canvasGroup.DOFade(0, 0.2f).SetDelay(0.5f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        });
    }
}
