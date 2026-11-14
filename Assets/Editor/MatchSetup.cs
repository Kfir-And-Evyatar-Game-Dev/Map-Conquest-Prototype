using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// One-click editor helper: adds the enemy commander, the match rules, the match clock label
/// and the end screen to the open scene, wires their references, and removes the RedTest soldier.
/// </summary>
public static class MatchSetup
{
    private const string PrefabFolder = "Assets/Prefabs";

    [MenuItem("Map Conquest/Set Up Enemy And Match")]
    public static void Run()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("Stop Play mode first, then run Map Conquest > Set Up Enemy And Match.");
            return;
        }

        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        UnitFactory redFactory = FindNamed<UnitFactory>("RedFactory");
        MatchSettings settings = AssetDatabase.LoadAssetAtPath<MatchSettings>("Assets/Data/MatchSettings.asset");
        if (canvas == null || redFactory == null || settings == null)
        {
            Debug.LogError("Missing Canvas, RedFactory or Assets/Data/MatchSettings.asset; nothing was changed.");
            return;
        }

        AddEnemyCommander(redFactory);
        MatchRules rules = AddMatchRules(settings);
        AddStatusLabel(canvas, rules);
        AddEndScreen(canvas, rules);
        RemoveRedTest();

        EditorSceneManager.MarkSceneDirty(canvas.gameObject.scene);
        Debug.Log("Enemy and match set up: EnemyCommander, MatchRules, MatchStatusText and EndScreen added. Save the scene (Ctrl+S).");
    }

    private static void AddEnemyCommander(UnitFactory redFactory)
    {
        if (Object.FindFirstObjectByType<EnemyCommander>() != null)
        {
            return;
        }

        Unit infantry = AssetDatabase.LoadAssetAtPath<Unit>($"{PrefabFolder}/Infantry.prefab");
        Unit tank = AssetDatabase.LoadAssetAtPath<Unit>($"{PrefabFolder}/Tank.prefab");

        var enemy = new GameObject("EnemyCommander").AddComponent<EnemyCommander>();
        var settings = new SerializedObject(enemy);
        settings.FindProperty("factory").objectReferenceValue = redFactory;

        SerializedProperty buildOrder = settings.FindProperty("buildOrder");
        Unit[] order = { infantry, infantry, tank };
        buildOrder.arraySize = order.Length;
        for (int i = 0; i < order.Length; i++)
        {
            buildOrder.GetArrayElementAtIndex(i).objectReferenceValue = order[i];
        }

        settings.ApplyModifiedPropertiesWithoutUndo();
        Undo.RegisterCreatedObjectUndo(enemy.gameObject, "Add EnemyCommander");
    }

    private static MatchRules AddMatchRules(MatchSettings matchSettings)
    {
        MatchRules existing = Object.FindFirstObjectByType<MatchRules>();
        if (existing != null)
        {
            return existing;
        }

        var rules = new GameObject("MatchRules").AddComponent<MatchRules>();
        var settings = new SerializedObject(rules);
        settings.FindProperty("settings").objectReferenceValue = matchSettings;
        settings.ApplyModifiedPropertiesWithoutUndo();
        Undo.RegisterCreatedObjectUndo(rules.gameObject, "Add MatchRules");
        return rules;
    }

    private static void AddStatusLabel(Canvas canvas, MatchRules rules)
    {
        if (Object.FindFirstObjectByType<MatchStatusLabel>() != null)
        {
            return;
        }

        TMP_Text text = CreateText("MatchStatusText", canvas.transform, "5:00", 36);
        var rect = (RectTransform)text.transform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -20f);
        rect.sizeDelta = new Vector2(700f, 50f);

        var label = text.gameObject.AddComponent<MatchStatusLabel>();
        var settings = new SerializedObject(label);
        settings.FindProperty("rules").objectReferenceValue = rules;
        settings.FindProperty("label").objectReferenceValue = text;
        settings.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void AddEndScreen(Canvas canvas, MatchRules rules)
    {
        if (Object.FindFirstObjectByType<EndScreen>() != null)
        {
            return;
        }

        var root = new GameObject("EndScreen", typeof(RectTransform));
        root.transform.SetParent(canvas.transform, false);
        Stretch((RectTransform)root.transform);

        var panel = new GameObject("Panel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(root.transform, false);
        Stretch((RectTransform)panel.transform);
        panel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);

        TMP_Text title = CreateText("Title", panel.transform, "Victory!", 96);
        var titleRect = (RectTransform)title.transform;
        titleRect.anchoredPosition = new Vector2(0f, 80f);
        titleRect.sizeDelta = new Vector2(900f, 140f);

        var resources = new TMP_DefaultControls.Resources
        {
            standard = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd"),
        };
        GameObject buttonObject = TMP_DefaultControls.CreateButton(resources);
        buttonObject.name = "PlayAgainButton";
        buttonObject.transform.SetParent(panel.transform, false);
        var buttonRect = (RectTransform)buttonObject.transform;
        buttonRect.anchoredPosition = new Vector2(0f, -60f);
        buttonRect.sizeDelta = new Vector2(320f, 80f);
        TMP_Text buttonText = buttonObject.GetComponentInChildren<TMP_Text>();
        buttonText.text = "Play Again";
        buttonText.fontSize = 36;

        var endScreen = root.AddComponent<EndScreen>();
        var settings = new SerializedObject(endScreen);
        settings.FindProperty("rules").objectReferenceValue = rules;
        settings.FindProperty("panel").objectReferenceValue = panel;
        settings.FindProperty("title").objectReferenceValue = title;
        settings.FindProperty("playAgainButton").objectReferenceValue = buttonObject.GetComponent<Button>();
        settings.ApplyModifiedPropertiesWithoutUndo();

        root.transform.SetAsLastSibling(); // drawn on top of the HUD
        Undo.RegisterCreatedObjectUndo(root, "Add EndScreen");
    }

    private static TMP_Text CreateText(string name, Transform parent, string content, float fontSize)
    {
        GameObject textObject = TMP_DefaultControls.CreateText(new TMP_DefaultControls.Resources());
        textObject.name = name;
        textObject.transform.SetParent(parent, false);

        var text = textObject.GetComponent<TMP_Text>();
        text.text = content;
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Center;
        return text;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void RemoveRedTest()
    {
        GameObject redTest = GameObject.Find("RedTest");
        if (redTest != null)
        {
            Undo.DestroyObjectImmediate(redTest);
        }
    }

    private static T FindNamed<T>(string name) where T : Component
    {
        GameObject found = GameObject.Find(name);
        return found != null ? found.GetComponent<T>() : null;
    }
}
