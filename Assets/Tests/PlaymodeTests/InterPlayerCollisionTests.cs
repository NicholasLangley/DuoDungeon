using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class InterPlayerCollisionTests : CustomInputTestFixture
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

    ////////////////////////////////
    /// PlayerToBlockedPlayer///////
    ////////////////////////////////
    //Test flat movement ontop of full blocks, player collision
    [UnityTest]
    public IEnumerator PlayerToBlockedPlayerFullFlat()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/PlayerToBlockedPlayerTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetTimeScaleMultiplier(20.0f);

        Transform redPlayer = GameObject.Find("RedPlayerPrefab(Clone)").transform;
        Transform bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)").transform;

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //forward
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        //////////
        //UNDOING
        /////////

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }

    //Test movement ontop of partial blocks, player collision
    [UnityTest]
    public IEnumerator PlayerToBlockedPlayerPartialToPartial()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/PlayerToBlockedPlayerTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetTimeScaleMultiplier(20.0f);

        Transform redPlayer = GameObject.Find("RedPlayerPrefab(Clone)").transform;
        Transform bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)").transform;

        //align players with test
        yield return PressThenRelease(keyboard.aKey, 0.7f);

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //forward
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        //////////
        //UNDOING
        /////////

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }


    //Test player climbing full stairs to blocked player
    [UnityTest]
    public IEnumerator PlayerToBlockedPlayerIncreasingY()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/PlayerToBlockedPlayerTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetTimeScaleMultiplier(20.0f);

        Transform redPlayer = GameObject.Find("RedPlayerPrefab(Clone)").transform;
        Transform bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)").transform;

        //align players with test
        yield return PressThenRelease(keyboard.dKey, 0.7f);
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        yield return PressThenRelease(keyboard.wKey, 0.7f);

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //forward
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        //////////
        //UNDOING
        /////////

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }

    //Test player trying to move from full down to full stairs with blocked player
    [UnityTest]
    public IEnumerator PlayerToBlockedPlayerDecreasingY()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/PlayerToBlockedPlayerTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetTimeScaleMultiplier(20.0f);

        Transform redPlayer = GameObject.Find("RedPlayerPrefab(Clone)").transform;
        Transform bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)").transform;

        //align players with test
        yield return PressThenRelease(keyboard.dKey, 0.7f);
        yield return PressThenRelease(keyboard.dKey, 0.7f);
        yield return PressThenRelease(keyboard.wKey, 0.7f);

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //forward
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        //////////
        //UNDOING
        /////////

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }

    //Test player trying to move from full to partial same y with blocked player
    [UnityTest]
    public IEnumerator PlayerToBlockedPlayerFullToPartialSameY()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/PlayerToBlockedPlayerTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetTimeScaleMultiplier(20.0f);

        Transform redPlayer = GameObject.Find("RedPlayerPrefab(Clone)").transform;
        Transform bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)").transform;

        //align players with test
        yield return PressThenRelease(keyboard.aKey, 0.7f);
        yield return PressThenRelease(keyboard.aKey, 0.7f);
        yield return PressThenRelease(keyboard.wKey, 0.7f);

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //forward
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        //////////
        //UNDOING
        /////////

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }


    //Test player trying to climb to full with blocked player
    [UnityTest]
    public IEnumerator PlayerToBlockedPlayerPartialClimbUpYFlat()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/PlayerToBlockedPlayerTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetTimeScaleMultiplier(20.0f);

        Transform redPlayer = GameObject.Find("RedPlayerPrefab(Clone)").transform;
        Transform bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)").transform;

        //align players with test
        yield return PressThenRelease(keyboard.aKey, 0.7f);
        yield return PressThenRelease(keyboard.aKey, 0.7f);
        yield return PressThenRelease(keyboard.aKey, 0.7f);

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //forward
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        //////////
        //UNDOING
        /////////

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }

    //Test player trying to climb to full+partial with blocked player
    [UnityTest]
    public IEnumerator PlayerToBlockedPlayerPartialClimbUpYpartial()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/PlayerToBlockedPlayerTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetTimeScaleMultiplier(20.0f);

        Transform redPlayer = GameObject.Find("RedPlayerPrefab(Clone)").transform;
        Transform bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)").transform;

        //align players with test
        yield return PressThenRelease(keyboard.aKey, 0.7f);
        yield return PressThenRelease(keyboard.aKey, 0.7f);
        yield return PressThenRelease(keyboard.aKey, 0.7f);
        yield return PressThenRelease(keyboard.aKey, 0.7f);

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //forward
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        //////////
        //UNDOING
        /////////

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }

}
