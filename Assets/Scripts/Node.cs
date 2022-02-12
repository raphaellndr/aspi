using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PremierTest
{

    public class Node
    {
        static System.Random random;


        Sprite objectSprite;
        Sprite emptySprite;
        Sprite dustSprite;
        Sprite jewelSprite;
        Sprite jewelAndDustSprite;


        public Vector2 position;
        double dustProbability;
        double jewelProbability;
        public bool hasDust = false;
        public bool hasJewel = false;
        private GameObject visibleNode;

        public int gCost;
        public int hCost;
        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }

        public int gridX;
        public int gridY;

        public Node parent;

        // Constructeur de la classe Node. On y précise sa position, les probabilités d'apparation des objets ainsi que le Sprite à appliquer.
        public Node(Vector2 pos, double probaP, double probaB, Sprite _emptySprite, Sprite _dustSprite, Sprite _jewelSprite, Sprite _jewelAndDustSprite, int _gridX, int _gridY)
        {
            position = pos;
            dustProbability = probaP;
            jewelProbability = probaB;
            emptySprite=_emptySprite;
            dustSprite=_dustSprite;
            jewelSprite=_jewelSprite;
            jewelAndDustSprite=_jewelAndDustSprite;

            gridX = _gridX;
            gridY = _gridY;
            random = new System.Random();
        }

        // Fonction qui crée un gameobject et lui ajoute un composant SpriteRendrer afin de pouvoir lui ajouter un Sprite.
        // Une fois créé, lui applique une couleur correspondant à un type d'objet et le positionne sur la grille.
        public void CreateObject(String content)
        {
            if(visibleNode == null)
                visibleNode = new GameObject();
            if (visibleNode.GetComponent<SpriteRenderer>() == null)
                visibleNode.AddComponent<SpriteRenderer>();

            SpriteRenderer spriteRenderer = visibleNode.GetComponent<SpriteRenderer>();

            switch (content) {
                case "Both":
                    spriteRenderer.sprite = jewelAndDustSprite;
                    break;
                case "Dust":
                    spriteRenderer.sprite = dustSprite;
                    break;
                case "Jewel":
                    spriteRenderer.sprite = jewelSprite;
                    break;
                case "Nothing":
                    spriteRenderer.sprite = emptySprite;
                    break;
            }
            visibleNode.transform.position = position;
            visibleNode.name = "Node " + gridX + gridY;
        }
     
        // Fonction qui a une certaine probabilité de faire apparaître une poussière lorsqu'appelée.
        public void SpawnDust(Grid grid)
        {
            // Si la case/node ne contient pas déjà de poussière et qu'on la probabilité d'apparation est "respectée",
            // alors on fait apparaître une poussière.
            if (!hasDust && random.Next(100) <= dustProbability*100) //TODO: vérifier que l'aléatoire est correct
            {
                grid.scoreToChange -= 2;
                hasDust = true;
                // S'il y a déjà un bijou sur la case, combine les deux.
                if (hasJewel) CreateObject( "Both");
                // Sinon on fait apparaître la poussière.
                else CreateObject("Dust");
            }
        }

        // Fonction qui fait disparaître une poussière lorsqu'appelée.
        public void RemoveDust(Agent agent)
        {
            hasDust = false;
            // S'il y a un bijou sur la case, on ne le supprime pas.
            if (hasJewel) agent.nodeToCreate.Add(new int[] { gridX, gridY }, "Jewel");
            else agent.nodeToCreate.Add(new int[] { gridX, gridY }, "Nothing");
        }

        // Fonction qui a une certaine probabilité de faire apparaître un bijou lorsqu'appelée.
        // Même procédé que pour faire apparaître une poussière.
        public void SpawnJewel()
        {
            if (!hasJewel && random.Next(100) <= jewelProbability*100) //TODO: vérifier que l'aléatoire est correct
            {
                hasJewel = true;
                if (hasDust) CreateObject("Both");
                else CreateObject( "Jewel");
            }
        }

        // Fonction qui fait disparaître un bijou lorsqu'appelée.
        // Même procédé que pour faire disparaître une poussière.
        public void RemoveJewel(Agent agent)
        {
            hasJewel = false;
            if (hasDust) agent.nodeToCreate.Add(new int[] { gridX, gridY }, "Dust");
            else agent.nodeToCreate.Add(new int[] { gridX, gridY }, "Nothing");
        }
    }
}