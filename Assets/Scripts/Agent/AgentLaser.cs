using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchPressed = InputGestion.TouchPressed;

public class AgentLaser : PhysicObject
{
    public List<PhysicObject> rayCells;
    public List<PhysicObject> topRaysOn;
    public List<GameObject> topRaysOff;
    public AudioSource sourceLaser;

    public TouchPressed inputOn = TouchPressed.Down;
    public TouchPressed inputOff = TouchPressed.Up;

    public bool allInputToggles = false;
    
    private bool _previousIsInactive;

    private bool _onPressed;
    private bool _offPressed;

    // Start is called before the first frame update
    void Start()
    {
        ParentStart();
        _previousIsInactive = _isInactive;
        ActiveLaser();
    }

    public override StepInfo BeforeStep()
    {
        _onPressed = _inputGestion.GetDown(inputOn);
        _offPressed = _inputGestion.GetDown(inputOff);

        if (allInputToggles)
        {
            if (_inputGestion.GetDown(TouchPressed.Up) || _inputGestion.GetDown(TouchPressed.Down)
                                                       || _inputGestion.GetDown(TouchPressed.Left) ||
                                                       _inputGestion.GetDown(TouchPressed.Right))
            {
                _onPressed = true;
                _offPressed = false;
            }
        }
        return base.BeforeStep();
    }

    // Update is called once per frame
    public override void LateStep()
    {
        bool stateChanged = false;
        if (allInputToggles && _onPressed)
        {
            _isInactive = !_isInactive;
            stateChanged = true;
        }
        else if (_onPressed)
        {
            if (inputOn == inputOff)
                _isInactive = !_isInactive;
            else
            {
                _isInactive = false;
            }
            stateChanged = true;
        }
        else if (_offPressed)
        {
            _isInactive = true;
            stateChanged = true;
        }
        // The level was restarted
        else if (_previousIsInactive != _isInactive)
        {
            stateChanged = true;
        }

        if (stateChanged)
        {
            ActiveLaser();
        }
        

        _previousIsInactive = _isInactive;
    }

    public override void RestartLevel()
    {
        base.RestartLevel();
        ActiveLaser();
    }

    void ActiveLaser()
    {
        if (_isInactive)
            sourceLaser.Stop();
        else
        {
            if (!sourceLaser.isPlaying)
                sourceLaser.Play();
        }
            
        foreach (PhysicObject ray in rayCells)
        {
            ray.SetActiveState(!_isInactive);
        }

        for (int i = 0; i < topRaysOn.Count; i++)
        {
            topRaysOff[i].SetActive(_isInactive);
            topRaysOn[i].SetActiveState(!_isInactive);
        }
    }
    
    
    
}
