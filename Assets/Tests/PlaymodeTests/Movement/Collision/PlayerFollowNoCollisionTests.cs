using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerFollowNoCollisionTests : CustomInputTestFixture
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

    //Test forward partial stair climb
    [UnityTest]
    public IEnumerator PlayerFollowFullPartialPartialFull()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/PlayerFolowTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetTimeScaleMultiplier(20.0f);

        Transform redPlayer = GameObject.Find("RedPlayerPrefab(Clone)").transform;
        Transform bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)").transform;

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //forward
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + redPlayer.transform.forward + 0.125f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + bluePlayer.transform.forward, bluePlayer.transform.localPosition), 0.1f);

        //forward
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 2.0f * redPlayer.transform.forward + 0.375f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 2.0f * bluePlayer.transform.forward + 0.125f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //forward
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 3.0f * redPlayer.transform.forward + 0.625f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 3.0f * bluePlayer.transform.forward + 0.375f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //forward
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 4.0f * redPlayer.transform.forward + 0.875f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 4.0f * bluePlayer.transform.forward + 0.625f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //forward
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 5.0f * redPlayer.transform.forward + redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 5.0f * bluePlayer.transform.forward + 0.875f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //forward
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 6.0f * redPlayer.transform.forward + redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 6.0f * bluePlayer.transform.forward + bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //////////
        //UNDOING
        /////////

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 5.0f * redPlayer.transform.forward + redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 5.0f * bluePlayer.transform.forward + 0.875f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 4.0f * redPlayer.transform.forward + 0.875f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 4.0f * bluePlayer.transform.forward + 0.625f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 3.0f * redPlayer.transform.forward + 0.625f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 3.0f * bluePlayer.transform.forward + 0.375f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 2.0f * redPlayer.transform.forward + 0.375f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 2.0f * bluePlayer.transform.forward + 0.125f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + redPlayer.transform.forward + 0.125f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + bluePlayer.transform.forward, bluePlayer.transform.localPosition), 0.1f);

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }

    //full stiar backward climb
    [UnityTest]
    public IEnumerator PlayerFollowFullStairsFull()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/PlayerFolowTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetTimeScaleMultiplier(20.0f);

        Transform redPlayer = GameObject.Find("RedPlayerPrefab(Clone)").transform;
        Transform bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)").transform;

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //back
        yield return PressThenRelease(keyboard.sKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos - redPlayer.transform.forward, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos - bluePlayer.transform.forward - 0.5f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //back
        yield return PressThenRelease(keyboard.sKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos - 2.0f * redPlayer.transform.forward - 0.5f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos - 2.0f * bluePlayer.transform.forward - 1.5f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //back
        yield return PressThenRelease(keyboard.sKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos - 3.0f * redPlayer.transform.forward - 1.5f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos - 3.0f * bluePlayer.transform.forward -2f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //back
        yield return PressThenRelease(keyboard.sKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos - 4.0f * redPlayer.transform.forward - 2f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos - 4.0f * bluePlayer.transform.forward - 2f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //////////
        //UNDOING
        /////////

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos - 3.0f * redPlayer.transform.forward - 1.5f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos - 3.0f * bluePlayer.transform.forward - 2f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos - 2.0f * redPlayer.transform.forward - 0.5f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos - 2.0f * bluePlayer.transform.forward - 1.5f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos - redPlayer.transform.forward, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos - bluePlayer.transform.forward - 0.5f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }

    [UnityTest]
    public IEnumerator PlayerFollowPartialClimbs()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/PlayerFolowTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetTimeScaleMultiplier(15.0f);

        Transform redPlayer = GameObject.Find("RedPlayerPrefab(Clone)").transform;
        Transform bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)").transform;

        //align
        yield return PressThenRelease(keyboard.dKey, 0.7f);

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //forward
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + redPlayer.transform.forward - 0.2f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + bluePlayer.transform.forward, bluePlayer.transform.localPosition), 0.1f);

        //forward
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 2.0f * redPlayer.transform.forward - 0.1f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 2.0f * bluePlayer.transform.forward - 0.2f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //forward
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 3.0f * redPlayer.transform.forward + 0.1f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 3.0f * bluePlayer.transform.forward - 0.1f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //forward
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 4.0f * redPlayer.transform.forward + 0.1f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 4.0f * bluePlayer.transform.forward + 0.1f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //////////
        //UNDOING
        /////////

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 3.0f * redPlayer.transform.forward + 0.1f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 3.0f * bluePlayer.transform.forward - 0.1f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 2.0f * redPlayer.transform.forward - 0.1f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 2.0f * bluePlayer.transform.forward - 0.2f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + redPlayer.transform.forward - 0.2f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + bluePlayer.transform.forward, bluePlayer.transform.localPosition), 0.1f);

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }
}
