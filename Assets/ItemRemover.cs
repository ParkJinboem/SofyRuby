using UnityEngine;

public class ItemRemover : MonoBehaviour
{
    public ItemAttacher itemAttacher;

    private RaycastHit hit;
    private bool isAttach;

    private void Awake()
    {
        hit = new RaycastHit();
    }

    void Update()
	{
		if (isAttach)
		{
			return;
		}

		Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, transform.position.z);

		Ray ray = new Ray(transform.position, Vector3.forward);
		Physics.Raycast(ray, out hit);
		if (hit.collider != null)
		{
			if (hit.collider.transform.childCount == 0 &&
				hit.collider.transform == itemAttacher.trTarget)
			{
				isAttach = true;
				SoundMgr.GetInst().OnClickSoundBtn();
				transform.parent = hit.collider.transform;
				gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			}
		}
	}

	public void OnPointDown()
    {
		Return();
    }

	private void Return()
	{
		SoundMgr.GetInst().OnClickSoundBtn();
		itemAttacher.Return(gameObject);
		isAttach = false;
	}
}
