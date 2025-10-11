using UnityEngine;

public class Enemy : Entity
{
    private Config config;

    [SerializeField]
    private string targetTag = "Player";

    [SerializeField]
    private Transform targetPosition;

    [SerializeField]
    private CharacterController controller;

    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float rotationSpeed = 5f;

    [SerializeField]
    private bool isPlayerInRange = false;

    // attack

    [SerializeField]
    private float attackCooldown = 1f;

    [SerializeField]
    private int attackDamage = 50;

    [SerializeField]
    private float attackRange = 2f;

    [SerializeField]
    private Transform attackPoint;

    [SerializeField]
    private LayerMask playerMask;

    private float lastAttackTime = -Mathf.Infinity;

    protected override void Awake()
    {
        base.Awake();
        controller = GetComponent<CharacterController>();
        config = new Config();
    }

    void Update()
    {
        if (isPlayerInRange && targetPosition)
        {
            Vector3 direction = (targetPosition.position - transform.position).normalized;
            direction.y = config.GRAVITY;
            float distanceFromTarget = Vector3.Distance(transform.position, targetPosition.position);

            RotateTowardsPlayer(direction);
            if (distanceFromTarget > attackRange)
            {
                MoveTowardsPlayer(direction);
            }
            else
            {
                Attack();
            }
        }
    }

    void RotateTowardsPlayer(Vector3 direction)
    {
        // Face the player
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void MoveTowardsPlayer(Vector3 direction)
    {
        controller.Move(direction * moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            Debug.Log("Player is on range" + other.transform.position);
            isPlayerInRange = true;
            targetPosition = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        isPlayerInRange = false;
        targetPosition = null;
    }

    void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            Collider[] hitObjects = Physics.OverlapSphere(attackPoint.position, attackRange, playerMask);
            foreach (Collider hitObject in hitObjects)
            {
                Debug.Log("Attacking Player!");
                hitObject.GetComponent<Entity>()?.TakeDamage(attackDamage);
            }

        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(attackPoint.position, attackRange);
    }
}
