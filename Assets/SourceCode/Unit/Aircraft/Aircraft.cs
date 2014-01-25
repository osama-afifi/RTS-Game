using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Aircraft : Unit
{
	protected float lastShotTime = 0.0f;
	protected float reloadTime = 2.0f;
	protected Vector3 targetPosition;
	protected  static float verticalHeight;
	protected int left;
	protected float accelerationFactor;
	protected  GameObject explosionSound = (GameObject)Resources.Load("TankExpFireSound");
    protected  GameObject explosionFire = (GameObject)Resources.Load("TankFire");
	
	public Aircraft (GameObject Obj, Vector3 Pos)
        : base(Obj, Pos)
	{
	
		currentState = UnitState.Check;
		movingInSection = false;
		pathCalculated = false;
		attackableTypes = new List<unitType>() {unitType.Tank , unitType.Artillery , unitType.AirDefense , unitType.Infanty, unitType.Ship, unitType.LightVehicle, unitType.Helicopter};
		//Intialize constants
		stopDistanceOffset = 100.0f;
		speed = 2.0f;
		rotationSpeed = 1.0f;
		maxHeightDiff = 0.5f;
		slopeVariationFactor = 0;
		unitAttackRange = 1000.0f;
		firingState = false;
		verticalHeight = 10.0f;
		left = 1;
		reloadTime = 2.0f;
		type = unitType.Helicopter;
	}

	public override void Update ()
	{
		switch (currentState) {
		case UnitState.Check:
			{
				check ();
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
 
		checkNearbyHits ();
	//	rotateBlade();
	}

	public override void Move () // Motion using Pathfinding
	{
		destination = new Vector3(destination.x,verticalHeight,destination.z);
		directMove(destination);
	}

	protected override void directMove (Vector3 destinationPoint)
	{
		
		if (destination != Vector3.zero && getPosition () != destinationPoint) {
			
			Vector3 bodyDirection = (destinationPoint - getPosition ()).normalized;

			float timeStepRot = rotationSpeed * Time.deltaTime;
			float timeStepMov = speed * Time.deltaTime;

			Vector3 newBodyDir = Vector3.RotateTowards (unitObject.transform.forward, bodyDirection, timeStepRot, 0.0f);
			
			unitObject.transform.rotation = Quaternion.LookRotation (newBodyDir);				
			unitObject.transform.position = Vector3.MoveTowards (unitObject.transform.position, destinationPoint, timeStepMov);
			
			if (Vector3.Distance (unitObject.transform.position, destinationPoint) < stopDistanceOffset) {
				movingInSection = false;
			}
		}
		
	}

	public override void attack ()
	{
		  
		if (targetUnit == null || targetUnit.isDead() || attackableTypes.Contains(targetUnit.getUnitType())==false) {
			firingState = false;
			currentState = previousState;
		} else {
			destination = targetUnit.getPosition();
			if (!isInDefenceRange()) {
				Debug.Log ("out range");
				destination = targetUnit.getPosition();
				firingState = false;		
				Move ();
				pathCalculated=true;
				if(targetUnit.getPosition()!=targetPosition)
				{
					antiLagCounter++;
					if(antiLagCounter>10)
					{
					pathCalculated = false;
						antiLagCounter=0;
					}
				}
				targetPosition = targetUnit.getPosition();
			} else
				firingState = rotateTurretToTarget ();
		}
	}

	public override void fire ()
	{
		if (firingState && lastShotTime > reloadTime) {
		
			Vector3 pos  = new Vector3(0,-0.8f,0);
		if(left==1)
				pos  = new Vector3(0,-0.8f,0);
			if(left==1)
			WeaponManager.getInstance ().addFireMissile (new AGM (unitObject.transform.position +  (unitObject.transform.right*0.8f) ,ref targetUnit));
			else 
				WeaponManager.getInstance ().addFireMissile (new AGM (unitObject.transform.position +  (-unitObject.transform.right*0.8f) ,ref targetUnit));
			lastShotTime = 0;
			left^=1;
		}
			lastShotTime += Time.deltaTime;
		
			
	}

	public void setTarget (Unit targetUnit)
	{
		this.targetUnit = targetUnit;
	}

	public void changeFiringState (bool state)
	{
		firingState = state;
	}

	protected bool rotateTurretToTarget ()
	{
		Vector3 direction = destination - unitObject.transform.position;
		direction.Normalize();
		Vector3 newDir = Vector3.RotateTowards (unitObject.transform.transform.forward, direction, Time.deltaTime, 0.0f);
		if (unitObject.transform.rotation != Quaternion.LookRotation (newDir))
		{
			unitObject.transform.rotation = Quaternion.LookRotation (newDir);
		}
		else
			return true;
		return false;
	}
	
	protected virtual void rotateBlade()
	{	
		unitObject.transform.FindChild("main_rotor").transform.RotateAround (unitObject.transform.FindChild("main_rotor").transform.position, unitObject.transform.FindChild("main_rotor").transform.TransformDirection(Vector3.up), 1000 * Time.deltaTime); 
		unitObject.transform.FindChild("tail_rotor").transform.RotateAround (unitObject.transform.FindChild("tail_rotor").transform.position, unitObject.transform.FindChild("tail_rotor").transform.TransformDirection(Vector3.forward), 1000 * Time.deltaTime); 
	}
		public override void destroyBody()
	{
		        dead = true;
		selection.SetActive(false);
        Vector3 explosionPos = unitObject.transform.position;
        ProjectMain.Destroy(GameObject.Instantiate(explosionFire, explosionPos, Quaternion.identity), 7f);
        ProjectMain.Destroy(GameObject.Instantiate(explosionSound, explosionPos, Quaternion.identity), 7f);
        ProjectMain.Destroy(unitObject, 7f);
        UnitManager.getInstance().removeSelectedDead();
        unitObject = null;
		
	}
}
