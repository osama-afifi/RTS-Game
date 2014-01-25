using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class UnitManager
{
	private static UnitManager instance;
	public static List<Player> playerIndex;
	private static List<Unit> selectedUnits;
	

	public UnitManager ()
	{
		selectedUnits = new List<Unit> ();
		playerIndex = new List<Player> ();
		   
		
	}

	public static UnitManager getInstance ()
	{
		if (instance == null) {
			instance = new UnitManager ();
		}
		return instance;
	}
//    public void addPlayer()
//    {
//        Player P = new Player();
//        P.addUnit(new Vector3(50,10,50));
//		 P.addUnit(new Vector3(40,10,50));
//		
//        playerIndex.Add(P);
//    }
	public void addPlayersUnit (Unit U, int playerID)
	{
		playerIndex [playerID].addUnit (U);
	}

	public void resetSelection ()
	{
		for (int i = 0; i < selectedUnits.Count; ++i)
			selectedUnits [i].hideSelection ();
		selectedUnits.Clear ();
	}

	void addSelection (Unit U)
	{
		selectedUnits.Add (U);
		U.showSelection ();
	}

	public void orderMove (Vector3 target)
	{
		Debug.Log ("Selected Units:" + selectedUnits.Count);
		for (int i = 0; i < selectedUnits.Count; ++i)
			if (selectedUnits [i].getPlayerID () == 0) 
			{
				selectedUnits [i].currentState = UnitState.Move;
				selectedUnits [i].changeFiringState (false);
				selectedUnits [i].destination = target;
				selectedUnits [i].pathCalculated = false;
			}
	}

	public void trySelect (GameObject trg)
	{
		for (int k = 0; k < playerIndex.Count; ++k)
			for (int i = 0; i < Player.UnitTypesNumber; i++)
				for (int j = 0; j < playerIndex[k].unitIndex[i].Count; j++)
					if (trg.transform.position == playerIndex [k].unitIndex [i] [j].getPosition ()) {
						addSelection (playerIndex [k].unitIndex [i] [j]);
						break;
					}
	}

	public void Update ()
	{
		for (int i = 0; i < playerIndex.Count; ++i)
			playerIndex [i].Update ();
	}

	public void removeSelectedDead ()
	{
		for (int i = 0; i < selectedUnits.Count; ++i)
			if (selectedUnits [i].isDead ())
				selectedUnits.RemoveAt (i);
	}

	public void boxSelection (Vector3 from, Vector3 to, GameObject type)
	{
		resetSelection ();
		for (int k = 0; k < playerIndex.Count; ++k)
			for (int i = 0; i < Player.UnitTypesNumber; i++)
				for (int j = 0; j < playerIndex[k].unitIndex[i].Count; j++) {
					Unit u = playerIndex [k].unitIndex [i] [j];
					if (((u.getPosition ().x > from.x && u.getPosition ().x < to.x) || (u.getPosition ().x < from.x && u.getPosition ().x > to.x)) &&
                       ((u.getPosition ().z > from.z && u.getPosition ().z < to.z) || (u.getPosition ().z < from.z && u.getPosition ().z > to.z)))
					if (type == null || type.name == u.getUnitName ())
						addSelection (u);
				}
	}

	public void orderAttack (GameObject targetObject)
	{
		Debug.Log ("Attacking " + targetObject.name);
		Unit targetUnit = null;
		unitType targetType = getType (targetObject);
		for (int i = 1; i < playerIndex.Count; ++i)
			if(playerIndex[i].getTeam()!=0)
			if (targetUnit == null)
				for (int j = 0; j < playerIndex[i].unitIndex[(int)targetType].Count; ++j) {
					if (playerIndex [i].unitIndex [(int)targetType] [j].getModel () == targetObject) {
						targetUnit = playerIndex [i].unitIndex [(int)targetType] [j];
						break;
					}
				}
		for (int i = 0; i < selectedUnits.Count; ++i)
			if (selectedUnits [i].playerID == 0) 
			{
				selectedUnits [i].currentState = UnitState.Attack;
				selectedUnits [i].unitAttackTarget (targetUnit);
			}
	}
	
		public List<Unit> getUnitsinRange( Vector3 center , float visibleRange)
	{
		List<Unit> UnitList = new List<Unit>();
		for (int i = 0; i < playerIndex.Count; ++i)
			for (int k = 0; k < Player.UnitTypesNumber; ++k)
				for (int j = 0; j < playerIndex[i].unitIndex[k].Count; ++j)
			{
				Unit U = playerIndex [i].unitIndex [k] [j];
					if (U!=null && !U.isDead() && (Vector3.Distance(center, U.getPosition())<=visibleRange))	
						UnitList.Add(playerIndex [i].unitIndex [k] [j]);
			}						
		return UnitList;
	}

	public Unit getUnit(GameObject obj)
	{
		if(obj.name=="gun")
			obj = obj.transform.parent.gameObject;
			if(obj.name=="turret")
			obj = obj.transform.parent.gameObject;
		
		unitType targetType = getType (obj);
		for (int i = 0; i < playerIndex.Count; ++i)
				for (int j = 0; j < playerIndex[i].unitIndex[(int)targetType].Count; ++j)
					if (playerIndex [i].unitIndex [(int)targetType] [j].getModel () == obj) 
						return playerIndex [i].unitIndex [(int)targetType] [j];						
		return null;
	}
		public Unit getEnemyUnit(GameObject obj, int myPlayer)
	{
		if(obj.name=="gun")
			obj = obj.transform.parent.gameObject;
			if(obj.name=="turret")
			obj = obj.transform.parent.gameObject;
		
		unitType targetType = getType (obj);
		for (int i = 0; i < playerIndex.Count; ++i)
			if(i!=myPlayer)
				for (int j = 0; j < playerIndex[i].unitIndex[(int)targetType].Count; ++j)
					if (playerIndex [i].unitIndex [(int)targetType] [j].getModel () == obj) 
						return playerIndex [i].unitIndex [(int)targetType] [j];						
		return null;
	}
	
	private unitType getType (GameObject trg)
	{
		unitType targetType = 0;
		if (trg.name.Split ('-') [0] == "Tank")
			targetType = unitType.Tank;
		else if (trg.name.Split ('-') [0] == "Artillery")
			targetType = unitType.Artillery;
		else if (trg.name.Split ('-') [0] == "AirDefence")
			targetType = unitType.AirDefense;
		else if (trg.name.Split ('-') [0] == "Aircraft")
			targetType = unitType.Aircraft;
		else if (trg.name.Split ('-') [0] == "LightVehicle")
			targetType = unitType.LightVehicle;
		else if (trg.name.Split ('-') [0] == "Infanty")
			targetType = unitType.Infanty;
		else if (trg.name.Split ('-') [0] == "Ship")
			targetType = unitType.Ship;
		else if (trg.name.Split ('-') [0] == "Helicopter")
			targetType = unitType.Helicopter;
		
		return targetType;
	}
	    public List<Player> compAI()
    {
        return playerIndex;
    }
	public void addNewPlayer(int race, int team, int color)
	{
		Player P = new Player();
		P.setRace(race);
		P.setTeam(team);
		P.setColor(color);
		playerIndex.Add(P);
	}
	
	public void addTwoDefaultPlayers ()
	{
		addNewPlayer(0, 0, 0);
		addNewPlayer(1, 1, 1);
	}
}
