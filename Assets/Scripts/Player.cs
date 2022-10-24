using UnityEngine;

public class Player : MonoBehaviour
{
    private float _speed = 5f;
    private float _look_sensitive_x = 0.1f;
    private float _look_sensitive_y = 0.05f;

    private GameObject _mesh, _cam, _camParent;
    private PlayerInputHandler _input;
    private CharacterController _characon;

    private void Awake()
    {
        TryGetComponent(out _input);
        TryGetComponent(out _characon);
        _mesh = transform.Find("Mesh").gameObject;
        _camParent = transform.Find("CameraParent").gameObject;
        _cam = _camParent.transform.Find("Main Camera").gameObject;
    }

    private void Aim()
    {
        Vector2 direction = _input.GetLook();
        if (direction == Vector2.zero) return;
        float x = direction.x * _look_sensitive_x;
        float y = direction.y * _look_sensitive_y * -1;
        _camParent.transform.Rotate(0, x, 0, Space.World);
        _camParent.transform.Rotate(y, 0, 0, Space.Self);
        _mesh.transform.rotation = Quaternion.Euler(0, _camParent.transform.eulerAngles.y, 0);
    }

    private void Move()
    {
        Vector2 direction = _input.GetMove();
        if (direction == Vector2.zero) return;
        float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        targetAngle += _camParent.transform.eulerAngles.y;
        Vector3 targetDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        _characon.Move(targetDirection * _speed * Time.deltaTime);
    }

    private void Update()
    {
        Aim();
        Move();
    }
}
