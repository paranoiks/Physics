using UnityEngine;
using System.Collections;

public class TestController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
    private void HandleKeyboard()
    { 
        float f = 0;
        if(Input.GetKey(KeyCode.RightArrow))
        {
           f = 1;
        }
        else if(Input.GetKey(KeyCode.LeftArrow))
        {
           f = -1;
        }
        gameObject.GetComponent<Rigidbody>().AddForce(Vector3.right * f, ForceMode.Force);
    }

	// Update is called once per frame
	void Update () {
        HandleKeyboard();
	}
}
