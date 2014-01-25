using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Com.Mission_Base.Pbl;

class AStar
{
	private static AStar instance;
	private bool searchLimit = false;
	private int searchLimitValue; //Limit the Search of the Nodes
	
	private bool PunishChangeDirection = false;
	private int PunishChangeDirectionValue = 0; // less likely to change directions too much
	
	private bool HeavyDiagonals = false;
	private int HeavyDiagonalValue = 0;
	
	private  GridNode goalNode;
	private static GridNode[] movableNodes = new GridNode[8]
    { 
	// Starts from south-west and moves clockwise.
	//even indices represent diagonal moves while odd represent hor & ver moves.
	//cost of diagonal moves is 1.414 more than hor & ver moves.
        new GridNode (-1, -1, 14, 0),
        new GridNode (-1, 0, 10, 0),
        new GridNode (-1, 1, 14, 0),
        new GridNode (0, 1, 10, 0),
        new GridNode (1, 1, 14, 0),
        new GridNode (1, 0, 10, 0),
        new GridNode (1, -1, 14, 0), 
        new GridNode (0, -1, 10, 0)
    };

	AStar ()
	{
	}

	public static AStar getInstance ()
	{
		if (instance == null) {
			instance = new AStar ();
		}
		return instance;
	}
	
	void setSearchLimit (int limit)
	{
		searchLimitValue = limit;
	}
	
	public bool doAStar (ref List<Vector3> pathVectorList, Vector3 sourcePoint, Vector3 destinationPoint, ref Unit unit)
	{
		try {
			
			Dictionary<Point,int> fromDirection = new Dictionary<Point,int> ();
			Dictionary<Point,int> EstimatedValue = new Dictionary<Point,int> ();
			PriorityQueue<GridNode> openList = new PriorityQueue<GridNode> ();
			
			GridNode sourceNode = TerrainManager.nearestNodeToPoint (sourcePoint, destinationPoint);
			GridNode destinationNode = TerrainManager.nearestNodeToPoint (destinationPoint);
			goalNode = destinationNode;
			GridNode.goalNode = destinationNode;
			sourceNode.setHeuristic ();
			openList.Enqueue (sourceNode);
			GridNode currentNode;
			bool found = false; // default the path not found
			
			while (openList.Count > 0) { 
				currentNode = openList.Dequeue ();
				TerrainUnit currentTerrainUnit = TerrainManager.Map [currentNode.x, currentNode.y]; 
				if (currentNode.x == destinationNode.x && currentNode.y == destinationNode.y) {
					found = true;
					break;
				}
				EstimatedValue [currentNode.toPoint ()] = currentNode.g;
			
				for (int i = 0; i <8; i++) {
					GridNode nextNode = currentNode + movableNodes [i]; // g calculated by adding
				
					if (TerrainManager.isInRange (nextNode)) { // Check that it's int the Terrain Map Dimensions
						TerrainUnit nextTerrainUnit = TerrainManager.Map [nextNode.x, nextNode.y];
						if (!nextTerrainUnit.isObstacle ()) { // not obctacle i.e. walkable
							float heightDiff = TerrainManager.getHeightDifference (currentTerrainUnit.getHeight (), nextTerrainUnit.getHeight ());
							if (heightDiff <= unit.getMaxHeightDiff ()) {  // max Height Diff a unit can pass

								bool inClosedList = EstimatedValue.ContainsKey (nextNode.toPoint ());
					
								if (inClosedList) {
									if (nextNode.g >= EstimatedValue [nextNode.toPoint ()])
										continue;
								}
								if (!inClosedList || (inClosedList && nextNode.g < EstimatedValue [nextNode.toPoint ()])) {
									int slopeCost = (int)(heightDiff * (float)unit.getSlopeVariationFactor ()); 
									int changeDirectionCost;	
									if (currentNode.x == sourceNode.x && currentNode.y == sourceNode.y)
										changeDirectionCost = 0;
									else
										changeDirectionCost = 4-Mathf.Min((Math.Abs (fromDirection [currentNode.toPoint ()] - i)),7-(Math.Abs (fromDirection [currentNode.toPoint ()] - i))) * PunishChangeDirectionValue;
									nextNode.setHeuristic ();
									nextNode.addPenaltyValue (slopeCost); // Slope Penalty
								//	nextNode.addPenaltyValue (changeDirectionCost); // Change Direction Penalty
									//nextNode.addDistanceValue (changeDirectionCost); // Change Direction Penalty
									nextNode.addPenaltyValue ((int)(((float)unit.getSlopeVariationFactor () / 3.0f) * (float)nextTerrainUnit.getHeight ()));  // Height Penalty
									nextNode.addPenaltyValue(nextTerrainUnit.getCost()); // distance from Obstacles
									nextNode.addPenaltyValue(nextNode.getHeat());
									nextNode.addDistanceValue(nextNode.getHeat()/1000);
								//w	nextNode.addDistanceValue(nextTerrainUnit.getCost()/2);
									EstimatedValue [nextNode.toPoint ()] = nextNode.g;	
									fromDirection [nextNode.toPoint ()] = i;
									if (!inClosedList) {
										openList.Enqueue (nextNode);
									}
								}
							}
						}
					}
				}
			}

			// the Path List Reversed
			if (!found) {
				Debug.Log ("Path not found");
				return false;
			} else {
				
			//	pathVectorList.Add (destinationPoint);				// this adds a point not on a grid
				Point p = destinationNode.toPoint ();
				while (true) {
					if (p.x == sourceNode.x && p.y == sourceNode.y)
						break;
					pathVectorList.Add (p.toVector3 ());
					p = new Point (p.x - movableNodes [fromDirection [p]].x, p.y - movableNodes [fromDirection [p]].y);
				}
				pathVectorList.Add (sourceNode.toVector3 ());

				//Drawing the Path Lines
				Debug.DrawLine (sourcePoint, destinationPoint, Color.green, 5, false);
				for (int i = 1; i<pathVectorList.Count; i++)
					Debug.DrawLine (pathVectorList [i - 1], pathVectorList [i], Color.red, 30, false);
				Debug.DrawLine (sourcePoint, sourceNode.toVector3 (), Color.red, 30, false);
				
				return true;
			}
		} catch (InvalidCastException e) {
			Debug.Log ("Error in Pathfinding: " + e.ToString ());
			return false;
		}
		
		
	}		
}
