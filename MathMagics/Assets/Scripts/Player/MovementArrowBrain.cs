using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MovementArrowBrain : MonoBehaviour
{
    [SerializeField] private PlayerMovement _pm;

    private void Awake()
    {
        if(_pm != null)
        {
            _pm.OnMovementInput += HandleMovementInput;
        }
    }

    private void OnDestroy()
    {
        if(_pm != null)
        {
            _pm.OnMovementInput -= HandleMovementInput;
        }
    }

    private void HandleMovementInput(Vector2 direction)
    {
        //ONly handle up and down, sprite fliping will handle left and right
        if(direction.y > 0)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else if (direction.y < 0)
        {
            transform.localRotation = Quaternion.Euler(0, 0, -90);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }   
}

