using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Vector2 gridSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    // Start is called before the first frame update
    void Start()
    {
        // Combien de Nodes peut-on avoir dans notre grid 
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
        CreateGrid();
        InvokeRepeating("SpawnStuff", 0, 1);
    }

    void CreateGrid()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        grid = new Node[gridSizeX, gridSizeY];
        Vector2 bottomLeft = pos - Vector2.right * gridSize.x / 2 - Vector2.up * gridSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 point = bottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                grid[x, y] = new Node(point, 0.007f, 0.005f, false, false, false);
            }
        }
    }

    public Node NodeFromPoint(Vector2 position)
    {
        float percentX = (position.x + gridSize.x / 2) / gridSize.x;
        float percentY = (position.y + gridSize.y / 2) / gridSize.y;
        Mathf.Clamp01(percentX);
        Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    void SpawnStuff()
    {
        for(int i = 0; i < gridSizeX-1; i++) {
            for (int j = 0; j < gridSizeY-1; j++) {
                grid[i, j].apparitionPoussiere();
                grid[i, j].apparitionBijou();
            }
        }
    }

    void OnDrawGizmos()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        Gizmos.DrawWireCube(pos, new Vector2(gridSize.x, gridSize.y));

        if (grid != null)
        {
            foreach (Node node in  grid)
            {
                if (node.contientPoussiere && node.contientBijou)
                {
                    Gizmos.color = Color.blue;
                }
                else if (node.contientPoussiere)
                {
                    Gizmos.color = Color.yellow;
                }
                else if (node.contientBijou)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.cyan;
                }
                Gizmos.DrawCube(node.position, Vector2.one * (nodeDiameter - .1f));
            }
        }
    }
}
