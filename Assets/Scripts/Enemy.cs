using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Moving")]
    public float moveSpeed = 1;
    public float jumpForce = 1;

    [Header("Rotating")]
    public float rotateDelay = 1;

    public int playerID = 0;

    public void Move(Vector3 newPos)
    {
        transform.position = newPos;
    }
}
