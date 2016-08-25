using UnityEngine;
using System.Collections;

public class GrenadeController : MonoBehaviour {

  public float ExplosionForce = 1000.0f;
  public float ExplosionRadius = 10.0f;
  public float FuseTime = 4.0f;

  public GameObject ExplosionPrefab;

	// Use this for initialization
	void Start () 
  {
    // Start the fuse running
    StartCoroutine (BeginEnd ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}


  IEnumerator BeginEnd()
  {
    // Five seconds afer this is called, it will destroy the object itself
    yield return new WaitForSeconds(FuseTime);
    Explode ();

    yield return new WaitForSeconds (2.0f);
    Destroy (gameObject);
  }

  void Explode()
  {
    // Make some noise
    GetComponent<AudioSource> ().Play ();

    // Instansiate an explosion prefab
    GameObject explosion = Instantiate(ExplosionPrefab) as GameObject;
    explosion.transform.position = transform.position;

    // Hide the grenade's body
    MeshRenderer meshRender = GetComponent<MeshRenderer>();
    meshRender.enabled = false;

    // Find all colliders within the explosion radius
    Collider[] nearbyColliders = Physics.OverlapSphere (transform.position, ExplosionRadius);

    // Loop over the colliders
    foreach( Collider collider in nearbyColliders )
    {
      // Get the attached gameobject
      GameObject target = collider.gameObject;

      // Only consider those currently active
      if( target.activeInHierarchy == false || target.CompareTag("Target") == false )
        continue;

      // Apply an explosive force to them originating from the
      // grenade's position
      Rigidbody targetRB = target.GetComponent<Rigidbody>();
      targetRB.AddExplosionForce (ExplosionForce, 
                                  transform.position, 
                                  ExplosionRadius);
      
    }
  }
}
