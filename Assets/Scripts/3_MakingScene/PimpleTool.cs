using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PimpleTool : MonoBehaviour
{
	public Pimples _targetObj;
	public GameObject _tooltip;

	public bool _check;


    void Update()
    {
		if (!_check) return;
	
		//RaycastHit _testhit = new RaycastHit();
		//Physics.Raycast(_raytest, _testhit);

		Ray ray = new Ray(_tooltip.transform.position, Vector3.forward);

		//Debug.DrawRay(_tooltip.transform.position, Vector3.forward, Color.red);
		RaycastHit hit = new RaycastHit();
		Physics.Raycast(ray, out hit);
		if (hit.collider != null)
		{
			//if(!hit.collider.name.Contains("Dirty"))
			//Debug.Log(hit.collider.name);
			if (hit.collider.gameObject.transform.parent == _targetObj.transform)
			{
				//SoundMgr.GetInst().OnClickSoundBtn();
				GetComponentInChildren<ParticleSystem>().Play(true);
				hit.collider.gameObject.SetActive(false);
			}
		}
	}

	
}
