using System.Collections;
using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class Player : Entity
{
    public Transform cameraTransform;
    private Config config;

    // Movement
    private PlayerAction actions;
    private Vector2 moveInput;
    private CharacterController controller;
    private Vector3 moveDirection;
    

    [SerializeField]
    private float moveSpeed = 10f;

    [SerializeField]
    private float rotationSpeed = 750f;

    // Attack
    public Transform attackPoint;
    public LayerMask attackableLayer;
    public int attackDamage = 10;
    public float attackRange = 1.5f;

    [SerializeField]
    private bool isVulnerable = false;

    // Dodge
    [SerializeField]
    private bool isDodging = false;

    [SerializeField]
    private bool canDodge = true;

    [SerializeField]
    private float dodgeDuration = 0.3f;

    [SerializeField]
    private float dodgeDistance = 6f;

    [SerializeField]
    private float dodgeCooldown = 1f;

    private float MIN_MOVEMENT_THRESHOLD = 0.1f;

    // Interact
    public LayerMask interactableMask;

    [SerializeField]
    private Transform interactPoint;

    [SerializeField]
    private float interactRange = 1.5f;

    protected override void Awake()
    {
        base.Awake();
        config = new Config();
        actions = new PlayerAction();
        controller = GetComponent<CharacterController>();
        CheckRequiredComponents();

        actions.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        actions.Movement.Move.canceled += ctx => moveInput = Vector2.zero;

        actions.Movement.Attack.performed += ctx => OnAttack();
        actions.Movement.Dodge.performed += ctx => OnDodge();
        actions.Movement.Interact.performed += ctx => OnPickUpItem();
    }

    void CheckRequiredComponents()
    {
        if (!interactPoint)
        {
            Debug.LogError("Interact point is missing");
        }
    }

    void Update()
    {
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

        Vector3 cameraForward = new Vector3(cameraTransform.forward.x, 0f, cameraTransform.forward.z).normalized;
        Vector3 cameraRight = new Vector3(cameraTransform.right.x, 0f, cameraTransform.right.z).normalized;

        // rotate and move player based on camera direction
        Vector3 move = cameraForward * moveDirection.z + cameraRight * moveDirection.x;
        move.y = config.GRAVITY;


        if (move.magnitude >= MIN_MOVEMENT_THRESHOLD && !isDodging)
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

    void OnAttack()
    {
        Debug.Log("Attacking!");
        Collider[] hitObjects = Physics.OverlapSphere(attackPoint.position, attackRange, attackableLayer);
        foreach (Collider hitObject in hitObjects)
        {
            Debug.Log("Hit!");
            hitObject.GetComponent<Entity>()?.TakeDamage(attackDamage);
        }
    }

    void OnPickUpItem()
    {
        Collider[] hitObjects = Physics.OverlapSphere(attackPoint.position, interactRange, interactableMask);
        foreach (Collider hitObject in hitObjects)
        {
           Item itemDetails = hitObject.GetComponent<ItemComponent>()?.OnPickUp();
            Debug.Log("Picked up Item: " + itemDetails.ItemName);
        }
    }

    void OnDodge()
    {
        if (isDodging || !canDodge)
        {
            return;
        }
        StartCoroutine(PerformDodge(transform.forward));
    }

    private IEnumerator PerformDodge(Vector3 direction)
    {
        canDodge = false;
        isDodging = true;
        isVulnerable = true;

        float elapse = 0f;
        Vector3 dodgeVelocity = direction * (dodgeDistance / dodgeDuration);

        while (elapse < dodgeDuration)
        {
            controller.Move(dodgeVelocity * Time.deltaTime);
            elapse += Time.deltaTime;
            yield return null;
        }

        isDodging = false;
        isVulnerable = false;
        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        if (interactPoint == null)
            return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(interactPoint.position, interactRange); 
    }

}
