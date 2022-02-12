using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace PremierTest
{
    public class GameManager : MonoBehaviour
    {
        //Une reference à notre agent
        public Agent agent;

        //Une réference à notre grille de case
        public Grid map;

        //Un thread sur lequel on va appeler le cycle de vie de notre agent
        private Thread _tAgent;
        
        //Une variable servant à faire apparaitre de la poussière et des bijoux toute les 1secondes
        private float timeElapsed = 0;

        //Une variable pour synchroniser le score sur l'agent
        int scoreToChange;

        private void Start()
        {
            //Le thread de l'agent executera la fonction LifeCycle
            _tAgent = new Thread(agent.LifeCycle);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            //On stock le temps qu'il s'est passé depuis le dernier passage à cet endroit
            timeElapsed += Time.deltaTime;

            //Si le thread de l'agent n'est pas défini ou qu'il n'est pas lancé
            if (_tAgent == null || _tAgent.ThreadState == ThreadState.Stopped || _tAgent.ThreadState == ThreadState.Unstarted)
            {
                //Alors on met à jour l'affichage suivant les nouvelles données qui viennent du thread
                agent.UpdateDisplay(map);
                map.UpdateDisplay(ref agent.score);
                agent.score += scoreToChange;
                scoreToChange = 0;

                //Et on relance le thread
                switch (_tAgent.ThreadState)
                {
                    case ThreadState.Stopped:
                        _tAgent = new Thread(agent.LifeCycle);
                        _tAgent.Start();
                        break;
                    case ThreadState.Unstarted:
                        _tAgent.Start();
                        break;
                }
            }
            if (timeElapsed >= 1) //Si plus d'une seconde est passée
            {
                //Alors on fait apparaitre de la poussière et des bijoux sur la map
                map.SpawnStuff();
                timeElapsed = 0; 
            }
        }
    }
}