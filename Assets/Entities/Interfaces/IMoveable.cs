using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveable
{
    //Movement variables
    Vector3 srcPosition { get; set; }
    Vector3 destPosition { get; set; }
    float movementLerpTimer { get; set; }
    float movementLerpDuration { get; set; }
    bool moving { get; set; }

    public void MoveTo(Vector3 dest);

    public void Move();
}
