using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomInputTestFixture : InputTestFixture
{
   public IEnumerator PressThenRelease(UnityEngine.InputSystem.Controls.ButtonControl button, float finalDelay)
    {
        Press(button);
        yield return new WaitForSeconds(0.05f);
        Release(button);

        yield return new WaitForSeconds(finalDelay);
        yield return null;
    }
}
