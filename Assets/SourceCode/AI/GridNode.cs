using UnityEngine;
using System;
using System.Collections;

class GridNode : IComparable<GridNode> , IEquatable<GridNode>
{
	public int x, y;
	public int g, h, f;
	public static GridNode goalNode;
	public GridNode ()
	{
		x = y = g = h = f = 0;
	}

	public GridNode (int x, int y)
	{
		this.x = x;
		this.y = y;
		this.g = 0;
		this.h = 0;
	}

	public GridNode (int x, int y, int g, int h)
	{
		this.x = x;
		this.y = y;
		this.g = g;
		this.h = h;
	}
	
	# region IEquatable
	   public bool Equals(GridNode other) 
   {
      if (other == null) 
         return false;
      if (this.x == other.x && this.y==other.y)
         return true;
      else 
         return false;
   }
	  
		  public override bool Equals(object obj)
   {
      if (obj == null) 
         return false;

      GridNode p = obj as GridNode;
      if (p == null)
         return false;
      else    
         return Equals(p);   
   }   

   public static bool operator == (GridNode p1, GridNode p2)
   {
      if ((object)p1 == null || ((object)p2) == null)
         return object.Equals(p1, p2);

      return p1.Equals(p2);
   }

   public static bool operator != (GridNode p1, GridNode p2)
   {
      if (p1 == null || p2 == null)
         return ! object.Equals(p1, p2);

      return ! (p1.Equals(p2));
   }
	
	# endregion
	
	public bool hasReached()
	{
		return (x==goalNode.x && y==goalNode.y);		
	}
	
	public int getCost ()
	{
		return f = h + g;
	}
	
	public static GridNode operator + (GridNode g1, GridNode g2)
	{
		return new GridNode (g1.x + g2.x, g1.y + g2.y , g1.g+g2.g , 0);
	}


	public int CompareTo (GridNode x)
	{
		if (f < x.f)
			return -1;
		else if (f > x.f)
			return 1;
		else
			return 0;
	}
	public void setHeuristic()
	{
		h =  manhattenHeuristic();
		f = g+h;
	}
	public void addPenaltyValue(int cost)
	{
		h+=cost;
		f=g+h;
	}
	
		public void addDistanceValue(int cost)
	{
		g+=cost;
		f=g+h;
	}
	
	
	public int manhattenHeuristic ()
	{
	//	return 0;
		
		int xDistance = Mathf.Abs (x - goalNode.x);
		int yDistance = Mathf.Abs (y - goalNode.y);
		if (xDistance > yDistance)
			return 14 * yDistance + 10 * (xDistance - yDistance);
		else
			return 14 * xDistance + 10 * (yDistance - xDistance);
	}
	
	public int getHeat()
	{
		return TerrainManager.getHeat(x, y);
	}
	
	public Point toPoint()
	{
		return new Point(x,y);		
	}
		
	public Vector3 toVector3 ()
	{
		return new Vector3 (this.x * TerrainManager.getNodeSize (), 0, this.y * TerrainManager.getNodeSize ());	
	}
}