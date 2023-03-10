using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Build.Editor
{
    class UITemplate
    {
        VisualTreeAsset m_Template;
        StyleSheet m_StyleSheet;
        StyleSheet m_StyleSheetSkinVariant;

        public UITemplate(string name)
        {
            m_Template = Package.LoadResource<VisualTreeAsset>($"uxml/{name}.uxml", true);
            m_StyleSheet = Package.LoadResource<StyleSheet>($"uss/{name}.uss", false);
            m_StyleSheetSkinVariant = Package.LoadResource<StyleSheet>($"uss/{name}{(EditorGUIUtility.isProSkin ? "_dark" : "_light")}.uss", false);
        }

        public VisualElement Clone(VisualElement root = null)
        {
            root = CloneTemplate(root);
            AddStyleSheets(root);
            return root;
        }

        VisualElement CloneTemplate(VisualElement target)
        {
            if (target == null)
            {
                return m_Template.CloneTree();
            }
            m_Template.CloneTree(target);
            return target;
        }

        void AddStyleSheets(VisualElement root)
        {
            if (m_StyleSheet != null)
            {
                root.styleSheets.Add(m_StyleSheet);
            }
            if (m_StyleSheetSkinVariant != null)
            {
                root.styleSheets.Add(m_StyleSheetSkinVariant);
            }
        }
    }
}
