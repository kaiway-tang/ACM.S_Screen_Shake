using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] Vector2[] knockbackVectors;
    [SerializeField] Collider2D m_collider;
    [SerializeField] GameObject slashFX;
    [SerializeField] int traumaAmount;

    bool knockbackDirection;
    // Start is called before the first frame update
    
    public void Activate(bool direction)
    {
        knockbackDirection = direction;
        m_collider.enabled = true;
    }

    public void Deactivate()
    {
        m_collider.enabled = false;
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8) //if hitting an entity
        {
            DealKnockback(collision);
            CreateSlashFX(collision);
        }
    }

    void DealKnockback(Collider2D collision)
    {
        if (knockbackDirection)
        {
            collision.GetComponent<Rigidbody2D>().velocity = knockbackVectors[0];
        }
        else
        {
            collision.GetComponent<Rigidbody2D>().velocity = knockbackVectors[1];
        }
    }

    void CreateSlashFX(Collider2D collision)
    {
        Instantiate(slashFX, collision.transform.position, Quaternion.identity).transform.parent = collision.transform;
    }
}
