using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemAttacher : MonoBehaviour
{
	public Transform trTarget;
	public BoxCollider col;
	public GameObject prefabItem;

	private Queue<GameObject> objItems;
	private GameObject objMove;

	public Image imgState;
	public Sprite spIcon;

	private void Awake()
	{
		prefabItem.SetActive(false);
		col.enabled = false;
		objItems = new Queue<GameObject>();
	}

	public void OnPointerDown()
	{
		SoundMgr.GetInst().OnClickSoundBtn();
		imgState.gameObject.SetActive(true);
		imgState.sprite = spIcon;

		DotweenMgr.DoPopupOpen(0f, 1f, .6f, imgState.transform);

		col.enabled = true;

		objMove = Get();
		objMove.SetActive(true);
	}

	public void OnPointerUp()
	{
		DotweenMgr.DoPopupOpen(1f, 0f, .6f, imgState.transform);

		col.enabled = false;

		if (objMove != null)
		{
			if (transform == objMove.transform.parent)
			{
				Return(objMove);
			}
		}
	}

	public GameObject Get()
	{
		GameObject obj;
		if (objItems.Count > 0)
		{
			obj = objItems.Dequeue();
		}
		else
		{
			obj = Instantiate(prefabItem, transform);
		}
		Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		obj.transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, transform.position.z);
		return obj;
	}

	public void Return(GameObject obj)
	{
		obj.transform.parent = transform;
		objItems.Enqueue(obj);
		obj.SetActive(false);
	}

	public void Show()
	{
		ItemRemover[] itemRemovers = trTarget.GetComponentsInChildren<ItemRemover>();
        for (int i = 0; i < itemRemovers.Length; i++)
        {
			itemRemovers[i].OnPointDown();
		}
		trTarget.gameObject.SetActive(true);
	}

	public void Hide()
    {
		trTarget.gameObject.SetActive(false);
    }
}
