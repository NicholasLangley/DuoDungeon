using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : EnvironmentObject
{
    bool currentlyColliding, isActive;
    PlayerEntity player;

    // Start is called before the first frame update
    void Start()
    {
        currentlyColliding = false;
        player = null;
        isActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void CollectPickup(Entity entity)
    {
        Debug.Log("Pickup Activated");
        isActive = false;
        GetComponent<MeshRenderer>().enabled = false;
    }

    public virtual void UncollectPickup(Entity entity)
    {
        Debug.Log("Pickup Deactivated");
        isActive = true;
        GetComponent<MeshRenderer>().enabled = true;
    }

    #region Commands
    //Active decisions by the entity such as to move or attack
    public override Command GetCommand() { return null; }

    //commands that arise from the enemies current environment (sliding on ice, or falling in a hole for example)
    public override Command GetPassiveCommand() 
    {
        if (currentlyColliding && isActive && player != null)
        {
            return new PickupCommand(this, player);
        }
        return null;
    }

    #endregion

    #region collision

    void OnTriggerEnter(Collider col)
    {
        GameObject collideObject = col.gameObject;
        if(collideObject.CompareTag("Player"))
        {
            currentlyColliding = true;
            player = collideObject.GetComponent<PlayerEntity>();
        }
    }

    void OnTriggerExit(Collider col)
    {
        GameObject collideObject = col.gameObject;
        if (collideObject.CompareTag("Player"))
        {
            currentlyColliding = false;
            if (player != collideObject.GetComponent<PlayerEntity>()) { player = null; }
        }
    }

    #endregion
}
