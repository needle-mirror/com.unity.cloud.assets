#if !UC_EXCLUDE_SAMPLES && UNITY_EDITOR
#if !USE_UIELEMENTS
#error Missing dependency to com.unity.modules.uielements
#else

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetDatabaseUploader.Editor
{
    public class AssetsUploaderEditorUI : VisualElement
    {
        readonly AssetsUploader m_AssetsUploader;

        VisualElement m_CloudKnownAssetsSubSection;

        VisualElement m_UploaderSubSection;

        bool m_Uploading;
        Label m_AssetNameLabel;
        Label m_ProgressionLabel;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serializedObject"></param>
        public AssetsUploaderEditorUI(SerializedObject serializedObject)
        {
            m_AssetsUploader = (AssetsUploader)serializedObject.targetObject;
            m_AssetsUploader.AssetsUpdated += () => DrawKnownAssetSubSection(true);

            DrawUI();
        }

        void DrawUI()
        {
            var localAssetSourcePath = new TextField("Local Assets path");
            localAssetSourcePath.value = m_AssetsUploader.AssetsSourcePath;
            localAssetSourcePath.RegisterValueChangedCallback(evt =>
            {
                m_AssetsUploader.AssetsSourcePath = evt.newValue;
            });
            Add(localAssetSourcePath);

            Add(EditorUIUtils.CreateSpaceBox());

            Add(DrawKnownAssets());

            Add(EditorUIUtils.CreateSpaceBox());

            Add(DrawUploadSection());
        }

        VisualElement DrawKnownAssets()
        {
            var cloudProjectAssets = new Box();
            cloudProjectAssets.Add(new Label("Already known assets from the Cloud project:"));
            cloudProjectAssets.Add(EditorUIUtils.CreateSpaceBox());

            DrawKnownAssetSubSection(false);

            cloudProjectAssets.Add(m_CloudKnownAssetsSubSection);

            return cloudProjectAssets;
        }

        void DrawKnownAssetSubSection(bool clear)
        {
            if(clear && m_CloudKnownAssetsSubSection != null)
                m_CloudKnownAssetsSubSection.Clear();

            m_CloudKnownAssetsSubSection ??= new VisualElement();

            if (m_AssetsUploader.Assets == null || m_AssetsUploader.Assets.Count == 0)
            {
                var knowAssetsLabelText = $"The assets to upload from the path {m_AssetsUploader.AssetsSourcePath}, don't already exist in the selected cloud project.";

                m_CloudKnownAssetsSubSection.Add(new Label(knowAssetsLabelText));
            }
            else
            {
                var assetsListView = new ListView(m_AssetsUploader.Assets, 20, () => new Label(), (element, index) =>
                {
                    var asset = m_AssetsUploader.Assets[index];
                    (element as Label).text = asset.Name;
                });

                m_CloudKnownAssetsSubSection.Add(assetsListView);
            }

            m_CloudKnownAssetsSubSection.Add(EditorUIUtils.CreateSpaceBox());

            var searchAssetsBtn = new Button(SearchAssetsAction)
            {
                text = "Search Assets",
                tooltip = "Search the assets in the cloud to know if they already exist"
            };

            m_CloudKnownAssetsSubSection.Add(searchAssetsBtn);
        }

        async void SearchAssetsAction()
        {
            await m_AssetsUploader.SearchAssetsAsync();

            DrawKnownAssetSubSection(true);
        }

        VisualElement DrawUploadSection()
        {
            var uploaderSection = new Foldout { text = "Upload assets:"};

            var intField = new IntegerField("Upload Timeout (ms)");
            intField.value = m_AssetsUploader.UploadTimeout;
            intField.RegisterValueChangedCallback(evt =>
            {
                m_AssetsUploader.UploadTimeout = evt.newValue;
            });
            uploaderSection.Add(intField);

            var uploadMode = new Toggle("Step by step");
            uploadMode.value = m_AssetsUploader.StepByStep;
            uploadMode.RegisterValueChangedCallback(evt =>
            {
                if (m_AssetsUploader.StepByStep != evt.newValue)
                {
                    m_AssetsUploader.StepByStep = evt.newValue;
                    EditorUtility.SetDirty(m_AssetsUploader);

                    DrawUploadButtons(true);
                }
            });
            uploaderSection.Add(EditorUIUtils.CreateSpaceBox());
            uploaderSection.Add(uploadMode);

            DrawUploadButtons(false);

            uploaderSection.Add(m_UploaderSubSection);

            return uploaderSection;
        }

        void DrawUploadButtons(bool clear)
        {
            if(clear && m_UploaderSubSection != null)
                m_UploaderSubSection.Clear();

            m_UploaderSubSection ??= new VisualElement();

            if (m_AssetsUploader.StepByStep)
            {
                DrawCreateAssetsButton();
                DrawCreateAssetFileButton();
            }
            else
            {
                DrawCreateAndUploadAssetsAllInOneButton();
            }

            DrawProgression();
        }

        void DrawProgression()
        {
            m_AssetNameLabel = new Label
            {
                visible = false
            };
            m_UploaderSubSection.Add(m_AssetNameLabel);

            m_UploaderSubSection.Add(EditorUIUtils.CreateSpaceBox());

            m_ProgressionLabel = new Label
            {
                visible = false
            };
            m_UploaderSubSection.Add(m_ProgressionLabel);
        }

        void DrawCreateAssetsButton()
        {
            m_UploaderSubSection.Add(EditorUIUtils.CreateSpaceBox());

            var btn = new Button(CreateAssetsAction)
            {
                text = "Create Assets",
                tooltip = "Create the assets into the cloud"
            };

            m_UploaderSubSection.Add(btn);
        }

        async void CreateAssetsAction()
        {
            await m_AssetsUploader.CreateAssetsAsync();
        }

        void DrawCreateAssetFileButton()
        {
            m_UploaderSubSection.Add(EditorUIUtils.CreateSpaceBox());

            var btn = new Button(CreateAssetFilesAction)
            {
                text = "Create Asset Files",
                tooltip = "Create the asset files into the cloud"
            };

            m_UploaderSubSection.Add(btn);
        }

        async void CreateAssetFilesAction()
        {
            await m_AssetsUploader.CreateAssetFilesAsync();
        }

        void DrawCreateAndUploadAssetsAllInOneButton()
        {
            m_UploaderSubSection.Add(EditorUIUtils.CreateSpaceBox());

            var btn = new Button(CreateAndUploadAssetsAllInOneAction)
            {
                text = "Create and Upload Assets",
                tooltip = "Create the assets, the asset's files and upload them to the cloud"
            };

            m_UploaderSubSection.Add(btn);
        }

        async void CreateAndUploadAssetsAllInOneAction()
        {
            m_Uploading = true;
            RefreshLabelsVisibility();

            await m_AssetsUploader.CreateAndUploadAssetsAsync();

            m_Uploading = false;
            RefreshLabelsVisibility();
        }

        void RefreshLabelsVisibility()
        {
            m_AssetNameLabel.visible = !m_Uploading;
            m_ProgressionLabel.visible = !m_Uploading;
        }
    }
}
#endif
#endif
