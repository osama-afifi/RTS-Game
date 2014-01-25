using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Stinger : Weapon
{

    private static GameObject bodyPrefab = (GameObject)Resources.Load("Prefabs/SAM");
    private static GameObject explosionBody = (GameObject)Resources.Load("Sparks");
    private static GameObject soundMissile = (GameObject)Resources.Load("MissileSound");
	public Unit targetUnit;
    private GameObject soundIns;

	
    public Stinger(Vector3 from, ref Unit targetUnit):base()
    {
		damageEffectViaType = new List<int>()	{0 ,0 ,0 ,0 ,40 ,0 ,40 ,0};
		this.targetUnit = targetUnit;
	// { Infanty, Tank, AirDefense, Artillery, Helicopter, LightVehicle , Aircraft, Ship};
       destination = targetUnit.getPosition()+0.5f*Vector3.down;
        fireSpot = from;
        direction = destination - from;
		effectRange = 2.5f;
        fireBody = (GameObject)GameObject.Instantiate(bodyPrefab, fireSpot, Quaternion.LookRotation(direction));
        soundIns = (GameObject)GameObject.Instantiate(soundMissile, fireBody.transform.position, Quaternion.identity);
        speed = 15;
        lastFire = 0;
		lifeTime = 7f;
		//fireBody.tag = "weapon";
      
    }
    public override void destroyBody()
    {
        dead = true;
        GameObject exp = (GameObject)GameObject.Instantiate(explosionBody, fireBody.transform.position, Quaternion.identity);
        ProjectMain.Destroy(fireBody);
        ProjectMain.Destroy(exp, 1.5f);
        ProjectMain.Destroy(soundIns, 1.5f);
        fireBody = null;
    }
	
	    public override void checkMissileLife()
    {
		if( Physics.CheckSphere(fireBody.transform.position,0.001f) || targetUnit==null || lastFire>=lifeTime) //  0.005 is the radius of the collison sphere
		{
            
			WeaponManager.getInstance().addExplosion(this);
			explodedPosition = fireBody.transform.position;
			destroyBody();
		}
    }
	
    public override void Update()
    {
		if(targetUnit==null || targetUnit.unitObject==null || targetUnit.isDead())
		{
			destroyBody();
			return;
		}
		
        destination = targetUnit.getPosition();
		direction = destination - fireBody.transform.position;
		speed+=0.15f; // Accelearation
        float timeStepRot = speed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(fireBody.transform.forward, direction, timeStepRot, 0.0f);
        fireBody.transform.rotation = Quaternion.LookRotation(newDir);
        fireBody.transform.position = Vector3.MoveTowards(fireBody.transform.position, destination, timeStepRot);
        lastFire += Time.deltaTime;
		
		checkMissileLife();
  
    }
}
