using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class TankProjectile : Weapon
{

    private static GameObject bodyPrefab = (GameObject)Resources.Load("Prefabs/Projectile");
    private static GameObject explosionBody = (GameObject)Resources.Load("Sparks");
    private static GameObject soundMissile = (GameObject)Resources.Load("MissileSound");
    private GameObject soundIns;

	
    public TankProjectile(Vector3 from, Vector3 dest):base()
    {
		damageEffectViaType = new List<int>()	{1000 ,50 ,70 ,0 ,70 ,0 ,70 ,0};
		//{ Infanty, Tank, AirDefense, Ship, Artillery, Helicopter, LightVehicle , Aircraft};
        destination = dest;
        fireSpot = from;
        direction = dest - from;
		effectRange = 2.5f;
        fireBody = (GameObject)GameObject.Instantiate(bodyPrefab, fireSpot, Quaternion.LookRotation(direction));
        soundIns = (GameObject)GameObject.Instantiate(soundMissile, fireBody.transform.position, Quaternion.identity);
        speed = 40;
        lastFire = 0;
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
    public override void Update()
    {
        float timeStepRot = speed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(fireBody.transform.forward, direction, timeStepRot, 0.0f);
        fireBody.transform.rotation = Quaternion.LookRotation(newDir);
        fireBody.transform.position = Vector3.MoveTowards(fireBody.transform.position, destination, timeStepRot);
        lastFire += Time.deltaTime;
        checkMissileLife();
    }
}
