using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public void PlayerTakeDamage(int damage)
    {
        GetComponentInParent<Player>().health -= damage;
    }

    public void EnemyTakeDamage(int damage)
    {
        GetComponentInParent<Enemy>().health -= damage;
    }
}
