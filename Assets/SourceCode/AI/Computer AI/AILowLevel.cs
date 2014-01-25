using System;
using System.Threading;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class AILowLevel
{
    private static AILowLevel instance;
    private List<Player> playerIndex;
    private Unit target;
    private static int framePlayerTypeCounter = 0;
    private static int framePlayerUnitCounter = 0;
    private static int frameCompTypeCounter = 0;
    private static int frameCompUnitCounter = 0;
    private Thread th = null;
    AILowLevel()
    {
        playerIndex = UnitManager.getInstance().compAI();
    }
    public static AILowLevel getInstance()
    {
        if (instance == null)
            instance = new AILowLevel();
        return instance;
    }
    public void checkUnitInRange()
    {
        //Debug.Log("frameCompTypeCounter " + frameCompTypeCounter);
        //Debug.Log("frameCompUnitCounter " + frameCompUnitCounter);
        //Debug.Log("framePlayerTypeCounter " + framePlayerTypeCounter);
        //Debug.Log("framePlayerUnitCounter " + framePlayerUnitCounter);
        /////////////////////////////////////////////////////////
        //Debug.Break();
        //if (framePlayerTypeCounter == Player.UnitTypesNumber)
        //{
        //    framePlayerTypeCounter = 0;
        //    framePlayerUnitCounter = 0;
        //    frameCompUnitCounter++;
        //    if (frameCompUnitCounter == playerIndex[1].unitIndex[frameCompTypeCounter].Count)
        //    {
        //        for (++frameCompTypeCounter; frameCompTypeCounter < Player.UnitTypesNumber &&
        //        playerIndex[1].unitIndex[frameCompTypeCounter].Count == 0; ++frameCompTypeCounter) ;
        //        frameCompUnitCounter = 0;
        //    }
        //    if (frameCompTypeCounter == Player.UnitTypesNumber)
        //    {
        //        frameCompUnitCounter = 0;
        //        frameCompTypeCounter = 0;
        //    }
        //    return;
        //}
        //if (framePlayerUnitCounter == playerIndex[0].unitIndex[framePlayerTypeCounter].Count)
        //{
        //    for (++framePlayerTypeCounter; framePlayerTypeCounter < Player.UnitTypesNumber &&
        //        playerIndex[0].unitIndex[framePlayerTypeCounter].Count == 0; ++framePlayerTypeCounter) ;
        //    framePlayerUnitCounter = 0; 
        //}

        //////////////////////////////////////////////////////
        //if (playerIndex[1].unitIndex[frameCompTypeCounter][frameCompUnitCounter].hasTarget())
        //    return;
        //if (playerIndex[1].unitIndex[frameCompTypeCounter][frameCompUnitCounter].isInAttackRange(
        //    playerIndex[0].unitIndex[framePlayerTypeCounter][framePlayerUnitCounter]))
        //{
        //    Debug.Log("attack in range true");
        //    playerIndex[1].unitIndex[frameCompTypeCounter][frameCompUnitCounter].unitAttackTarget(
        //    playerIndex[0].unitIndex[framePlayerTypeCounter][framePlayerUnitCounter]);
        //    playerIndex[1].unitIndex[frameCompTypeCounter][frameCompUnitCounter].currentState = UnitState.Attack;
        //}
        ///////////
        //++framePlayerUnitCounter;
        ///////////
    }
}
