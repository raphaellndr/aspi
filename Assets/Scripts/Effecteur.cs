using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace PremierTest
{
    public class Effecteur : MonoBehaviour
    {
        Stack<string> actions;
        public Agent agent;
        public Grid map;

        // Affichage du score de l'agent
        public Text scoreText;

        // Constructeur de la classe Effecteur
        public Effecteur(Grid _map)
        {
            agent = gameObject.GetComponent<Agent>();
            map = _map;
        }


        // Méthode appelée lors de la création de l'objet
        void Start()
        {
            actions = new Stack<string>();
        }

        // Méthode de mise à jour de l'effecteur 
        public void SendActionToEffecteur(Stack<string> intention)
        {
            actions = intention;
            ReadActionPile();
        }

        // Méthode qui lit la première action de la pile d'action, effectue cette action et qui la retire de la pile 
        void ReadActionPile()
        {
            switch (actions.Pop())
            {
                case "up":
                    agent.actionToAddToUI = "Moving up";
                    MoveUp();
                    break;
                case "down":
                    agent.actionToAddToUI = "Moving down";
                    MoveDown();
                    break;
                case "left":
                    agent.actionToAddToUI = "Moving left";
                    MoveLeft();
                    break;
                case "right":
                    agent.actionToAddToUI = "Moving right";
                    MoveRight();
                    break;
                case "vacuum":
                    agent.actionToAddToUI = "Vacuuming";
                    Vacuum();
                    break;
                case "pickUp":
                    agent.actionToAddToUI = "Picking up";
                    PickUp();
                    break;
                case "failure":
                    break;
                default:
                    break;
            }
        }

        // Méthode qui déplace l'agent vers le haut
        void MoveUp()
        {
            if (agent.state.agentPosition[1] > 0)
            {
                agent.state.agentPosition[1]--;
                agent.changePosition = new Vector2(0, 1);
            }
            // Perte d'énergie car mouvement
            agent.energy++;
        }

        // Méthode qui déplace l'agent vers le bas
        void MoveDown()
        {
            if (agent.state.agentPosition[1] < 4)
            {
                agent.state.agentPosition[1]++;
                agent.changePosition = new Vector2(0, - 1);
            }
            // Perte d'énergie car mouvement
            agent.energy++;
        }

        // Méthode qui déplace l'agent vers la gauche
        void MoveLeft()
        {
            if (agent.state.agentPosition[0] > 0)
            {
                agent.state.agentPosition[0]--;
                agent.changePosition = new Vector2(-1, 0);
            }
            // Perte d'énergie car mouvement
            agent.energy++;
        }

        // Méthode qui déplace l'agent vers la droite
        void MoveRight()
        {
            if (agent.state.agentPosition[0] < 4)
            {
                agent.state.agentPosition[0]++;
                agent.changePosition = new Vector2(1, 0);
            }
            // Perte d'énergie car mouvement
            agent.energy++;
        }

        // Méthode qui fait aspirer l'agent
        void Vacuum()
        {
            if (map.GetNode(agent.state.agentPosition[0], agent.state.agentPosition[1]).hasJewel)
            {
                // N'est pas censé aspirer un bijou : pénalité.
                agent.score -= 25;
            }
            map.GetNode(agent.state.agentPosition[0], agent.state.agentPosition[1]).RemoveJewel(agent);

            map.GetNode(agent.state.agentPosition[0], agent.state.agentPosition[1]).RemoveDust(agent);

            // Récompense car il a bien aspiré une poussière.
            agent.score += 10;

            // Perte d'énergie car mouvement
            agent.energy++;
        }

        // Méthode qui fait ramasser l'agent
        void PickUp()
        {
            map.GetNode(agent.state.agentPosition[0], agent.state.agentPosition[1]).RemoveJewel(agent);
            agent.score += 15;

            // Perte d'énergie car mouvement
            agent.energy++;
        }
        
        // Met à jour le score de l'agent.
        // Un point d'énergie dépensé fait perdre un point de score.
        // Le score augmente lorsque l'agent a aspiré une poussière ou ramassé un bijou.
        public void UpdateScore()
        {
            scoreText.text = "Score: " + (agent.score - agent.energy).ToString();
        }
    }
}