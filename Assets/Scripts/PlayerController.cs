using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController3D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // Velocidad de movimiento del jugador
    [SerializeField] private float sprintSpeed = 5f; // Velocidad de sprint del jugador
    [SerializeField] private float jumpForce = 1.2f; // Fuerza de salto del jugador
    [SerializeField] private GameObject bulletPrefab; // Prefab de la bala
    [SerializeField] private Transform firePoint; // Punto de disparo de la bala
    [SerializeField] private Transform playerObj;
    [SerializeField] private float underGroundLimit = -10f; // Límite de caída bajo el suelo
    [SerializeField] private float gravity = -9.81f; // Gravedad del jugador
    [SerializeField] private float groundedOffset = 2f;
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float fallTimeout = 0.15f;
    [SerializeField] private float jumpTimeout;
    
    private bool _isGrounded;
    private Vector3 _originalGravity;
    private PlayerInputs _input;
    private Vector3 _respawnPoint;
    private CharacterController _controller;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    private Vector3 _moveDirection;
    private float _fallTimeoutDelta;
    private float _jumpTimeoutDelta;

    void Start()
    {
        _input = GetComponent<PlayerInputs>();
        _controller = GetComponent<CharacterController>();
        _originalGravity = Physics.gravity;
        _respawnPoint = transform.position;
        _fallTimeoutDelta = fallTimeout;
        _jumpTimeoutDelta = jumpTimeout;
    }

    void Update()
    {
        CheckOutOfBounds();
        JumpAndGravity();
        GroundedCheck();
        Fire();
        Move();
    }

    private void Move()
    {
        float targetSpeed = _input.sprint ? sprintSpeed : moveSpeed;

        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        _moveDirection = new Vector3(_input.move.x * targetSpeed, _verticalVelocity, 0.0f);

        if (_input.move.x != 0)
        {
            playerObj.rotation = Quaternion.Euler(0, Mathf.Approximately(_input.move.x, 1) ? 0 : 180, 0);
        }

        _controller.Move(_moveDirection * (Time.deltaTime));
    }

    private void JumpAndGravity()
    {
        if (_isGrounded)
        {
            _fallTimeoutDelta = fallTimeout;

            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
            }
            
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = jumpTimeout;
            
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            
            _input.jump = false;
        }

        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void GroundedCheck()
    {
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, groundedOffset, groundLayers);
    }

    private void Fire()
    {
        if (_input.fire)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            _input.fire = false;
        }
    }

    private void CheckOutOfBounds()
    {
        if (transform.position.y < underGroundLimit)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        transform.position = _respawnPoint;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Respawn"))
        {
            _respawnPoint = collision.transform.position;
        }
    }
    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Obstacle"))
        {
            Respawn();
        }
    }
}