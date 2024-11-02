using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerMovementBlockedTests : CustomInputTestFixture
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

    ////////////////////
    //FULL BLOCKS Mainly
    /////////////////////
    
    //Test flat movement ontop of full blocks, with no collisions
    [UnityTest]
    public IEnumerator MovementBlockedFlatFullBlocks()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/MovementBlockedBasicTestMap.json");
        yield return new WaitForSeconds(1.0f);

        Transform redPlayer = GameObject.Find("RedPlayerPrefab(Clone)").transform;
        Transform bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)").transform;

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //front
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        //back
        yield return PressThenRelease(keyboard.sKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        //left
        yield return PressThenRelease(keyboard.aKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        //right (one block of space to wall)
        yield return PressThenRelease(keyboard.dKey, 0.7f);
        yield return PressThenRelease(keyboard.dKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + redPlayer.transform.right, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + bluePlayer.transform.right, bluePlayer.transform.localPosition), 0.1f);

        //////
        ///UNDO
        ///////

        //undo right
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + redPlayer.transform.right, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + bluePlayer.transform.right, bluePlayer.transform.localPosition), 0.1f);

        //undo left
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        //undo forward
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }

    //Test moving from a partial stair to an increasing but full blocked space and a stair to y zero thats full blocked
    [UnityTest]
    public IEnumerator MovementBlockedFullStairToFullBlock()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/MovementBlockedBasicTestMap.json");
        yield return new WaitForSeconds(1.0f);

        Transform redPlayer = GameObject.Find("RedPlayerPrefab(Clone)").transform;
        Transform bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)").transform;

        //align player
        yield return PressThenRelease(keyboard.dKey, 0.7f);
        yield return PressThenRelease(keyboard.wKey, 0.7f);

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //front, hit wall
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        //UNDO
        //////
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);
    }

    //Test moving from a partial block to a full block that's too high to climb
    [UnityTest]
    public IEnumerator MovementBlockedPartialToFullBlockNoClimb()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/MovementBlockedBasicTestMap.json");
        yield return new WaitForSeconds(1.0f);

        Transform redPlayer = GameObject.Find("RedPlayerPrefab(Clone)").transform;
        Transform bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)").transform;

        //align player
        yield return PressThenRelease(keyboard.dKey, 0.7f);
        yield return PressThenRelease(keyboard.sKey, 0.7f);

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //back, hit wall
        yield return PressThenRelease(keyboard.sKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        //UNDO
        //////
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);
    }


    //Test moving from a partial block to a full block that's climbable but has a block on top thats not
    //blue is partial ontop of full
    //red is full ontop of full
    [UnityTest]
    public IEnumerator MovementBlockedPartialToFullBlockClimbButBlocked()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/MovementBlockedBasicTestMap.json");
        yield return new WaitForSeconds(1.0f);

        Transform redPlayer = GameObject.Find("RedPlayerPrefab(Clone)").transform;
        Transform bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)").transform;

        //align player
        yield return PressThenRelease(keyboard.dKey, 0.7f);
        yield return PressThenRelease(keyboard.sKey, 0.7f);
        yield return PressThenRelease(keyboard.dKey, 0.7f);

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //right, hit wall
        yield return PressThenRelease(keyboard.dKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        //UNDO
        //////
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);
    }

    ////////////////
    //Partial BLOCKS
    ////////////////

    //Test moving from a full stair to a blocking partial
    //blue is "decreasing" y
    //red is increasing y
    [UnityTest]
    public IEnumerator MovementBlockedFullStairToPartial()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/MovementBlockedPartialTestMap.json");
        yield return new WaitForSeconds(1.0f);

        Transform redPlayer = GameObject.Find("RedPlayerPrefab(Clone)").transform;
        Transform bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)").transform;

        //align player
        yield return PressThenRelease(keyboard.wKey, 0.7f);

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //forward, hit partial block
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        //UNDO
        //////
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);
    }

    //Test moving from a partial to a blocking partial
    //blue is climbable but above space blocked
    //red is not climbable
    [UnityTest]
    public IEnumerator MovementBlockedPartialToPartial()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/MovementBlockedPartialTestMap.json");
        yield return new WaitForSeconds(1.0f);

        Transform redPlayer = GameObject.Find("RedPlayerPrefab(Clone)").transform;
        Transform bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)").transform;

        //align player
        yield return PressThenRelease(keyboard.sKey, 0.7f);

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //forward, hit partial block
        yield return PressThenRelease(keyboard.sKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        //UNDO
        //////
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);
    }
}
