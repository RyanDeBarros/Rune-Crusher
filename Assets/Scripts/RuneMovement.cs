using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneMovement : MonoBehaviour
{
    public Vector2Int cellOffset;

    public void Move(int x, int y)
    {
        transform.position = new Vector2(transform.position.x + x * cellOffset.x,
            transform.position.z + y * cellOffset.y);
    }
}
