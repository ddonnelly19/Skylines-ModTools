﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModTools.Explorer
{
    public static class GUIEnumerable
    {
        public static void OnSceneTreeReflectIEnumerable(SceneExplorerState state, ReferenceChain refChain, object myProperty)
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain)) return;

            var enumerable = myProperty as IEnumerable;
            if (enumerable == null)
            {
                return;
            }

            int count = 0;
            var oldRefChain = refChain;

            foreach (var value in enumerable)
            {
                refChain = oldRefChain.Add(count);

                var type = value.GetType();

                GUILayout.BeginHorizontal();
                GUILayout.Space(ModTools.Instance.config.sceneExplorerTreeIdentSpacing * refChain.Ident);

                GUIExpander.ExpanderControls(state, refChain, type);

                GUI.contentColor = ModTools.Instance.config.typeColor;

                GUILayout.Label(type.ToString() + " ");

                GUI.contentColor = ModTools.Instance.config.nameColor;

                GUILayout.Label($"{oldRefChain.LastItemName}.[{count}]");

                GUI.contentColor = Color.white;

                GUILayout.Label(" = ");

                GUI.contentColor = ModTools.Instance.config.valueColor;

                GUILayout.Label(value == null ? "null" : value.ToString());

                GUI.contentColor = Color.white;

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if (!TypeUtil.IsSpecialType(type) && state.expandedObjects.ContainsKey(refChain))
                {
                    if (value is GameObject)
                    {
                        var go = value as GameObject;
                        foreach (var component in go.GetComponents<Component>())
                        {
                            GUIComponent.OnSceneTreeComponent(state, refChain, component);
                        }
                    }
                    else if (value is Transform)
                    {
                        GUITransform.OnSceneTreeReflectUnityEngineTransform(refChain, (Transform)value);
                    }
                    else
                    {
                        GUIReflect.OnSceneTreeReflect(state, refChain, value);
                    }
                }

                count++;
                if (count >= 1024)
                {
                    SceneExplorerCommon.OnSceneTreeMessage(refChain, "Array too large to display");
                    break;
                }
            }
        }
    }
}