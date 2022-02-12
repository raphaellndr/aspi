using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace PremierTest
{
    class Effecteur : MonoBehaviour
    {
        String[] actions;
        Agent agent;
        //Methode à créer:
        //lirePileActions
        //bougerEnHaut
        //bougerEnBas
        //bougerAGauche
        //bougerADroite
        //aspirer

        Rigidbody2D rig;
        public float speed = 3f;
        float xVelocity;
        float yVelocity;

        Grid map;

        public Effecteur(Grid _map)
        {
            agent = gameObject.GetComponent<Agent>();
            map = _map;
        }


        void Start()
        {
            rig = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            xVelocity = Input.GetAxisRaw("Horizontal");
            xVelocity = Mathf.Clamp(xVelocity, -12, 12);

            yVelocity = Input.GetAxisRaw("Vertical");
            yVelocity = Mathf.Clamp(yVelocity, -12, 12);

            rig.velocity = new Vector2(xVelocity * speed, yVelocity * speed);

            Mathf.Clamp(transform.position.x, -12f, 12f);
            Mathf.Clamp(transform.position.y, -12f, 12f);


            Debug.Log(transform.position);
        }

        void ReadActionPile()
        {
            foreach (var a in actions)
            {
                switch (a)
                {
                    case "up":
                        MoveUp(agent);
                        break;
                    case "down":
                        MoveDown(agent);
                        break;
                    case "left":
                        MoveLeft(agent);
                        break;
                    case "right":
                        MoveRight(agent);
                        break;
                    case "vacuums":
                        Vacuum(agent);
                        break;
                    default:
                        //Debug.Log("Action non reconnue");
                        break;
                }
            }
        }

        void MoveUp(Agent agent)
        {
            if (agent.position[1] > 0)
                agent.position[1]--;
        }

        void MoveDown(Agent agent)
        {
            if (agent.position[1] < 4)
                agent.position[1]++;
        }

        void MoveLeft(Agent agent)
        {
            if (agent.position[0] > 0)
                agent.position[0]--;
        }

        void MoveRight(Agent agent)
        {
            if (agent.position[0] < 0)
                agent.position[0]++;
        }

        void Vacuum(Agent agent)
        {
            map.getNode(agent.position[0], agent.position[1]).RemoveJewel();
            map.getNode(agent.position[0], agent.position[1]).RemoveDust();
            return;
        }

        /* sert juste à effectuer les actions
        attributs : 
        liste/pile des actions
        
        méthodes :
        faire toutes les actions puis retourne un état, l'énergie consommée et le score de performance*/

    }
}