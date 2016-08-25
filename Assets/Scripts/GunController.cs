using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Text;

public class GunController : MonoBehaviour {

  public enum GunType { Rifle = 0, CannonBall = 1, Grenade = 2 };
  private int NumGuns = Enum.GetNames(typeof(GunType)).Length;

  public Text WeaponName;

  public float RifleShotStrength = 1000.0f;
  public float RifleROF = 1.0f;
  public AudioClip GunShot;

  public float CannonBallOffset = 1.0f;
  public float CannonBallSpeed = 10.0f;
  public float CannonBallROF = 1.0f;
  public GameObject CannonBallPrefab;
  public AudioClip CannonShot;

  public float GrenadeOffset = 1.0f;
  public float GrenadeSpeed = 10.0f;
  public float GrenadeROF = 1.0f;
  public GameObject GrenadePrefab;

  public GunType CurrentGun = GunType.Rifle;

  private AudioSource audioSource;
  private Camera fpsCamera;

  private float timeSinceLastRifleShot = float.MaxValue;
  private float timeSinceLastCannonShot = float.MaxValue;
  private float timeSinceLastGrenadeShot = float.MaxValue;

	// Use this for initialization
	void Start () {
    audioSource = GetComponent<AudioSource> ();
    fpsCamera = GetComponentInChildren<Camera> ();

    UpdateGunName ();
	}
	
	// Update is called once per frame  
	void Update () 
  {
    UpdateShotRateTimes ();

    CheckWeaponChoice ();
    FireOnCommand ();
  }

  void UpdateShotRateTimes()
  {
    timeSinceLastRifleShot += Time.deltaTime;
    timeSinceLastCannonShot += Time.deltaTime;
    timeSinceLastGrenadeShot += Time.deltaTime;
  }

  void CheckWeaponChoice()
  {
    if (Input.GetKeyDown ("["))
      SwitchGun (false);
    else if (Input.GetKeyDown("]"))
      SwitchGun(true);
  }

  void SwitchGun(bool next)
  {
    int gunNum = (int)CurrentGun;

    if (next)
    {
      if (gunNum == NumGuns - 1)
        gunNum = 0;
      else
        gunNum++;
    } else
    {
      if (gunNum == 0)
        gunNum = NumGuns - 1;
      else
        gunNum--;
    }

    CurrentGun = (GunType)gunNum;

    UpdateGunName ();
  }

  void FireOnCommand()
  {
    float fireInput = Input.GetAxis ("Fire1");

    if (fireInput == 0.0f)
      return;

    switch (CurrentGun)
    {
    case GunType.CannonBall:
      FireCannonBall ();
      break;
    case GunType.Grenade:
      FireGrenade ();
      break;
    default:
      FireRifle ();
      break;
    }
  }

  void FireRifle()
  {
    // Check the rate of fire
    if (timeSinceLastRifleShot < RifleROF)
      return;

    // Gunshot sound
    audioSource.clip = GunShot;
    audioSource.Play ();


    // Player position and forward vector
    Vector3 cameraPos = fpsCamera.transform.position;
    Vector3 cameraForward = fpsCamera.transform.forward;

    // Check for ray-cast hit
    RaycastHit hitInfo;

    bool hitSomething = Physics.Raycast ( cameraPos, 
                                          cameraForward, out hitInfo, 1000.0f );


    // It is now zero seconds since we fired
    timeSinceLastRifleShot = 0.0f;

    // Hit nothing? Done...
    if (hitSomething == false)
      return;

    // Hit someting that wasn't a target? Done...
    if (hitInfo.collider.gameObject.CompareTag ("Target") == false)
      return;

    // Push the target, hard!
    hitInfo.collider.attachedRigidbody.AddForce (cameraForward * RifleShotStrength);
  }

  void FireCannonBall()
  {
    // Make sure we're not firing too quickly
    if (timeSinceLastCannonShot < CannonBallROF)
      return;

    // Cannon shot sound
    audioSource.clip = CannonShot;
    audioSource.Play ();

    // Player position and forward vector
    Vector3 cameraPos = fpsCamera.transform.position;
    Vector3 cameraForward = fpsCamera.transform.forward;

    // Instansiate a cannon ball, place it at the player and give it velocity
    // away from the player
    GameObject newCannonBall = Instantiate (CannonBallPrefab) as GameObject;
    Rigidbody newCBRB = newCannonBall.GetComponent<Rigidbody> ();

    newCBRB.MovePosition (cameraPos + cameraForward * CannonBallOffset);
    newCBRB.velocity = cameraForward * CannonBallSpeed;

    // Zero seconds since we fired
    timeSinceLastCannonShot = 0.0f;
  }

  void FireGrenade()
  {
    // Make sure we're not firing too quickly
    if (timeSinceLastGrenadeShot < GrenadeROF)
      return;

    // Player position and forward vector
    Vector3 cameraPos = fpsCamera.transform.position;
    Vector3 cameraForward = fpsCamera.transform.forward;

    // Instansiate a grenade, place it at the player and give it velocity
    // away from the player
    GameObject newGrenade = Instantiate (GrenadePrefab) as GameObject;
    Rigidbody newGrenadeRB = newGrenade.GetComponent<Rigidbody> ();

    newGrenadeRB.MovePosition (cameraPos + cameraForward * GrenadeOffset);
    newGrenadeRB.velocity = cameraForward * GrenadeSpeed;

    // Zero seconds since we fired
    timeSinceLastGrenadeShot = 0.0f;
  }

  void UpdateGunName()
  {
    WeaponName.text = CurrentGun.ToString ();
  }
}
