using System;
using UnityEngine;

	public class M1Abrams : Tank
	{
	    private static GameObject unitPrefab = (GameObject)Resources.Load("prefabs/Tank-M1Abrams");
		public M1Abrams (Vector3 Pos)
        : base(unitPrefab, Pos)
		{
		
        turretBody = unitObject.transform.FindChild("turret");
        tankGun = turretBody.FindChild("gun");
        speed = 3.0f;
        maxHeightDiff = 0.5f;
        turretRotationSpeedFactor = 2.0f;
        reloadTime = 3.0f;

        unitDefenceRange = 10;
        unitAttackRange = 20;

        maxHealth = 150;
        health = maxHealth;
		}
	}


