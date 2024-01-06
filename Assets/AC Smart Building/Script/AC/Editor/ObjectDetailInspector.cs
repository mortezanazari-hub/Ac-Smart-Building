using UnityEditor;

namespace AC
{
    [CustomEditor(typeof(ObjectDetail))]
    public class ObjectDetailInspector : Editor
    {
        public override void OnInspectorGUI()
        {
           // base.OnInspectorGUI();
            DrawDefaultInspector();
        }
    }
}