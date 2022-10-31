using UnityEngine;

public class Player : MonoBehaviour
{
    // スピード関連
    private float _gravity = -9.8f;
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
    private bool isCrouching, isGrounded;
    private float distanceFromGround;
    private Vector3 verocity = Vector3.zero;

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
        float speed = GetMoveSpeed();
        if (isGrounded)
        {
            if (direction == Vector2.zero)
            {
                verocity = Vector3.zero;
                speed = 0;
            }
            else
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
                Vector3 targetDirection = Quaternion.Euler(0, targetAngle, 0) * transform.forward;
                verocity = targetDirection * speed;
            }
        }
        JumpAndGravity();
        _characon.Move(verocity * Time.deltaTime);
        _animator.SetFloat("MoveSpeed", speed);
        _animator.SetFloat("MoveForward", direction.y);
        _animator.SetFloat("MoveRight", direction.x);
        _animator.SetBool("IsGrounded", isGrounded);
    }

    private float GetMoveSpeed()
    {
        if (isCrouching)
        {
            return _speed * _crouch_speed_rate;
        }
        else
        {
            return _input.GetRun() ? _speed * _run_speed_rate : _speed;
        }
    }

    private void JumpAndGravity()
    {
        if (isGrounded)
        {
            if (_input.GetJump())
            {
                if (isCrouching)
                {
                    Crouch(false);
                }
                else
                {
                    verocity.y = 10f;
                }
            }
            else if (distanceFromGround > 0.01)
            {
                verocity.y = _gravity * Time.deltaTime;
            }
            else
            {
                verocity.y = 0;
            }
        }
        else
        {
            verocity.y += _gravity * Time.deltaTime;
        }
    }

    private void Crouch(bool crouching)
    {
        if (isCrouching == crouching) return;
        if (crouching)
        {
            _characon.height = _player_crouch_height;
            _characon.radius = _player_crouch_radius;
            _characon.center = new Vector3(0, _player_crouch_height / 2f, 0);
            isCrouching = true;
            _animator.SetBool("IsCrouching", isCrouching);
        }
        else
        {
            Vector3 start = transform.position;
            Vector3 end = transform.position;
            start.y += _characon.radius;
            end.y += _player_crouch_height - _characon.radius;
            float distance = _player_height - _player_crouch_height;
            RaycastHit hit;
            bool canNotStandUp = Physics.CapsuleCast(start, end, _characon.radius, Vector3.up, out hit, distance, -1);
            if (!canNotStandUp)
            {
                _characon.height = _player_height;
                _characon.radius = _player_radius;
                _characon.center = new Vector3(0, _player_height / 2f, 0);
                isCrouching = false;
                _animator.SetBool("IsCrouching", isCrouching);
            }
        }
    }

    private void CheckGrounded()
    {
        Vector3 start = transform.position;
        Vector3 end = transform.position;
        float height = isCrouching ? _player_crouch_height : _player_height;
        start.y += _characon.radius;
        end.y += height - _characon.radius;
        float distance = 0.05f;
        RaycastHit hit;
        isGrounded = Physics.CapsuleCast(start, end, _characon.radius, Vector3.down, out hit, distance, -1);
        distanceFromGround = hit.distance;
    }

    private void Update()
    {
        Aim();
        CheckGrounded();
        Crouch(_input.GetCrouch());
        Move();
    }
}
