#region Namespaces/Directives

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

#endregion

public class PlayerCharacter : MonoBehaviour, IDamageable
{
    #region Declarations

    [Header("Movement Settings")]
    [SerializeField] private float _jumpForce;
    private int _weaponIndex = 0;
    [SerializeField] private float groundCheckDistance;    
    [SerializeField] private bool _isWalled;
    [SerializeField] private State _currentState;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private LayerMask _hLayerMask;
    [SerializeField] private int _jumpcount;
    private Vector3 _targetPosition;
    private MeshRenderer _meshRenderer;
    private bool _aGrappled;
    [SerializeField] private float _swingSpeed;
    [SerializeField] private GameObject weapon1;
    [SerializeField] private GameObject weapon2;


    private Player_control controls; 

    private Vector2 moveInput;


    public UnityEvent OnPlayerDeath;
    public UnityEvent OnPlayerJump;
    public UnityEvent<bool> OnGroundedChanged;


    [Header("My References")]
    private Rigidbody _rigidBody;
    private BaseGun _equippedGun;
    private Sci_FiGun _FiGun;
    [SerializeField] private GameObject _vFX;
    private LineRenderer _lineRenderer;

    [SerializeField] private int _health = 100;
    private int _deathchance;
    private GameManager _gameManager;
    private UIManager _uiManager;
    [SerializeField] private Animator _animator;
    private int _selectedWeapon;
    Gun[] _guns;
    
    
    private bool _freeze;
    private bool _swinging;

    private float hookedMaxDistance;

    private Vector3 lastFakePos;

    private static PlayerCharacter _instance;

    public int Health { get => _health; set => _health = value; }
    public static PlayerCharacter Instance { get => _instance; set => _instance = value; }
    
    public LineRenderer LineRenderer { get => _lineRenderer; set => _lineRenderer = value; }
    
    public bool Freeze { get => _freeze; set => _freeze = value; }
    
    public bool Swinging { get => _swinging; set => _swinging = value; }


    #endregion

    public enum State
    {
        None,Grounded,Walled,OnAir,Freeze,Swinging
        
        
    }

    public void OnJump(InputAction.CallbackContext ctx)=>Jump();
    #region MonoBehaviour

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,2f);
    }


    
    
    
    private void Awake()
    {
        
        controls = new Player_control();
        _guns = GetComponentsInChildren<Gun>();

        ChangeWeapon();

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

    void ChangeWeapon()
    {
        foreach (Gun gun in _guns)
        {
            gun.gameObject.SetActive(false);
        }
        _guns[_weaponIndex].gameObject.SetActive(true);
    }

    void OnEnable()
    {
        controls.locomotion.sprint.performed += ctx => moveInput *= 2;
        controls.locomotion.walk.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.locomotion.walk.canceled += ctx => moveInput = Vector3.zero;

       // controls.locomotion.hook.performed += ctx => 
        controls.actions.shot.performed += ctx => _guns[_weaponIndex].Fire();
        controls.actions.reload.performed += ctx => ReloadGun();
        controls.actions.taunt.performed += ctx => taunt();
        controls.Enable();
    }

    void OnDisable()    
    {
        controls.Disable();
    }



    private void Start()
    {
        _vFX.SetActive(false);
        _gameManager = GameManager.Instance;
        _uiManager = UIManager.Instance;
    }

    void Update()
    {
        if ( _aGrappled) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _weaponIndex = 0 ;
            ChangeWeapon();
        }


        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _weaponIndex = 1;
            ChangeWeapon();
        }

        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        transform.Translate(move * Time.deltaTime * 5f);


       
        _uiManager.HealthUpdate(Health);
        

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
        else if (_freeze)
        {
         _currentState = State.Freeze;
         _rigidBody.velocity = Vector3.zero;
        }
        else if (_swinging)
        {
            _currentState = State.Swinging;
            moveInput *= _swingSpeed;
        }
        else
        {
            _currentState = State.OnAir; 
        }
        
        //MoveInDirection(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
    }


    public void ResetRestrictions()
    {
        _aGrappled = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();
            GetComponent<Hook>().StopGrapple();
        }
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
    
    private void Interact()
    {

    }

    private void ReloadGun()
    {
        _guns[_weaponIndex].Reload();
    }

    private void Jump()
    {
        

        if (_currentState == State.Grounded || _currentState == State.Walled && _jumpcount >= 1)
        {
            _jumpcount--;
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.AddForce(Vector2.up * _jumpForce, ForceMode.Impulse);
            OnPlayerJump?.Invoke();
        }
    }
    private void taunt()
    {
       
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

    private bool enableMovementOnNextTouch;
    
    
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        _aGrappled = true;
        _rigidBody.velocity = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        
        
       Invoke(nameof(ResetRestrictions), 3f);
    }

    private Vector3 velocityToSet;

    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        _rigidBody.velocity = velocityToSet;
    }

    private IEnumerator RussianRollet()
    {
        
        _animator.SetTrigger("Death");
        yield return new WaitForSeconds(0.80f);
        _vFX.SetActive(true);
        yield return new WaitForSeconds(0.50f);
        this.TakeDamage(100);
        



    }

    public Vector3 CalculateJumpVelocity(Vector3 StartPos, Vector3 EndPos, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementeY = EndPos.y - StartPos.y;
        Vector3 displacementXZ = new Vector3(EndPos.x - StartPos.x, 0, EndPos.z - StartPos.z);
        
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity ) 
        + Mathf.Sqrt(2 *(displacementeY - trajectoryHeight) / gravity));
        
        return velocityXZ + velocityY;
    }

    

}
