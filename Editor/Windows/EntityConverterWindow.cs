using Scellecs.Morpeh.EntityConverter.Editor.Baking;
using Scellecs.Morpeh.EntityConverter.Logs;
using Scellecs.Morpeh.EntityConverter.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    using SettingsService = Scellecs.Morpeh.EntityConverter.Editor.Settings.SettingsService;

    public sealed class EntityConverterWindow : EditorWindow
    {
        private IEntityConverterDataNotifier dataNotifier;
        private IReadOnlyAuthoringDataService authoringDataService;
        private IAuthoringBakingService bakingService;
        private SettingsService settingsService;

        private StyleSheet baseStyleSheet;

        [MenuItem("Tools/Morpeh/Entity Converter")]
        public static void ShowWindow()
        {
            EntityConverterWindow window = GetWindow<EntityConverterWindow>();
            window.titleContent = new GUIContent("Entity Converter");
        }

        internal void Initialize(
            IEntityConverterDataNotifier dataNotifier,
            IReadOnlyAuthoringDataService authoringDataService,
            IAuthoringBakingService bakingService,
            SettingsService settingsService)
        {
            this.dataNotifier = dataNotifier;
            this.settingsService = settingsService;
            this.authoringDataService = authoringDataService;
            this.bakingService = bakingService;
            dataNotifier.DataChanged += ImplCreateGUI;
        }

        public void CreateGUI()
        {
            if (dataNotifier == null)
            {
                EditorWindowFactory.InitializeEntityConverterWindow(this);
            }

            ImplCreateGUI();
        }

        private void ImplCreateGUI()
        {
            rootVisualElement.Clear();

            if (dataNotifier.IsValid() == false)
            {
                var creationButton = CreateConverterDataAssetCreationButton();
                rootVisualElement.Add(creationButton);
                return;
            }

            LoadStyleSheet();

            var scenesRoot = CreateScenesGUI();
            var optionsRoot = CreateOptionsGUI();
            var bakeButtonsRoot = CreateBakeButtonsGUI();

            rootVisualElement.Add(scenesRoot);
            rootVisualElement.Add(optionsRoot);
            rootVisualElement.Add(bakeButtonsRoot);
        }

        private VisualElement CreateConverterDataAssetCreationButton()
        {
            var creationButton = new Button(() => EntityConverterUtility.CreateDataAssetInstance());
            creationButton.text = "Create EntityConverterAsset";
            return creationButton;
        }

        private void LoadStyleSheet()
        {
            baseStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.scellecs.morpeh.entity-converter/Editor/Styles/EntityConverterWindow.uss");
            rootVisualElement.styleSheets.Add(baseStyleSheet);
        }

        private VisualElement CreateScenesGUI()
        {
            var scenesRoot = new VisualElement();

            var scenesFoldout = new Foldout();
            scenesFoldout.AddToClassList("scenes-foldout");
            scenesFoldout.text = "Scenes";
            scenesRoot.Add(scenesFoldout);

            var scenes = authoringDataService.GetSceneGuids();

            foreach (var sceneGuid in scenes)
            {
                var pair = new VisualElement();
                pair.AddToClassList("scene-scene-data-pair");

                var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
                var sceneField = new ObjectField();
                sceneField.AddToClassList("scene-object-field");
                sceneField.objectType = typeof(SceneAsset);
                sceneField.value = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                sceneField.SetEnabled(false);
                pair.Add(sceneField);

                if (authoringDataService.TryGetSceneBakedDataAsset(sceneGuid, out var sceneBakedData))
                {
                    var bakedDataField = new ObjectField();
                    bakedDataField.AddToClassList("scene-baked-data-field");
                    bakedDataField.objectType = typeof(SceneBakedDataAsset);
                    bakedDataField.value = sceneBakedData;
                    bakedDataField.SetEnabled(false);
                    pair.Add(bakedDataField);
                }
                else
                {
                    var creationButton = new Button(() =>
                    {
                        if (dataNotifier.IsValid())
                        {
                            var asset = EntityConverterUtility.CreateSceneBakedDataAsset(scenePath);
                        }
                    });
                    creationButton.AddToClassList("scene-baked-data-button");
                    creationButton.text = "Create SceneBakedDataAsset";
                    pair.Add(creationButton);
                }

                scenesFoldout.Add(pair);
            }

            return scenesRoot;
        }

        private VisualElement CreateOptionsGUI()
        {
            var optionsRoot = new VisualElement();
            var optionsFoldout = new Foldout();
            optionsFoldout.AddToClassList("options-foldout");
            optionsFoldout.text = "Options";
            optionsRoot.Add(optionsFoldout);

            var bakingOptionsRoot = CreateBakingOptionsGUI();
            var logsOptionsRoot = CreateLogsOptionsGUI();

            optionsFoldout.Add(bakingOptionsRoot);
            optionsFoldout.Add(logsOptionsRoot);

            return optionsRoot;
        }

        private VisualElement CreateBakingOptionsGUI()
        {
            var optionsRoot = new VisualElement();
            optionsRoot.AddToClassList("options-suboptions-container");

            var bakingOptionsHeader = new Label();
            bakingOptionsHeader.AddToClassList("options-suboptions-header");
            bakingOptionsHeader.text = "Baking";

            var rebakeOnDomainReloadToggle = CreateBakingFlagsToggle("Force Global Rebake On Domain Reload", BakingFlags.BakeOnDomainReload);
            var rebakePrefabsToggle = CreateBakingFlagsToggle("Auto Rebake Prefabs", BakingFlags.BakePrefabs);
            var rebakeScenesToggle = CreateBakingFlagsToggle("Auto Rebake Scenes", BakingFlags.BakeScenes);

            optionsRoot.Add(bakingOptionsHeader);
            optionsRoot.Add(rebakeOnDomainReloadToggle);
            optionsRoot.Add(rebakePrefabsToggle);
            optionsRoot.Add(rebakeScenesToggle);

            Toggle CreateBakingFlagsToggle(string toggleText, BakingFlags flag)
            {
                var bakingFlagToggle = new Toggle();
                bakingFlagToggle.text = toggleText;
                bakingFlagToggle.value = settingsService.GetBakingFlagEnabled(flag);
                bakingFlagToggle.RegisterValueChangedCallback(v =>
                {
                    if (dataNotifier.IsValid())
                    {
                        settingsService.SetBakingFlagState(flag, v.newValue);
                    }
                });

                return bakingFlagToggle;
            }

            return optionsRoot;
        }

        private VisualElement CreateLogsOptionsGUI()
        {
            var optionsRoot = new VisualElement();
            optionsRoot.AddToClassList("options-suboptions-container");

            var logsOptionsHeader = new Label();
            logsOptionsHeader.AddToClassList("options-suboptions-header");
            logsOptionsHeader.text = "Logs";

            var internalDebugToggle = CreateLogFlagsToggle("Internal Debug", LogFlags.InternalDebug, true);
            var debugToggle = CreateLogFlagsToggle("Debug", LogFlags.Debug, true);
            var infoToggle = CreateLogFlagsToggle("Info", LogFlags.Info, true);

            optionsRoot.Add(logsOptionsHeader);
            optionsRoot.Add(internalDebugToggle);
            optionsRoot.Add(debugToggle);
            optionsRoot.Add(infoToggle);

            Toggle CreateLogFlagsToggle(string toggleText, LogFlags flag, bool setEnabled)
            {
                var bakingFlagToggle = new Toggle();
                bakingFlagToggle.text = toggleText;
                bakingFlagToggle.value = settingsService.GetLogFlagEnabled(flag);
                bakingFlagToggle.SetEnabled(setEnabled);
                bakingFlagToggle.RegisterValueChangedCallback(v =>
                {
                    if (dataNotifier.IsValid())
                    {
                        settingsService.SetLogFlagState(flag, v.newValue);
                    }
                });

                return bakingFlagToggle;
            }


            return optionsRoot;
        }

        private VisualElement CreateBakeButtonsGUI()
        {
            var root = new VisualElement();

            var bakePrefabButton = new Button(() => bakingService.BakePrefab(Utilities.PrefabUtility.GetActivePrefabGUID()));
            bakePrefabButton.text = "Bake Active Prefab";

            var bakeSceneButton = new Button(() => bakingService.BakeScene(SceneUtility.GetActiveSceneGUID()));
            bakeSceneButton.text = "Bake Active Scene";

            var globalRebakeButton = new Button(() => bakingService.ForceGlobalBake());
            globalRebakeButton.text = "Force Global Rebake";

            root.Add(bakePrefabButton);
            root.Add(bakeSceneButton);
            root.Add(globalRebakeButton);

            return root;
        }

        private void OnDestroy()
        {
            if (dataNotifier != null)
            {
                dataNotifier.DataChanged -= ImplCreateGUI;
            }
        }
    }
}
