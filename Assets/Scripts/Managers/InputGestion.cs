
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputGestion : MonoBehaviour
{
    public enum TouchPressed
    {
        Up = 0,
        Down = 1,
        Right = 2,
        Left = 3,
        Restart = 4,
        Skip = 5
    }

    public KeyCode UpKey = KeyCode.UpArrow;
    public KeyCode DownKey = KeyCode.DownArrow;
    public KeyCode RightKey = KeyCode.RightArrow;
    public KeyCode LeftKey = KeyCode.LeftArrow;
    public KeyCode RestartKey = KeyCode.R;
    public KeyCode SkipKey = KeyCode.S;
    public KeyCode QuitKey = KeyCode.Q;
    
    bool[] buttonPressed = new bool[(int)TouchPressed.Skip + 1];
    //bool _listenInputs = true;
    private int _waitPoolLength;
    private LevelGridManager _levelGridManager;

    void Start()
    {
        _levelGridManager = FindObjectOfType<LevelGridManager>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(QuitKey))
        {
            Application.Quit();
        }
        //Reset inputs
        for(int i = 0; i < buttonPressed.Length; i++)
        {
            buttonPressed[i] = false;
        }

        if (_waitPoolLength > 0)
            return;
        
        if (Input.GetKeyDown(UpKey))
        {
            buttonPressed[(int)TouchPressed.Up] = true;
        }
        else if (Input.GetKeyDown(DownKey))
        {
            buttonPressed[(int)TouchPressed.Down] = true;
        }
        else if (Input.GetKeyDown(LeftKey))
        {
            buttonPressed[(int)TouchPressed.Left] = true;
        }
        else if (Input.GetKeyDown(RightKey))
        {
            buttonPressed[(int)TouchPressed.Right] = true;
        }
        else if (Input.GetKeyDown(RestartKey))
        {
            buttonPressed[(int)TouchPressed.Restart] = true;
        }
        else if (Input.GetKeyDown(SkipKey))
        {
            buttonPressed[(int) TouchPressed.Skip] = true;
        }

        if (buttonPressed.Contains(true))
        {
            _levelGridManager.LaunchStep();
        }
    }

    public void SetListenState(bool state)
    {
        if (state == false)
        {
            _waitPoolLength++;
        }
        else
        {
            _waitPoolLength --;
        }

        if (_waitPoolLength <= 0)
        {
            _waitPoolLength = 0;
        }
        
    }

    // Return true if listening input and good key is down
    public bool GetDown(TouchPressed touch)
    {
        return buttonPressed[(int) touch];
    }
}
