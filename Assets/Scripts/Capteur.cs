using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PremierTest{ 
    public class Capteur : MonoBehaviour
    {
        public Grid map;

        public Capteur(Grid m){
            map = m;
        }

        // Fonction qui permet de mettre � jour la map
        public void UpdateMap(Grid m){
            map = m;
        }

        // Fonction qui permet de r�cup�rer la map 
        public Grid GetMap(){
            return map;
        }
        
    }
}