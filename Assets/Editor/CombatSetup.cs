using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// One-click editor helper: builds the Shell and Bullet prefabs, adds Health and Gun to the
/// Tank and Infantry prefabs, and drops a red test soldier into the open scene.
/// </summary>
public static class CombatSetup
{
    private const string PrefabFolder = "Assets/Prefabs";
    private const string InfantryPath = PrefabFolder + "/Infantry.prefab";
    private const string TankPath = PrefabFolder + "/Tank.prefab";

    [MenuItem("Map Conquest/Set Up Combat")]
    public static void Run()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("Stop Play mode first, then run Map Conquest > Set Up Combat.");
            return;
        }

        Shell shell = CreateProjectile("Shell", 0.25f, 40, 3f);
        Shell bullet = CreateProjectile("Bullet", 0.15f, 15, 1f);

        int unitsMask = LayerMask.GetMask("Units");
        AddCombat(TankPath, shell, 4f, 1.5f, 8f, unitsMask);
        AddCombat(InfantryPath, bullet, 2.5f, 1f, 10f, unitsMask);
        AddRedTestSoldier();

        AssetDatabase.SaveAssets();
        Debug.Log("Combat set up: Shell + Bullet prefabs, Health + Gun on Tank and Infantry, RedTest in the scene.");
    }

    private static Shell CreateProjectile(string name, float size, int damage, float knockback)
    {
        string path = $"{PrefabFolder}/{name}.prefab";
        Shell existing = AssetDatabase.LoadAssetAtPath<Shell>(path);
        if (existing != null)
        {
            return existing;
        }

        SpriteRenderer template = AssetDatabase.LoadAssetAtPath<SpriteRenderer>(InfantryPath);

        var projectile = new GameObject(name);
        projectile.transform.localScale = new Vector3(size, size, 1f);

        SpriteRenderer spriteRenderer = projectile.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = template.sprite;
        spriteRenderer.sharedMaterial = template.sharedMaterial;
        spriteRenderer.color = new Color(0.1f, 0.1f, 0.1f);
        spriteRenderer.sortingOrder = 3;

        Rigidbody2D body = projectile.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        projectile.AddComponent<CircleCollider2D>();

        var settings = new SerializedObject(projectile.AddComponent<Shell>());
        settings.FindProperty("damage").intValue = damage;
        settings.FindProperty("knockbackForce").floatValue = knockback;
        settings.ApplyModifiedPropertiesWithoutUndo();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(projectile, path);
        Object.DestroyImmediate(projectile);
        return prefab.GetComponent<Shell>();
    }

    private static void AddCombat(string prefabPath, Shell projectile, float range, float interval, float force, int targetMask)
    {
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);

        if (root.GetComponent<Health>() == null)
        {
            root.AddComponent<Health>();
        }

        Gun gun = root.GetComponent<Gun>();
        if (gun == null)
        {
            gun = root.AddComponent<Gun>();
        }

        var settings = new SerializedObject(gun);
        settings.FindProperty("shellPrefab").objectReferenceValue = projectile;
        settings.FindProperty("range").floatValue = range;
        settings.FindProperty("secondsBetweenShots").floatValue = interval;
        settings.FindProperty("launchForce").floatValue = force;
        settings.FindProperty("targetLayers").intValue = targetMask;
        settings.ApplyModifiedPropertiesWithoutUndo();

        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        PrefabUtility.UnloadPrefabContents(root);
    }

    private static void AddRedTestSoldier()
    {
        if (GameObject.Find("RedTest") != null)
        {
            return;
        }

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(InfantryPath);
        var soldier = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        soldier.name = "RedTest";
        soldier.transform.position = Vector3.zero;

        var unitSettings = new SerializedObject(soldier.GetComponent<Unit>());
        unitSettings.FindProperty("team").enumValueIndex = (int)Team.Red;
        unitSettings.ApplyModifiedProperties();

        Undo.RegisterCreatedObjectUndo(soldier, "Add RedTest soldier");
        EditorSceneManager.MarkSceneDirty(soldier.scene);
    }
}
