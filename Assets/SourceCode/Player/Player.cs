using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum unitType { Infanty, Tank, AirDefense, Artillery, Helicopter, LightVehicle , Aircraft, Ship};

class Player
{
    public static int PlayerNumbers = 0;
    public int id; // Players Unique ID
    public int race; // Player's Race 1. Egyptian,  2.Israeli , 3. American,  etc..
	public int team;
    public int color; //Player's Color on Unit Model
	public static int UnitTypesNumber = 8; // No. of Unit Typesw
    public List<Unit>[] unitIndex = new List<Unit>[UnitTypesNumber];
    

    public Player()
    {
        for (int i = 0; i < UnitTypesNumber; i++)
            unitIndex[i] = new List<Unit>();
        id = PlayerNumbers++;
    }
    public void Update()
    {
        for (int i = 0; i < UnitTypesNumber; ++i)
            for (int j = 0; j < unitIndex[i].Count ; ++j)
         	  if (!unitIndex[i][j].isDead())
                    unitIndex[i][j].Update();
                else
                {
                    Debug.Log("removing");
                    unitIndex[i].RemoveAt(j);
                }
    }
    public void addUnit(Unit U)
    {
		        U.playerID = id;
        unitIndex[(int)U.getUnitType()].Add(U);
    }
	public int getRace()
	{
		return race;
	}
	public int getTeam()
	{
		return team;
	}
			public int getColor()
	{
		return color;
	}
	public void setRace(int race)
	{
		this.race = race;		
	}
		public void setTeam(int team)
	{
		this.team = team;		
	}
		public void setColor(int color)
	{
		this.color = color;		
	}
}
