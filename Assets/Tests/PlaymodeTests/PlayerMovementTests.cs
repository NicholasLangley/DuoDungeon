using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerMovementTests : InputTestFixture
{
    Keyboard keyboard;

    [SetUp]
    public void Setup()
    {
        SceneManager.LoadScene("GameplayScene");
        keyboard = InputSystem.AddDevice<Keyboard>();
    }

    [UnityTest]
    public IEnumerator PlayerMovementFlatSurface()
    {
        GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/BasicTestMap.json");

        yield return new WaitForSeconds(1);

        GameObject redPlayer = GameObject.Find("RedPlayerPrefab(Clone)");
        GameObject bluePlayer = GameObject.Find("RedPlayerPrefab(Clone)");

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //forward
        Press(keyboard.wKey);
        yield return new WaitForSeconds(0.1f);
        Release(keyboard.wKey);
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(redPlayer.transform.localPosition, redStartPos + redPlayer.transform.forward);
        Assert.AreEqual(bluePlayer.transform.localPosition, blueStartPos + bluePlayer.transform.forward);

        //back
        Press(keyboard.sKey);
        yield return new WaitForSeconds(0.1f);
        Release(keyboard.sKey);
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(redPlayer.transform.localPosition, redStartPos);
        Assert.AreEqual(bluePlayer.transform.localPosition, blueStartPos);

        //left
        Press(keyboard.aKey);
        yield return new WaitForSeconds(0.1f);
        Release(keyboard.aKey);
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(redPlayer.transform.localPosition, redStartPos - redPlayer.transform.right);
        Assert.AreEqual(bluePlayer.transform.localPosition, blueStartPos - bluePlayer.transform.right);

        //right
        Press(keyboard.dKey);
        yield return new WaitForSeconds(0.1f);
        Release(keyboard.dKey);
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(redPlayer.transform.localPosition, redStartPos);
        Assert.AreEqual(bluePlayer.transform.localPosition, blueStartPos);

        yield return null;
    }
}
