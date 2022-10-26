using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput _input;
    private Vector2 _move, _look;
    private bool _run = false;

    private void Awake()
    {
        TryGetComponent(out _input);
    }

    private void OnEnable()
    {
        _input.actions["Move"].performed += OnMove;
        _input.actions["Move"].canceled += OnMove;
        _input.actions["Look"].performed += OnLook;
        _input.actions["Look"].canceled += OnLook;
        _input.actions["Run"].started += OnRunStart;
        _input.actions["Run"].canceled += OnRunStop;
    }

    private void OnDisable()
    {
        _input.actions["Move"].performed -= OnMove;
        _input.actions["Move"].canceled -= OnMove;
        _input.actions["Look"].performed -= OnLook;
        _input.actions["Look"].canceled -= OnLook;
        _input.actions["Run"].started -= OnRunStart;
        _input.actions["Run"].canceled -= OnRunStop;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector2>();
    }
    private void OnLook(InputAction.CallbackContext context)
    {
        _look = context.ReadValue<Vector2>();
    }

    private void OnRunStart(InputAction.CallbackContext context)
    {
        _run = true;
    }

    private void OnRunStop(InputAction.CallbackContext context)
    {
        _run = false;
    }

    public Vector2 GetMove()
    {
        return _move;
    }

    public Vector2 GetLook()
    {
        return _look;
    }

    public bool GetRun()
    {
        return _run;
    }

}
