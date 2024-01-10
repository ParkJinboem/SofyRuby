using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlertViewOptions
{
    // 본문
    public string message;
    // 취소 버튼 눌렀을 때 실행되는 대리자
    public System.Action cancelButtonDelegate;
    // OK 버튼을 눌렀을 때 실행되는 대리자
    public System.Action okButtonDelegate;
    // Dismiss일 때 실행되는 대리자
    public System.Action dismissDelegate;
    public string okButtonText = null;
    public string cancelButtonText = null;
    public string chkButtonText = null;
}

public class AlertView : MonoBehaviour
{
    public GameObject goPanel;
    public Transform root;
    public TextMeshProUGUI txtMessage;

    public Transform nomalTextPosition;
    public Transform okTextPosition;
    public Transform inputTextPosition;
    public Transform imageTextPosition;

    public Transform imagePosition;
    public Transform imagePositionWithButton;
    
    public Text thisText;
    public InputField inputField;
    public Button okButton;
    public Text okButtonText;
    public Button noButton;
    public Text noButtonText;
    public Text chkButtonText;

    public GameObject BackImgObj;
    public Image ItemImg;

    public GameObject checkObj;
    public GameObject oknoObj;

    const int SFontSize = 44;
    const int MFontSize = 48;
    const int LFontSize = 52;


    // 취소 버튼 눌렀을 때 실행되는 대리자
    private System.Action cancelButtonDelegate;
    // OK 버튼을 눌렀을 때 실행되는 대리자
    private System.Action okButtonDelegate;
    // Dismiss일 때 실행되는 대리자
    public System.Action dismissDelegate;

    private void OnEnable()
    {
        DotweenMgr.DoPopupOpen(0f, 1f, .4f, root.transform);
    }

    public void Show(AlertViewOptions options)
    {
        if (txtMessage != null &&
            !string.IsNullOrEmpty(options.message))
        {
            txtMessage.text = options.message;
        }
        okButtonDelegate = options.okButtonDelegate;
        cancelButtonDelegate = options.cancelButtonDelegate;
        dismissDelegate = options.dismissDelegate;
        goPanel.SetActive(true);
    }

    public void SetOkNoButtonText(string okText, string noText)
    {
        okButtonText.text = okText;
        noButtonText.text = noText;
    }

    public void SetCheckButtonText(string chkText)
    {
        chkButtonText.text = chkText;
    }

    public void Alert(string text)
    {
        goPanel.SetActive(true);
        thisText.text = text;
        thisText.fontSize = LFontSize;

        thisText.transform.position = nomalTextPosition.position;

        inputField.gameObject.SetActive(false);
        checkObj.SetActive(false);
        oknoObj.SetActive(false);
        BackImgObj.SetActive(false);
    }

    public void Alert(string text, int fontSize)
    {
        goPanel.SetActive(true);
        thisText.text = text;
        thisText.fontSize = fontSize;

        thisText.transform.position = nomalTextPosition.position;

        inputField.gameObject.SetActive(false);
        checkObj.SetActive(false);
        oknoObj.SetActive(false);
        BackImgObj.SetActive(false);
    }
    public void Alert(string text, AlertViewOptions options)
    {
        goPanel.SetActive(true);
        thisText.text = text;
        thisText.fontSize = MFontSize;

        if (options != null)
        {
            okButtonDelegate = options.okButtonDelegate;
            cancelButtonDelegate = options.cancelButtonDelegate;
            dismissDelegate = options.dismissDelegate;
            if (options.okButtonText != null)
            {
                okButtonText.text = options.okButtonText;
            }
            if (options.cancelButtonText != null)
            {
                noButtonText.text = options.cancelButtonText;
            }
            if (options.chkButtonText != null)
            {
                chkButtonText.text = options.chkButtonText;
            }
            oknoObj.SetActive(true);
        }
        else
        {
            oknoObj.SetActive(true);
        }

        thisText.transform.position = okTextPosition.position;

        inputField.gameObject.SetActive(false);
        checkObj.SetActive(false);
        BackImgObj.SetActive(false);
    }
    public void Alert(string text, int fontSize, AlertViewOptions options)
    {
        goPanel.SetActive(true);
        thisText.text = text;
        thisText.fontSize = fontSize;

        if (options != null)
        {
            okButtonDelegate = options.okButtonDelegate;
            cancelButtonDelegate = options.cancelButtonDelegate;
            dismissDelegate = options.dismissDelegate;
            if (options.okButtonText != null)
            {
                okButtonText.text = options.okButtonText;
            }
            if (options.cancelButtonText != null)
            {
                noButtonText.text = options.cancelButtonText;
            }
            if (options.chkButtonText != null)
            {
                chkButtonText.text = options.chkButtonText;
            }
            oknoObj.SetActive(true);
        }
        else
        {
            oknoObj.SetActive(false);
        }

        thisText.transform.position = okTextPosition.position;

        inputField.gameObject.SetActive(false);
        checkObj.SetActive(false);
        BackImgObj.SetActive(false);
    }
    public void Alert(string text, bool input, string inputText, AlertViewOptions options = null)
    {
        goPanel.SetActive(true);
        thisText.text = text;
        thisText.fontSize = SFontSize;

        if (options != null)
        {
            okButtonDelegate = options.okButtonDelegate;
            cancelButtonDelegate = options.cancelButtonDelegate;
            dismissDelegate = options.dismissDelegate;
            oknoObj.SetActive(true);
            if (options.okButtonText != null)
            {
                okButtonText.text = options.okButtonText;
            }
            if (options.cancelButtonText != null)
            {
                noButtonText.text = options.cancelButtonText;
            }
            if (options.chkButtonText != null)
            {
                chkButtonText.text = options.chkButtonText;
            }
        }
        else
        {
            oknoObj.SetActive(false);
        }
        if (input)
        {
            thisText.transform.position = inputTextPosition.position;

            inputField.gameObject.SetActive(true);
            inputField.text = inputText;
            checkObj.SetActive(false);
            BackImgObj.SetActive(false);
        }
    }

    public void CheckAlert(string text, bool input, string inputText, AlertViewOptions options = null)
    {
        goPanel.SetActive(true);
        thisText.text = text;
        thisText.fontSize = SFontSize;

        if (options != null)
        {
            okButtonDelegate = options.okButtonDelegate;
            cancelButtonDelegate = options.cancelButtonDelegate;
            dismissDelegate = options.dismissDelegate;
            if (options.okButtonText != null)
            {
                okButtonText.text = options.okButtonText;
            }
            if (options.cancelButtonText != null)
            {
                noButtonText.text = options.cancelButtonText;
            }
            if (options.chkButtonText != null)
            {
                chkButtonText.text = options.chkButtonText;
            }
        }
        if (input)
        {
            thisText.transform.position = inputTextPosition.position;

            inputField.gameObject.SetActive(true);
            inputField.text = inputText;
            checkObj.SetActive(true);
            oknoObj.SetActive(false);
            BackImgObj.SetActive(false);
        }
    }
    public void CheckAlert(string text, AlertViewOptions options = null)
    {
        goPanel.SetActive(true);
        thisText.text = text;
        thisText.fontSize = MFontSize;

        if (options != null)
        {
            okButtonDelegate = options.okButtonDelegate;
            cancelButtonDelegate = options.cancelButtonDelegate;
            dismissDelegate = options.dismissDelegate;
            if (options.okButtonText != null)
            {
                okButtonText.text = options.okButtonText;
            }
            if (options.cancelButtonText != null)
            {
                noButtonText.text = options.cancelButtonText;
            }
            if (options.chkButtonText != null)
            {
                chkButtonText.text = options.chkButtonText;
            }
        }
        thisText.transform.position = okTextPosition.position;

        inputField.gameObject.SetActive(false);
        checkObj.SetActive(true);
        oknoObj.SetActive(false);
        BackImgObj.SetActive(false);
    }

    public void ImageAlert(string text, Sprite sprite, AlertViewOptions options = null)
    {
        goPanel.SetActive(true);
        thisText.text = text;
        thisText.fontSize = SFontSize;

        BackImgObj.SetActive(true);
        ItemImg.sprite = sprite;

        if (options != null)
        {
            BackImgObj.transform.position = imagePositionWithButton.position;
            thisText.transform.position = imageTextPosition.position;

            okButtonDelegate = options.okButtonDelegate;
            cancelButtonDelegate = options.cancelButtonDelegate;
            dismissDelegate = options.dismissDelegate;
            if (options.okButtonText != null)
            {
                okButtonText.text = options.okButtonText;
            }
            if (options.cancelButtonText != null)
            {
                noButtonText.text = options.cancelButtonText;
            }
            if (options.chkButtonText != null)
            {
                chkButtonText.text = options.chkButtonText;
            }
            oknoObj.SetActive(true);
            BackImgObj.SetActive(true);
        }
        else
        {
            BackImgObj.transform.position = imagePosition.position;
            thisText.transform.position = okTextPosition.position;

            oknoObj.SetActive(false);
            BackImgObj.SetActive(false);
        }

        inputField.gameObject.SetActive(false);
        checkObj.SetActive(false);
    }

    public void OnPressCancelButton()
    {
        if (cancelButtonDelegate != null)
        {
            cancelButtonDelegate.Invoke();
        }
        OnDismissButtonClick();
    }

    // OK 버튼을 눌렀을 때 호출되는 메소드
    public void OnPressOKButton()
    {
        if (okButtonDelegate != null)
        {
            okButtonDelegate.Invoke();
        }
        OnDismissButtonClick();
    }

    public void OnDismissButtonClick()
    {
        if (dismissDelegate != null)
        {
            dismissDelegate.Invoke();
        }
        goPanel.SetActive(false);
    }

    public void OnCheckButtonClick()
    {
        if (okButtonDelegate != null)
        {
            okButtonDelegate.Invoke();
        }
        OnDismissButtonClick();
    }
}
