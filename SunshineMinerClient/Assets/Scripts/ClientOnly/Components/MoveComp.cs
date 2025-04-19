using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveComp : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody rb;

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

        if (InputManager.Instance.GetActionDown("Rpc"))
        {
            Msg msg = new Msg("", "", "TestRpc");
            msg.AddArgInt("IntArg", 1);
            msg.AddArgFloat("FloatArg", 2f);
            msg.AddArgString("StringArg", "Hello");
            Debug.Log("Try send rpc");
            Gate.Instance.SendMsg(msg);
        }
    }
}
