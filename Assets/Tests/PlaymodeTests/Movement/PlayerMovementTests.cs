using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerMovementTests : CustomInputTestFixture
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

    //Test flat movement ontop of full blocks, with no collisions
    [UnityTest]
    public IEnumerator PlayerMovementNoBlocksOccupied()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/MovementTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetTimeScaleMultiplier(20.0f);

        Transform redPlayer = GameObject.Find("RedPlayerPrefab(Clone)").transform;
        Transform bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)").transform;

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //back
        yield return PressThenRelease(keyboard.sKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos - redPlayer.transform.forward, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos - bluePlayer.transform.forward, bluePlayer.transform.localPosition), 0.1f);

        //forward
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        //right
        yield return PressThenRelease(keyboard.dKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + redPlayer.transform.right, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + bluePlayer.transform.right, bluePlayer.transform.localPosition), 0.1f);

        //left
        yield return PressThenRelease(keyboard.aKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        //////////
        //UNDOING
        /////////
        //undo left
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + redPlayer.transform.right, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + bluePlayer.transform.right, bluePlayer.transform.localPosition), 0.1f);

        //undo right
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        //undo forward
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos - redPlayer.transform.forward, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos - bluePlayer.transform.forward, bluePlayer.transform.localPosition), 0.1f);

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }


    //test smooth moving up/down a straight quarter staircase
    //floor to partial - smooth
    //partial to partial - smooth
    //partial to floor - smooth
    [UnityTest]
    public IEnumerator PlayerMovementQuarterStairs()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/MovementTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetTimeScaleMultiplier(15.0f);

        GameObject redPlayer = GameObject.Find("RedPlayerPrefab(Clone)");
        GameObject bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)");

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //floor to partial
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + redPlayer.transform.forward + 0.125f * Vector3.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + bluePlayer.transform.forward + 0.125f * Vector3.down, bluePlayer.transform.localPosition), 0.1f);

        //partial to partial
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 2.0f * redPlayer.transform.forward + 0.375f * Vector3.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 2.0f * bluePlayer.transform.forward + 0.375f * Vector3.down, bluePlayer.transform.localPosition), 0.1f);

        //partial to partial
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 3.0f * redPlayer.transform.forward + 0.625f * Vector3.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 3.0f * bluePlayer.transform.forward + 0.625f * Vector3.down, bluePlayer.transform.localPosition), 0.1f);

        //partial to partial
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 4.0f * redPlayer.transform.forward + 0.875f * Vector3.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 4.0f * bluePlayer.transform.forward + 0.875f * Vector3.down, bluePlayer.transform.localPosition), 0.1f);

        //partial to floor
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 5.0f * redPlayer.transform.forward + Vector3.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 5.0f * bluePlayer.transform.forward + Vector3.down, bluePlayer.transform.localPosition), 0.1f);

        ////////////
        ///UNDOING//
        ///////////

        //undo partial to floor
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 4.0f * redPlayer.transform.forward + 0.875f * Vector3.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 4.0f * bluePlayer.transform.forward + 0.875f * Vector3.down, bluePlayer.transform.localPosition), 0.1f);

        //undo partial to partial
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 3.0f * redPlayer.transform.forward + 0.625f * Vector3.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 3.0f * bluePlayer.transform.forward + 0.625f * Vector3.down, bluePlayer.transform.localPosition), 0.1f);

        //undo partial to partial
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 2.0f * redPlayer.transform.forward + 0.375f * Vector3.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 2.0f * bluePlayer.transform.forward + 0.375f * Vector3.down, bluePlayer.transform.localPosition), 0.1f);

        //undo partial to partial
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + redPlayer.transform.forward + 0.125f * Vector3.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + bluePlayer.transform.forward + 0.125f * Vector3.down, bluePlayer.transform.localPosition), 0.1f);

        //undo floor to partial
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }

    //Tests small climbing steps from small differences in partial block height ends -> starts
    //floor to partial - small step
    //partial to partial - small step
    //partial to floor - small step
    [UnityTest]
    public IEnumerator PlayerMovementSmallSteps()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/MovementTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetTimeScaleMultiplier(10.0f);

        GameObject redPlayer = GameObject.Find("RedPlayerPrefab(Clone)");
        GameObject bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)");

        //align players with test row
        yield return PressThenRelease(keyboard.dKey, 0.7f);

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //floor to partial
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + redPlayer.transform.forward + 0.1f * Vector3.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + bluePlayer.transform.forward + 0.1f * Vector3.down, bluePlayer.transform.localPosition), 0.1f);

        //partial to partial
        yield return PressThenRelease(keyboard.wKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 2.0f * redPlayer.transform.forward + 0.2f * Vector3.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 2.0f * bluePlayer.transform.forward + 0.2f * Vector3.down, bluePlayer.transform.localPosition), 0.1f);

        //partial to floor (initial partial to partial alignment)
        yield return PressThenRelease(keyboard.sKey, 0.7f);
        yield return PressThenRelease(keyboard.sKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        /////////
        ///UNDO//
        ////////
        
        //undo partial to floor
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + redPlayer.transform.forward + 0.1f * Vector3.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + bluePlayer.transform.forward + 0.1f * Vector3.down, bluePlayer.transform.localPosition), 0.1f);

        //undo partial to partial (initial realignment skip)
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + redPlayer.transform.forward + 0.1f * Vector3.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + bluePlayer.transform.forward + 0.1f * Vector3.down, bluePlayer.transform.localPosition), 0.1f);

        //undo floor to partial
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }


    //Tests steps too big to climb
    //floor to partial - big step
    //partial to partial - big step
    //partial to floor - big step
    [UnityTest]
    public IEnumerator PlayerMovementBigSteps()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/MovementTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetTimeScaleMultiplier(20.0f);

        GameObject redPlayer = GameObject.Find("RedPlayerPrefab(Clone)");
        GameObject bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)");

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //floor drop down to partial
        yield return PressThenRelease(keyboard.aKey, 1f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos - redPlayer.transform.right + 0.5f * Vector3.down, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos - bluePlayer.transform.right + 0.5f * Vector3.down, bluePlayer.transform.localPosition), 0.1f);

        //partial to full movement blocked
        yield return PressThenRelease(keyboard.dKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos - redPlayer.transform.right + 0.5f * Vector3.down, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos - bluePlayer.transform.right + 0.5f * Vector3.down, bluePlayer.transform.localPosition), 0.1f);

        //drop from partial
        //partial to partial (red)
        //partial to full (blue)
        yield return PressThenRelease(keyboard.aKey, 1f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos - 2.0f * redPlayer.transform.right + 0.9f * Vector3.down, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos - 2.0f * bluePlayer.transform.right + 1.0f * Vector3.down, bluePlayer.transform.localPosition), 0.1f);

        //movement blocked
        //partial to partial (red)
        //full to partial (blue)
        yield return PressThenRelease(keyboard.dKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos - 2.0f * redPlayer.transform.right + 0.9f * Vector3.down, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos - 2.0f * bluePlayer.transform.right + 1.0f * Vector3.down, bluePlayer.transform.localPosition), 0.1f);

        /////////
        ///UNDO//
        ////////

        //undo movement blocked
        //partial to partial (red)
        //full to partial (blue)
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos - 2.0f * redPlayer.transform.right + 0.9f * Vector3.down, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos - 2.0f * bluePlayer.transform.right + 1.0f * Vector3.down, bluePlayer.transform.localPosition), 0.1f);

        //undo drop from partial
        //partial to partial (red)
        //partial to full (blue)
        yield return PressThenRelease(keyboard.backspaceKey, 1f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos - redPlayer.transform.right + 0.5f * Vector3.down, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos - bluePlayer.transform.right + 0.5f * Vector3.down, bluePlayer.transform.localPosition), 0.1f);

        //undo movement blocked
        //partial to full
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos - redPlayer.transform.right + 0.5f * Vector3.down, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos - bluePlayer.transform.right + 0.5f * Vector3.down, bluePlayer.transform.localPosition), 0.1f);

        //undo falling from full to partial
        yield return PressThenRelease(keyboard.backspaceKey, 1f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }

    //Tests steps too big to climb
    //floor to partial - big step
    //partial to partial - big step
    //partial to floor - big step
    [UnityTest]
    public IEnumerator PlayerRotationBasic()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/MovementTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetTimeScaleMultiplier(20.0f);

        GameObject redPlayer = GameObject.Find("RedPlayerPrefab(Clone)");
        GameObject bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)");

        Quaternion redStartRotation = redPlayer.transform.rotation;
        Quaternion blueStartRotation = bluePlayer.transform.rotation;

        Quaternion redRightRotation = Quaternion.Euler(redStartRotation.eulerAngles.x, redStartRotation.eulerAngles.y + 90, redStartRotation.eulerAngles.z);
        Quaternion blueRightRotation = Quaternion.Euler(blueStartRotation.eulerAngles.x, blueStartRotation.eulerAngles.y + 90, blueStartRotation.eulerAngles.z);

        //rotate right
        yield return PressThenRelease(keyboard.eKey, 0.7f);
        Assert.LessOrEqual(Quaternion.Angle(redRightRotation, redPlayer.transform.rotation), 0.1f);
        Assert.LessOrEqual(Quaternion.Angle(blueRightRotation, bluePlayer.transform.rotation), 0.1f);

        //rotate left
        yield return PressThenRelease(keyboard.qKey, 0.7f);
        Assert.LessOrEqual(Quaternion.Angle(redStartRotation, redPlayer.transform.rotation), 0.1f);
        Assert.LessOrEqual(Quaternion.Angle(blueStartRotation, bluePlayer.transform.rotation), 0.1f);

        ////////
        //UNDO//
        ////////

        //undo rotate left
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Quaternion.Angle(redRightRotation, redPlayer.transform.rotation), 0.1f);
        Assert.LessOrEqual(Quaternion.Angle(blueRightRotation, bluePlayer.transform.rotation), 0.1f);

        //undo rotate right
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Quaternion.Angle(redStartRotation, redPlayer.transform.rotation), 0.1f);
        Assert.LessOrEqual(Quaternion.Angle(blueStartRotation, bluePlayer.transform.rotation), 0.1f);

        yield return null;
    }


    //Tests climbing from a partial block to a full block with a partial ontop
    //red should be blocked because too high
    //blue should succeed
    [UnityTest]
    public IEnumerator PlayerMovementPartialClimbFullStackedWithPartial()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/MovementTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetTimeScaleMultiplier(20.0f);

        GameObject redPlayer = GameObject.Find("RedPlayerPrefab(Clone)");
        GameObject bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)");

        

        //initial align
        yield return PressThenRelease(keyboard.sKey, 1f);
        yield return PressThenRelease(keyboard.sKey, 1f);

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        yield return PressThenRelease(keyboard.sKey, 1f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos - bluePlayer.transform.forward + 0.2f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        /////////
        ///UNDO//
        ////////

        //undo movement blocked
        //partial to partial (red)
        //full to partial (blue)
        yield return PressThenRelease(keyboard.backspaceKey, 0.7f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }
}
