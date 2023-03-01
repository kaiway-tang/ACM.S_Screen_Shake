using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashFX : MonoBehaviour
{
    [SerializeField] Vector3 initialScale, changeScale;
    [SerializeField] int life;
    Transform trfm;
    static Vector3 vect3;
    // Start is called before the first frame update

    private void Start()
    {
        trfm = transform;
        trfm.localScale = initialScale;
        vect3.z = 1;

        trfm.Rotate(Vector3.forward * Random.Range(0,360));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        vect3.x = trfm.localScale.x * changeScale.x;
        vect3.y = trfm.localScale.y + changeScale.y;
        trfm.localScale = vect3;

        life--;
        if (life < 1) { Destroy(gameObject); }
    }
}
