using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

namespace OukeUtils
{
    public class AutoHighlightAnimSelect
    {
        [MenuItem("ArtTools/Scene/HighlightAnimSelect &f")]
        public static void CheckAnimationWindow()
        {
            init();
            //GetAllProp();
            if (_window != null)
            {
                if (FindNodeIDByPath(out int nodeid))
                {
                    UpdateAnimationWindowSelect(nodeid);
                    UpdateAnimationPanel(nodeid);
                    var slectFunc = animationWindowType.GetMethod("Focus");
                    slectFunc.Invoke(_window, null);
                }

            }

        }

        private static Action mAnimationWindowOnchangeDelegate;

        static UnityEngine.Object _window;

        static BindingFlags _flags;
        static FieldInfo _animEditor;

        static Type _animEditorType;
        static System.Object _animEditorObject;
        static FieldInfo _animWindowState;
        static Type _windowStateType;
        static System.Object _animStateObject;

        static Type animationWindowType = null;


        static UnityEngine.Object GetOpenAnimationWindow()
        {
            UnityEngine.Object[] openAnimationWindows = Resources.FindObjectsOfTypeAll(GetAnimationWindowType());
            if (openAnimationWindows.Length > 0)
            {
                return openAnimationWindows[0];
            }
            return null;
        }

        static System.Type GetAnimationWindowType()
        {
            if (animationWindowType == null)
            {
                animationWindowType = Type.GetType("UnityEditor.AnimationWindow,UnityEditor");
            }
            return animationWindowType;
        }

        public static void init()
        {
            _window = GetOpenAnimationWindow();

            if (_window == null)
                return;

            _flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
            _animEditor = GetAnimationWindowType().GetField("m_AnimEditor", _flags);

            _animEditorType = _animEditor.FieldType;
            _animEditorObject = _animEditor.GetValue(_window);
            _animWindowState = _animEditorType.GetField("m_State", _flags);
            _windowStateType = _animWindowState.FieldType;
            _animStateObject = _animWindowState.GetValue(_animEditorObject);
        }


        public static bool FindNodeIDByPath(out int nodeID)
        {
            nodeID = -1;

            if (Selection.activeGameObject != null && _window != null)
            {
                var selectPath = Selection.activeGameObject.transform.GetFullPath();
                if(!selectPath.StartsWith("Bip001"))
                    selectPath = selectPath.Substring(selectPath.IndexOf("/") + 1);
                Debug.Log("find path : " + selectPath);


                var hierarchyNodeprop = _windowStateType.GetProperty("selectedHierarchyNodes", _flags);
                var hierarchyNodeObj = hierarchyNodeprop.GetValue(_animStateObject) as IList;
                var hierarchyNodeType = hierarchyNodeObj[0].GetType();

                var datasprop = _windowStateType.GetField("hierarchyData", _flags);
                var datasobj = datasprop.GetValue(_animStateObject);

                var rowListFunc = datasprop.FieldType.GetMethod("GetRows", _flags);
                var itemList = rowListFunc.Invoke(datasobj, null) as IList;

                foreach (var listitem in itemList)
                {
                    if (hierarchyNodeType.IsAssignableFrom(listitem.GetType()))
                    {
                        var path = hierarchyNodeType.GetField("path", _flags).GetValue(listitem) as String;
                        if (path == selectPath)
                        {
                            nodeID = (int)hierarchyNodeType.GetProperty("id", _flags).GetValue(listitem);
                            Debug.Log("catch ID  " + nodeID);
                            return true;
                        }
                    }
                }

            }

            return false;
        }

        public static void UpdateAnimationWindowSelect(int nodeID)
        {
            var mts = _windowStateType.GetMethods();
            MethodInfo method = null;
            object[] parametersArray = new object[] { nodeID, false, true };
            foreach (var mt in mts)
            {
                if (mt.ToString().Contains("SelectHierarchyItem(Int32, Boolean, Boolean)"))
                {
                    method = mt;
                    break;
                }
            }
            if (method != null)
            {
                method.Invoke(_animStateObject, parametersArray);
            }
        }


        public static void UpdateAnimationPanel(int nodeID)
        {
            object[] parametersArray = new object[] { nodeID };
            var dopeline = _windowStateType.GetMethod("GetDopeline").Invoke(_animStateObject, parametersArray);
            if (dopeline != null)
            {
                var positionField = dopeline.GetType().GetField("position", _flags);
                if (positionField != null)
                {
                    var posRect = (Rect)positionField.GetValue(dopeline);
                    var hierarchyState = _windowStateType.GetField("hierarchyState", _flags);
                    var hierarchyStateObj = hierarchyState.GetValue(_animStateObject);
                    Vector2 updatePos = new Vector2(0f, Mathf.Max(0f, posRect.position.y - 100f));
                    object[] posArray = new object[] { updatePos };
                    hierarchyStateObj.GetType().InvokeMember("scrollPos", BindingFlags.SetField | _flags, null, hierarchyStateObj, posArray);

                    var repaintFunc = animationWindowType.GetMethod("Repaint");
                    repaintFunc.Invoke(_window, null);
                }
            }

        }

    }
}