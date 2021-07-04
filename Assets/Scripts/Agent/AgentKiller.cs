using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentKiller : PhysicObject
{
    public bool killSpecificRobots = false;
    public AgentRobot.RobotType dontKillRobot = AgentRobot.RobotType.Normal;
    // Start is called before the first frame update
    void Start()
    {
        ParentStart();
    }



    public override void LateStep()
    {
        if (_isInactive)
            return;


        PhysicObject objectOn = _levelGridManager.CellOccupied(transform.position, this);

        // Check if an object is upon it and can die
        if (objectOn != null && objectOn is AgentRobot)
        {
            AgentRobot agentRobot = (AgentRobot) objectOn;
            if (killSpecificRobots && agentRobot.robotType != dontKillRobot)
            {
                agentRobot.die();
                _levelGridManager.RestartLevel();
            }
            else if (!killSpecificRobots)
            {
                agentRobot.die();
                _levelGridManager.RestartLevel();
            }
        }

    }
}
