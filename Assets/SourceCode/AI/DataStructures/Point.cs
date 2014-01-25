using System;
using UnityEngine;

	public class Point : IEquatable<Point>
	{
		public int x, y;

		public Point ()
		{
			x = y = 0;
		}
	
		public Point (int X, int Y)
		{
			x = X;
			y = Y;		
		}

		public Vector3 toVector3 ()
		{
			return new Vector3 (x * TerrainManager.getNodeSize (), 0 , y * TerrainManager.getNodeSize ());
		}
	
		public static Point operator + (Point g1, Point g2)
	{
		return new Point (g1.x + g2.x, g1.y + g2.y);
	}
	
	
	#region IEquable
   public bool Equals(Point other) 
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

      Point p = obj as Point;
      if (p == null)
         return false;
      else    
         return Equals(p);   
   }   

   public static bool operator == (Point p1, Point p2)
   {
      if ((object)p1 == null || ((object)p2) == null)
         return object.Equals(p1, p2);

      return p1.Equals(p2);
   }

   public static bool operator != (Point p1, Point p2)
   {
      if (p1 == null || p2 == null)
         return ! object.Equals(p1, p2);

      return ! (p1.Equals(p2));
   }
	   public override int GetHashCode()
    {
        return x * y + y;
    }
	
	#endregion
}

