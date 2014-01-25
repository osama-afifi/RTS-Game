using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

enum TerrainType
{
	Land,
	Sea,
	Rocky
}

class TerrainUnit
{
	public float X, Y, Z;
	bool obstacle = false;
	int cost;
	
	public TerrainUnit (float X, float Y, float Z)
	{
		this.X = X;
		this.Y = Y;
		this.Z = Z;
		obstacle = false;
		cost = 0;
	}

	public TerrainUnit (Vector3 Position)
	{
		this.X = Position.x;
		this.Y = Position.z;
		this.Z = Position.y;
		obstacle = false;
		cost = 0;
	}

	public void setHeight (float Z)
	{
		this.Z = Z;
	}

	public float getHeight ()
	{
		return Z;
	}

	public void setCost (int cost)
	{
		this.cost = cost;
	}
	
		public void addCost (int cost)
	{
		this.cost += cost;
	}


	public int getCost ()
	{
		return cost;
	}

	public bool isObstacle ()
	{
		return obstacle;
	}

	public void setObstacle (bool obstacle)
	{
		this.obstacle = obstacle;
	}
	
	public Vector3 getPosition ()
	{
		return new Vector3 (X, Z, Y);		
	}
	

	
}
