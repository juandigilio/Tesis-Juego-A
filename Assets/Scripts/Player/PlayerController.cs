using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerView))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference _moveIAR;
    [SerializeField] private InputActionReference _jumpIAR;
    [SerializeField] private PlayerSettings _settings;

    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask _groundLayer;

    private ICoopCameraService _camService;
    private PlayerModel _model;
    private PlayerView _view;

    private void Awake()
    {
        _model = new PlayerModel(_settings);
        _view = GetComponent<PlayerView>();

        if (_view.Rb == null)
            _view.Rb = GetComponent<Rigidbody2D>();

        _camService = ServiceProvider.Instance.ContainsService<ICoopCameraService>() ?
            ServiceProvider.Instance.GetService<ICoopCameraService>() :
            null;
    }

    private void OnEnable()
    {
        _moveIAR.action.performed += OnMove;
        _moveIAR.action.canceled += OnMove;

        _jumpIAR.action.performed += OnJump;
    }

    private void OnDisable()
    {
        _moveIAR.action.performed -= OnMove;
        _moveIAR.action.canceled -= OnMove;

        _jumpIAR.action.performed -= OnJump;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        input.y = 0;
        _model.SetMoveXInput(input.x);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        _model.RequestJump();
    }

    private void Update()
    {
        bool grounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);

        _model.SetGrounded(grounded);
    }

    private void FixedUpdate()
    {
        float xVel = _model.MoveXInputDir * _model.Settings.speed;

        var bounds = _camService.GetBounds();

        float posX = _view.Rb.position.x;

        if ((posX <= bounds.left + bounds.margin && xVel < 0) ||
            (posX >= bounds.right - bounds.margin && xVel > 0))
        {
            xVel = 0;
        }

        _view.ApplyHVelocity(xVel);

        if (_model.JumpRequested)
        {
            _view.ApplyJumpForce(_model.Settings.jumpForce);
            _model.ConsumeJump();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
    }
}