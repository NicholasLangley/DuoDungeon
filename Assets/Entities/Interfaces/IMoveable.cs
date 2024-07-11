using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementDirection { NONE, FORWARD, BACKWARD, LEFT, RIGHT }

public interface IMoveable
{
    public static MovementDirection ReverseMovementDirection(MovementDirection dir)
    {
        switch(dir)
        {
            case MovementDirection.FORWARD:
                return MovementDirection.BACKWARD;
            case MovementDirection.BACKWARD:
                return MovementDirection.FORWARD;
            case MovementDirection.RIGHT:
                return MovementDirection.LEFT;
            case MovementDirection.LEFT:
                return MovementDirection.RIGHT;
            default:
                return MovementDirection.NONE;
        }
    }

    //Movement variables
    MovementDirection movementDirection { get; set; }
    float movementLerpDuration { get; set; }

    LayerMask movementCollisionMask { get; set; }

    bool affectedByGravity { get; set; }
    float fallLerpDuration { get; set; }

    float degreesToRotate { get; set; }

    public void MoveTo(MovementDirection dir);

    public void FailToMoveTo(MovementDirection dir);

    public void RotateBy(float degrees);

    public void Fall();

    public Vector3 GetProjectedDestinationPosition(MovementDirection dir);
}
