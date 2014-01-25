using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class TerrainManager
{
	static TerrainManager instance;
	static public TerrainUnit[,] Map;
	static public KeyValuePair<int,bool> [,] Heat;
	static GameObject TerrainModel;
	public static Terrain currentTerrain;
	static Skybox skyBox;
	static int NodeSize = 3; // the Distance between two Nodes
	static int heightCostFactor = 20;   // Stay Away from Heights Level
	static int maxHeightPenaltyLevel = 4;  // Stay Away from Heights Level
	static int width;
	static int height;
	static int terrainLength;
	static int terrainWidth;
	static int XNodes;
	static int YNodes;
	static int mapResolution;
	static float cleanHeatTime;
	static float maxCleanHeatTime = 100.0f;
	
	private static Point[] movableDirection = new Point[8]
    { 
	// Starts from south-west and moves clockwise.
	//even indices represent diagonal moves while odd represent hor & ver moves.
	//cost of diagonal moves is 1.414 more than hor & ver moves.
        new Point (-1, -1),
        new Point (-1, 0),
        new Point (-1, 1),
        new Point (0, 1),
        new Point (1, 1),
        new Point (1, 0),
        new Point (1, -1), 
        new Point (0, -1)
    };
	
	public static TerrainManager getInstance ()
	{
		if (instance == null)
			instance = new TerrainManager ();
		return instance;
	}

	public void Start ()
	{
		//loadTerrain();
		currentTerrain = Terrain.activeTerrain;
		mapResolution = currentTerrain.terrainData.baseMapResolution;
		terrainLength = (int)currentTerrain.terrainData.size.x;
		terrainWidth = (int)currentTerrain.terrainData.size.z;
		YNodes = terrainLength / NodeSize;
		XNodes = terrainWidth / NodeSize;
		Map = new TerrainUnit[XNodes, YNodes];
		Heat = new KeyValuePair<int,bool>[XNodes, YNodes];
		generateNodes ();
		Debug.Log ("terrainLength: " + terrainLength);
		Debug.Log ("terrainWidth: " + terrainWidth);
		Debug.Log ("YNodes: " + YNodes);
		Debug.Log ("XNodes: " + XNodes);
		//
		
	}
	
	public TerrainManager ()
	{
	}

	public static void Update ()
	{
		cleanHeatTime += Time.deltaTime;
		if(cleanHeatTime>maxCleanHeatTime)
			cleanHeat();
		
	}
	
	
	static void cleanHeat()
	{		
			for (int i = 0; i < XNodes; i++)
			for (int j = 0; j < YNodes; j++)
			Heat[i,j] = new KeyValuePair<int,bool>(0,false);
			cleanHeatTime = 0.0f;
		Debug.Log("Heat Map Cleaned");	
	}
	
	
	
	public int manhattenDistance (int x1, int y1, int x2, int y2)
	{

		return Mathf.Abs (x1 - x2) + Mathf.Abs (y1 - y2);

	}
	
	void dfs (int x, int y, int level, ref HashSet<Point> visited , float initialHeight)
	{
		Point cur = new Point (x, y);
		if (level > maxHeightPenaltyLevel || !isPointInRange (cur) || visited.Contains (cur))
			return;
		visited.Add (cur);
		
		Map [x, y].addCost ((int)((1.0f / (float)level) * initialHeight * (float)heightCostFactor));
		
		for (int i = 0; i <8; i++) {
			Point next = cur + movableDirection [i];
			dfs (next.x, next.y, level + 1, ref visited, initialHeight);
		}
	}
	
	void spreadCost (int x, int y)
	{
		HashSet<Point> visited = new  HashSet<Point> ();
		dfs (x, y, 1, ref visited ,  Map [x, y].getHeight ());
	}
	
	void generateNodes ()
	{
		for (int i = 0; i < XNodes; i++) {
			
			//Drawing Grid Lines
	//		Debug.DrawLine (new Vector3 (i * NodeSize, 0, 0), new Vector3 (i * NodeSize, 0, 256), Color.gray, 200000, false);
	//		Debug.DrawLine (new Vector3 (0, 0, i * NodeSize), new Vector3 (256, 0, i * NodeSize), Color.gray, 200000, false);
			// Tiles
	//		Debug.DrawLine (new Vector3 (i * NodeSize  + NodeSize/2 , 0, 0), new Vector3 (i * NodeSize  + NodeSize/2, 0, 256), Color.grey, 200000, false);
	//		Debug.DrawLine (new Vector3 (0, 0, i * NodeSize  + NodeSize/2 ), new Vector3 (256, 0, i * NodeSize  + NodeSize/2 ), Color.grey, 200000, false);
			
			for (int j = 0; j < YNodes; j++) {
				TerrainUnit tu = new TerrainUnit (i * NodeSize, j * NodeSize, currentTerrain.SampleHeight (new Vector3 (i * NodeSize, 0, j * NodeSize)));				
				Map [i, j] = tu;
			}
		}
		
		for (int i = 0; i < XNodes; i++)
			for (int j = 0; j < YNodes; j++) {
				TerrainUnit tu = Map [i, j];
				if (tu.Z < 0)
					tu.setObstacle (true);
				if (tu.Z > 2.0f)
					spreadCost (i, j);		
			
			}
		
		Debug.Log (XNodes * YNodes + " Nodes Created");
	}
	


	//Utility Functions
	public static int getNodeSize ()
	{
		return NodeSize;
	}

	public static float getSlope (float from_z, float to_z)
	{
		return (float)Mathf.Abs (from_z - to_z) / (float)NodeSize;
	}

	public static float getHeightDifference (float from_z, float to_z)
	{
		return Mathf.Abs (from_z - to_z);		
	}

	public static float distance3D (Vector3 from, Vector3 to)
	{
		return Mathf.Pow (((from.x - to.x) * (from.x - to.x) + (from.y - to.y) * (from.y - to.y) + (from.z - to.z) * (from.z - to.z)), (1f / 3f));
	}

	public static float distance2D (Vector3 from, Vector3 to)
	{
		return Mathf.Sqrt ((from.x - to.x) * (from.x - to.x) + (from.z - to.z) * (from.z - to.z));
	}

	public static GridNode nearestNodeToPoint (Vector3 Point, Vector3 relativePoint)
	{ 
		GridNode nearestNode = pointToNode (Point);
		Vector3 nodePosition = new Vector3 (((int)Point.x / NodeSize) * NodeSize, 0, ((int)Point.z / NodeSize) * NodeSize);
		float minimumDistance = Vector3.Distance (nodePosition, relativePoint);
		//Check the 4 Nodes around the Point
		GridNode adjacentNode = new GridNode (nearestNode.x + 1, nearestNode.y + 1);
		if (Vector3.Distance (nodeToPoint (adjacentNode), relativePoint) < minimumDistance) {
			minimumDistance = Vector3.Distance (nodeToPoint (adjacentNode), relativePoint);
			nearestNode = adjacentNode;
		}
		adjacentNode = new GridNode (nearestNode.x + 1, nearestNode.y);
		if (Vector3.Distance (nodeToPoint (adjacentNode), relativePoint) < minimumDistance) {
			minimumDistance = Vector3.Distance (nodeToPoint (adjacentNode), relativePoint);
			nearestNode = adjacentNode;
		}
		adjacentNode = new GridNode (nearestNode.x, nearestNode.y + 1);
		if (Vector3.Distance (nodeToPoint (adjacentNode), relativePoint) < minimumDistance) {
			minimumDistance = Vector3.Distance (nodeToPoint (adjacentNode), relativePoint);
			nearestNode = adjacentNode;
		}
		return nearestNode; 
	}
	
		public static GridNode nearestNodeToPoint (Vector3 Point)
	{ 
		GridNode nearestNode = pointToNode (Point);
		Vector3 nodePosition = new Vector3 (((int)Point.x / NodeSize) * NodeSize, 0, ((int)Point.z / NodeSize) * NodeSize);
		float minimumDistance = Vector3.Distance (nodePosition, Point);
		//Check the 4 Nodes around the Point
		GridNode adjacentNode = new GridNode (nearestNode.x + 1, nearestNode.y + 1);
		if (Vector3.Distance (nodeToPoint (adjacentNode), Point) < minimumDistance) {
			minimumDistance = Vector3.Distance (nodeToPoint (adjacentNode), Point);
			nearestNode = adjacentNode;
		}
		adjacentNode = new GridNode (nearestNode.x + 1, nearestNode.y);
		if (Vector3.Distance (nodeToPoint (adjacentNode), Point) < minimumDistance) {
			minimumDistance = Vector3.Distance (nodeToPoint (adjacentNode), Point);
			nearestNode = adjacentNode;
		}
		adjacentNode = new GridNode (nearestNode.x, nearestNode.y + 1);
		if (Vector3.Distance (nodeToPoint (adjacentNode), Point) < minimumDistance) {
			minimumDistance = Vector3.Distance (nodeToPoint (adjacentNode), Point);
			nearestNode = adjacentNode;
		}
		return nearestNode; 
	}
	
	public static GridNode pointToNode (Vector3 Point)
	{ 
		return new GridNode ((int)Point.x / NodeSize, (int)Point.z / NodeSize); 
	}

	public static Vector3 nodeToPoint (GridNode Node)
	{ 
		return new Vector3 (Node.x * NodeSize, 0, Node.y * NodeSize); 
	}

	public static bool isInRange (GridNode Node)
	{
		return (Node.x >= 0 && Node.y >= 0 && Node.x < XNodes && Node.y < YNodes);    
	}
		public static bool isInRange (int x, int y)
	{
		return (x >= 0 && y >= 0 && x < XNodes && y < YNodes);    
	}
	
	public static bool isPointInRange (Point p)
	{
		return (p.x >= 0 && p.y >= 0 && p.x < XNodes && p.y < YNodes);    
	}

	public static int getXAxisNodesNumber ()
	{
		return XNodes;
	}

	public static int getYAxisNodesNumber ()
	{
		return YNodes;
	}

	
	
	
// World coordinates to terrain coordinates
	public static Vector3 WorldToTerrainPosition (Vector3 worldPos)
	{
		Vector3 terrainPos = currentTerrain.transform.position;
		Vector3 sizeOfTerrain = currentTerrain.terrainData.size;
		Vector3 relativePos = worldPos - terrainPos;
		float terrainX = relativePos.x / sizeOfTerrain.x * mapResolution;
		float terrainY = relativePos.y / sizeOfTerrain.y * mapResolution;
		float terrainZ = relativePos.z / sizeOfTerrain.z * mapResolution;
		return new Vector3 (terrainX, terrainY, terrainZ);
	}		
// Terrain coordinates to world coordinates
	public static Vector3 TerrainToWorldPosition (Vector3 terrainPos)
	{
		Vector3 sizeOfTerrain = currentTerrain.terrainData.size;
		float worldX = terrainPos.x / mapResolution * sizeOfTerrain.x;
		float worldY = terrainPos.y / mapResolution * sizeOfTerrain.y;
		float worldZ = terrainPos.z / mapResolution * sizeOfTerrain.z;
		return new Vector3 (worldX, worldY, worldZ);
	}

	public static Vector3 RaycastPosition (Vector3 location)
	{
	
		Ray ray = new Ray (location + currentTerrain.terrainData.size.z * Vector3.up, Vector3.down);
		RaycastHit hit;
		Physics.Raycast (ray, out hit);
		float offset = 0;
		return hit.point + new Vector3 (0, offset, 0); 		
	
	}
	
	public static void setHeat(int x , int y ,int heat)
	{
			Heat[x,y] = new KeyValuePair<int,bool>(heat, Heat[x,y].Value);
	}
		public static void addHeat(int x , int y ,int heat)
	{
			Heat[x,y] = new KeyValuePair<int,bool>(Heat[x,y].Key+heat, Heat[x,y].Value);
	}
		public static void subtractHeat(int x , int y ,int heat)
	{
			Heat[x,y] = new KeyValuePair<int,bool>(Heat[x,y].Key-heat, Heat[x,y].Value);
	}
			public static void setHottness(int x , int y ,bool hottness)
	{
			Heat[x,y] = new KeyValuePair<int,bool>(Heat[x,y].Key, hottness);
	}

		public static int getHeat(int x , int y)
	{
		return Heat[x,y].Key;
	}
			public static bool isHot(int x , int y)
	{
		return Heat[x,y].Value;
	}
	public void clearHeat()
	{
		Array.Clear(Heat, 0, Heat.Length);
	}
	
	void loadTerrain ()
	{
		//Load the Terrain from a prefab currently we are using a ready Terrain named "Terrain"
	}

}

