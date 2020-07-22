using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndButton : MonoBehaviour {

	public void killParent(){
		Destroy(transform.parent.gameObject);
	}
}
