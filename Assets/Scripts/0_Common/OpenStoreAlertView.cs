using UnityEngine;

public class OpenStoreAlertView : MonoBehaviour
{
    private OpenStore openStore;

    public void Show(OpenStore openStore)
    {
        this.openStore = openStore;

        gameObject.SetActive(true);
    }

    public void Open()
    {
        openStore.Open();

        gameObject.SetActive(false);
    }
}
