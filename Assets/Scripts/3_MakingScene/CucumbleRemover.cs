using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CucumbleRemover : MonoBehaviour
{
	public Transform trTarget;
	public CucumbleTool cucumbleTool;

	private bool isAttach;

	public void OnPointDown()
	{
		SoundMgr.GetInst().OnClickSoundBtn();
		Attach();
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
		RaycastHit hit = new RaycastHit();
		Physics.Raycast(ray, out hit);
		if (hit.collider != null)
		{
			if (hit.collider.gameObject.tag == "Cucumble" &&
				hit.collider.gameObject.transform.parent == trTarget &&
				hit.collider.gameObject.GetComponentInChildren<CucumbleRemover>() == null)
			{
				isAttach = true;
				SoundMgr.GetInst().OnClickSoundBtn();
				gameObject.GetComponent<RectTransform>().parent = hit.collider.gameObject.GetComponent<RectTransform>();
				gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2();
			}
		}
	}

	public void Clear()
	{
		Attach();
	}

	private void Attach()
	{
		cucumbleTool.Return(gameObject);
		isAttach = false;
	}
}
