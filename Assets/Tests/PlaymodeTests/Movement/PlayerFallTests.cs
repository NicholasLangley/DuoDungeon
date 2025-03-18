using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
public class PlayerFallTests : CustomInputTestFixture
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

    //Test Fall From Full Block To Full block
    [UnityTest]
    public IEnumerator PlayerFallFullFull()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/PlayerFallTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetPlayerClassicDungeonCrawlerMode(true);
        gameController.SetTimeScaleMultiplier(20.0f);

        Transform redPlayer = GameObject.Find("RedPlayerPrefab(Clone)").transform;
        Transform bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)").transform;

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //forward
        yield return PressThenRelease(keyboard.wKey, 2f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + redPlayer.transform.forward - 2f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + bluePlayer.transform.forward - bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //////////
        //UNDOING
        /////////

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 2f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }

    //Test Fall From Full Block To Partial block
    [UnityTest]
    public IEnumerator PlayerFallFullPartial()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/PlayerFallTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetPlayerClassicDungeonCrawlerMode(true);
        gameController.SetTimeScaleMultiplier(20.0f);

        Transform redPlayer = GameObject.Find("RedPlayerPrefab(Clone)").transform;
        Transform bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)").transform;

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //forward
        yield return PressThenRelease(keyboard.sKey, 2f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos - redPlayer.transform.forward - 2.8f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos - bluePlayer.transform.forward - 0.8f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //////////
        //UNDOING
        /////////

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 2f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }

    //Test Fall From Partial Block (partial height exit) To Full block
    [UnityTest]
    public IEnumerator PlayerFallPartialFull()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/PlayerFallTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetPlayerClassicDungeonCrawlerMode(true);
        gameController.SetTimeScaleMultiplier(20.0f);

        Transform redPlayer = GameObject.Find("RedPlayerPrefab(Clone)").transform;
        Transform bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)").transform;

        //align player
        yield return PressThenRelease(keyboard.dKey, 1f);

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //forward
        yield return PressThenRelease(keyboard.wKey, 2f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + redPlayer.transform.forward - 1.8f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + bluePlayer.transform.forward - 0.8f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //////////
        //UNDOING
        /////////

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 2.5f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }

    //Test Fall From Partial Block (partial height exit) To Partial
    [UnityTest]
    public IEnumerator PlayerFallPartialPartial()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/PlayerFallTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetPlayerClassicDungeonCrawlerMode(true);
        gameController.SetTimeScaleMultiplier(20.0f);

        Transform redPlayer = GameObject.Find("RedPlayerPrefab(Clone)").transform;
        Transform bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)").transform;

        //align player
        yield return PressThenRelease(keyboard.dKey, 1f);

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //forward
        yield return PressThenRelease(keyboard.sKey, 2f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos - redPlayer.transform.forward - 1.6f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos - bluePlayer.transform.forward - 0.6f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //////////
        //UNDOING
        /////////

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 2f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }

    //Test Fall From Partial Block (zero exit height)
    [UnityTest]
    public IEnumerator PlayerFallPartialZeroHeightExit()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.LoadLevel("/Autotests/PlayerFallTestMap.json");
        yield return new WaitForSeconds(1.0f);
        gameController.SetPlayerClassicDungeonCrawlerMode(true);
        gameController.SetTimeScaleMultiplier(20.0f);

        Transform redPlayer = GameObject.Find("RedPlayerPrefab(Clone)").transform;
        Transform bluePlayer = GameObject.Find("BluePlayerPrefab(Clone)").transform;

        //align player
        yield return PressThenRelease(keyboard.dKey, 1f);
        yield return PressThenRelease(keyboard.dKey, 1.5f);

        Vector3 redStartPos = redPlayer.transform.localPosition;
        Vector3 blueStartPos = bluePlayer.transform.localPosition;

        //forward
        yield return PressThenRelease(keyboard.dKey, 2f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos + redPlayer.transform.right - 1.125f * redPlayer.transform.up, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos + bluePlayer.transform.right - 1.925f * bluePlayer.transform.up, bluePlayer.transform.localPosition), 0.1f);

        //////////
        //UNDOING
        /////////

        //undo back
        yield return PressThenRelease(keyboard.backspaceKey, 2f);
        Assert.LessOrEqual(Vector3.Distance(redStartPos, redPlayer.transform.localPosition), 0.1f);
        Assert.LessOrEqual(Vector3.Distance(blueStartPos, bluePlayer.transform.localPosition), 0.1f);

        yield return null;
    }
}
