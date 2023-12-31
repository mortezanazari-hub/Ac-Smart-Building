using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace AC
{
    public class AcSmartBuilding : EditorWindow
    {
        private Button _firstInitialize;
        private SliderInt _floorNumber;
        private SliderInt _sideSize;
        private SliderInt _buildingWidth;

        private DropdownField _typeSelector;



       // public static GameObject SelectedGameObject = Methods.FindBuildingMesh("Test01");
        private string _typeSelected = "";


        #region Windows


        [MenuItem("test/TestGui")]
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
            _firstInitialize = root.Q<Button>("firstInitialize");
            _floorNumber = root.Q<SliderInt>("floorNumber");
            _sideSize = root.Q<SliderInt>("sideSize");
            _buildingWidth = root.Q<SliderInt>("buildingWidth");



            //type selector dropdown field
            //_typeSelector = root.Q<DropdownField>("typeSelector");
            //_typeSelector.choices = Automation.Types;


            //-----------------------------------------------------------------------------------------//
            //Assign CallBacks
             _firstInitialize.clicked += EnableObjectPlacing;
            //_typeSelector.RegisterValueChangedCallback(evt => TypeSelect(evt.newValue));
            _floorNumber.RegisterValueChangedCallback(evt => FloorNumberManager(Methods.FindBuildingMesh("Test01"), evt.newValue));
            _sideSize.RegisterValueChangedCallback(evt => SideNumberManager(Methods.FindBuildingMesh("Test01"), evt.newValue));
            _buildingWidth.RegisterValueChangedCallback(evt => MiddleNumberManager(Methods.FindBuildingMesh("Test01"), evt.newValue));

            //-----------------------------------------------------------------------------------------//
        }


        #endregion


        private string TypeSelect(string str)
        {
            return _typeSelected = str;
        }


        private static void FloorNumberManager(GameObject gObject,int num)
        {
            Methods.FloorManagement(gObject,num);
        }
        private static void SideNumberManager(GameObject gObject,int num)
        {
            Methods.SideSizeManagement(gObject,num);
        }
        private static void MiddleNumberManager(GameObject gObject,int num)
        {
            Methods.WidthSizeManagement(gObject,num);
        }
        //------------------------------------------------------------------------------------------------------------//


        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        

       // [MenuItem("Tools/Test First Initialize")]
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



   

        
        
        
        
        
        //
        //
        //
        //
        //
        // [MenuItem("Tools/Test Add Level")]
        // private static void AddLeveler()
        // {
        //     Methods.AddLevel(SelectedGameObject);
        // }
        //
        //
        // [MenuItem("Tools/Test Add Side")]
        // private static void AddSide()
        // {
        //     Methods.AddSide(SelectedGameObject);
        // }
        //
        //
        // [MenuItem("Tools/Test Add Middle")]
        // private static void AddMiddle()
        // {
        //     Methods.AddMiddle(SelectedGameObject);
        // }
        //
        //
        // [MenuItem("Tools/Test Level Reducer")]
        // private static void LevelReducer()
        // {
        //     Methods.LevelReducer(SelectedGameObject);
        // }
        //
        //
        // [MenuItem("Tools/Test Side Reducer")]
        // private static void SideReducer()
        // {
        //     Methods.SideReducer(SelectedGameObject);
        // }
        //
        //
        // [MenuItem("Tools/Test Middle Reducer")]
        // private static void MiddleReducer()
        // {
        //     Methods.MiddleReducer(SelectedGameObject);
        // }
    }
}