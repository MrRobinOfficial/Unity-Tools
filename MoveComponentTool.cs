#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UnityTools.Editor
{
    public static class MoveComponentTool
    {
        const string k_MoveToTop = "CONTEXT/Component/Move To Top";
        const string k_MoveToBottom = "CONTEXT/Component/Move To Bottom";

        [MenuItem(k_MoveToTop, isValidateFunction: true)]
        private static bool MoveComponentToTopValidate(MenuCommand command)
        {
            var component = (Component)command.context;
            var components = component.gameObject.GetComponents<Component>();

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] != component || i != 1)
                    continue;

                return false;
            }

            return true;
        }

        [MenuItem(k_MoveToTop, priority = 500)]
        private static void MoveComponentToTop(MenuCommand command)
        {
            while (ComponentUtility.MoveComponentUp((Component)command.context)) ;
        }

        [MenuItem(k_MoveToBottom, isValidateFunction: true)]
        private static bool MoveComponentToBottomValidate(MenuCommand command)
        {
            var component = (Component)command.context;
            var components = component.gameObject.GetComponents<Component>();

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] != component || i != (components.Length - 1))
                    continue;

                return false;
            }

            return true;
        }

        [MenuItem(k_MoveToBottom, priority = 500)]
        private static void MoveComponentToBottom(MenuCommand command)
        {
            while (ComponentUtility.MoveComponentDown((Component)command.context)) ;
        }
    }
} 
#endif