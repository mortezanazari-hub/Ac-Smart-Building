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
        private static void OnSceneGUI(SceneView sceneView)
        {
            Event guiEvent = Event.current;
            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
            {
                guiEvent.Use();
                Ray ray = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
                Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // A ground plane with normal Vector3.up and passing through the origin
                if (groundPlane.Raycast(ray, out float enter))
                {
                    var hitPoint = ray.GetPoint(enter);
                    Methods.FirstInitialize(StaticResources.BuildingsList[0],hitPoint);
                    SceneView.duringSceneGui -= OnSceneGUI; // Unsubscribe to prevent multiple subscriptions
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------//


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