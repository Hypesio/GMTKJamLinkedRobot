using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentFinishLevel : PhysicObject
{
    private void Start()
    {
        ParentStart();
    }

    // Update is called once per frame
    void Update()
    {
        PhysicObject objectOn = _levelGridManager.CellOccupied(transform.position, this);
        
        // Check if an object is upon it and can die
        if (objectOn != null && objectOn is AgentRobot)
        {
            _levelGridManager.EndLevel();
        }
    }
}
