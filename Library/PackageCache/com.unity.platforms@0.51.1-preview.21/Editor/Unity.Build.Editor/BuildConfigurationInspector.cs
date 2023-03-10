using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Properties.UI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIExtras;

namespace Unity.Build.Editor
{
    class BuildConfigurationInspector : Inspector<BuildConfigurationInspectorData>
    {
        struct Classes
        {
            public const string BaseClass = "build-configuration";
            public const string ActionPopup = BaseClass + "__action-popup";
        }

        struct BuildAction : IEquatable<BuildAction>
        {
            readonly Func<BuildConfiguration, ResultBase> m_Action;

            public string Name { get; }

            public BuildAction(string name, Func<BuildConfiguration, ResultBase> action)
            {
                Name = name;
                m_Action = action;
            }

            public void Execute(BuildConfiguration config)
            {
                var result = m_Action(config);
                if (!result)
                {
                    if (result is ResultPackageNotInstalled)
                    {
                        var platform = config.GetPlatform();
                        var title = "Missing Platform Package";
                        var message = $"The selected build configuration platform requires {platform.DisplayName} platform package to be installed.";
                        if (EditorUtility.DisplayDialog(title, message, "Install Package", "Close"))
                            platform.InstallPackage();

                        return;
                    }
                }

                if (BuildConfigurationInspectorEvents.OnAfterBuildAction(config, result))
                    return;

                result.LogResult();
            }

            public bool Equals(BuildAction other) => Name == other.Name;
        }

        const string k_ShowPipelineUsedComponentsKey = nameof(BuildConfigurationInspector) + "." + nameof(ShowUsedComponents);
        const string k_CurrentActionIndexKey = nameof(BuildConfigurationInspector) + "." + nameof(CurrentActionIndex);
        const string k_DependenciesFoldoutOpenKey = nameof(BuildConfigurationInspector) + "." + nameof(DependenciesFoldoutOpen);
        const string k_AvailableActionsKey = nameof(BuildConfigurationInspector) + "." + nameof(AvailableActionsKey);
        private const int k_AvailableActions_NoRun = 0;
        private const int k_AvailableActions_Run = 1;

        static readonly string s_Show = L10n.Tr("Show");
        static readonly string s_ShowUsedComponents = L10n.Tr("Show Suggested Components");
        static readonly string s_Dependencies = L10n.Tr("Shared Configurations");
        static readonly string s_AddDependency = L10n.Tr("Add Configuration");
        static readonly string s_AddComponent = L10n.Tr("Add Component");

        static readonly BuildAction s_BuildAction = new BuildAction(L10n.Tr("Build"), Build);
        static readonly BuildAction s_BuildAndRunAction = new BuildAction(L10n.Tr("Build and Run"), BuildAndRun);
        static readonly BuildAction s_RunAction = new BuildAction(L10n.Tr("Run"), Run);
        static readonly BuildAction s_CleanAction = new BuildAction(L10n.Tr("Clean"), Clean);
        static readonly BuildAction[] s_Actions = new[] { s_BuildAction, s_BuildAndRunAction, s_RunAction, s_CleanAction };
        static readonly BuildAction[] s_ActionsNoRun = new[] { s_BuildAction, s_BuildAndRunAction, s_CleanAction };
        static Platform lastPlatform;

        SearchElement m_Search;
        VisualElement m_Components;
        Dictionary<BuildComponentInspectorData, PropertyElement> m_ComponentsMap;
        bool m_SearchBindingRegistered;
        bool actionsEnabled;

        public static bool ShowUsedComponents
        {
            get => EditorPrefs.GetBool(k_ShowPipelineUsedComponentsKey, false);
            set => EditorPrefs.SetBool(k_ShowPipelineUsedComponentsKey, value);
        }

        bool DependenciesFoldoutOpen
        {
            get => SessionState.GetBool(k_DependenciesFoldoutOpenKey, false);
            set => SessionState.SetBool(k_DependenciesFoldoutOpenKey, value);
        }

        static int CurrentActionIndex
        {
            get
            {
                var actions = AvaliableActions;

                // The selected index is always relative to the whole list of actions,
                // so we can keep the same option across different sets of actions
                int internalDefaultValue = Array.IndexOf(s_Actions, s_BuildAndRunAction);

                var internalSelectedIndex = EditorPrefs.GetInt(k_CurrentActionIndexKey, internalDefaultValue);
                var selectedAction = s_Actions[internalSelectedIndex];

                // We need to find the selected action relative to our current list
                int selectedIndex = Array.IndexOf(actions, selectedAction);

                // If the action is not valid, then we need to select a default
                if (selectedIndex == -1)
                {
                    // The stored value is not right, select a valid one
                    selectedIndex = Array.IndexOf(actions, s_BuildAndRunAction);
                    CurrentActionIndex = selectedIndex;
                }
                return selectedIndex;
            }
            set
            {
                // We need to store the index relative to the whole list, not the current one
                // so we can keep the same option across different sets of actions
                var actions = AvaliableActions;
                int internalValue;
                if (value == -1 || value > actions.Length)
                {
                    internalValue = Array.IndexOf(s_Actions, s_BuildAndRunAction);
                }
                else
                {
                    BuildAction currentAction = actions[value];
                    internalValue = Array.IndexOf(s_Actions, currentAction);
                }
                EditorPrefs.SetInt(k_CurrentActionIndexKey, internalValue);
            }
        }

        static int AvailableActionsKey
        {
            get => EditorPrefs.GetInt(k_AvailableActionsKey, k_AvailableActions_NoRun);
            set => EditorPrefs.SetInt(k_AvailableActionsKey, value);
        }

        static BuildAction[] AvaliableActions => AvailableActionsKey == k_AvailableActions_NoRun ? s_ActionsNoRun : s_Actions;
        static BuildAction CurrentAction => AvaliableActions[CurrentActionIndex];

        public static bool IsCurrentActionBuildAndRun => CurrentAction.Equals(s_BuildAndRunAction);

        static ResultBase Build(BuildConfiguration config)
        {
            var result = config.CanBuild();
            return result ? config.Build() : result;
        }

        static ResultBase Run(BuildConfiguration config)
        {
            var canRun = config.CanRun();
            if (!canRun)
                return canRun;

            using (var result = config.Run())
            {
                return result;
            }
        }

        static ResultBase BuildAndRun(BuildConfiguration config)
        {
            var result = Build(config);
            if (result && config.GetPlatform().HasPackage)
            {
                return Run(config);
            }
            return result;
        }

        static ResultBase Clean(BuildConfiguration config)
        {
            var result = config.Clean();
            if (!result)
                result.LogResult();

            return result;
        }

        private void UpdateAvaliableActions()
        {
            actionsEnabled = true;
            var context = GetContext<BuildConfigurationContext>();
            if (context != null)
            {
                var platform = context.ImporterEditor.GetPlatform();
                if (platform != null)
                {
                    AvailableActionsKey = platform.HasPackage ? k_AvailableActions_Run : k_AvailableActions_NoRun;
                }
                else
                {
                    actionsEnabled = false;
                }
            }
        }

        public override VisualElement Build()
        {
            var root = Resources.BuildConfiguration.Clone();
            root.AddToClassList(Classes.BaseClass);

            UpdateAvaliableActions();

            var header = root.Q("header");
            var optionsButton = header.Q<Button>("options");
            optionsButton.RegisterCallback<ClickEvent>(e =>
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(s_Show), Target.Show, () => Target.Show = !Target.Show);
                menu.AddItem(new GUIContent(s_ShowUsedComponents), ShowUsedComponents, () =>
                {
                    ShowUsedComponents = !ShowUsedComponents;
                    Target.RefreshComponents();
                });
                menu.DropDown(optionsButton.worldBound);
                e.StopPropagation();
            });

            var actions = header.Q("actions");
            var actionButton = actions.Q<Button>("action");
            actionButton.text = CurrentAction.Name;
            actionButton.clickable.clicked += () =>
            {
                var context = GetContext<BuildConfigurationContext>();
                var config = context.ImporterEditor.HandleUnappliedImportSettings();
                if (config != null && config)
                    EditorApplication.delayCall += () => CurrentAction.Execute(config);
            };
            actionButton.SetEnabled(actionsEnabled);

            var actionPopup = new PopupField<BuildAction>(AvaliableActions.ToList(), CurrentActionIndex, a => string.Empty, a => a.Name);
            actionPopup.AddToClassList(Classes.ActionPopup);
            actionPopup.RegisterValueChangedCallback(evt =>
            {
                CurrentActionIndex = Array.IndexOf(AvaliableActions, evt.newValue);
                actionButton.text = CurrentAction.Name;
            });
            actions.Add(actionPopup);

            var dependenciesFoldout = root.Q<Foldout>("dependencies");
            dependenciesFoldout.RegisterValueChangedCallback(e => DependenciesFoldoutOpen = e.newValue);
            dependenciesFoldout.text = s_Dependencies;
            dependenciesFoldout.value = DependenciesFoldoutOpen;

            var dependencies = dependenciesFoldout.Q<ListView>();
            dependencies.SetEnabled(!Target.IsReadOnly);
            dependencies.itemsSource = Target.Dependencies;
            dependencies.makeItem = () =>
            {
                var dependency = Resources.BuildConfigurationDependency.Clone();
                var objectField = dependency.Q<ObjectField>("object");
                objectField.objectType = typeof(BuildConfiguration);
                objectField.allowSceneObjects = false;
                return dependency;
            };
            dependencies.bindItem = (element, index) =>
            {
                var objectField = element.Q<ObjectField>("object");
                objectField.RegisterValueChangedCallback(e => Target.Dependencies[index] = e.newValue as BuildConfiguration);
                objectField.value = Target.Dependencies[index];
                element.Q<Button>("remove").clickable = new Clickable(() =>
                {
                    Target.Dependencies.RemoveAt(index);
                    dependencies.style.minHeight = Target.Dependencies.Count * dependencies.itemHeight;
                    dependencies.Refresh();
                });
            };
            dependencies.itemHeight = 22;
            dependencies.selectionType = SelectionType.Single;
            dependencies.reorderable = true;
            dependencies.style.flexGrow = 1;
            dependencies.style.minHeight = Target.Dependencies.Count * dependencies.itemHeight;

            var addDependency = dependenciesFoldout.Q<Button>("add");
            addDependency.SetEnabled(!Target.IsReadOnly);
            addDependency.text = "+ " + s_AddDependency;
            addDependency.clickable.clicked += () =>
            {
                Target.Dependencies.Add(default);
                dependencies.style.minHeight = Target.Dependencies.Count * dependencies.itemHeight;
                dependencies.Refresh();
            };

            m_Search = root.Q<SearchElement>("search");
            m_Search.AddSearchDataCallback<BuildComponentInspectorData>(c => new[] { c.ComponentName }.Concat(c.FieldNames));

            m_Components = root.Q("components");
            m_ComponentsMap = new Dictionary<BuildComponentInspectorData, PropertyElement>(Target.Components.Length);
            UpdateComponents();

            var addComponentButton = root.Q<Button>("add-component");
            addComponentButton.SetEnabled(!Target.IsReadOnly);
            addComponentButton.text = s_AddComponent;
            addComponentButton.clickable.clicked += () =>
            {
                var items = new List<SearchView.Item>();
                var types = TypeCache.GetTypesDerivedFrom<IBuildComponent>();
                foreach (var type in types)
                {
                    if (type.IsAbstract || type.IsInterface ||
                        type.HasAttribute<HideInInspector>() ||
                        type.HasAttribute<ObsoleteAttribute>())
                    {
                        continue;
                    }

                    if (Target.ComponentTypes.Contains(type))
                    {
                        continue;
                    }

                    string name;
                    if (type.HasAttribute<DisplayNameAttribute>())
                    {
                        name = type.GetCustomAttribute<DisplayNameAttribute>().Name;
                    }
                    else
                    {
                        name = ObjectNames.NicifyVariableName(type.Name);
                    }
                    var category = type.Namespace ?? "Global";

                    items.Add(new SearchView.Item
                    {
                        Path = !string.IsNullOrEmpty(category) ? $"{category}/{name}" : name,
                        Data = type
                    });
                }
                items = items.OrderBy(item => item.Path).ToList();

                SearchWindow searchWindow = SearchWindow.Create();
                searchWindow.Title = "Component";
                searchWindow.Items = items;
                searchWindow.OnSelection += item => Target.SetComponent((Type)item.Data);

                var rect = EditorWindow.focusedWindow.position;
                var button = addComponentButton.worldBound;
                searchWindow.position = new Rect(rect.x + button.x, rect.y + button.y + button.height, button.width, 315);
                searchWindow.ShowPopup();
            };

            Target.OnComponentsChanged += () =>
            {
                m_Search.value = string.Empty;
                m_Components.Clear();
                m_ComponentsMap.Clear();
                UpdateComponents();
            };
            Target.Dependencies.OnChanged += () => Target.RefreshComponents();

            return root;
        }

        private void UpdatePlatform()
        {
            var context = GetContext<BuildConfigurationContext>();
            if (context != null)
            {
                var importer = context.ImporterEditor;
                var platform = importer.GetPlatform();
                if (lastPlatform != platform)
                {
                    // We need to rebuild
                    importer.Rebuild();
                    lastPlatform = platform;
                }
            }
        }

        public override void Update()
        {
            UpdatePlatform();

            if (!m_SearchBindingRegistered)
            {
                var handler = m_Search.GetUxmlSearchHandler() as SearchHandler<BuildComponentInspectorData>;
                if (handler == null)
                {
                    return;
                }

                handler.OnBeginSearch += query =>
                {
                    foreach (var element in m_ComponentsMap.Values)
                    {
                        element.style.display = DisplayStyle.None;
                    }
                };

                handler.OnFilter += (query, filtered) =>
                {
                    foreach (var component in filtered)
                    {
                        m_ComponentsMap[component].style.display = DisplayStyle.Flex;
                    }
                };

                m_SearchBindingRegistered = true;
            }
        }

        void UpdateComponents()
        {
            foreach (var component in Target.Components)
            {
                var element = new PropertyElement();
                element.SetTarget(component);
                m_Components.Add(element);
                m_ComponentsMap.Add(component, element);
            }
        }
    }
}
