using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveable
{
    //Movement variables
    Vector3 srcPosition { get; set; }
    Vector3 destPosition { get; set; }
    float movementLerpDuration { get; set; }

    LayerMask movementCollisionMask { get; set; }

    bool affectedByGravity { get; set; }

    float degreesToRotate { get; set; }

    public void MoveTo(Vector3 dest);

    public void FailToMoveTo(Vector3 dest);

    public void RotateBy(float degrees);
}
