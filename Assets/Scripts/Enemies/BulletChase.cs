using UnityEngine;

public class BulletChase : MonoBehaviour
{
    public Transform player;
    public float speed = 5f;
    public float rotationSpeed = 5f;

    private void Start()
    {
        // Automatically find the player by tag
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    private void Update()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Do something when hitting the player
            Debug.Log("Bullet hit the player!");

            // Example: reduce health (you can call a method on the player's script)
            // collision.GetComponent<PlayerHealth>()?.TakeDamage(1);

            Destroy(gameObject); // Destroy the bullet
        }
    }
}