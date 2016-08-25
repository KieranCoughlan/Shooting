using UnityEngine;
using System.Collections;

public class CannonBallController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnCollisionEnter(Collision collision)
  {
    StartCoroutine (BeginEnd ());
  }

  IEnumerator BeginEnd()
  {
    // Two seconds afer this is called, it will destroy the object itself
    yield return new WaitForSeconds(2.0f);
    Destroy(gameObject);
  }
}
