using UnityEngine;
public class Entity: MonoBehaviour
{
    [SerializeField]
    protected int fullHp = 5;
    private int hp = 0;

    protected virtual void Awake()
    {
        hp = fullHp;
    }
    public virtual void TakeDamage(int damage)
    {
        Debug.Log("Taking Damage: " + damage);
        Debug.Log("current HP: " + hp);
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