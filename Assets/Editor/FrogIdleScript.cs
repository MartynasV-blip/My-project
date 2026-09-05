using UnityEditor;
using UnityEngine;

public static class FrogIdleClipCreator
{
    private const float Duration = 2.4f;
    private const float FrameRate = 30f;

    [MenuItem("Tools/Frog Marauder/Create Idle Clip")]
    private static void CreateIdleClip()
    {
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            EditorUtility.DisplayDialog(
                "Frog Idle Creator",
                "Select the FrogMarauder GameObject in the scene Hierarchy first.",
                "OK"
            );

            return;
        }

        Transform animationRoot = FindAnimationRoot(selected.transform);

        if (animationRoot == null)
        {
            EditorUtility.DisplayDialog(
                "Frog Idle Creator",
                "Could not find a Hips bone.\n\n" +
                "Select the top-level FrogMarauder object in the scene Hierarchy, " +
                "then run the tool again.",
                "OK"
            );

            return;
        }

        Transform hips = FindBone(animationRoot, "Hips");

        Debug.Log(
            "Frog Idle Creator: Animation root is \"" +
            animationRoot.name +
            "\". Hips found at \"" +
            AnimationUtility.CalculateTransformPath(hips, animationRoot) +
            "\"."
        );

        string savePath = EditorUtility.SaveFilePanelInProject(
            "Save Frog Idle Animation",
            "FrogMarauder_Idle",
            "anim",
            "Choose where to save the generated animation clip."
        );

        if (string.IsNullOrEmpty(savePath))
            return;

        AnimationClip clip = new AnimationClip
        {
            name = "FrogMarauder_Idle",
            frameRate = FrameRate
        };

        // ------------------------------------------------------------
        // Hips: gentle vertical motion and weight shifting
        // ------------------------------------------------------------

        AddPosition(
            clip,
            animationRoot,
            "Hips",
            new Vector3(0f, 0f, 0f),
            new Vector3(0f, 0.012f, 0f)
        );

        AddRotation(
            clip,
            animationRoot,
            "Hips",
            new Vector3(0f, -0.8f, -0.5f),
            new Vector3(0.8f, 0.8f, 0.5f)
        );

        // ------------------------------------------------------------
        // Torso: breathing
        // ------------------------------------------------------------

        AddRotation(
            clip,
            animationRoot,
            "Spine",
            new Vector3(-0.5f, -0.5f, -0.4f),
            new Vector3(1.2f, 0.5f, 0.4f)
        );

        AddRotation(
            clip,
            animationRoot,
            "Chest",
            new Vector3(-0.8f, -0.4f, -0.5f),
            new Vector3(1.8f, 0.4f, 0.5f)
        );

        AddRotation(
            clip,
            animationRoot,
            "UpperChest",
            new Vector3(-0.4f, 0.3f, -0.3f),
            new Vector3(1.2f, -0.3f, 0.3f)
        );

        // ------------------------------------------------------------
        // Neck and head
        // ------------------------------------------------------------

        AddRotation(
            clip,
            animationRoot,
            "Neck",
            new Vector3(-0.5f, -1.1f, -0.4f),
            new Vector3(0.7f, 1.1f, 0.4f)
        );

        AddRotation(
            clip,
            animationRoot,
            "Head",
            new Vector3(0.3f, 0.6f, -0.4f),
            new Vector3(-0.3f, -0.6f, 0.4f)
        );

        // ------------------------------------------------------------
        // Left arm
        // ------------------------------------------------------------

        AddRotation(
            clip,
            animationRoot,
            "LeftArm",
            new Vector3(-0.5f, 0f, -0.6f),
            new Vector3(0.7f, 0f, 0.6f)
        );

        AddRotation(
            clip,
            animationRoot,
            "LeftShoulder",
            new Vector3(-0.4f, 0f, -0.5f),
            new Vector3(0.6f, 0f, 0.5f)
        );

        AddRotation(
            clip,
            animationRoot,
            "LeftUpperArm",
            new Vector3(-0.5f, 0f, -0.7f),
            new Vector3(0.7f, 0f, 0.7f)
        );

        AddRotation(
            clip,
            animationRoot,
            "LeftLowerArm",
            new Vector3(-0.3f, 0f, 0f),
            new Vector3(0.4f, 0f, 0f)
        );

        AddRotation(
            clip,
            animationRoot,
            "LeftHand",
            new Vector3(0f, -0.4f, -0.3f),
            new Vector3(0f, 0.4f, 0.3f)
        );

        // ------------------------------------------------------------
        // Right arm
        // ------------------------------------------------------------

        AddRotation(
            clip,
            animationRoot,
            "RightArm",
            new Vector3(0.7f, 0f, 0.6f),
            new Vector3(-0.5f, 0f, -0.6f)
        );

        AddRotation(
            clip,
            animationRoot,
            "RightShoulder",
            new Vector3(0.6f, 0f, 0.5f),
            new Vector3(-0.4f, 0f, -0.5f)
        );

        AddRotation(
            clip,
            animationRoot,
            "RightUpperArm",
            new Vector3(0.7f, 0f, 0.7f),
            new Vector3(-0.5f, 0f, -0.7f)
        );

        AddRotation(
            clip,
            animationRoot,
            "RightLowerArm",
            new Vector3(0.4f, 0f, 0f),
            new Vector3(-0.3f, 0f, 0f)
        );

        AddRotation(
            clip,
            animationRoot,
            "RightHand",
            new Vector3(0f, 0.4f, 0.3f),
            new Vector3(0f, -0.4f, -0.3f)
        );

        // ------------------------------------------------------------
        // Left leg
        // ------------------------------------------------------------

        AddRotation(
            clip,
            animationRoot,
            "LeftUpperLeg",
            new Vector3(-0.4f, 0f, -0.3f),
            new Vector3(0.6f, 0f, 0.3f)
        );

        AddRotation(
            clip,
            animationRoot,
            "LeftLowerLeg",
            new Vector3(0f, 0f, 0f),
            new Vector3(0.8f, 0f, 0f)
        );

        AddRotation(
            clip,
            animationRoot,
            "LeftFoot",
            new Vector3(0.2f, 0f, 0f),
            new Vector3(-0.3f, 0f, 0f)
        );

        // ------------------------------------------------------------
        // Right leg
        // ------------------------------------------------------------

        AddRotation(
            clip,
            animationRoot,
            "RightUpperLeg",
            new Vector3(0.6f, 0f, 0.3f),
            new Vector3(-0.4f, 0f, -0.3f)
        );

        AddRotation(
            clip,
            animationRoot,
            "RightLowerLeg",
            new Vector3(0.8f, 0f, 0f),
            new Vector3(0f, 0f, 0f)
        );

        AddRotation(
            clip,
            animationRoot,
            "RightFoot",
            new Vector3(-0.3f, 0f, 0f),
            new Vector3(0.2f, 0f, 0f)
        );

        clip.EnsureQuaternionContinuity();
        SetLooping(clip);

        AssetDatabase.CreateAsset(clip, savePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = clip;
        EditorGUIUtility.PingObject(clip);

        EditorUtility.DisplayDialog(
            "Frog Idle Creator",
            "Idle animation created successfully:\n\n" + savePath,
            "OK"
        );

        Debug.Log("Created frog idle animation at: " + savePath);
    }

    // Finds the most appropriate object to use as the animation root.
    private static Transform FindAnimationRoot(Transform selected)
    {
        // First look for an Animator on the selected object or its parents.
        Transform current = selected;

        while (current != null)
        {
            Animator animator = current.GetComponent<Animator>();

            if (animator != null && FindBone(current, "Hips") != null)
                return current;

            current = current.parent;
        }

        // If there is no Animator, check the selected object's hierarchy.
        if (FindBone(selected, "Hips") != null)
            return selected;

        // The user may have selected one of the character's bones.
        // Walk upward until an ancestor containing Hips is found.
        current = selected.parent;
        Transform highestValidRoot = null;

        while (current != null)
        {
            if (FindBone(current, "Hips") != null)
                highestValidRoot = current;

            current = current.parent;
        }

        return highestValidRoot;
    }

    // Searches all children, including inactive children.
    // Also supports names such as "mixamorig:Hips" and "Armature|Hips".
    private static Transform FindBone(Transform root, string boneName)
    {
        if (root == null)
            return null;

        Transform[] allTransforms =
            root.GetComponentsInChildren<Transform>(true);

        // Prefer an exact name match.
        foreach (Transform child in allTransforms)
        {
            if (child.name == boneName)
                return child;
        }

        // Fall back to a prefixed name.
        foreach (Transform child in allTransforms)
        {
            if (child.name.EndsWith(":" + boneName) ||
                child.name.EndsWith("|" + boneName) ||
                child.name.EndsWith("_" + boneName))
            {
                return child;
            }
        }

        return null;
    }

    private static void AddRotation(
        AnimationClip clip,
        Transform root,
        string boneName,
        Vector3 firstOffset,
        Vector3 secondOffset)
    {
        Transform bone = FindBone(root, boneName);

        if (bone == null)
        {
            Debug.LogWarning(
                "Frog Idle Creator: Bone not found: " + boneName
            );

            return;
        }

        string path =
            AnimationUtility.CalculateTransformPath(bone, root);

        Quaternion baseRotation = bone.localRotation;

        float quarter = Duration * 0.25f;
        float half = Duration * 0.5f;
        float threeQuarter = Duration * 0.75f;

        float[] times =
        {
            0f,
            quarter,
            half,
            threeQuarter,
            Duration
        };

        Quaternion[] rotations =
        {
            baseRotation * Quaternion.Euler(firstOffset),
            baseRotation,
            baseRotation * Quaternion.Euler(secondOffset),
            baseRotation,
            baseRotation * Quaternion.Euler(firstOffset)
        };

        SetQuaternionCurve(
            clip,
            path,
            times,
            rotations
        );
    }

    private static void AddPosition(
        AnimationClip clip,
        Transform root,
        string boneName,
        Vector3 firstOffset,
        Vector3 secondOffset)
    {
        Transform bone = FindBone(root, boneName);

        if (bone == null)
        {
            Debug.LogWarning(
                "Frog Idle Creator: Bone not found: " + boneName
            );

            return;
        }

        string path =
            AnimationUtility.CalculateTransformPath(bone, root);

        Vector3 basePosition = bone.localPosition;

        float quarter = Duration * 0.25f;
        float half = Duration * 0.5f;
        float threeQuarter = Duration * 0.75f;

        float[] times =
        {
            0f,
            quarter,
            half,
            threeQuarter,
            Duration
        };

        Vector3 middleOffset =
            Vector3.Lerp(firstOffset, secondOffset, 0.5f);

        Vector3[] positions =
        {
            basePosition + firstOffset,
            basePosition + middleOffset,
            basePosition + secondOffset,
            basePosition + middleOffset,
            basePosition + firstOffset
        };

        SetVectorCurve(
            clip,
            path,
            "m_LocalPosition",
            times,
            positions
        );
    }

    private static void SetQuaternionCurve(
        AnimationClip clip,
        string path,
        float[] times,
        Quaternion[] values)
    {
        AnimationCurve xCurve = new AnimationCurve();
        AnimationCurve yCurve = new AnimationCurve();
        AnimationCurve zCurve = new AnimationCurve();
        AnimationCurve wCurve = new AnimationCurve();

        for (int i = 0; i < times.Length; i++)
        {
            xCurve.AddKey(times[i], values[i].x);
            yCurve.AddKey(times[i], values[i].y);
            zCurve.AddKey(times[i], values[i].z);
            wCurve.AddKey(times[i], values[i].w);
        }

        SmoothCurve(xCurve);
        SmoothCurve(yCurve);
        SmoothCurve(zCurve);
        SmoothCurve(wCurve);

        SetCurve(
            clip,
            path,
            "m_LocalRotation.x",
            xCurve
        );

        SetCurve(
            clip,
            path,
            "m_LocalRotation.y",
            yCurve
        );

        SetCurve(
            clip,
            path,
            "m_LocalRotation.z",
            zCurve
        );

        SetCurve(
            clip,
            path,
            "m_LocalRotation.w",
            wCurve
        );
    }

    private static void SetVectorCurve(
        AnimationClip clip,
        string path,
        string property,
        float[] times,
        Vector3[] values)
    {
        AnimationCurve xCurve = new AnimationCurve();
        AnimationCurve yCurve = new AnimationCurve();
        AnimationCurve zCurve = new AnimationCurve();

        for (int i = 0; i < times.Length; i++)
        {
            xCurve.AddKey(times[i], values[i].x);
            yCurve.AddKey(times[i], values[i].y);
            zCurve.AddKey(times[i], values[i].z);
        }

        SmoothCurve(xCurve);
        SmoothCurve(yCurve);
        SmoothCurve(zCurve);

        SetCurve(
            clip,
            path,
            property + ".x",
            xCurve
        );

        SetCurve(
            clip,
            path,
            property + ".y",
            yCurve
        );

        SetCurve(
            clip,
            path,
            property + ".z",
            zCurve
        );
    }

    private static void SetCurve(
        AnimationClip clip,
        string path,
        string property,
        AnimationCurve curve)
    {
        EditorCurveBinding binding =
            EditorCurveBinding.FloatCurve(
                path,
                typeof(Transform),
                property
            );

        AnimationUtility.SetEditorCurve(
            clip,
            binding,
            curve
        );
    }

    private static void SmoothCurve(AnimationCurve curve)
    {
        for (int i = 0; i < curve.length; i++)
        {
            AnimationUtility.SetKeyLeftTangentMode(
                curve,
                i,
                AnimationUtility.TangentMode.Auto
            );

            AnimationUtility.SetKeyRightTangentMode(
                curve,
                i,
                AnimationUtility.TangentMode.Auto
            );
        }
    }

    private static void SetLooping(AnimationClip clip)
    {
        SerializedObject serializedClip =
            new SerializedObject(clip);

        SerializedProperty settings =
            serializedClip.FindProperty(
                "m_AnimationClipSettings"
            );

        if (settings == null)
            return;

        SerializedProperty loopTime =
            settings.FindPropertyRelative("m_LoopTime");

        if (loopTime != null)
            loopTime.boolValue = true;

        SerializedProperty loopBlend =
            settings.FindPropertyRelative("m_LoopBlend");

        if (loopBlend != null)
            loopBlend.boolValue = true;

        SerializedProperty keepOriginalPositionY =
            settings.FindPropertyRelative(
                "m_KeepOriginalPositionY"
            );

        if (keepOriginalPositionY != null)
            keepOriginalPositionY.boolValue = true;

        serializedClip.ApplyModifiedProperties();
    }
}
