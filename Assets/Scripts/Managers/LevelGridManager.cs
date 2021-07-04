using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using RobotType = AgentRobot.RobotType;

public class LevelGridManager : MonoBehaviour
{
    public int gridSize;
    public float timeStep = 0.10f;
    public float timeRestart = 0.5f;

    private List<PhysicObject> _livingObjectList;
    private List<FloorCase> _floorCases;
    private List<AgentRobot> _robots;
    private List<StepInfo> _infoBeforeStep;

    private LevelTransitionManager _levelTransitionManager;
    private InputGestion _inputGestion;
    private bool _finalLevel;

    // Start is called before the first frame update
    void Start()
    {
        _livingObjectList = FindObjectsOfType<PhysicObject>().ToList();
        _floorCases = FindObjectsOfType<FloorCase>().ToList();
        _levelTransitionManager = FindObjectOfType<LevelTransitionManager>();
        _robots = FindObjectsOfType<AgentRobot>().ToList();
        _inputGestion = FindObjectOfType<InputGestion>();

        if (SceneManager.GetActiveScene().buildIndex == 13)
        {
            _finalLevel = true;
            FinalLevel();
        }
    }

    // Update is called on each frame
    private void Update()
    {
        if (_finalLevel)
            return;

        if (_inputGestion.GetDown(InputGestion.TouchPressed.Restart))
        {
            RestartLevel();
        }
        else if (_inputGestion.GetDown(InputGestion.TouchPressed.Skip))
        {
            _levelTransitionManager.SkipLevel();
            _inputGestion.SetListenState(false);
        }
    }

    // Tell if two vector point reference the same case
    bool SameCase(Vector3 a, Vector3 b)
    {
        // If difference is too big then that's not the same case
        if (Mathf.Abs(a.x - b.x) >= ( (float) gridSize / 2))
            return false;
        if (Mathf.Abs(a.y - b.y) >= ( (float) gridSize / 2))
            return false;
        return true;
    }

    // Does cell have a floor ?
    public bool CellContainFloor(Vector3 position, RobotType robotType = RobotType.None)
    {
        // Check all floor case to see if one is at the position
        foreach (var floor in _floorCases)
        {
            if (SameCase(position, floor.transform.position) && (floor.acceptedRobot == RobotType.None || floor.acceptedRobot == robotType))
            {
                return true;
            }
        }

        return false;
    }

    // If the cell is occupied return the object on it
    public PhysicObject CellOccupied(Vector3 position, PhysicObject except = null)
    {
        foreach (var obj in _livingObjectList)
        {
            // The skip the wanted object
            if (obj == except)
                continue;

            // Object is alive and on the same spot
            if (!obj.GetIsInactive() && !obj.canPassThrough && SameCase(obj.transform.position, position))
            {
                return obj;
            }
        }

        return null;
    }

    // Get the appropriate step info
    private StepInfo GetStepInfo(PhysicObject obj)
    {
        foreach (var infos in _infoBeforeStep)
        {
            if (infos.physicObject == obj)
            {
                return infos;
            }
        }

        return new StepInfo() { doNothing = true };
    }

    // Detect if the cell will be free after the step of other objects
    private bool WillCellBeFree(StepInfo robotOnCell)
    {
        // It won't Move
        if (robotOnCell.hasMove || SameCase(robotOnCell.targetPosition, robotOnCell.physicObject.transform.position))
            return false;

        if (CanGoOnCell(robotOnCell.targetPosition, robotOnCell.physicObject, ((AgentRobot)robotOnCell.physicObject).robotType))
        {
            return true;
        }
        return false;
    }

    // Detect is the asking robot can go on cell wanted
    public bool CanGoOnCell(Vector3 position, PhysicObject askingObject, RobotType robotType = RobotType.None)
    {
        // Check if robot can move to the cell
        if (!CellContainFloor(position, robotType))
            return false;


        // Start by checking normal
        foreach (var obj in _livingObjectList)
        {
            // Object is alive and blocking the path
            if (!obj.GetIsInactive() && !obj.canPassThrough && SameCase(obj.transform.position, position))
            {
                //Will it change during the step ?
                StepInfo objectBeforeInfo = GetStepInfo(obj);

                // Won't change then the cell will stay blocked
                if (objectBeforeInfo.doNothing)
                    return false;

                // Avoid robots to switch place
                if (SameCase(objectBeforeInfo.targetPosition, askingObject.transform.position))
                {
                    return false;
                }


                // Check if the blocking robot will move
                if (objectBeforeInfo.physicObject is AgentRobot && !WillCellBeFree(objectBeforeInfo))
                    return false;
            }
        }

        // Check is an other robot want to take the place
        foreach (var infosStep in _infoBeforeStep)
        {
            if (infosStep.physicObject == askingObject)
                continue;

            if (infosStep.physicObject is AgentRobot)
            {
                if (SameCase(position, infosStep.targetPosition))
                {
                    // The robot already start to go on target
                    if (infosStep.hasMove)
                        return false;

                    // Check priority
                    AgentRobot robot = (AgentRobot) infosStep.physicObject;
                    if ((int)robotType > (int)robot.robotType)
                    {
                        return false;
                    }
                }
            }
        }


        StepInfo objInfo = GetStepInfo(askingObject);
        objInfo.hasMove = true;

        return true;
    }

    // Restart the level
    public void RestartLevel()
    {
        _levelTransitionManager.PlayRestartTransition();
        StartCoroutine(IRestartEffect());
    }


    // End the actual level
    public void EndLevel()
    {
        // Player can't move
        _inputGestion.SetListenState(false);
        foreach (var robot in _robots)
        {
            robot.SetHappy();
        }
        _levelTransitionManager.NextLevel();
    }

    // Function use only on the final level
    public void FinalLevel()
    {
        foreach (var robot in _robots)
        {
            robot.SetHappy();

        }
        StartCoroutine(InfiniteConvertion());
    }

    // Check if all robot are converted
    public void CheckEndLevel()
    {
        foreach (var robot in _robots)
        {
            if (robot.converted == false || robot.isDown)
                return;
        }
        EndLevel();
    }


    // Restart if it is impossible to win
    public void CheckGameOver()
    {
        foreach (var robot in _robots)
        {
            //At least one robot alive
            if (robot.converted && !robot.isDown)
            {
                CheckEndLevel();
                return;
            }
        }

        RestartLevel();
    }

    // Start step process
    public void LaunchStep()
    {
        if (_finalLevel)
            return;
        StartCoroutine(IStepProcess());
    }

    // Routine launching the full step process
    IEnumerator IStepProcess()
    {
        // To be sure there won't be input during midstep
        _inputGestion.SetListenState(false);

        // Start by getting what want every object do
        _infoBeforeStep = new List<StepInfo>();
        foreach (var obj in _livingObjectList)
        {
            StepInfo newStepInfo = obj.BeforeStep();
            // Take only if the object do something
            if (!newStepInfo.doNothing)
            {
                _infoBeforeStep.Add(newStepInfo);
            }
        }

        // Call step on all objects
        foreach (var obj in _livingObjectList)
        {
            obj.Step();
        }

        yield return new WaitForSeconds(timeStep);

        // Want to be sure kill will be the last thing done
        List<PhysicObject> killers = new List<PhysicObject>();

        foreach (var obj in _livingObjectList)
        {
            if (obj is AgentKiller)
                killers.Add(obj);
            else
                obj.LateStep();
        }

        // Call the late step on all object
        foreach (var agent in killers)
        {
            agent.LateStep();
        }

        CheckGameOver();

        _inputGestion.SetListenState(true);
    }

    // Routine for restart level effect
    IEnumerator IRestartEffect()
    {
        _inputGestion.SetListenState(false);
        yield return new WaitForSeconds(timeRestart);
        foreach (var obj in _livingObjectList)
        {
            obj.RestartLevel();
        }
        _inputGestion.SetListenState(true);
    }

    // Routine to play conversion effect in loop (use for game end screen)
    IEnumerator InfiniteConvertion()
    {
        while (true)
        {
            foreach (var robot in _robots)
            {
                robot.PlayConvertion();
            }

            yield return new WaitForSeconds(1.2f);
        }
    }
}