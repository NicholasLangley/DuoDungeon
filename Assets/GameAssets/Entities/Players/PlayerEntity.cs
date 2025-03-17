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
    public float mouseSensitivityX, mouseSensitivityY, maxMouseAngleX, maxMouseAngleY;
    float currentAimXAngle, currentAimYAngle;
    [SerializeField]
    bool invertMouseX, invertMouseY;
    

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

    public void UpdateCameraAim(Vector2 mouseMovementVector)
    {
        if (invertMouseX) { mouseMovementVector.x *= -1; }
        if (invertMouseY) { mouseMovementVector.y *= -1; }

        //_cameraBaseTransform.Rotate(_cameraBaseTransform.up, mouseMovementVector.x * mouseSensitivityX);
        currentAimXAngle += mouseMovementVector.x * mouseSensitivityX;
        currentAimXAngle = Mathf.Clamp(currentAimXAngle, -maxMouseAngleX, maxMouseAngleX);
        _cameraBaseTransform.localRotation = Quaternion.Euler(0, currentAimXAngle, 0);

        currentAimYAngle += mouseMovementVector.y * mouseSensitivityY;
        currentAimYAngle = Mathf.Clamp(currentAimYAngle, -maxMouseAngleY, maxMouseAngleY);
        _cameraTransform.localRotation = Quaternion.Euler(-currentAimYAngle, 0, 0);
    }
}
