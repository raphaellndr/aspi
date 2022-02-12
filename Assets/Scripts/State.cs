using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace PremierTest
{
    public class State
    {
        public int[] agentPosition;
        public int[,] stateMap;


        public State(int[,] stateM, int[] ap)
        {
            stateMap = stateM;
            agentPosition = ap;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
