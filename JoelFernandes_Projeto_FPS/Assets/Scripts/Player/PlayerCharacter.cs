#region Namespaces/Directives

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

#endregion

public class PlayerCharacter : MonoBehaviour, IDamageable
{
    #region Declarations

    [Header("Movement Settings")]
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
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float archPower = 1;

    private float hookedMaxDistance;

    private Vector3 lastFakePos;

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
        controls.locomotion.sprint.performed += ctx => moveInput *= 2;
        controls.locomotion.walk.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.locomotion.walk.canceled += ctx => moveInput = Vector3.zero;

        controls.locomotion.hook.performed += ctx => HookShot();
        controls.actions.shot.performed += ctx => FireGun();
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
        Hooked = false;
        _vFX.SetActive(false);
        _gameManager = GameManager.Instance;
        _uiManager = UIManager.Instance;
    }

    void Update()
    {
        if (controls.locomotion.jump.triggered)
        {
            Jump();
        }
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


            lastFakePos = Vector3.MoveTowards(lastFakePos, _targetPosition, _hookSpeed * Time.deltaTime);

            float currentDistance = Vector3.Distance(lastFakePos, _targetPosition);

            float percentage = currentDistance/hookedMaxDistance;

            float yOffset = curve.Evaluate(percentage) * archPower;

            transform.position = lastFakePos + new Vector3(0,yOffset,0);

            if(Vector3.Distance(transform.position, _targetPosition) < 2)
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
    
        
        
    



    private void HookShot()
    {
        _hookClone = Instantiate(_hook, transform.position+new Vector3(0,1,0), Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up));
        _hookClone.Movement(Camera.main.transform.forward);
        
        

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
        lastFakePos = transform.position;
        hookedMaxDistance = Vector3.Distance(transform.position,targetPosition);
        _targetPosition = targetPosition;
        _rigidBody.useGravity = false;
        _rigidBody.velocity = Vector3.zero;
    }

    public void Push(Transform _parent)
    {
        _hooked = true;
        _parent.parent = this.transform;
    }

}
