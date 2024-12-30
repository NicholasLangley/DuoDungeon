using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    MapEditorInputHandler inputHandler;

    [SerializeField]
    float cameraSpeed, cameraSensitivity;

    float horizontalLookRotation, verticalLookRotation;
    [SerializeField]
    float maxVertRotation = 90f;
    [SerializeField]
    float minVertRotation = -90f;

    // Start is called before the first frame update
    void Start()
    {
        horizontalLookRotation = 0f;
        verticalLookRotation = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Aim();
    }

    public void Move()
    {
        transform.position += transform.TransformVector((inputHandler.cameraMoveInput * cameraSpeed * Time.deltaTime));
    }

    public void Aim()
    {
        if (!inputHandler.cameraAimEnabledInput) { return; }

        horizontalLookRotation += inputHandler.cameraAimInput.x * cameraSensitivity;

        verticalLookRotation += inputHandler.cameraAimInput.y * cameraSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, minVertRotation, maxVertRotation);
        transform.localEulerAngles = new Vector3(-verticalLookRotation, horizontalLookRotation, 0);
    }
}
