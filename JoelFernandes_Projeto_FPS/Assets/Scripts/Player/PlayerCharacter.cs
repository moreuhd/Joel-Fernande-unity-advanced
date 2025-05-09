#region Namespaces/Directives

using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

#endregion

public class PlayerCharacter : MonoBehaviour, IDamageable
{
    #region Declarations

    [Header("Movement Settings")]
    private float _movementSpeed;
    [SerializeField] private float _runningSpeed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float groundCheckDistance;    
    [SerializeField] private bool _isWalled;
    [SerializeField] private State _currentState;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private LayerMask _hLayerMask;
    [SerializeField] private int _jumpcount;
    [SerializeField] private float _hookSpeed;
    private Vector3 _targetPosition;
    private bool _hooked;
    private Hook _hookClone;
    private MeshRenderer _meshRenderer;


    private Player_control controls; 

    private Vector2 moveInput;


    public UnityEvent OnPlayerDeath;
    public UnityEvent OnPlayerJump;
    public UnityEvent<bool> OnGroundedChanged;


    [Header("My References")]
    private Rigidbody _rigidBody;
    private BaseGun _equippedGun;
    [SerializeField] private Hook _hook;
    [SerializeField] private GameObject _vFX;
    private LineRenderer _lineRenderer;

    [SerializeField] private int _health = 100;
    private int _deathchance;
    private GameManager _gameManager;
    private UIManager _uiManager;
    [SerializeField] private Animator _animator;

    private static PlayerCharacter _instance;

    public int Health { get => _health; set => _health = value; }
    public bool Hooked { get => _hooked; set => _hooked = value; }
    public static PlayerCharacter Instance { get => _instance; set => _instance = value; }
    public float HookSpeed { get => _hookSpeed; set => _hookSpeed = value; }
    public LineRenderer LineRenderer { get => _lineRenderer; set => _lineRenderer = value; }


    #endregion

    private enum State
    {
        None,Grounded,Walled,OnAir
    }


    #region MonoBehaviour

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,2f);
    }


    private void Awake()
    {


        controls = new Player_control();
        _equippedGun = GetComponentInChildren<BaseGun>();
        _rigidBody = GetComponent<Rigidbody>();
        LineRenderer = GetComponent<LineRenderer>();

        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        _meshRenderer = GetComponent<MeshRenderer>();
        _animator = GetComponent<Animator>();

       
        if (OnPlayerDeath == null) OnPlayerDeath = new UnityEvent();
        if (OnPlayerJump == null) OnPlayerJump = new UnityEvent();
        
    }

    void OnEnable()
    {
        controls.locomotion.walk.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.locomotion.walk.canceled += ctx => moveInput = Vector3.zero;
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }



    private void Start()
    {
        Hooked = false;
        _vFX.SetActive(false);
        _gameManager = GameManager.Instance;
        _uiManager = UIManager.Instance;
    }

    void Update()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        transform.Translate(move * Time.deltaTime * 5f);


        if (_hookClone != null)
        {
            LineRenderer.enabled = true;
            LineRenderer.SetPosition(0, transform.position);
            LineRenderer.SetPosition(1, _hookClone.transform.position);
        }
        _uiManager.HealthUpdate(Health);
  
        if (_hooked == true)
        {
            LineRenderer.SetPosition(0, transform.position);
            LineRenderer.SetPosition(1, _targetPosition);
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _hookSpeed * Time.deltaTime);

            if(Vector3.Distance(transform.position, _targetPosition) < 1)
            {
                _rigidBody.useGravity = true;
                _hooked = false;
                LineRenderer.enabled = false;
            }
            


            return;
        }



        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance))
        {
            _currentState = State.Grounded;
            _jumpcount = 3;
        }
        else if (Physics.OverlapSphere(transform.position, 0.75f,_layerMask).Length >0) 
        {
            _currentState = State.Walled;
        }
        else
        {
            
            _currentState = State.OnAir;
            }
 
        Debug.DrawRay(transform.position, Vector2.down * groundCheckDistance, Color.yellow);
        PlayerInput();
        Run();
        //MoveInDirection(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
    }

    private void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        IConsumeables consumeables = other.GetComponent<IConsumeables>();
        if (consumeables != null)
        {
            consumeables.Collect();
        }
        else if(interactable != null)
        {
            interactable.interactable();
        }
    }



    #endregion

    private void PlayerInput()
    {
        if(Input.GetKey(KeyCode.Mouse0))
        {
            FireGun();
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            OnPlayerJump?.Invoke();
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadGun();
        }
        if( Input.GetKeyDown(KeyCode.Mouse1) && Hooked == false)
        {
            HookShot();
        }
        if (Input.GetKeyDown(KeyCode.T ))
        {
            taunt();
        }
    }



    private void HookShot()
    {
        _hookClone = Instantiate(_hook, transform.position, Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up));
        _hookClone.Movement(Camera.main.transform.forward);
        
        

    }

    private void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _movementSpeed = _runningSpeed;
        }
        else
        {
            _movementSpeed = _walkSpeed;
        }
    }

    private void Interact()
    {

    }

    private void FireGun()
    {
       
            _equippedGun?.Fire();
            
    }

    private void ReloadGun()
    {
        _equippedGun.Reload();
    }

    private void Jump()
    {
        

        if (_currentState == State.Grounded || _currentState == State.Walled && _jumpcount >= 1)
        {
            _jumpcount--;
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.AddForce(Vector2.up * _jumpForce, ForceMode.Impulse);
        }
        
    }


    //private void MoveInDirection(Vector2 direction)
    //{
    //    Vector3 finalVelocity = (direction.x * transform.right + direction.y * transform.forward).normalized * _movementSpeed;

    //    finalVelocity.y = _rigidBody.velocity.y;
    //    _rigidBody.velocity = finalVelocity;
    //}

    private void taunt()
    {
        Debug.Log("aaa");
        _deathchance = Random.Range(0, 7);
        if (_deathchance == 6)
        {
            StartCoroutine(RussianRollet());
        }
        else
        {
            _animator.SetTrigger("Taunt");
        }
    }


    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            OnPlayerDeath?.Invoke();
            _gameManager.PlayerDeath();
        }
    }

    private IEnumerator RussianRollet()
    {
        
        _animator.SetTrigger("Death");
        yield return new WaitForSeconds(0.80f);
        _vFX.SetActive(true);
        yield return new WaitForSeconds(0.50f);
        this.TakeDamage(100);
        



    }

    public void Attract(Vector3 targetPosition)
    {
        _hooked = true;
        _targetPosition = targetPosition;
        _rigidBody.useGravity = false;
        _rigidBody.velocity = Vector3.zero;
    }
}
