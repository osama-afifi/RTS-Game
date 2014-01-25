using System;
using UnityEngine;

	public class Humvee : LightVehicle
	{
		private static GameObject unitPrefab = (GameObject)Resources.Load("prefabs/LightVehicle-Humvee");
		public Humvee (Vector3 Pos)
        : base(unitPrefab, Pos)
		{
		
		turretBody = unitObject.transform.FindChild("turret");
		stopDistanceOffset = 2.5f;
		speed = 5.0f;
		rotationSpeed = 3.0f;
		maxHeightDiff = 0.5f;
		turretRotationSpeedFactor = 2.0f;
		reloadTime = 5.0f;
		
		        maxHealth = 50;
        health = maxHealth;
		
		}
	}


