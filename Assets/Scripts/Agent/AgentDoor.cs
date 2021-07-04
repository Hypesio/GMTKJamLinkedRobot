using System;
using UnityEngine;
using TouchPressed = InputGestion.TouchPressed;

public class AgentDoor : PhysicObject
{
    public TouchPressed inputOpen = TouchPressed.Down;
    public TouchPressed inputClose = TouchPressed.Up;
    public TouchPressed alternativeClose = TouchPressed.Up;

    public bool allInputToggles = false;
    
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public AudioSource sourceDoor;

    private bool _previousCanPassThrough;
    private bool _stateChanged;
    private bool _OpenPressed;
    private bool _ClosePressed;
    // Start is called before the first frame update
    void Start()
    {
        ParentStart();
        PlayAnimation();
    }

    public override void RestartLevel()
    {
        base.RestartLevel();

            PlayAnimation();

    }

    public override StepInfo BeforeStep()
    {
        if (!allInputToggles)
        {
            _ClosePressed = _inputGestion.GetDown(inputClose);
            _OpenPressed = _inputGestion.GetDown(inputOpen);
            if (!_ClosePressed)
            {
                _ClosePressed = _inputGestion.GetDown(alternativeClose);
            }

            if (_ClosePressed && canPassThrough && !_levelGridManager.CellOccupied(transform.position, this))
            {
                canPassThrough = false;
                _stateChanged = true;
            }
            else if (_OpenPressed && !canPassThrough)
            {
                canPassThrough = true;
                _stateChanged = true;
            }
            else if (_previousCanPassThrough != canPassThrough)
            {
                _stateChanged = true;
            }
        }
        else
        {
            if (_inputGestion.GetDown(TouchPressed.Up) || _inputGestion.GetDown(TouchPressed.Down)
                                                       || _inputGestion.GetDown(TouchPressed.Left) ||
                                                       _inputGestion.GetDown(TouchPressed.Right))
            {
                _OpenPressed = true;
                _ClosePressed = true;
                if (canPassThrough && !_levelGridManager.CellOccupied(transform.position, this))
                {
                    _stateChanged = true;
                    canPassThrough = !canPassThrough;
                }
                else if (!canPassThrough)
                {
                    _stateChanged = true;
                    canPassThrough = !canPassThrough;
                }

            }
            else if (_previousCanPassThrough != canPassThrough)
            {
                _stateChanged = true;
            }
        }

        if (_stateChanged)
            PlayAnimation();
        return base.BeforeStep();
    }

    // Update is called Opence per frame
    public override void Step()
    {
    }

    public override void LateStep()
    {
        if (!_stateChanged)
        {
            // Si c'est ouvert et que la case est libre -> on peut fermer
            if (_ClosePressed && canPassThrough && !_levelGridManager.CellOccupied(transform.position, this))
            {
                canPassThrough = false;
                PlayAnimation();
            }
        }
        
        _stateChanged = false;
        _previousCanPassThrough = canPassThrough;
    }

    private void PlayAnimation()
    {
        bool previous = animator.GetBool("Open");
        animator.SetBool("Open", canPassThrough);

        if (previous != canPassThrough)
        {
            sourceDoor.Play();
        }
        
    }
}
