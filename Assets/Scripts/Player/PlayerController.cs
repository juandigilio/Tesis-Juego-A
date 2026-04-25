using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerView))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference _moveIAR;
    [SerializeField] private PlayerSettings _settings;
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
    }

    private void OnDisable()
    {
        _moveIAR.action.performed -= OnMove;
        _moveIAR.action.canceled -= OnMove;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        input.y = 0;
        _model.SetMoveXInput(input.x);
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
    }
}