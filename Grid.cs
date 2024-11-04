using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grid : MonoBehaviour {

    // Variables
    public Transform player;
    public LayerMask unwalkableMask;            // The layer we want to check for obstacles in our grid
    public Vector2 gridWorldSize;               // World Dimensions
    public float nodeRadius;                    // Radius of each node(Node Size)
    Node[,] grid;                               // Grid of 
    float nodeDiameter;                         // Radius * 2
    int gridSizeX, gridSizeY;                   // variables used to store how many nodes are going to fit into the program
    public List<Node> path;                     // list of nodes for path


    // Start(), its purpose is to run the grid script
    void Start() {
        // gets basic dimensions for creating the grid
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        createGrid();
    }

    // CreateGrid() creates the grid to be used with A* pathfinding
    void createGrid() {
        grid = new Node[gridSizeX,gridSizeY];
        Vector2 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                // Calculate the coordinate for the node
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);

                // Some troublsome code here, had issues getting collision to work. I think the problem lies on this line 
                bool walkable = !(Physics2D.OverlapBox(worldPoint, Vector2.one * nodeRadius, 90.0f, unwalkableMask) != null);
                grid[x,y] = new Node(walkable, worldPoint, x, y);

            }
        }
    }

    // 
    public Node NodeFromWorldPoint(Vector2 worldPosition) {

        // 
        float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y/2) / gridWorldSize.y;

        // stops values from being greater than 1
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x,y];
    }

    public List<Node> GetNeighbors(Node node) {
        List<Node> neighbors = new List<Node>();

        // Iterate in a 3x3 grid around the origin Node 
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {

                // Skip origin Node
                if (x == 0 && y== 0) {
                    continue;
                }

                // variables to check if inside of grid
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                // If node is not outside of grid, then adds it to the neighbors list
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    
    // OnDrawGizmos() pretty much just draws the node grid out to visualize whats happening to the grid
    void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x, gridWorldSize.y, 1));
        
        // If the grid square is walkable, thus has no obstacle in the way, then it is white. If there is an obstacle then gizmos will draw a red square. If player is on node, then blue.
        if (grid != null) {
            Node playerNode = NodeFromWorldPoint(player.position);
            foreach (Node n in grid) {
                if (!(n.walkable)) {                                                        // For obstacles
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(n.coordinates, Vector3.one * (nodeDiameter - .1f));   
                }
                else if (n == playerNode) {                                                 // For Player
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawCube(n.coordinates, Vector3.one * (nodeDiameter - .1f));
                }
                else if (path.Contains(n)) {                                                 // For Path
                        Gizmos.color = Color.black;
                        Gizmos.DrawCube(n.coordinates, Vector3.one * (nodeDiameter - .1f));
                }
                else {
                    Gizmos.color = Color.white;                                             // For walkable nodes
                    Gizmos.DrawCube(n.coordinates, Vector3.one * (nodeDiameter - .1f));  
                }
                
            }
        }
    }
}
