using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CucumbleTool : MonoBehaviour
{
	public GameObject _targetObj;

	public GameObject prefabCucumber;
	public Queue<GameObject> objCucumbers;
	private GameObject objMoveCucumber;

	public GameObject _creamTarget;
	private CucumbleChecker _cucmbleChecker;

	public Image _NowStatBtn;
	public Sprite _IconCucumble;

    private void Awake()
    {
		prefabCucumber.SetActive(false);
		objCucumbers = new Queue<GameObject>();
	}

    private void Start()
	{
		_cucmbleChecker = FindObjectOfType<CucumbleChecker>();
	}

	public GameObject GetCucumber()
    {
		GameObject obj;
		if (objCucumbers.Count > 0)
        {
			obj = objCucumbers.Dequeue();
		}
		else
        {
			obj = Instantiate(prefabCucumber, transform);
        }
		Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		obj.transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, transform.position.z);
		return obj;
	}

	public void Return(GameObject obj)
    {
		obj.transform.parent = transform;
		objCucumbers.Enqueue(obj);
		obj.SetActive(false);
	}

	public void OnPointerDown()
	{
		SoundMgr.GetInst().OnClickSoundBtn();
		_creamTarget.SetActive(false);
		_NowStatBtn.gameObject.SetActive(true);
        _NowStatBtn.sprite = _IconCucumble;

        DotweenMgr.DoPopupOpen(0f, 1f, .6f, _NowStatBtn.transform);

        BoxCollider[] _tempBoxCu = _cucmbleChecker.GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider _box in _tempBoxCu)
        {
            _box.enabled = true;
        }

		objMoveCucumber = GetCucumber();
		objMoveCucumber.SetActive(true);
	}

	public void OnPointerUp()
	{
		_creamTarget.SetActive(true);

		DotweenMgr.DoPopupOpen(1f, 0f, .6f, _NowStatBtn.transform);

        BoxCollider[] _tempBoxCu = _cucmbleChecker.GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider _box in _tempBoxCu)
        {
            _box.enabled = false;
        }

		if (objMoveCucumber != null)
        {
			if (transform == objMoveCucumber.transform.parent)
			{
				Return(objMoveCucumber);
			}
		}
    }
}
