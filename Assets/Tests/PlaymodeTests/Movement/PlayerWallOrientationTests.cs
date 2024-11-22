using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerWallOrientationTests : CustomInputTestFixture
{
    GameController gameController;
    Keyboard keyboard;

    [SetUp]
    public void TestSetup()
    {
        SceneManager.LoadScene("GameplayScene");
        keyboard = InputSystem.AddDevice<Keyboard>();
    }

    [TearDown]
    public void TestTearDown()
    {
        InputSystem.RemoveDevice(keyboard);
    }

    //Test Partial Stair Climb with rotated orientation
    [UnityTest]
    public IEnumerator PlayerWallOrientationPartialStairClimb()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/PlayerWallOrientationTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetTimeScaleMultiplier(1.0f);

        GameObject redPlayer = GameObject.Find("RedPlayerPrefab(Clone)");
        GameObject bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)");

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //floor to partial
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + redPlayer.transform.forward + 0.125f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + bluePlayer.transform.forward - 0.125f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //partial to partial
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 2.0f * redPlayer.transform.forward + 0.375f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 2.0f * bluePlayer.transform.forward - 0.375f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //partial to partial
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 3.0f * redPlayer.transform.forward + 0.625f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 3.0f * bluePlayer.transform.forward - 0.625f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //partial to partial
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 4.0f * redPlayer.transform.forward + 0.875f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 4.0f * bluePlayer.transform.forward - 0.875f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //partial to floor
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 5.0f * redPlayer.transform.forward + redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 5.0f * bluePlayer.transform.forward - bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        ////////////
        ///UNDOING//
        ///////////

        //undo partial to floor
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 4.0f * redPlayer.transform.forward + 0.875f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 4.0f * bluePlayer.transform.forward - 0.875f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //undo partial to partial
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 3.0f * redPlayer.transform.forward + 0.625f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 3.0f * bluePlayer.transform.forward - 0.625f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //undo partial to partial
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 2.0f * redPlayer.transform.forward + 0.375f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 2.0f * bluePlayer.transform.forward - 0.375f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //undo partial to partial
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + redPlayer.transform.forward + 0.125f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + bluePlayer.transform.forward - 0.125f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //undo floor to partial
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }

    //Test Fall From Full Block To Full block
    [UnityTest]
    public IEnumerator PlayerWallOrientationBasicMovementBlocked()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/PlayerWallOrientationTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetTimeScaleMultiplier(1.0f);

        GameObject redPlayer = GameObject.Find("RedPlayerPrefab(Clone)");
        GameObject bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)");

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //left player/wall collision
        yield return PressThenRelease(keyboard.aKey, 1f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        //right wall/player collision
        yield return PressThenRelease(keyboard.dKey, 1f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        ////////////
        ///UNDOING//
        ///////////

        //undo right collide
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        //undo left collide
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }

    //Test Simple Fall with rotated orientation
    [UnityTest]
    public IEnumerator PlayerWallOrientationBasicFall()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/PlayerWallOrientationTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetTimeScaleMultiplier(1.0f);

        GameObject redPlayer = GameObject.Find("RedPlayerPrefab(Clone)");
        GameObject bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)");

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //Fall
        yield return PressThenRelease(keyboard.sKey, 1.5f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos - redPlayer.transform.forward - redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos - bluePlayer.transform.forward - 1.2f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        ////////////
        ///UNDOING//
        ///////////

        //undo fall
        yield return PressThenRelease(keyboard.backspaceKey, 1.5f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }
}
