using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour {
	
	void Start () {
		GetComponent<TextMeshProUGUI>().SetText(Application.productName
		                                        + " "
		                                        + Application.version
		                                        + (Application.buildGUID != ""
		                                           ? ", build " + Application.buildGUID
		                                           : ""));
	}
	
	void Update () {
		
	}
	
}
