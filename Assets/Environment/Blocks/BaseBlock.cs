using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBlock : MonoBehaviour
{
    Collider antiMovementCollider, antiProjectileCollider;

    

    // Start is called before the first frame update
    void Start()
    {
        antiMovementCollider = null;
        antiProjectileCollider = null;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(/*bool isInbetweenBlock,*/ bool movementBlocked, bool projectilesBlocked)
    {
        if (movementBlocked)
        {
            GameObject child = new GameObject();
            child.name = "movementBlocker";
            antiMovementCollider = child.AddComponent(typeof(BoxCollider)) as BoxCollider;
            child.layer = LayerMask.NameToLayer("BlockMovement");
            child.transform.SetParent(transform);
            child.transform.localPosition = Vector3.zero;
        }

        if (projectilesBlocked)
        {
            GameObject child = new GameObject();
            child.name = "projectileBlocker";
            antiProjectileCollider = child.AddComponent(typeof(BoxCollider)) as BoxCollider;
            child.gameObject.layer = LayerMask.NameToLayer("BlockProjectiles");
            child.transform.SetParent(transform);
            child.transform.localPosition = Vector3.zero;
        }
    }
}
