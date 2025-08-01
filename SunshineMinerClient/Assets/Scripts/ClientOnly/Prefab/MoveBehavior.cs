using UnityEngine;

public class MoveBehavior : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody rb;
    public bool isPrimary = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFromInput();
    }

    private void UpdateFromInput()
    {
        if (!isPrimary) return;
        float verticalInput = 0f;
        if (InputManager.Instance.GetAction("Forward")) verticalInput += 1f;
        if (InputManager.Instance.GetAction("Backward")) verticalInput -= 1f;

        float horizontalInput = 0f;
        if (InputManager.Instance.GetAction("Rightward")) horizontalInput += 1f;
        if (InputManager.Instance.GetAction("Leftward")) horizontalInput -= 1f;

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        Vector3 moveVelocity = movement * moveSpeed;
        moveVelocity.y = rb.velocity.y;
        rb.velocity = moveVelocity;
    }
}
