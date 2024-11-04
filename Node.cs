using System.Collections;
using UnityEngine;

public class Node {

    // Variables 
    public bool walkable;           // bool value that determines if Node is walkable or not or in other words is an obstacle
    public Vector2 coordinates;     // Node Location
    public int gCost;               // gCost for A* algorithm, distance from starting node
    public int hCost;               // hCost for A* algorithm, distance from end node
    public int gridX;
    public int gridY;
    public Node parent;             // Parent Node

    // Node constructor
    public Node(bool _walkable, Vector2 _worldPos, int _gridX, int _gridY) {
        walkable = _walkable;
        coordinates = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }
    
    // fCost of the node
    public int fCost {
        get {
            return gCost + hCost;
        }
    }
}