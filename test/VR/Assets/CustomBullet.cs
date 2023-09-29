using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBullet : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask whatIsEnemies;

    //stats
    [Range(0f, 1f)]
    public float bounciness;
    public bool useGravity;

    //Damage
    public int explosionDamage;
    public float explosionRange;
    public float explosionForce;

    //lifetime
    public int maxCollisions;
    public float maxLifetime;
    public bool explodeOnTouch;

    int collisions;
    PhysicMaterial physics_mat;

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        //when to boom boom
        if (collisions > maxCollisions)
            Explode();

        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Explode();
    }

    private void Explode()
    {
        if (explosion != null)
            Instantiate(explosion, transform.position, Quaternion.identity);

        //check for opps
        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
        for(int i = 0; i < enemies.Length; i++)
        {
            //enemies[i].GetComponent<Enemy>().TakeDamage(explosionDamage);

            if(enemies[i].GetComponent<Rigidbody>())
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRange);
        }

        //lil delay
        Invoke("Delay", 0.05f);
    }

    private void Delay()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("BOULLET")) return;

        collisions++;

        //Explode if bullets hit enemy + explodeOnTouch is activated
        if (collision.collider.CompareTag("Enemy") && explodeOnTouch)
            Explode();

    }
    private void Setup()
    {
        //new physic material
        physics_mat = new PhysicMaterial();
        physics_mat.bounciness = bounciness;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;
        //assign material to collider
        GetComponent<SphereCollider>().material = physics_mat;

        //set gravity
        rb.useGravity = useGravity;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
