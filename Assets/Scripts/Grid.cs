using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PremierTest
{
    public class Grid : MonoBehaviour
    {
        public Sprite emptySprite;
        public Sprite dustSprite;
        public Sprite jewelSprite;
        public Sprite jewelAndDustSprite;

        public Vector2 gridSize;
        public float nodeRadius;
        Node[,] grid;

        float nodeDiameter;
        int gridSizeX, gridSizeY;

        //Var used to update the display
        public int scoreToChange;

        void Start()
        {
            // Combien de Nodes peut-on avoir dans notre grid.
            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
            CreateGrid();
        }

        internal void UpdateDisplay(ref int score)
        {
            score += scoreToChange;
            scoreToChange = 0;
        }

        // Méthode pour créer la map.
        void CreateGrid()
        {
            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
            grid = new Node[gridSizeX, gridSizeY];
            Vector2 topLeft = pos - Vector2.right * gridSize.x / 2 + Vector2.up * gridSize.y / 2;

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector2 point = topLeft + Vector2.right * (x * nodeDiameter + nodeRadius) - Vector2.up * (y * nodeDiameter + nodeRadius);
                    grid[x, y] = new Node(point, 0.001f, 0.001f, emptySprite,dustSprite,jewelSprite,jewelAndDustSprite,x,y);
                }
            }
        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neightbours = new List<Node>();

            for (int x = -1; x <=1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }
                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neightbours.Add(grid[checkX, checkY]);
                    }
                }
            }

            return neightbours;
        }

        // Méthode qui permet d'obtenir le noeuds équivalent à une position dans l'espace.
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

        // Méthode qui fait apparaitre de la poussière et des bijoux.
        public void SpawnStuff()
        {
            for (int i = 0; i < gridSizeX; i++)
            {
                for (int j = 0; j < gridSizeY; j++)
                {
                    grid[i, j].SpawnDust(this);
                    grid[i, j].SpawnJewel();
                }
            }
        }

        // Méthode qui permet d'obtenir les coordonées à partir d'un tableau d'une dimension.
        public int[] GetCoordinatesFromPosition(int pos)
        {
            int[] coordinates = { 0, 0 };
            coordinates[0] = pos % 5;
            coordinates[1] = (int)((pos - (pos % 5)) / 5.0);
            return coordinates;
        }
        
        // Méthode qui retourne le noeud correspondant à une position dans la grille.
        public Node GetNode(int i, int j)
        {
            Node c = grid[i, j];
            return c;
        }

    }
}