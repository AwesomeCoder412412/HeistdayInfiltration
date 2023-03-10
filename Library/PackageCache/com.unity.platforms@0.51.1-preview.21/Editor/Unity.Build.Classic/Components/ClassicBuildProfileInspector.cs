using Unity.Build.Editor;
using Unity.Properties.UI;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Build.Classic
{
    sealed class ClassicBuildProfileInspector : Inspector<ClassicBuildProfile>
    {
        static readonly string s_InstallPackage = L10n.Tr("Install Package");
        static readonly string s_InstallWithUnityHub = L10n.Tr("Install with Unity Hub");
        static readonly string s_OpenDownloadPage = L10n.Tr("Open Download Page");

        VisualElement m_HelpBoxPackage;
        TextElement m_MessagePackage;
        Button m_InstallPackage;

        VisualElement m_HelpBoxModule;
        TextElement m_MessageModule;
        Button m_InstallModule;

        public override VisualElement Build()
        {
            var root = Resources.ClassicBuildProfile.Clone();

            var platform = root.Q("platform");
            DoDefaultGui(platform, nameof(ClassicBuildProfile.Platform));

            m_HelpBoxPackage = root.Q("helpbox-package");
            m_MessagePackage = m_HelpBoxPackage.Q<TextElement>("message");
            m_InstallPackage = m_HelpBoxPackage.Q<Button>("install");
            m_InstallPackage.text = s_InstallPackage;
            m_InstallPackage.RegisterCallback<ClickEvent>(e => Target.Platform.InstallPackage());

            m_HelpBoxModule = root.Q("helpbox-module");
            m_MessageModule = m_HelpBoxModule.Q<TextElement>("message");
            m_InstallModule = m_HelpBoxModule.Q<Button>("install");
            m_InstallModule.RegisterCallback<ClickEvent>(e => Target.Platform.InstallModule());

            var configuration = root.Q("configuration");
            DoDefaultGui(configuration, nameof(ClassicBuildProfile.Configuration));

            Update();
            return root;
        }

        public override void Update()
        {
            var platform = Target.Platform;

            if (platform != null && platform.HasPackage && !platform.IsPackageInstalled)
            {
                string message = $"Selected platform requires {platform.DisplayName} platform package to be installed.";
                if (m_MessagePackage.text != message)
                    m_MessagePackage.text = message;
                m_HelpBoxPackage.style.display = DisplayStyle.Flex;
            }
            else
            {
                m_HelpBoxPackage.style.display = DisplayStyle.None;
            }

            if (platform != null && !platform.IsModuleInstalled())
            {
                string message = $"Selected platform requires {platform.DisplayName} platform module to be installed.";
                if (m_MessageModule.text != message)
                    m_MessageModule.text = message;
                var installText = PlatformExtensions.IsEditorInstalledWithHub && Target.Platform.IsPublic ? s_InstallWithUnityHub : s_OpenDownloadPage;
                if (m_InstallModule.text != installText)
                    m_InstallModule.text = installText;
                m_HelpBoxModule.style.display = DisplayStyle.Flex;
            }
            else
            {
                m_HelpBoxModule.style.display = DisplayStyle.None;
            }
        }
    }
}
