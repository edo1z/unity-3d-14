using UnityEngine;

public class Player : MonoBehaviour
{
    private float _speed = 5f;
    private float _look_sensitive_x = 0.1f;
    private float _look_sensitive_y = 0.05f;

    private GameObject _camTarget;
    private PlayerInputHandler _input;
    private CharacterController _characon;

    private void Awake()
    {
        TryGetComponent(out _input);
        TryGetComponent(out _characon);
        _camTarget = transform.Find("CameraTarget").gameObject;
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
        if (direction == Vector2.zero) return;
        float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Vector3 targetDirection = Quaternion.Euler(0, targetAngle, 0) * transform.forward;
        _characon.Move(targetDirection * _speed * Time.deltaTime);
    }

    private void Update()
    {
        Aim();
        Move();
    }
}
