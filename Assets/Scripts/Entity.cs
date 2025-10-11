using UnityEngine;
public class Entity: MonoBehaviour
{
    [SerializeField]
    protected int fullHp = 100;
    private int hp = 0;

    protected virtual void Awake()
    {
        hp = fullHp;
    }
    public virtual void TakeDamage(int damage)
    {
        Debug.Log("Taking Damage: " + damage);
        hp -= damage;
        if (hp <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        Destroy(gameObject);
    }
}