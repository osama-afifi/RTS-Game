using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class LightVehicle : Unit
{
	protected Transform turretBody;
	
	protected float turretRotationSpeedFactor = 2.0f;
	float lastShotTime = 0.0f;
	protected float reloadTime = 5.0f;
	Vector3 targetPosition;
	protected  GameObject explosionSound = (GameObject)Resources.Load("TankExpFireSound");
    protected  GameObject explosionFire = (GameObject)Resources.Load("TankFire");
	
	public LightVehicle (GameObject Obj, Vector3 Pos)
        : base(Obj, Pos)
	{
		//unitObject = (GameObject)Resources.Load ("prefabs/Tank");
		attackableTypes = new List<unitType>() {unitType.Tank , unitType.Artillery , unitType.AirDefense , unitType.Infanty, unitType.Ship, unitType.LightVehicle};
		currentState = UnitState.Check;
		movingInSection = false;
		pathCalculated = false;
		
		//Intialize constants
		stopDistanceOffset = 2.5f;
		speed = 5.0f;
		rotationSpeed = 3.0f;
		maxHeightDiff = 0.5f;
		slopeVariationFactor = 20;
		unitAttackRange = 15.0f;
		firingState = false;
		type = unitType.LightVehicle;
	}

	public override void Update ()
	{
base.Update();
	}

	public override void Move () // Motion using Pathfinding
	{
base.Move();
	}

	protected override void directMove (Vector3 destinationPoint)
	{
		
		if (destination != Vector3.zero && getPosition () != destinationPoint) {
			
			Vector3 bodyDirection = (destinationPoint - getPosition ()).normalized;
			Vector3 turretDirection = (destination - getPosition ()).normalized;

			float timeStepRot = rotationSpeed * Time.deltaTime;
			float timeStepMov = speed * Time.deltaTime;
			
			bodyDirection = new Vector3(bodyDirection.x,0,bodyDirection.z);
			turretDirection = new Vector3(turretDirection.x,0,turretDirection.z);
			
			Vector3 newBodyDir = Vector3.RotateTowards (unitObject.transform.forward, bodyDirection, timeStepRot, 0.0f);
			Vector3 newTurretDir = Vector3.RotateTowards (unitObject.transform.FindChild ("turret").transform.forward, turretDirection, turretRotationSpeedFactor * Time.deltaTime, 0.0f);
			
			newBodyDir = new Vector3(newBodyDir.x,0,newBodyDir.z);
			newTurretDir = new Vector3 (newTurretDir.x, newTurretDir.y, newTurretDir.z);
			
			unitObject.transform.rotation = Quaternion.LookRotation (newBodyDir);
			unitObject.transform.FindChild ("turret").transform.rotation = Quaternion.LookRotation (newTurretDir);
			
	
			if (unitObject.transform.rotation == Quaternion.LookRotation (newBodyDir))
			unitObject.transform.position = Vector3.MoveTowards (unitObject.transform.position, destinationPoint, timeStepMov);
			
			if (Vector3.Distance (unitObject.transform.position, destinationPoint) < stopDistanceOffset) {
				movingInSection = false;
			}
		}
		
	}

	public override void attack ()
	{
	
	if (targetUnit == null || targetUnit.unitObject == null || targetUnit.isDead() || attackableTypes.Contains(targetUnit.getUnitType())==false) {
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
                if (targetUnit.getPosition() != targetPosition)
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
			WeaponManager.getInstance ().addFireMissile (new TankProjectile (turretBody.transform.position+turretBody.transform.forward*1.5f, targetUnit.getPosition() + new Vector3(0,0.75f,0)));
			lastShotTime = 0;
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

	private bool rotateTurretToTarget ()
	{
		Vector3 direction = destination - unitObject.transform.FindChild ("turret").position;
		direction.Normalize();
		//	direction = new Vector3(0,direction.y,0);
		Vector3 newDir = Vector3.RotateTowards (unitObject.transform.FindChild ("turret").transform.forward, direction, turretRotationSpeedFactor * Time.deltaTime, 0.0f);
		//	newDir = new Vector3(0,newDir.y,0);
		if (unitObject.transform.FindChild ("turret").rotation != Quaternion.LookRotation (newDir))
		{
			unitObject.transform.FindChild ("turret").rotation = Quaternion.LookRotation (newDir);
		}
		else
			return true;
		return false;
	}
		public override void destroyBody()
	{
		        dead = true;
		selection.SetActive(false);
        Vector3 explosionPos = unitObject.transform.position;
        ProjectMain.Destroy(GameObject.Instantiate(explosionFire, explosionPos, Quaternion.identity), 4f);
        ProjectMain.Destroy(GameObject.Instantiate(explosionSound, explosionPos, Quaternion.identity), 4f);
        ProjectMain.Destroy(unitObject, 4f);
        UnitManager.getInstance().removeSelectedDead();
        unitObject = null;
	}
}
