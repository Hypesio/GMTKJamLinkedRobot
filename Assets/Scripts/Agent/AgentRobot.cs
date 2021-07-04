using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchPressed = InputGestion.TouchPressed;

public class AgentRobot : PhysicObject, ICanDie
{
    public enum RobotType
    {
        None,
        Normal,
        Inverse,
        Other
    }

    public RobotType robotType = RobotType.Normal;
    private int _sizeStep = 1;
    public TouchPressed upInput = TouchPressed.Up;
    public TouchPressed downInput = TouchPressed.Down;
    public TouchPressed rightInput = TouchPressed.Right;
    public TouchPressed leftInput = TouchPressed.Left;
    
    [Header("Animations")]
    public float timeToMove = 1f;
    public Animator animator;
    public GameObject prefabExplosion;
    public Animator conversionAnimator;
    public float timeConversion = 0.2f;

    [Header("State")]
    public bool converted = false;
    public bool isDown = false;
    public SpriteRenderer spriteRenderer;

    [Header("Sound")] 
    public AudioSource sourceConversion;

    public List<AudioSource> sourceWalk;

    private bool _startConverted;
    private bool _startDown;
    private bool _hasMove;
    private Vector3 _targetPosition;
    private int actualSource = 0;
    void Start()
    {
        ParentStart();
        _sizeStep = _levelGridManager.gridSize;
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        animator.SetBool("Converted", converted);
        animator.SetBool("IsDown", isDown);
        _startConverted = converted;
        _startDown = isDown;

        if (converted)
        {
            StartCoroutine(IConvertionEffect());
        }
 
    }


    public override StepInfo BeforeStep()
    {
        StepInfo infos = base.BeforeStep();
        
        // Basic move for the player
        if (!isDown)
        {
            _targetPosition = transform.position;
            if (_inputGestion.GetDown(upInput))
            {
                _targetPosition.y += _sizeStep;
            }
            else if (_inputGestion.GetDown(downInput))
            {
                _targetPosition.y -= _sizeStep;
            }
            else if (_inputGestion.GetDown(leftInput))
            {
                if (!spriteRenderer.flipX)
                {
                    spriteRenderer.flipX = true;
                }

                _targetPosition.x -= _sizeStep;
            }
            else if (_inputGestion.GetDown(rightInput))
            {
                if (spriteRenderer.flipX)
                {
                    spriteRenderer.flipX = false;
                }

                _targetPosition.x += _sizeStep;
            }
            
            // Check if robot can move to the cell 
            if (!_levelGridManager.CellContainFloor(_targetPosition, robotType))
                return infos;

            if (_targetPosition != transform.position)
            {
                infos.doNothing = false;
                infos.targetPosition = _targetPosition;
            }
        }

        return infos;
    }
    
    // Update is called once per frame
    public override void Step()
    {
        _hasMove = false;

        // Basic move for the player
        if (!isDown)
        {
            // Apply move only if player can
            if (_targetPosition != transform.position && _levelGridManager.CanGoOnCell(_targetPosition, this, robotType))
            {
                StartCoroutine(IMovePlayer(_targetPosition));
                PlayWalkSound();
                _hasMove = true;
            }
        }
    }

    private void PlayWalkSound()
    {
        actualSource++;
        if (actualSource >= sourceWalk.Count)
            actualSource = 0;
        sourceWalk[actualSource].Play();
    }

    public override void LateStep()
    {
        if (converted && !_hasMove)
        {
            PhysicObject obj = _levelGridManager.CellOccupied(_targetPosition);
            if (obj && obj is AgentRobot)
            {
                // Convert the robot
                if (((AgentRobot) obj).ConvertRobot())
                {
                    conversionAnimator.Play("Base Layer.SendConversion");
                }
            }
        }
    }

    public void die()
    {
        if (isDown)
            return;
        
        isDown = true;
        animator.SetBool("IsDown", isDown);
        
        GameObject explosion = Instantiate(prefabExplosion, transform.position, transform.rotation);
        Destroy(explosion, 5f);
    }

    // Return true if the robot is converted, false otherwise (already converted)
    public bool ConvertRobot()
    {
        if (converted && !isDown)
            return false;
        StartCoroutine(IConvertionEffect());
        return true;
    }

    public void PlayConvertion()
    {
        conversionAnimator.Play("Base Layer.SendConversion");
    }

    IEnumerator IMovePlayer(Vector3 targetPosition)
    {
        //Disable input during the move
        _inputGestion.SetListenState(false);
        
        animator.SetBool("Moving", true);
        float time = 0f;
        while (time <= timeToMove)
        {
            time += Time.deltaTime;
            Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, Mathf.SmoothStep(0f, 1f, time / timeToMove));
            transform.position = newPosition;
            yield return null;
        }
        
        animator.SetBool("Moving", false);
        transform.position = targetPosition;
        
        _inputGestion.SetListenState(true);
    }


    public override void RestartLevel()
    {
        base.RestartLevel();
        converted = _startConverted;
        animator.SetBool("Converted", converted);
        animator.SetBool("Moving", false);
        isDown = _startDown;
        animator.SetBool("IsDown", isDown);
        if (converted)
        {
            StartCoroutine(IConvertionEffect());
        }
        
    }

    public void SetHappy()
    {
        animator.SetBool("Happy", true);
    }

    IEnumerator IConvertionEffect()
    {
        _inputGestion.SetListenState(false);
        converted = true;
        
        conversionAnimator.Play("Base Layer.ReceiveConversion");
        sourceConversion.Play();
        
        yield return new WaitForSeconds(timeConversion);
        
        // Todo add sound
        if (isDown)
        {
            isDown = false;
            animator.SetBool("IsDown", isDown);
        }
        animator.SetBool("Converted", converted);
        //_levelGridManager.RobotConverted();
        
        _inputGestion.SetListenState(true);
    }
}
