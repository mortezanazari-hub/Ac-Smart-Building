using UnityEditor;
using UnityEngine.UIElements;

namespace AC
{
    using UnityEngine;
    using UnityEditor;

    public class InputWindow : EditorWindow
    {
        private static int num;
        public void OnGUI()
        {
            // نمایش عنوان پنجره
            GUILayout.Label("Test Level By Number");
            // نمایش یک کادر متنی برای ورود عدد
            num = EditorGUILayout.IntField(10);
            // نمایش یک دکمه OK
            GUILayout.Button("OK");
        }
    }
}