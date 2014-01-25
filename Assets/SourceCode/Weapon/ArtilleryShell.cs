using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class ArtilleryShell : Weapon
{
	

	
	private static GameObject bodyPrefab = (GameObject)Resources.Load ("Prefabs/ArtilleryShell");
	private static GameObject explosionBody = (GameObject)Resources.Load ("Sparks");
	   private static GameObject soundMissile = (GameObject)Resources.Load("MissileSound");
	   private GameObject soundIns;
	public ArtilleryShell (Vector3 from, Vector3 dest):base()
	{
		damageEffectViaType = new List<int>()	{500 ,70 ,70 ,30 ,50 ,0 ,300 ,0};
	//{ Infanty, Tank, AirDefense, Ship, Artillery, Helicopter, LightVehicle , Aircraft};
		
		lifeTime = 6.0f;
		destination = dest;
		fireSpot = from;
		direction = dest - from;
		effectRange = 4.0f;
		speed = 40; // No Need
		lastFire = 0;
		fireBody = (GameObject)GameObject.Instantiate (bodyPrefab, fireSpot, Quaternion.LookRotation (direction));
		fireBody.rigidbody.velocity = BallisticVel (destination, 60.0f); 
		fireBody.transform.rotation = Quaternion.LookRotation (fireBody.rigidbody.velocity);
		soundIns = (GameObject)GameObject.Instantiate(soundMissile, fireBody.transform.position, Quaternion.identity);
//		fireBody.tag = "weapon";

	}

	public override void destroyBody ()
	{
		dead = true;
		GameObject exp = (GameObject)GameObject.Instantiate (explosionBody, fireBody.transform.position, Quaternion.identity);
		ProjectMain.Destroy (fireBody);
		ProjectMain.Destroy (exp, 1.5f);
	    ProjectMain.Destroy(soundIns, 1.5f);
		fireBody = null;
	}

	public Vector3 BallisticVel (Vector3 target, float angle)
	{
		Vector3 dir = target - fireBody.transform.position;  // get target direction
		float h = dir.y;  // get height difference
		dir.y = 0;  // retain only the horizontal direction
		float dist = dir.magnitude;  // get horizontal distance
		float a = angle * Mathf.Deg2Rad;  // convert angle to radians
		dir.y = dist * Mathf.Tan (a);  // set dir to the elevation angle
		dist += h / Mathf.Tan (a);  // correct for small height differences
		var vel = Mathf.Sqrt (dist * Physics.gravity.magnitude / Mathf.Sin (2 * a));// calculate the velocity magnitude
		return vel * dir.normalized;
	}
   public override void checkMissileLife()
    {
		if( Physics.CheckSphere(fireBody.transform.position+fireBody.transform.forward*0.5f,0.15f) || lastFire>=lifeTime ) //  0.005 is the radius of the collison sphere
		{
			WeaponManager.getInstance().addExplosion(this);
			explodedPosition = fireBody.transform.position;
            destroyBody();
		}
    }
	public override void Update ()
	{	
		lastFire += Time.deltaTime;
		fireBody.transform.rotation = Quaternion.LookRotation (fireBody.rigidbody.velocity);   
		checkMissileLife ();
	}
}
