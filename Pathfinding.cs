using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class Pathfinding : MonoBehaviour
{
    public Transform seeker, target;
    Grid grid;

    void Awake() {
        grid = GetComponent<Grid>();
    }

    void Update() {
        FindPath(seeker.position, target.position);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos) {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        List<Node> closedSet = new List<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0) {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++) {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost) {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode) {
                RetracePath(startNode, targetNode);
                return;
            }

            // Check if each neighbor is valid walkable node or Closed
            foreach (Node neighbor in grid.GetNeighbors(currentNode)) {
                if (!neighbor.walkable || closedSet.Contains(neighbor)) {
                    continue;
                }

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < currentNode.gCost || !openSet.Contains(neighbor)) {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor)) {
                        openSet.Add(neighbor);
                    }

                }


            }

        }
    }

    // Retraces the path found by creating a list of 
    void RetracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB) {
        int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        // The smaller vector will always tell us how many diagonal spaces we will need to move, then we subtract the diagonal steps from the horizontal steps and add the left over 
        // to obtain the total amount of steps that will be required to reach the target point.
        if (distanceX > distanceY)
            return 14 * distanceY + 10 * (distanceX - distanceY);   // Also 10 and 14 are used because according to special triangle rules where the non-hypotenus arms are 1, the hypotonuse
        return 14 * distanceX + 10 * (distanceY - distanceX);       // will be the sqrt(2) or 1.414.... so we multiply by 10 and round to get the 10 for up,down,left,right and 14 for 
                                                                    // diagonal movements.
    }
}
