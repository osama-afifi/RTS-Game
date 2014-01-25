using System;
using UnityEngine;

	public class Merkava4 : Tank
	{
	private static GameObject unitPrefab = (GameObject)Resources.Load("prefabs/Tank-Merkava4");
		public Merkava4 (Vector3 Pos)
        : base(unitPrefab, Pos)
		{
        turretBody = unitObject.transform.FindChild("turret");
        tankGun = turretBody.FindChild("gun");
        speed = 5.0f;

        maxHeightDiff = 0.5f;
        turretRotationSpeedFactor = 2.0f;
        reloadTime = 3.0f;

        unitDefenceRange = 15;
        unitAttackRange = 25;

        maxHealth = 200;
        health = maxHealth;
		}
	}


