using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : Entity
{
    [Header("Player aim")]
    [SerializeField]
    Transform _cameraBaseTransform;
    [SerializeField]
    Transform _cameraTransform;
    [SerializeField]
    Vector2 mouseSensitivity;
    [SerializeField]
    float maxMouseAngleY;
    public float currentAimYAngle;
    [SerializeField]
    bool invertMouseX, invertMouseY, classicDungeonCrawlerControlsEnabled;
    

    protected override void Awake()
    {
        base.Awake();
    }

    /*
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }*/
    //playerEntity ovverides this to account for direction being faced (non classic dungeoncrawler mode)
    public override MovementDirection GetFinalMovementDirection(MovementDirection dir)
    {
        if (classicDungeonCrawlerControlsEnabled) { return dir; }

        Vector3 nearestForwardLookDirection = GetNearestForwardLookDirection();

        if (nearestForwardLookDirection == transform.forward){ return dir;}

        switch (dir)
        {
            case MovementDirection.FORWARD:
                
               if (nearestForwardLookDirection == -transform.forward)
                {
                    return MovementDirection.BACKWARD;
                }
                else if (nearestForwardLookDirection == transform.right)
                {
                    return MovementDirection.RIGHT;
                }
                return MovementDirection.LEFT;


            case MovementDirection.BACKWARD:
                if (nearestForwardLookDirection == -transform.forward)
                {
                    return MovementDirection.FORWARD;
                }
                else if (nearestForwardLookDirection == transform.right)
                {
                    return MovementDirection.LEFT;
                }
                return MovementDirection.RIGHT;


            case MovementDirection.RIGHT:
                if (nearestForwardLookDirection == -transform.forward)
                {
                    return MovementDirection.LEFT;
                }
                else if (nearestForwardLookDirection == transform.right)
                {
                    return MovementDirection.BACKWARD;
                }
                return MovementDirection.FORWARD;


            case MovementDirection.LEFT:
                if (nearestForwardLookDirection == -transform.forward)
                {
                    return MovementDirection.RIGHT;
                }
                else if (nearestForwardLookDirection == transform.right)
                {
                    return MovementDirection.FORWARD;
                }
                return MovementDirection.BACKWARD;


            default:
                return movementDirection;
        }
    }

    Vector3 GetNearestForwardLookDirection()
    {
        Vector3 nearestForwardLookDirection = transform.forward;

        Vector3[] possibleDirections = new Vector3[4] { transform.forward, -transform.forward, transform.right, -transform.right };
        float minimumAbsoluteAngle = 361f;
        foreach (Vector3 direction in possibleDirections)
        {
            float absoluteAngle = Mathf.Abs(Vector3.Angle(_cameraBaseTransform.forward, direction));
            if(absoluteAngle < minimumAbsoluteAngle)
            {
                nearestForwardLookDirection = direction;
                minimumAbsoluteAngle = absoluteAngle;
            }
        }

        return nearestForwardLookDirection;
    }

    public override void Attack()
    {
        

        stateMachine.changeState(attackState);

        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, attackRange, attackMask))
        {
            IDamageable damageable = hit.collider.gameObject.GetComponent<IDamageable>();
            if(damageable != null)
            {
                //heal damage if player is undoing attack
                if (currentlyUndoing) { damageable.Heal(attackDamage); }
                else { damageable.Damage(attackDamage); }

            }
        }
    }

    public override Command GetPassiveCommand()
    {
        //checks for falling currently
        return base.GetPassiveCommand();
    }

    #region camera settings
    public void UpdateCameraAim(Vector2 mouseMovementVector)
    {
        if (classicDungeonCrawlerControlsEnabled) 
        { 
            _cameraBaseTransform.localRotation = Quaternion.identity;
            _cameraTransform.localRotation = Quaternion.identity;
            return;
        }

        if (invertMouseX) { mouseMovementVector.x *= -1; }
        if (invertMouseY) { mouseMovementVector.y *= -1; }

        _cameraBaseTransform.Rotate(_cameraBaseTransform.up, mouseMovementVector.x * mouseSensitivity.x);

        currentAimYAngle += mouseMovementVector.y * mouseSensitivity.y;
        currentAimYAngle = Mathf.Clamp(currentAimYAngle, -maxMouseAngleY, maxMouseAngleY);
        _cameraTransform.localRotation = Quaternion.Euler(-currentAimYAngle, 0, 0);
    }

    public void setMouseSensitivity(Vector2 sense)
    {
        mouseSensitivity.x = sense.x;
        mouseSensitivity.y = sense.y;
    }

    public void setMouseInversion(bool xInvert, bool yInvert)
    {
        invertMouseX = xInvert;
        invertMouseY = yInvert;
    }

    public void setClassicDungeonCrawlerMode(bool modeSetting)
    {
        classicDungeonCrawlerControlsEnabled = modeSetting;
    }

    public bool getClassicDungeonCrawlerMode()
    {
        return classicDungeonCrawlerControlsEnabled;
    }

    #endregion
}
