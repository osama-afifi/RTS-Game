using System;
using UnityEngine;

	public class M109 : Artillery
	{
	private static GameObject unitPrefab = (GameObject)Resources.Load("prefabs/Artillery-M109");
		public M109 (Vector3 Pos)
        : base(unitPrefab, Pos)
		{
		turretBody = unitObject.transform.FindChild("turret");
		tankGun = turretBody.FindChild("gun");
		speed = 2.0f;
		rotationSpeed = 3.0f;
		maxHeightDiff = 0.5f;
		slopeVariationFactor = 20;
		unitAttackRange = 70.0f;
		unitDefenceRange = 50.0f;
		        maxHealth = 100;
        health = maxHealth;
		}
	}


