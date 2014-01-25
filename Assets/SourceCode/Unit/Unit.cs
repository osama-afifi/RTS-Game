using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum UnitState
{
	Check,
	Attack,
	Move
};

public abstract class Unit
{
	public GameObject unitObject;
	public GameObject selection;
	protected Unit targetUnit;
	public int playerID;
	public UnitState currentState;
	public Vector3 destination;
	public bool pathCalculated;
	protected float stopDistanceOffset;
	protected int health;
	protected int maxHealth;
	protected  float speed;
	protected  float rotationSpeed;
	protected  float maxHeightDiff;
	protected  int slopeVariationFactor;
	protected  float unitAttackRange;
	protected float unitDefenceRange;
	protected bool movingInSection;
	protected int currentNodeCount;
	protected List<Vector3> pathList;
	protected List<Unit> enemiesInRange;
	protected bool firingState;
	protected bool dead = false;
	protected bool wait = false;
	protected unitType type;
	public List<unitType> attackableTypes;
	protected int antiLagCounter;
	protected float healthScale;
	protected float antiWaitFreeze;
	protected float minWaitingTime ;
	protected float maxWaitingTime;
	protected float checkUnitsTime;
	protected float maxCheckUnitsTime;
	protected float collisionAvoidanceDistance;
	protected UnitState previousState;
	protected int waitCounter;

	public Unit (GameObject Obj, Vector3 Pos)
	{	
		unitObject = (GameObject)GameObject.Instantiate (Obj, Pos, Quaternion.identity);
		enemiesInRange = new List<Unit> ();
		pathList = new List<Vector3> ();
		selection = unitObject.transform.FindChild ("HealthBar").gameObject;
		antiLagCounter = 0;
		selection.SetActive (false);
		currentState = UnitState.Check;
		movingInSection = false;
		pathCalculated = false;
		healthScale = unitObject.transform.FindChild ("HealthBar").FindChild ("GreenBar").transform.localScale.x;
		wait = false;
		antiWaitFreeze = 0.0f;
		minWaitingTime = 0.1f;
		maxWaitingTime = 2.5f;
		checkUnitsTime = 0.0f;
		maxCheckUnitsTime = 1.0f;
		collisionAvoidanceDistance = 2.5f;
		waitCounter = 0;
		addHeat ();
	}

	public virtual void Update ()
	{
		switch (currentState) {
		case UnitState.Check:
			{
				unitObject.particleSystem.enableEmission = false;
				check ();
				selectEnemy ();
				break;
			}
		case UnitState.Attack:
			{
				attack ();
				break;
			}
		case UnitState.Move:
			{
				check ();
				selectEnemy ();
				Move ();
				break;
			}
		}
		
		if (firingState)
			fire ();
 
		checkNearbyHits ();
		
	}

	public virtual void Move () // Motion using Pathfinding
	{
		try {
			checkForwardSpace (); 
			if (wait) {
				unitObject.particleSystem.enableEmission = false;
				return;
			}
			unitObject.particleSystem.enableEmission = true;			
			Unit u = (Unit)this;

			if (!pathCalculated) {
				cleanPathHeat (); // remove heat of previous path
				removeHeat (); // remove the Unit Heat
				pathList = new List<Vector3> ();
				AStar.getInstance ().doAStar (ref pathList, getPosition (), destination, ref u);
				pathCalculated = true;
				movingInSection = true;
				currentNodeCount = 0;
	
				makePathHeat (); // add heat of current path
			}
			if (pathList.Count != 0) { //Path Found
				if (movingInSection) {
					directMove (pathList [pathList.Count - 1 - currentNodeCount] + 1.2f*Vector3.up);
				} else {
					currentNodeCount++;
					movingInSection = true;			
					if (currentNodeCount == pathList.Count) {
						currentState = UnitState.Check;
						cleanPathHeat ();
						addHeat (); // add Heat around Unit Position
						movingInSection = false;
						pathCalculated = false;	
					}
				}
			} else {
				Debug.Log ("Not Found");
				currentState = UnitState.Check;
			}
		} catch (Exception e) {
			Debug.Log ("Error in Unit Motion: " + e.ToString ());
			currentState = UnitState.Check;
			pathCalculated = false;	
			
		}
	}

	protected virtual void directMove (Vector3 destinationPoint)
	{
		
		if (destination != Vector3.zero && getPosition () != destinationPoint) {
			
			Vector3 bodyDirection = (destinationPoint - getPosition ()).normalized;
			float timeStepRot = rotationSpeed * Time.deltaTime;
			float timeStepMov = speed * Time.deltaTime;		
			bodyDirection = new Vector3 (bodyDirection.x, 0, bodyDirection.z);	
			Vector3 newBodyDir = Vector3.RotateTowards (unitObject.transform.forward, bodyDirection, timeStepRot, 0.0f);
			newBodyDir = new Vector3 (newBodyDir.x, 0, newBodyDir.z);
			unitObject.transform.rotation = Quaternion.LookRotation (newBodyDir);
			if (unitObject.transform.rotation == Quaternion.LookRotation (newBodyDir))
				unitObject.transform.position = Vector3.MoveTowards (unitObject.transform.position, destinationPoint, timeStepMov);
			if (Vector3.Distance (unitObject.transform.position, destinationPoint) < stopDistanceOffset) {
				movingInSection = false;
			}
		}
		
	}

	public virtual void attack ()
	{
	}

	public virtual void fire ()
	{
	}

	public virtual bool isInAttackRange (Unit target)
	{
		return Vector3.Distance (target.getPosition (), unitObject.transform.position) < unitAttackRange;
	}

	public virtual bool isInDefenceRange ()
	{
		if (!(Vector3.Distance (targetUnit.getPosition (), unitObject.transform.position) < unitAttackRange))
			return false;
		return true;
	}

	public string getUnitName ()
	{
		return unitObject.name;
	}

	public unitType getUnitType ()
	{ 
		return type;
	}

	public void hideSelection ()
	{
		selection.SetActive (false);
	}

	public void showSelection ()
	{
		selection.SetActive (true);
	}

	public Vector3 getPosition ()
	{
		return unitObject.transform.position;
	}

	public bool hasTarget ()
	{
		return targetUnit != null;
	}

	public float getMaxHeightDiff ()
	{
		return maxHeightDiff;
	}

	public int getSlopeVariationFactor ()
	{
		return slopeVariationFactor;		
	}

	public void unitAttackTarget (Unit targetUnit)
	{
		this.targetUnit = targetUnit;
	}

	public void changeFiringState (bool state)
	{
		firingState = state;
	}

	public int getPlayerID ()
	{
		return playerID;
	}

	public GameObject getModel ()
	{
		return unitObject;
	}

	public virtual void changeHealth (int decrease)
	{
		health += decrease;
		if (health > maxHealth)
			health = maxHealth;
		if (health <= 0) {
			health = 0;
			destroyBody ();
			return;
		}
		float ratio = (float)health / maxHealth;
		Vector3 scl = unitObject.transform.FindChild ("HealthBar").FindChild ("GreenBar").transform.localScale;
		unitObject.transform.FindChild ("HealthBar").FindChild ("GreenBar").transform.localScale = new Vector3 (ratio * healthScale, scl.y, scl.z);
	}

	public void checkNearbyHits ()
	{
		foreach (Weapon w in WeaponManager.currentExplosions)
			if (Vector3.Distance (getPosition (), w.getExplodedPosition ()) <= w.getEffectRange ())
				changeHealth (-w.getDamage (type));		
	}

	public virtual void destroyBody ()
	{
	}

	public bool isDead ()
	{
		return dead;
	}

	public Point getCurrentNodeCoordinate ()
	{
		GridNode G = TerrainManager.nearestNodeToPoint (getPosition ());
		return new Point (G.x, G.y);
	}

	public virtual void addHeat ()
	{
		Point G = getCurrentNodeCoordinate ();
		TerrainManager.setHottness(G.x, G.y, true);
		TerrainManager.addHeat (G.x, G.y, 1000000);
		
		TerrainManager.addHeat (G.x, G.y + 1, 5000);
		TerrainManager.addHeat (G.x + 1, G.y, 5000);
		TerrainManager.addHeat (G.x + 1, G.y + 1, 5000);
		TerrainManager.addHeat (G.x, G.y - 1, 5000);
		TerrainManager.addHeat (G.x - 1, G.y, 5000);
		TerrainManager.addHeat (G.x - 1, G.y - 1, 5000);
		TerrainManager.addHeat (G.x + 1, G.y - 1, 5000);
		
	
	}

	protected void makePathHeat ()
	{
		for (int i = 0; i<pathList.Count; i++) {
			GridNode G = TerrainManager.nearestNodeToPoint (pathList [i]);
			if (TerrainManager.isInRange (G.x, G.y))
				TerrainManager.addHeat (G.x, G.y, 2);
		}
	}

	protected void cleanPathHeat ()
	{
		for (int i = 0; i<pathList.Count; i++) {
			GridNode G = TerrainManager.nearestNodeToPoint (pathList [i]);
			if (TerrainManager.isInRange (G.x, G.y))		
				TerrainManager.subtractHeat (G.x, G.y, 2);
		}
	}
		
	public virtual void removeHeat ()
	{
		Point G = getCurrentNodeCoordinate ();
		TerrainManager.setHottness(G.x, G.y, false);
		TerrainManager.setHeat (G.x, G.y, 0);	
		TerrainManager.setHeat (G.x, G.y + 1, 0);
		TerrainManager.setHeat (G.x + 1, G.y, 0);
		TerrainManager.setHeat (G.x + 1, G.y + 1, 0);
		TerrainManager.setHeat (G.x, G.y - 1, 0);
		TerrainManager.setHeat (G.x - 1, G.y, 0);
		TerrainManager.setHeat (G.x - 1, G.y - 1, 0);
		TerrainManager.setHeat (G.x + 1, G.y - 1, 0);
		TerrainManager.setHeat (G.x - 1, G.y + 1, 0);
		
	}
	
	public int getHealth ()
	{
		return health;
	}

	protected bool rotateToDestination ()
	{
		
		Vector3 direction = destination - getPosition ();
		direction.Normalize ();
		
		Vector3 newDir = Vector3.RotateTowards (unitObject.transform.forward, direction, rotationSpeed * Time.deltaTime, 0.0f);
		
		if (unitObject.transform.rotation != Quaternion.LookRotation (newDir)) {
			unitObject.transform.rotation = Quaternion.LookRotation (newDir);
		} else
			return true;
		return false;
	}

//	public virtual void checkForwardSpace ()
//	{
//	
//		Collider [] listOfObj;	
//		listOfObj = Physics.OverlapSphere (getPosition () + unitObject.transform.forward * 2.5f, collisionAvoidanceDistance);				
//		if (wait) {
//			
//			antiWaitFreeze += Time.deltaTime;
//			if (antiWaitFreeze <= minWaitingTime)
//				return;
//			if (antiWaitFreeze >= maxWaitingTime) {
//				antiWaitFreeze = 0.0f;
//				waitCounter++;
//				if (waitCounter >= 2) {
//					rotateToDestination ();
//				}	
//				pathCalculated = false;
//				wait = false;
//				return;
//			}
//		}
//		
//		foreach (Collider col in listOfObj) {
//
//			if (col.gameObject.name == "Terrain" || unitObject == col.gameObject || col.gameObject.tag == "Weapon")
//				continue;
//
//			Unit U = UnitManager.getInstance ().getUnit (col.gameObject);
//			if (U == null || U.getModel () == unitObject)
//				continue;
//			if (U.currentState == UnitState.Move) {
//	
//				wait = true;
//				waitCounter = 0;
//				return;
//			} else if (U.currentState == UnitState.Check && U.destination == destination) {
//				wait = false;
//				U.addHeat ();
//				currentState = UnitState.Check;
//				pathCalculated = false;
//			} else if (U.currentState == UnitState.Check) {
//				wait = false;
//				U.addHeat ();
//				antiLagCounter++;
//				if (antiLagCounter > 50) {
//					pathCalculated = false;
//					antiLagCounter = 0;
//				}
//	
//			} else if (U.currentState == UnitState.Attack && U.destination == destination) {
//				wait = false;
//				U.addHeat ();
//				U.addHeat ();
//				pathCalculated = false;
//				movingInSection = false;
//				
//	
//			}
//
//			wait = false;
//		}
//		wait = false;
//	}
	
		public virtual void checkForwardSpace ()
	{
	
		List<Unit> unitList = new List<Unit>();
		unitList = UnitManager.getInstance().getUnitsinRange(getPosition () + unitObject.transform.forward * 1.5f, collisionAvoidanceDistance);
		
		if (wait) {
			
			antiWaitFreeze += Time.deltaTime;
			if (antiWaitFreeze <= minWaitingTime)
				return;
			if (antiWaitFreeze >= maxWaitingTime) {
				antiWaitFreeze = 0.0f;
				waitCounter++;
				if (waitCounter >= 10) {
					rotateToDestination ();
				}	
				pathCalculated = false;
				wait = false;
				return;
			}
		}
		
		foreach (Unit U in unitList) {

			if (U == null || U.getModel () == unitObject)
				continue;
			if (U.currentState == UnitState.Move) {
	
				wait = true;
				waitCounter = 0;
				return;
			} else if (U.currentState == UnitState.Check && U.destination == destination) {
				wait = false;
				U.addHeat ();
				currentState = UnitState.Check;
				pathCalculated = false;
			} else if (U.currentState == UnitState.Check) {
				wait = false;
				U.addHeat ();
				antiLagCounter++;
				if (antiLagCounter > 50) {
					pathCalculated = false;
					antiLagCounter = 0;
				}
	
			} else if (U.currentState == UnitState.Attack && U.destination == destination) {
				wait = false;
				U.addHeat ();
				U.addHeat ();
				pathCalculated = false;
				movingInSection = false;
				
	
			}

			wait = false;
		}
		wait = false;
	}
	
	public virtual void check ()
	{
		checkUnitsTime += Time.deltaTime;
		if (checkUnitsTime >= maxCheckUnitsTime) {
			enemiesInRange.Clear ();
			checkUnitsTime = 0.0f;
			for (int i = 0; i<UnitManager.playerIndex.Count; i++)
				if (i != playerID)
					for (int j = 0; j<attackableTypes.Count; j++)
						for (int k = 0; k < UnitManager.playerIndex[i].unitIndex[(int)attackableTypes[j]].Count; ++k) {
							if (UnitManager.playerIndex [i].unitIndex [(int)attackableTypes [j]] [k] != null)
							if (!UnitManager.playerIndex [i].unitIndex [(int)attackableTypes [j]] [k].isDead ())
							if (isInAttackRange (UnitManager.playerIndex [i].unitIndex [(int)attackableTypes [j]] [k]) == true)
								enemiesInRange.Add (UnitManager.playerIndex [i].unitIndex [(int)attackableTypes [j]] [k]);
						}			
		}
	}
		
//		public virtual void check ()
//	{
//		checkUnitsTime += Time.deltaTime;
//		if(checkUnitsTime>=maxCheckUnitsTime)
//		{
//			enemiesInRange.Clear();
//			checkUnitsTime=0.0f;
//		Collider [] listOfObj;	
//		listOfObj = Physics.OverlapSphere (getPosition (), unitAttackRange);
//			
//			foreach (Collider col in listOfObj)
//			{
//			if (col.gameObject.name == "Terrain" || unitObject == col.gameObject || col.gameObject.tag == "Weapon")
//				continue;
//				Unit U = UnitManager.getInstance ().getEnemyUnit (col.gameObject , playerID);
//				if (U == null || U.getModel () == unitObject || attackableTypes.Contains(U.type)==false )
//					continue;
//				enemiesInRange.Add(U);	
//			}		
//		}
//
//	}
	public void selectEnemy ()
	{
		foreach (Unit enemy in enemiesInRange) {
			previousState = currentState;
			currentState = UnitState.Attack;
			unitAttackTarget (enemy);
			return;
		}
		
	}
	
}
