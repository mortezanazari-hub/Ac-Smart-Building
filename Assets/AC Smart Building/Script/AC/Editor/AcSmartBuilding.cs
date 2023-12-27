using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AC
{
    public class AcSmartBuilding : EditorWindow
    {
        private Button _actionSomething;

        private DropdownField _typeSelector;

        private string _typeSelected = "";

        #region Windows

        [MenuItem("Tools/ArenCg/Ac Building System")]
        public static void OpenEditorWindow()
        {
            var window = GetWindow<AcSmartBuilding>();
            window.titleContent = new GUIContent("Ac Building System");
            window.maxSize = new Vector2(320, 550);
            window.minSize = window.maxSize;
        }

        #endregion

        #region CreateGUI

        private void CreateGUI()
        {
            var root = rootVisualElement;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/AC Smart Building/Resource/Ui Documents/ImageAlphaEditorWindow.uxml");
            var tree = visualTree.Instantiate();
            root.Add(tree);
            //-----------------------------------------------------------------------------------------//
            //Assign Elements
            _actionSomething = root.Q<Button>("actionSomething");


            //type selector dropdown field
            _typeSelector = root.Q<DropdownField>("typeSelector");
            //_typeSelector.choices = Automation.Types;

            //-----------------------------------------------------------------------------------------//
            //Assign CallBacks
            // _actionSomething.clicked += ActionSomething;
            _typeSelector.RegisterValueChangedCallback(evt => TypeSelect(evt.newValue));

            //-----------------------------------------------------------------------------------------//
        }

        #endregion

        private string TypeSelect(string str)
        {
            return _typeSelected = str;
        }


        //------------------------------------------------------------------------------------------------------------//


        [MenuItem("Tools/Test First Initialize")]
        private static void EnableObjectPlacing()
        {
            Automation.AutoMakeBuilding();
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static float RotateAngle = 0f; // Add this variable to store the rotation angle

        private static void OnSceneGUI(SceneView sceneView)
        {
            Event guiEvent = Event.current;
            var selectedBuilding = StaticResources.SelectedBuilding(StaticResources.BuildingsList);
            Vector3 size = new Vector3(
                Methods.BuildingWidth(selectedBuilding),
                Methods.BuildingHeight(selectedBuilding),
                Methods.BuildingLength(selectedBuilding)
            );

            float enter;
            Ray ray = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
            var groundPlane = new Plane(Vector3.up, Vector3.zero);
            var rotation = Quaternion.Euler(0f, RotateAngle, 0f);
            switch (guiEvent.type)
            {
                case EventType.Repaint:
                case EventType.MouseDown:
                    Vector3 rotatedSize = rotation * size;
                    if (groundPlane.Raycast(ray, out enter))
                    {
                        var hitPoint = ray.GetPoint(enter);
                        var cubeCenterOffset = Math.Abs(RotateAngle - 90) < 0 || Math.Abs(RotateAngle - (-90)) < 0
                            ? new Vector3(-rotatedSize.x, rotatedSize.y, rotatedSize.z)
                            : new Vector3(rotatedSize.x, rotatedSize.y, -rotatedSize.z);
                        cubeCenterOffset /= 2;
                        var cubeCenter = hitPoint + cubeCenterOffset;
                        Handles.DrawWireCube(cubeCenter, rotatedSize);
                        if (guiEvent.type == EventType.Repaint)
                        {
                            Handles.color = Color.red;
                            Handles.ArrowHandleCap(0, hitPoint, rotation, 5, EventType.Repaint);
                        }
                        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 &&
                            guiEvent.modifiers == EventModifiers.None)
                        {
                            guiEvent.Use();
                            Methods.FirstInitialize(selectedBuilding, hitPoint, RotateAngle);
                            SceneView.duringSceneGui -= OnSceneGUI;
                        }
                        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1 &&
                            guiEvent.modifiers == EventModifiers.None)
                        {
                            SceneView.duringSceneGui -= OnSceneGUI;
                        }
                        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 2 &&
                            guiEvent.modifiers == EventModifiers.None)
                        {
                            RotateAngle += 90f;
                            if (RotateAngle > 180f) RotateAngle -= 360f;
                            sceneView.Repaint();
                        }
                    }
                    break;
                case EventType.KeyDown when guiEvent.keyCode == KeyCode.Space:
                    RotateAngle += 90f;
                    if (RotateAngle > 180f) RotateAngle -= 360f;
                    sceneView.Repaint();
                    break;
                case EventType.KeyDown when guiEvent.keyCode == KeyCode.Escape:
                    SceneView.duringSceneGui -= OnSceneGUI;
                    break;
                case EventType.Layout:
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                    break;
            }
        }


        //------------------------------------------------------------------------------------------------------------//


        [MenuItem("Tools/Test Level By Number")]
        private static void LevelNumber()
        {
            int num;
            InputWindow window = new InputWindow("Enter the level:");
            window.Result += (value) => {
                num = value;
            };
            
            window.Show();
            Methods.LevelManagement(GameObject.Find("Test01"),num);
        }
        [MenuItem("Tools/Test Add Level")]
        private static void AddLeveler()
        {
            Methods.AddLevel(GameObject.Find("Test01"));
        }

        [MenuItem("Tools/Test Add Side")]
        private static void AddSide()
        {
            Methods.AddSide(GameObject.Find("Test01"));
        }

        [MenuItem("Tools/Test Add Middle")]
        private static void AddMiddle()
        {
            Methods.AddMiddle(GameObject.Find("Test01"));
        }

        [MenuItem("Tools/Test Level Reducer")]
        private static void LevelReducer()
        {
            Methods.LevelReducer(GameObject.Find("Test01"));
        }

        [MenuItem("Tools/Test Side Reducer")]
        private static void SideReducer()
        {
            Methods.SideReducer(GameObject.Find("Test01"));
        }

        [MenuItem("Tools/Test Middle Reducer")]
        private static void MiddleReducer()
        {
            Methods.MiddleReducer(GameObject.Find("Test01"));
        }
    }
}