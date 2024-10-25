using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerMovementTests : InputTestFixture
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

    [UnityTest]
    public IEnumerator PlayerMovementFlatSurface()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/BasicTestMap.json");

        yield return new WaitForSeconds(1.0f);

        GameObject redPlayer = GameObject.Find("RedPlayerPrefab(Clone)");
        GameObject bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)");

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //forward
        Press(keyboard.wKey);
        yield return new WaitForSeconds(0.05f);
        Release(keyboard.wKey);
        yield return new WaitForSeconds(0.6f);
        Assert.AreEqual(redStartPos + redPlayer.transform.forward, redPlayer.transform.localPosition);
        Assert.AreEqual(blueStartPos + bluePlayer.transform.forward, bluePlayer.transform.localPosition);

        //back
        Press(keyboard.sKey);
        yield return new WaitForSeconds(0.05f);
        Release(keyboard.sKey);
        yield return new WaitForSeconds(0.6f);
        Assert.AreEqual(redStartPos, redPlayer.transform.localPosition);
        Assert.AreEqual(blueStartPos, bluePlayer.transform.localPosition);

        //left
        Press(keyboard.aKey);
        yield return new WaitForSeconds(0.05f);
        Release(keyboard.aKey);
        yield return new WaitForSeconds(0.6f);
        Assert.AreEqual(redStartPos - redPlayer.transform.right, redPlayer.transform.localPosition);
        Assert.AreEqual(blueStartPos - bluePlayer.transform.right, bluePlayer.transform.localPosition);

        //right
        Press(keyboard.dKey);
        yield return new WaitForSeconds(0.05f);
        Release(keyboard.dKey);
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(redStartPos, redPlayer.transform.localPosition);
        Assert.AreEqual(blueStartPos, bluePlayer.transform.localPosition);

        //////////
        //UNDOING
        /////////
        //undo right
        Press(keyboard.backspaceKey);
        yield return new WaitForSeconds(0.05f);
        Release(keyboard.backspaceKey);
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(redStartPos - redPlayer.transform.right, redPlayer.transform.localPosition);
        Assert.AreEqual(blueStartPos - bluePlayer.transform.right, bluePlayer.transform.localPosition);

        //undo left
        Press(keyboard.backspaceKey);
        yield return new WaitForSeconds(0.05f);
        Release(keyboard.backspaceKey);
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(redStartPos, redPlayer.transform.localPosition);
        Assert.AreEqual(blueStartPos, bluePlayer.transform.localPosition);

        //undo back
        Press(keyboard.backspaceKey);
        yield return new WaitForSeconds(0.05f);
        Release(keyboard.backspaceKey);
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(redStartPos + redPlayer.transform.forward, redPlayer.transform.localPosition);
        Assert.AreEqual(blueStartPos + bluePlayer.transform.forward, bluePlayer.transform.localPosition);

        //undo forward
        Press(keyboard.backspaceKey);
        yield return new WaitForSeconds(0.05f);
        Release(keyboard.backspaceKey);
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(redStartPos, redPlayer.transform.localPosition);
        Assert.AreEqual(blueStartPos, bluePlayer.transform.localPosition);

        yield return null;
    }



    [UnityTest]
    public IEnumerator PlayerMovementStraightStairs()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/BasicTestMap.json");

        yield return new WaitForSeconds(1.0f);

        GameObject redPlayer = GameObject.Find("RedPlayerPrefab(Clone)");
        GameObject bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)");

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //forward
        Press(keyboard.wKey);
        yield return new WaitForSeconds(0.05f);
        Release(keyboard.wKey);
        yield return new WaitForSeconds(0.6f);
        Assert.AreEqual(redStartPos + redPlayer.transform.forward, redPlayer.transform.localPosition);
        Assert.AreEqual(blueStartPos + bluePlayer.transform.forward, bluePlayer.transform.localPosition);

        //floor to stair
        Press(keyboard.wKey);
        yield return new WaitForSeconds(0.05f);
        Release(keyboard.wKey);
        yield return new WaitForSeconds(0.6f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 2.0f * redPlayer.transform.forward + 0.5f * Vector3.down, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 2.0f * bluePlayer.transform.forward + 0.5f * Vector3.up, bluePlayer.transform.localPosition), 0.1f);

        //stair to stair
        Press(keyboard.wKey);
        yield return new WaitForSeconds(0.05f);
        Release(keyboard.wKey);
        yield return new WaitForSeconds(0.6f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 3.0f * redPlayer.transform.forward + 1.5f * Vector3.down, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 3.0f * bluePlayer.transform.forward + 1.5f * Vector3.up, bluePlayer.transform.localPosition), 0.1f);

        //stair to floor
        Press(keyboard.wKey);
        yield return new WaitForSeconds(0.05f);
        Release(keyboard.wKey);
        yield return new WaitForSeconds(0.6f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 4.0f * redPlayer.transform.forward + 2.0f * Vector3.down, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 4.0f * bluePlayer.transform.forward + 2.0f * Vector3.up, bluePlayer.transform.localPosition), 0.1f);

        ////////////
        ///UNDOING//
        ///////////
        
        //undo stair to floor
        Press(keyboard.backspaceKey);
        yield return new WaitForSeconds(0.05f);
        Release(keyboard.backspaceKey);
        yield return new WaitForSeconds(0.6f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 3.0f * redPlayer.transform.forward + 1.5f * Vector3.down, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 3.0f * bluePlayer.transform.forward + 1.5f * Vector3.up, bluePlayer.transform.localPosition), 0.1f);

        //undo stair to stair
        Press(keyboard.backspaceKey);
        yield return new WaitForSeconds(0.05f);
        Release(keyboard.backspaceKey);
        yield return new WaitForSeconds(0.6f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 2.0f * redPlayer.transform.forward + 0.5f * Vector3.down, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 2.0f * bluePlayer.transform.forward + 0.5f * Vector3.up, bluePlayer.transform.localPosition), 0.1f);

        //undo floor to stair
        Press(keyboard.backspaceKey);
        yield return new WaitForSeconds(0.05f);
        Release(keyboard.backspaceKey);
        yield return new WaitForSeconds(0.6f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + 1.0f * redPlayer.transform.forward, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + 1.0f * bluePlayer.transform.forward, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }

}
