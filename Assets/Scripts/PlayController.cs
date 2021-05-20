using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayController : MonoBehaviour
{
    [Header("Moving")]
    public float moveSpeed = 1;
    public float jumpForce = 1;

    [Header("Rotating")]
    public float rotateDelay = 1;

    private bool isGround = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Обработать передвижение
        Move();
    }

    // Обработка перемещения
    void Move()
    {
        Vector3 newPos = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            newPos += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            newPos += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            newPos += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            newPos += Vector3.right;
        }
        newPos = Vector3.Normalize(newPos) * moveSpeed * Time.deltaTime;
        transform.Translate(newPos, Space.Self);

        if (Input.GetKey(KeyCode.Space))
        {

            if (isGround)
            {
                Vector3 jumpVector = Vector3.up * jumpForce;
                transform.GetComponent<Rigidbody>().AddForce(jumpVector, ForceMode.Impulse);
                isGround = false;
                //print("isGround: " + isGround.ToString());
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("block"))
        {
            isGround = true;
        }
    }






    //    if ((movingJoystick.Vertical != 0.0f) || (movingJoystick.Horizontal != 0.0f))
    //    {
    //        // Обработка передвижения
    //        Vector3 moveDirection = Vector3.forward * movingJoystick.Vertical + Vector3.right * movingJoystick.Horizontal;
    //        moveDirection = Vector3.Normalize(moveDirection) * moveSpeed * Time.deltaTime;
    //        transform.Translate(moveDirection, Space.World);

    //        if ((rotatingJoystick.Vertical == 0.0f) && (rotatingJoystick.Horizontal == 0.0f))
    //        {
    //            float andgleDirection = Mathf.Atan2(moveDirection.x, moveDirection.z) * 180 / Mathf.PI;
    //            Vector3 targetAngle = new Vector3(0, andgleDirection, 0);
    //            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetAngle), rotateDelay);
    //        }
    //    }

    //    if ((rotatingJoystick.Vertical != 0.0f) && (rotatingJoystick.Horizontal != 0.0f))
    //    {
    //        Vector3 rotateDirection = Vector3.forward * rotatingJoystick.Vertical + Vector3.right * rotatingJoystick.Horizontal;
    //        float andgleDirection = Mathf.Atan2(rotateDirection.x, rotateDirection.z) * 180 / Mathf.PI;
    //        Vector3 targetAngle = new Vector3(0, andgleDirection, 0);
    //        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetAngle), rotateDelay);
    //    }
    //}
}
