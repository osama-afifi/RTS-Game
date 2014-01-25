using System;
using UnityEngine;
using System.Collections.Generic;

	public class Avenger : AntiAircraft
	{
	 private static GameObject unitPrefab = (GameObject)Resources.Load("prefabs/AirDefence-Avenger");
		public Avenger (Vector3 Pos)
        : base(unitPrefab, Pos)
		{

		stopDistanceOffset = 2.5f;
		speed = 5.0f;
		rotationSpeed = 2.0f;
		maxHeightDiff = 0.5f;
		turretRotationSpeedFactor = 150.0f;
		reloadTime = 5.0f;
		unitAttackRange = 70.0f;
		maxHealth = 50;
        health = maxHealth;
		}

	public override void Move () // Motion using Pathfinding
	{
		try {
			Unit u = (Unit)this;

			if (!pathCalculated) {
				pathList = new List<Vector3> ();
				AStar.getInstance ().doAStar (ref pathList, getPosition (), destination, ref u);
				pathCalculated = true;
				movingInSection = true;
				currentNodeCount = 0;
			}
			if (pathList.Count != 0) { //Path Found
				if (movingInSection) {
					directMove (pathList [pathList.Count - 1 - currentNodeCount]);
				} else {
					currentNodeCount++;
					movingInSection = true;			
					if (currentNodeCount == pathList.Count) {
						currentState = UnitState.Check;
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
}