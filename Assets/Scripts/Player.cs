using UnityEngine;

public class Player : MonoBehaviour
{
    private float speed = 5f;

    private PlayerInputHandler _input;
    private GameObject _cam;
    private CharacterController _characon;

    private void Awake()
    {
        TryGetComponent(out _input);
        TryGetComponent(out _characon);
        _cam = GameObject.Find("Main Camera");
    }

    private void Move()
    {
        Vector2 direction = _input.GetMove();
        Vector3 move = new Vector3(direction.x, 0, direction.y);
        _characon.Move(move.normalized * speed * Time.deltaTime);
    }

    private void Update()
    {
        Move();
    }
}
