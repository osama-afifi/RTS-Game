using System;
using UnityEngine;

public class F16 : Helicopter
{
	private static GameObject unitPrefab = (GameObject)Resources.Load ("prefabs/Aircraft-F16");
	protected static float deathHeight = 1.0f;
	
	public F16 (Vector3 Pos)
        : base(unitPrefab, new Vector3(Pos.x, verticalHeight , Pos.z))
	{
		unitObject.particleSystem.enableEmission = false;
		currentVerticalHeight = verticalHeight;
		speed = 30.0f;
		rotationSpeed = 0.5f;
		stopDistanceOffset = 5.0f;
		maxHeightDiff = 0.5f;

		reloadTime = 3.0f;
		unitDefenceRange = 125;
		unitAttackRange = 100;
		destination = new Vector3 (128,currentVerticalHeight,128);
		maxHealth = 50;
		health = maxHealth;
		
		deadBefore = false;
	}

	public override void Update ()
	{
		switch (currentState) {
		case UnitState.Check:
			{
				check ();
				Move ();
				break;
			}
		case UnitState.Attack:
			{
				attack ();
				break;
			}
		case UnitState.Move:
			{
				Move ();
				break;
			}
		}
		
		if (firingState)
			fire ();
 		if(dead || getPosition().y<=deathHeight)
		{
			dead = true;
			destroyBody();
		}
		checkNearbyHits ();
	}
	
	protected override void directMove (Vector3 destinationPoint)
	{
		
		if (destination != Vector3.zero && getPosition () != destinationPoint) {
			
			Vector3 bodyDirection = (destinationPoint - getPosition ()).normalized;

			float timeStepRot = rotationSpeed * Time.deltaTime;
			float timeStepMov = speed * Time.deltaTime;

			Vector3 newBodyDir = Vector3.RotateTowards (unitObject.transform.forward, bodyDirection, timeStepRot, 0.0f);
			
			Vector3 rotationTilt = Quaternion.LookRotation (newBodyDir).eulerAngles;

			rotationTilt.z += -30 * bodyDirection.z;
			unitObject.transform.rotation = Quaternion.Euler(rotationTilt);
	
			unitObject.transform.position = Vector3.MoveTowards (unitObject.transform.position, unitObject.transform.position + unitObject.transform.forward , timeStepMov);
			
			if (Vector3.Distance (unitObject.transform.position, destinationPoint) < stopDistanceOffset) {
				movingInSection = false;
			}
		}
		
	}
	public override void attack()
	{
		
				if (targetUnit == null || targetUnit.unitObject == null || targetUnit.isDead()) {
			firingState = false;
			currentState = previousState;
			speed = 30.0f;
			return;
		}
		else base.attack();
		speed = 40.0f;
		if(targetUnit!=null)
		Move();
		speed = 30.0f;
	}
		protected override bool rotateTurretToTarget ()
	{
			return true;
	
	}
	
		public override void changeHealth (int decrease)
	{
		
		health += decrease;
		if (health > maxHealth)
			health = maxHealth;
		if (health <= 0) {
			health = 0;
			//dead = true;
			destroyBody ();
			return;
		}
		float ratio = (float)health / maxHealth;
		Vector3 scl = unitObject.transform.FindChild ("HealthBar").FindChild ("GreenBar").transform.localScale;
		unitObject.transform.FindChild ("HealthBar").FindChild ("GreenBar").transform.localScale = new Vector3 (ratio * healthScale, scl.y, scl.z);
	}
		
			public override void destroyBody()
	{

		unitObject.rigidbody.useGravity = true;
		unitObject.particleSystem.enableEmission = true;
		currentVerticalHeight += -6f;
		deadBefore = true;
		
		
		if(unitObject.transform.position.y >= deathHeight)
			return;
		 dead = true;       
		selection.SetActive(false);
        Vector3 explosionPos = unitObject.transform.position;
        ProjectMain.Destroy(GameObject.Instantiate(explosionFire, explosionPos, Quaternion.identity), 5f);
        ProjectMain.Destroy(GameObject.Instantiate(explosionSound, explosionPos, Quaternion.identity), 5f);
        ProjectMain.Destroy(unitObject, 5f);
        UnitManager.getInstance().removeSelectedDead();
        unitObject = null;
		
	}
}


