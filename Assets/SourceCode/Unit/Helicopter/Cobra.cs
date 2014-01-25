using System;
using UnityEngine;

public class Cobra : Helicopter
{
	private static GameObject unitPrefab = (GameObject)Resources.Load ("prefabs/Helicopter-AH1Cobra");

	public Cobra (Vector3 Pos)
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
		
	protected override void rotateBlade ()
	{	
		if(unitObject==null)return;
		unitObject.transform.FindChild ("main_rotor").transform.RotateAround (unitObject.transform.FindChild ("main_rotor").transform.position, unitObject.transform.FindChild ("main_rotor").transform.TransformDirection (Vector3.up), 1000 * Time.deltaTime); 
		unitObject.transform.FindChild ("tail_rotor").transform.RotateAround (unitObject.transform.FindChild ("tail_rotor").transform.position, unitObject.transform.FindChild ("tail_rotor").transform.TransformDirection (Vector3.right), 1000 * Time.deltaTime); 
	}
}


