using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class PlayerMovementTests
{
    [OneTimeSetUp]
    public void LoadScene()
    {
        SceneManager.LoadScene("GameplayScene");
    }

    [UnityTest]
    public IEnumerator PlayerMovementFlatSurface()
    {
        GameController gameController
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
