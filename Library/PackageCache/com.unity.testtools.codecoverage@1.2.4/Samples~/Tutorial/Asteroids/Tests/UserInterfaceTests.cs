﻿using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class UserInterfaceTests
{
    GameObject gameManagerPrefab;
    GameObject inGameMenuPrefab;
    LoadSceneParameters loadSceneParameters;
#if UNITY_EDITOR
    string asteroidsScenePath;
#endif

    [SetUp]
    public void Setup()
    {
        GameManager.InitializeTestingEnvironment(true, false, false, false, false);

        loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None);

        Object asteroidsScene = ((GameObject)Resources.Load("TestsReferences")).GetComponent<TestsReferences>().asteroidsScene;
#if UNITY_EDITOR
        asteroidsScenePath = AssetDatabase.GetAssetPath(asteroidsScene);
#endif
        gameManagerPrefab = ((GameObject)Resources.Load("TestsReferences", typeof(GameObject))).GetComponent<TestsReferences>().gameManagerPrefab;
        inGameMenuPrefab = ((GameObject)Resources.Load("TestsReferences", typeof(GameObject))).GetComponent<TestsReferences>().inGameMenuPrefab;
    }

    void ClearScene()
    {
        Transform[] objects = Object.FindObjectsOfType<RectTransform>();
        foreach (Transform obj in objects)
        {
            if (obj != null)
                Object.DestroyImmediate(obj.gameObject);
        }
    }
    
    [UnityTest]
    public IEnumerator _01_InGameMenuExistsInScene()
    {
#if UNITY_EDITOR
        EditorSceneManager.LoadSceneInPlayMode(asteroidsScenePath, loadSceneParameters);     
        yield return null;

        Assert.NotNull(Object.FindObjectOfType<InGameMenuController>());
#else
        yield return null;

        Assert.Pass();
#endif        

    }

    [UnityTest]
    public IEnumerator _02_InGameMenuHasCorrectUIElements()
    {
        ClearScene();
        GameObject inGameMenu = Object.Instantiate(inGameMenuPrefab, Vector3.zero, Quaternion.identity);
        
        yield return null;

        Assert.NotNull(inGameMenu);
        Canvas canvas = inGameMenu.GetComponent<Canvas>();
        Assert.NotNull(canvas);
        Assert.NotNull(canvas.GetComponent<InGameMenuController>());

        Assert.NotNull(canvas.transform.GetChild(0));
        Assert.IsFalse(canvas.transform.GetChild(0).gameObject.activeInHierarchy);
        Assert.NotNull(canvas.transform.GetChild(1));
        Assert.IsTrue(canvas.transform.GetChild(1).gameObject.activeInHierarchy);
        Assert.NotNull(canvas.transform.GetChild(2));
        Assert.IsTrue(canvas.transform.GetChild(2).gameObject.activeInHierarchy);
    }

    [UnityTest]
    public IEnumerator _03_InGameMenuContainsButtons()
    {
        ClearScene();
        GameObject inGameMenu = Object.Instantiate(inGameMenuPrefab, Vector3.zero, Quaternion.identity);
        
        yield return null;

        Transform container = inGameMenu.transform.GetChild(0);
        Assert.NotNull(container.GetComponent<VerticalLayoutGroup>());

        // InGameMenu has the game title
        Assert.NotNull(container.GetChild(1).GetComponent<Image>());
        Assert.IsTrue(container.GetChild(1).GetComponent<Image>().sprite != null);

        // Pause menu has a resume button
        Assert.NotNull(container.GetChild(1).GetComponent<Button>());
        Assert.IsTrue(container.GetChild(1).name == "ResumeButton");
        Assert.IsTrue(container.GetChild(1).GetComponent<Button>().onClick.GetPersistentMethodName(0) == "ChangeMenuState");
    }

    [UnityTest]
    public IEnumerator _04_PauseMenuCanBeEnabledAndDisabled()
    {
        ClearScene();
        GameObject inGameMenu = Object.Instantiate(inGameMenuPrefab, Vector3.zero, Quaternion.identity);
        
        yield return null;

        InGameMenuController menuController = inGameMenu.GetComponent<InGameMenuController>();
        Assert.IsTrue(menuController.transform.GetChild(0).name == "PauseMenu");

        menuController.ChangeMenuState(true);
        Assert.IsTrue(menuController.transform.GetChild(0).game                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     