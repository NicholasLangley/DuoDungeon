using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IMoveable, ICommandable
{

    //IMoveable variables
    public Vector3 srcPosition { get; set; }
    public Vector3 destPosition { get; set; }
    public float movementLerpTimer { get; set; }

    [field: SerializeField] public float movementLerpDuration { get; set; }
    public bool moving { get; set; }


    //ICommandable variables
    public bool busy { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        //IMoveable
        moving = false;

        //ICommandable
        busy = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void MoveTo(Vector3 dest)
    {
        destPosition = dest;
        srcPosition = transform.position;
        movementLerpTimer = 0;
        moving = true;
        busy = true;
    }

    public void Move()
    {
        movementLerpTimer += Time.deltaTime;
        Vector3 movePos = Vector3.Lerp(srcPosition, destPosition, movementLerpTimer / movementLerpDuration);
        transform.position = movePos;

        if (movementLerpTimer >= movementLerpDuration) { moving = false; busy = false; }
    }

    public Command GetCommand()
    {
        return null;
    }

    public List<Command> GetCommands() { return null; }
}
