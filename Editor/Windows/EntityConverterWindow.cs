using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    public class EntityConverterWindow : EditorWindow
    {
        private EntityConverterDatabase database;
        private StyleSheet baseStyleSheet;

        private VisualElement scenesRoot;

        [MenuItem("Tools/Morpeh/Entity Converter")]
        public static void ShowWindow()
        {
            EntityConverterWindow window = GetWindow<EntityConverterWindow>();
            window.titleContent = new GUIContent("Entity Converter");
        }

        public void CreateGUI()
        {
            if (database == null)
            {
                CreateDatabaseCreationButton();
                return;
            }

            Initialize();

            CreateScenesEditor();
            rootVisualElement.Add(scenesRoot);
        }

        private void ResetGUI()
        {
            rootVisualElement.Clear();
            CreateGUI();
        }

        private void OnEnable()
        {
            if (LoadDatabase())
            {
                database.DatabaseChanged += ResetGUI;
            }
        }

        private void OnDisable()
        {
            if (database != null)
            {
                database.DatabaseChanged -= ResetGUI;
            }
        }

        private void Initialize()
        {
            baseStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.scellecs.morpeh.entity-converter/Editor/Styles/EntityConverterWindow.uss");
            rootVisualElement.styleSheets.Add(baseStyleSheet);
        }

        private void CreateScenesEditor()
        {
            scenesRoot = new VisualElement();

            var scenesFoldout = new Foldout();
            scenesFoldout.AddToClassList("scenes-foldout");
            scenesFoldout.text = "Scenes";

            scenesRoot.Add(scenesFoldout);

            var scenes = database.sceneGuids;

            if (scenes.Count > 0)
            {
                for (int i = 0; i < scenes.Count; i++)
                {
                    var pair = new VisualElement();
                    pair.AddToClassList("scene-scene-data-pair");

                    var scenePath = AssetDatabase.GUIDToAssetPath(scenes[i]);
                    var sceneField = new ObjectField();
                    sceneField.AddToClassList("scene-object-field");
                    sceneField.objectType = typeof(SceneAsset);
                    sceneField.value = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                    sceneField.SetEnabled(false);
                    pair.Add(sceneField);

                    if (database.TryGetSceneBakedDataForScene(scenes[i], out var sceneBakedData))
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
                            if (database != null)
                            {
                                database.CreateSceneBakedDataAsset(scenePath);
                            }
                        });
                        creationButton.AddToClassList("scene-baked-data-field");
                        creationButton.text = "Create SceneBakedDataAsset";
                        pair.Add(creationButton);
                    }

                    scenesFoldout.Add(pair);
                }
            }
        }

        private bool LoadDatabase()
        {
            database = EntityConverterDatabase.GetInstance();
            return database != null;
        }

        private void CreateDatabaseCreationButton()
        {
            var createButton = new Button(() => CreateDatabaseAsset())
            {
                text = "Create Database"
            };
            rootVisualElement.Add(createButton);
        }

        private void CreateDatabaseAsset()
        {
            database = EntityConverterDatabase.CreateInstance();
            database.DatabaseChanged += ResetGUI;
            ResetGUI();
        }
    }
}
