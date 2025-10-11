using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class Player : Entity
{
    public Transform cameraTransform;
    private Config config;

    private PlayerAction actions;
    private Vector2 moveInput;
    private CharacterController controller;
    

    [SerializeField]
    private float moveSpeed = 10f;

    [SerializeField]
    private float rotationSpeed = 750f;

    // Attack
    public Transform attackPoint;
    public LayerMask attackableLayer;
    public int attackDamage = 10;
    public float attackRange = 1.5f;

    protected override void Awake()
    {
        base.Awake();
        config = new Config();
        actions = new PlayerAction();
        controller = GetComponent<CharacterController>();

        actions.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        actions.Movement.Move.canceled += ctx => moveInput = Vector2.zero;

        actions.Movement.Attack.performed += ctx => Attack();
    }

    void Update()
    {
        Vector3 direction = new Vector3(moveInput.x, 0, moveInput.y);

        Vector3 cameraForward = new Vector3(cameraTransform.forward.x, 0f, cameraTransform.forward.z).normalized;
        Vector3 cameraRight = new Vector3(cameraTransform.right.x, 0f, cameraTransform.right.z).normalized;

        // rotate and move player based on camera direction
        Vector3 move = cameraForward * direction.z + cameraRight * direction.x;
        move.y = config.GRAVITY;


        if (move.magnitude >= 0.01f)
        {
            MovePlayer(move);
            RotatePlayer(move);
        }
    }

    void MovePlayer(Vector3 move)
    {
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    private void RotatePlayer(Vector3 moveDirection)
    {
        Vector3 flatRotation = new Vector3(moveDirection.x, 0, moveDirection.z);
        // rotate if player rotation is not zero
        if (flatRotation.sqrMagnitude > 0.001f)
        {
            Quaternion rotateDirection = Quaternion.LookRotation(flatRotation);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateDirection, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnEnable()
    {
        actions.Enable();
    }

    private void OnDisable()
    {
        actions.Disable();
    }

    void Attack()
    {
        Debug.Log("Attacking!");
        Collider[] hitObjects = Physics.OverlapSphere(attackPoint.position, attackRange, attackableLayer);
        foreach (Collider hitObject in hitObjects)
        {
            Debug.Log("Hit!");
            hitObject.GetComponent<Entity>()?.TakeDamage(attackDamage);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
