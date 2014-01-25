using System;
using UnityEngine;

	public class Apache : Helicopter
	{
	private static GameObject unitPrefab = (GameObject)Resources.Load("prefabs/Helicopter-Apache");
		public Apache (Vector3 Pos)
        : base(unitPrefab, new Vector3(Pos.x, verticalHeight , Pos.z))
		{
        speed = 5.0f;
        rotationSpeed = 3.0f;
        maxHeightDiff = 0.5f;
        reloadTime = 3.0f;
        unitDefenceRange = 50;
        unitAttackRange = 50;

        maxHealth = 50;
        health = maxHealth;
		}
	}


