using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    public class EntityConverterWindow : EditorWindow
    {
        private IReadOnlyEntityConverterRepository repository;
        private EntityBakingService entityBakingService;

        private StyleSheet baseStyleSheet;

        private VisualElement scenesRoot;
        private VisualElement optionsRoot;

        [MenuItem("Tools/Morpeh/Entity Converter")]
        public static void ShowWindow()
        {
            EntityConverterWindow window = GetWindow<EntityConverterWindow>();
            window.titleContent = new GUIContent("Entity Converter");
        }

        public void CreateGUI()
        {
            rootVisualElement.Clear();

            if (repository.IsValid == false)
            {
                CreateConverterDataAssetCreationButton();
                return;
            }

            LoadStyleSheet();
            CreateScenesGUI();
            CreateOptionsGUI();

            rootVisualElement.Add(scenesRoot);
            rootVisualElement.Add(optionsRoot);

            var button = new Button(() => entityBakingService.BakeScene(AssetDatabase.AssetPathToGUID(EditorSceneManager.GetActiveScene().path)));
            button.text = "Bake Active Scene";
            rootVisualElement.Add(button);
        }

        private void OnEnable() => Initialize();

        private void OnDisable() => repository.RepositoryDataChanged -= CreateGUI;

        private void Initialize()
        {
            repository = EntityConverterServiceProvider.Instance.Repository;
            entityBakingService = EntityConverterServiceProvider.Instance.EntityBakingService;
            repository.RepositoryDataChanged += CreateGUI;
        }

        private void LoadStyleSheet()
        {                
            baseStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.scellecs.morpeh.entity-converter/Editor/Styles/EntityConverterWindow.uss");
            rootVisualElement.styleSheets.Add(baseStyleSheet);
        }

        private void CreateScenesGUI()
        {
            scenesRoot = new VisualElement();

            var scenesFoldout = new Foldout();
            scenesFoldout.AddToClassList("scenes-foldout");
            scenesFoldout.text = "Scenes";
            scenesRoot.Add(scenesFoldout);

            var scenes = repository.GetSceneGuids();
            while (scenes.MoveNext())
            {
                var sceneGuid = scenes.Current;
                var pair = new VisualElement();
                pair.AddToClassList("scene-scene-data-pair");

                var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
                var sceneField = new ObjectField();
                sceneField.AddToClassList("scene-object-field");
                sceneField.objectType = typeof(SceneAsset);
                sceneField.value = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                sceneField.SetEnabled(false);
                pair.Add(sceneField);

                if (repository.TryGetSceneBakedDataAsset(sceneGuid, out var sceneBakedData))
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
                        if (repository.IsValid)
                        {
                            var asset = EntityConverterUtility.CreateSceneBakedDataAsset(scenePath);
                        }
                    });
                    creationButton.AddToClassList("scene-baked-data-field");
                    creationButton.text = "Create Scene Baked Data Asset";
                    pair.Add(creationButton);
                }

                scenesFoldout.Add(pair);
            }
        }

        private void CreateOptionsGUI()
        {
            //optionsRoot = new VisualElement();

            //var optionsFoldout = new Foldout();
            //optionsFoldout.AddToClassList("options-foldout");
            //optionsFoldout.text = "Options";
            //optionsRoot.Add(optionsFoldout);

            //var bakingFlagsProperty = converterDataSerializedObject.FindProperty(nameof(EntityConverter.bakingFlags));

            //var rebakeOnDomainReloadToggle = new Toggle();
            //rebakeOnDomainReloadToggle.text = "Perform Full Rebake On Domain Reload";
            //rebakeOnDomainReloadToggle.value = ((EntityConverterBakingFlags)bakingFlagsProperty.intValue & EntityConverterBakingFlags.BakeOnDomainReload) != 0;
            //rebakeOnDomainReloadToggle.RegisterValueChangedCallback(v =>
            //{
            //    if (converterData != null)
            //    {
            //        bakingFlagsProperty.intValue = SetFlag(bakingFlagsProperty.intValue, EntityConverterBakingFlags.BakeOnDomainReload, v.newValue);
            //        converterDataSerializedObject.ApplyModifiedProperties();
            //    }
            //});
            //optionsFoldout.Add(rebakeOnDomainReloadToggle);

            //var rebakeOnEnterPlaymodeToggle = new Toggle();
            //rebakeOnEnterPlaymodeToggle.text = "Perform Full Rebake On Enter Playmode";
            //rebakeOnEnterPlaymodeToggle.value = ((EntityConverterBakingFlags)bakingFlagsProperty.intValue & EntityConverterBakingFlags.BakeOnEnterPlaymode) != 0;
            //rebakeOnEnterPlaymodeToggle.RegisterValueChangedCallback(v =>
            //{
            //    if (converterData != null)
            //    {
            //        bakingFlagsProperty.intValue = SetFlag(bakingFlagsProperty.intValue, EntityConverterBakingFlags.BakeOnEnterPlaymode, v.newValue);
            //        converterDataSerializedObject.ApplyModifiedProperties();
            //    }
            //});
            //optionsFoldout.Add(rebakeOnEnterPlaymodeToggle);

            //var rebakeOnBuildToggle = new Toggle();
            //rebakeOnBuildToggle.text = "Perform Full Rebake On Build";
            //rebakeOnBuildToggle.value = ((EntityConverterBakingFlags)bakingFlagsProperty.intValue & EntityConverterBakingFlags.BakeOnBuild) != 0;
            //rebakeOnBuildToggle.RegisterValueChangedCallback(v =>
            //{
            //    if (converterData != null)
            //    {
            //        bakingFlagsProperty.intValue = SetFlag(bakingFlagsProperty.intValue, EntityConverterBakingFlags.BakeOnBuild, v.newValue);
            //        converterDataSerializedObject.ApplyModifiedProperties();
            //    }
            //});
            //optionsFoldout.Add(rebakeOnBuildToggle);
        }

        private void CreateConverterDataAssetCreationButton()
        {
            var createButton = new Button(() => EntityConverterUtility.CreateDataAssetInstance());
            createButton.text = "Create Entity Converter Asset";
            rootVisualElement.Add(createButton);
        }

        private static int SetFlag(int intFlags, EntityConverterBakingFlags flag, bool value)
        {
            var flags = (EntityConverterBakingFlags)intFlags;
            return (int)(value ? flags | flag : flags & ~flag);
        }
    }
}
