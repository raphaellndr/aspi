using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Threading;

namespace PremierTest
{
    public class Agent : MonoBehaviour
    {
        public State state;
        public Capteur sensor;
        public Effecteur effector;
        public int energy;
        Grid belief;
        public int score;
        Stack<string> intention;


        // Variables utilisées pour mettre à jour l'affichage
        public string actionToAddToUI;
        public Dictionary<int[], String> nodeToCreate;
        public string energyToDisplay;
        public Vector2 changePosition;

        // Fonction appelée uniquement pour synchroniser ce qui est affiché
        internal void UpdateDisplay(Grid grid)
        {
            transform.position += (Vector3)changePosition;
            if (actionToAddToUI != null) AddActionToUI(actionToAddToUI);
            if (nodeToCreate != null)
            {
                for (int i = 0; i < nodeToCreate.Count; i++)
                {
                    var key = nodeToCreate.ElementAt(i).Key;
                    var value = nodeToCreate.ElementAt(i).Value;
                    grid.GetNode(key[0], key[1]).CreateObject(value);
                }
            }
            if (energyToDisplay != null) textEnergie.text = energyToDisplay;
            effector.UpdateScore();

            actionToAddToUI = null;
            nodeToCreate = new Dictionary<int[], String>();
            energyToDisplay = null;
            changePosition = new Vector2();
        }

        public Text textEnergie;
        public Text textActions;

        public Agent(State st, Capteur cap, Effecteur eff, int ener, int sco, Grid bf, Stack<string> intent)
        {
            state = st;
            sensor = cap;
            effector = eff;
            energy = ener;
            score = sco;
            belief = bf;
            intention = intent;
        }

        void Start()
        {
            this.sensor = gameObject.GetComponent<Capteur>();
            this.effector = gameObject.GetComponent<Effecteur>();

            ExploreWithSensor();

            int[,] stateMap = BeliefToState();
            int[] pos = new int[] { 2, 2 };

            this.state = new State(stateMap, pos);
            this.energy = 0;
            this.score = 0;
            this.intention = new Stack<string>();    //TODO

            nodeToCreate = new Dictionary<int[], String>();
        }

        public int[,] BeliefToState()
        {
            int[,] stateMap = new int[5, 5];
            // On convertit la map de la croyance en état
            for (int k = 0; k < 25; k++)
            {
                int i = k % 5;
                int j = k / 5;
                Node c = belief.GetNode(i, j);
                if (c.hasJewel & c.hasDust)
                {
                    stateMap.SetValue(3, i, j);
                }
                else
                {
                    if (c.hasJewel)
                    {
                        stateMap.SetValue(2, i, j);
                    }
                    else if (c.hasDust)
                    {
                        stateMap.SetValue(1, i, j);
                    }
                    else
                    {
                        stateMap.SetValue(0, i, j);
                    }
                }
            }
            return stateMap;
        }

        public void ExploreWithSensor()
        {
            this.belief = this.sensor.GetMap();
        }

        public bool GoalTest(State st)
        {
            int i = st.agentPosition[0];
            int j = st.agentPosition[1];
            if (st.stateMap[i, j] != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void LifeCycle()
        {
            // Soit l'effecteur n'a plus d'actions à effectuer, auquel cas l'agent en cherche de nouvelles à faire, soit il reste des actions
            // à effectuer, auquel cas il les effectues.
            if (this.intention.Count == 0)
            {
                ExploreWithSensor();
                state.stateMap = BeliefToState();
                //j'explore
                actionToAddToUI = "Thinking";
                int[] goalPos = BFS(state);
                this.intention = AStar(state.agentPosition, goalPos);
            }
            else
            {
                this.effector.SendActionToEffecteur(this.intention);
            }
            energyToDisplay = "Energie: " + energy;

            Thread.Sleep(1000);
        }

        public void AddActionToUI(string action)
        {
            var splitedText = textActions.text.Split('\n');
            var finalString = action + "\n";
            for (int i = 0; i < splitedText.Length; i++) { if (i == 2) break; finalString += splitedText[i] + "\n"; }
            textActions.text = finalString;
        }

        Stack<string> PickOrVacuum(int[] pos) //dit au robot d'aspirer ou ramasser selon sa nouvelle position
        {
            var returnValue = new Stack<string>();
            var stateOfNode = state.stateMap[pos[0], pos[1]];
            if (stateOfNode == 3 || stateOfNode == 2)
            {
                returnValue.Push("pickUp");
                if (stateOfNode == 3)
                {
                    returnValue.Push("vacuum");
                }
            }
            else if (stateOfNode == 1)
            {
                returnValue.Push("vacuum");
            }
            else
            {
                returnValue.Push("nothing");
            }
            return returnValue;
        }

        List<string> PossibleActions(int[] pos)
        {
            int i = pos[0];
            int j = pos[1];
            List<string> res = new List<string>();
            if (j < 4)
            { // Il n'est pas tout en bas
                res.Add("down");
            }
            if (j > 0)
            { // Il n'est pas tout en haut
                res.Add("up");
            }
            if (i < 4)
            { // Il n'est pas tout à droite
                res.Add("right");
            }
            if (i > 0)
            { // Il n'est pas tout à gauche
                res.Add("left");
            }
            return res;
        }

        State Successor(State st, string action)
        {
            // A partir d'un état et d'une action on donne l'état suivant
            int i = st.agentPosition[0];
            int j = st.agentPosition[1];
            if (action == "up")
            {
                j--;
            }
            else if (action == "down")
            {
                j++;
            }
            else if (action == "left")
            {
                i--;
            }
            else
            {
                i++;
            }
            int[] newPos = new int[2];
            newPos[0] = i;
            newPos[1] = j;
            State successor = new State(st.stateMap, newPos);
            return successor;
        }

        Dictionary<string, State> Successors(State st, List<string> actions)
        {
            // A partir d'un état et d'une liste d'acion on sort tous les états possibles suivants
            Dictionary<string, State> successors = new Dictionary<string, State>();
            foreach (string action in actions)
            {
                State successor = Successor(st, action);
                successors.Add(action, successor);
            }
            return successors;
        }

        int[] BFS(State initialState)
        {
            Dictionary<string, object> initialNode = new Dictionary<string, object>();
            initialNode.Add("state", initialState);
            if (GoalTest((State)initialNode["state"]) == true)
            {
                int[] initialPos = initialState.agentPosition;
                return initialPos;
            }
            // Liste des états déjà visités pour ne pas boucler
            List<State> visited = new List<State>();
            // Liste de la frontière sous forme de queue. Elle contient des noeuds qu'on a modélisé en dictionnaires
            Queue<Dictionary<string, object>> frontier = new Queue<Dictionary<string, object>>();
            frontier.Enqueue(initialNode);
            visited.Add(initialState);
            while (frontier.Count != 0)
            {
                // On récupère le dernier noeud de la frontière
                Dictionary<string, object> node = frontier.Dequeue();
                State st = (State)node["state"];
                // On récupère la liste des actions possible à partir de l'état correspondant à ce noeud
                List<string> actions = PossibleActions(st.agentPosition);
                // On génère les successeurs puis on boucle sur les successeurs
                Dictionary<string, State> successors = Successors(st, actions);
                foreach (KeyValuePair<string, State> successor in successors)
                {
                    // On crée le noeud enfant
                    Dictionary<string, object> child = new Dictionary<string, object>();
                    child.Add("state", (State)successor.Value);
                    child.Add("action", (string)successor.Key);
                    child.Add("parent", node);
                    // On vérifie que l'état correspondant n'a pas déjà été visité
                    var test1 = false;
                    foreach (var v in visited)
                    {
                        test1 = v.agentPosition.SequenceEqual(((State)child["state"]).agentPosition);
                        if (test1) break;
                    }
                    if (!test1)
                    {
                        // Test de but
                        if (GoalTest((State)child["state"]))
                        {
                            // Si le but est vérifié on renvoie la position du noeuf final
                            State childState = (State)child["state"];
                            int[] childPos = childState.agentPosition;
                            return childPos;
                        }
                        // Si test de but pas vérifié on ajoute le noeud enfant à la frontière
                        frontier.Enqueue(child);
                        visited.Add((State)child["state"]);
                    }
                }
            }
            return initialState.agentPosition;
        }

        Stack<string> AStar(int[] initialPos, int[] finalPos)
        {
            int initialX = initialPos[0];
            int initialY = initialPos[1];
            int finalX = finalPos[0];
            int finalY = finalPos[1];
            Node initialNode = belief.GetNode(initialX, initialY);
            Node finalNode = belief.GetNode(finalX, finalY);

            //création des listes frontière et visited comme pour le BFS
            List<Node> frontier = new List<Node>();  
            HashSet<Node> visited = new HashSet<Node>(); 
            frontier.Add(initialNode);

            //tant que la frontière n'est pas vide
            while (frontier.Count > 0)
            {   
                //on récupère le premier noeud
                Node currentNode = frontier[0];
                //on regarde si il a d'autres noeuds plus désirables dans la frontière
                for (int i = 1; i < frontier.Count; i++)
                {
                    if (frontier[i].fCost < currentNode.fCost || frontier[i].fCost == currentNode.fCost && frontier[i].hCost < currentNode.hCost)
                    {
                        currentNode = frontier[i];
                    }
                }

                frontier.Remove(currentNode);
                visited.Add(currentNode);

                //test de but
                if (currentNode == finalNode)
                {
                    return RetracePath(initialNode, finalNode);
                }

                //recherche de successeurs si le but n'est pas atteint
                foreach (Node neighbour in belief.GetNeighbours(currentNode))
                {
                    if (frontier.Contains(neighbour))
                    {
                        continue;
                    }

                    //mise à jour de hcost, gcost et le noeud parent du noeud enfant
                    int movementCost = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (movementCost < neighbour.gCost || !frontier.Contains(neighbour))
                    {
                        neighbour.gCost = movementCost;
                        neighbour.hCost = GetDistance(neighbour, finalNode);
                        neighbour.parent = currentNode;

                        //on ajoute le noeud à la frontière s'il n'y est pas déjà
                        if (!frontier.Contains(neighbour))
                        {
                            frontier.Add(neighbour);
                        }
                    }
                }
            }
            return new Stack<string>();
        }

        Stack<string> RetracePath(Node startNode, Node endNode)
        {   
            //on retourne une liste de noeuds qui retrace le chemin du noeud final jusqu'au noeud initial
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode && currentNode.parent!=null)
            {
                path.Add(currentNode);
                var tmp = currentNode.parent;
                currentNode.parent = null;
                currentNode = tmp;
            }
            path.Add(startNode);

            //une fois le chemin reconstitué on retrouve quelles sont les actions qui ont été faites pour arriver à ce chemin
            Stack<string> solution = RetraceActions(path);
            return solution;

        }

        Stack<string> RetraceActions(List<Node> path)
        {
            Node finalNode = path[0];
            int[] finalPos = new int[] { finalNode.gridX, finalNode.gridY };
            //comme on renvoit une pile on met d'abord les actions que devra effectuer l'agent quand il arrive sur la case finale
            Stack<string> solution = new Stack<string>();
            foreach (var s in (PickOrVacuum(finalPos))) 
                solution.Push(s);
            for (int i = 0; i < path.Count - 1; i++)
            {
                Node currentNode = path[i];
                Node formerNode = path[i + 1];
                //en fonction des différences de coordonnées on retrouve les actions qu'il faut faire
                if (currentNode.gridX + 1 == formerNode.gridX)
                {
                    solution.Push("left");
                    if (currentNode.gridY - 1 == formerNode.gridY)
                    {
                        solution.Push("down");
                    }
                    else if (currentNode.gridY + 1 == formerNode.gridY)
                    {
                        solution.Push("up");
                    }
                }
                else if (currentNode.gridX - 1 == formerNode.gridX)
                {
                    solution.Push("right");
                    if (currentNode.gridY + 1 == formerNode.gridY)
                    {
                        solution.Push("up");
                    }
                    else if (currentNode.gridY - 1 == formerNode.gridY)
                    {
                        solution.Push("down");
                    }
                }
                else
                {
                    if (currentNode.gridY - 1 == formerNode.gridY)
                    {
                        solution.Push("down");
                    }
                    else
                    {
                        solution.Push("up");
                    }
                }
            }

            return solution;
        }


        int GetDistance(Node node1, Node node2)
        {   
            //distance de 10 quand la case est à côté, 14 en diagonale
            int distanceX = Mathf.Abs(node1.gridX - node2.gridX);
            int distanceY = Mathf.Abs(node1.gridY - node2.gridY);

            if (distanceX > distanceY)
            {
                return (14 * distanceY + 10 * (distanceX - distanceY));
            }
            return 14 * distanceX + 10 * (distanceY - distanceX);
        }
    }
}