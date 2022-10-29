using UnityEngine;

public class Player : MonoBehaviour
{
    // スピード関連
    private float _speed = 5f;
    private float _run_speed_rate = 2f;
    private float _crouch_speed_rate = 0.5f;
    private float _look_sensitive_x = 0.1f;
    private float _look_sensitive_y = 0.05f;

    // Playerの高さ・半径
    private float _player_height = 1.9f;
    private float _player_radius = 0.35f;
    private float _player_crouch_height = 1.3f;
    private float _player_crouch_radius = 0.55f;

    // Object
    private GameObject _camTarget;
    private PlayerInputHandler _input;
    private CharacterController _characon;
    private Animator _animator;

    // State
    private bool isCrouching = false;

    private void Awake()
    {
        TryGetComponent(out _input);
        TryGetComponent(out _characon);
        _camTarget = transform.Find("CameraTarget").gameObject;
        _animator = transform.GetComponentInChildren<Animator>();
    }

    private void Aim()
    {
        Vector2 direction = _input.GetLook();
        if (direction == Vector2.zero) return;
        float horizontalAngle = direction.x * _look_sensitive_x;
        float verticalAngle = direction.y * _look_sensitive_y * -1;
        transform.Rotate(0, horizontalAngle, 0, Space.World);
        Vector3 camAngles = _camTarget.transform.eulerAngles;
        float x = camAngles.x;
        if (x > 180) x -= 360;
        verticalAngle = Mathf.Clamp(verticalAngle + x, -90f, 90f);
        _camTarget.transform.rotation = Quaternion.Euler(verticalAngle, camAngles.y, camAngles.z);
    }

    private void Move()
    {
        Vector2 direction = _input.GetMove();
        float speed = _input.GetRun() ? _speed * _run_speed_rate : _speed;
        if (_input.GetCrouch())
        {
            speed = _speed * _crouch_speed_rate;
        }

        if (direction == Vector2.zero)
        {
            _animator.SetFloat("MoveSpeed", 0);
        }
        else
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            Vector3 targetDirection = Quaternion.Euler(0, targetAngle, 0) * transform.forward;
            _characon.Move(targetDirection * speed * Time.deltaTime);

            _animator.SetFloat("MoveSpeed", speed);
        }

        _animator.SetFloat("MoveForward", direction.y);
        _animator.SetFloat("MoveRight", direction.x);
        _animator.SetBool("IsGrounded", true);
    }

    private void UpdatePlayerHeight()
    {
        if (isCrouching != _input.GetCrouch())
        {
            isCrouching = _input.GetCrouch();
            _animator.SetBool("IsCrouching", isCrouching);
            if (isCrouching)
            {
                _characon.height = _player_crouch_height;
                _characon.radius = _player_crouch_radius;
                _characon.center = new Vector3(0, _player_crouch_height / 2f, 0);
            }
            else
            {
                _characon.height = _player_height;
                _characon.radius = _player_radius;
                _characon.center = new Vector3(0, _player_height / 2f, 0);
            }
        }
    }

    private void Update()
    {
        Aim();
        UpdatePlayerHeight();
        Move();
    }
}
