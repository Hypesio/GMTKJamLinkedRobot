using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct StepInfo
{
    public bool doNothing;
    public Vector3 targetPosition;
    public PhysicObject physicObject;
    public bool hasMove;
    public bool done;
}
public class PhysicObject : MonoBehaviour
{
    public bool canPassThrough;
    protected LevelGridManager _levelGridManager;
    protected InputGestion _inputGestion;

    protected bool _isInactive;
    private Vector3 _startPosition;
    private bool _startCanPassThrough;

    protected void ParentStart()
    {
        _levelGridManager = FindObjectOfType<LevelGridManager>();
        _inputGestion = FindObjectOfType<InputGestion>();
        _startCanPassThrough = canPassThrough;
        _startPosition = transform.position;
    }

    public bool GetIsInactive()
    {
        return _isInactive;
    }

    // True to activate the object
    public void SetActiveState(bool active)
    {
        _isInactive = !active;
        gameObject.SetActive(active);
    }

    public virtual StepInfo BeforeStep()
    {
        StepInfo stepInfo = new StepInfo()
        {
            doNothing = true,
            hasMove = false,
            physicObject = this,
            targetPosition = Vector3.zero,
            done = false
        };
        return stepInfo;
    }

    public virtual void Step()
    {
        
    }

    public virtual void LateStep()
    {
        
    }

    public virtual void RestartLevel()
    {
        transform.position = _startPosition;
        canPassThrough = _startCanPassThrough;
        gameObject.SetActive(true);
        _isInactive = false;
    }
}
