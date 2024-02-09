using System.Collections.Generic;
using System.IO;
using System.Linq;
using H2V.ExtensionsCore.Editor.Helpers;
using H2V.GameplayAbilitySystem.TagSystem.ScriptableObjects;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.Editor.TagSystem
{
    public class TagsBrowser : EditorWindow
    {
        private static TagsBrowser _window;
        private static List<TagSO> _allTags;

        private static TreeViewState _treeViewState;
        private static TagsTreeView _tagTreeView;
        private SearchField _searchField;

        private Vector2 _scrollPosition;
        private static string _directoryToAdd = "ScriptableObjects/Tags";

        private CommonTags _commonTags;

        private void OnEnable()
        {
            FetchTags();
        }

        [MenuItem("Window/H2V/Gameplay Ability System/Tags Browser %#T")]
        public static void ShowWindow()
        {
            _window = GetWindow<TagsBrowser>($"Tags Browser");
            _window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            TagsTreeSection();
            EditorGUILayout.Space();
            AddTagSection();
        }

        private void TagsTreeSection()
        {
            _searchField ??= new SearchField();

            var searchRect = EditorGUILayout.GetControlRect(false, GUILayout.ExpandWidth(true),
                GUILayout.Height(EditorGUIUtility.singleLineHeight));
            if (_tagTreeView != null)
            {
                _tagTreeView.searchString = _searchField.OnGUI(searchRect, _tagTreeView.searchString);
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                _tagTreeView.OnGUI(new Rect(0, 0, position.width, position.height - 80));
                EditorGUILayout.EndScrollView();
            }
        }

        private void AddTagSection()
        {
            _directoryToAdd = EditorGUILayout.TextField("Directory to add:", _directoryToAdd);
            var directory = $"Assets/{_directoryToAdd}";
            EditorGUILayout.Space();
            if (_commonTags == null)
            {
                if (GUILayout.Button("Create common tags"))
                    _commonTags = AddNewSO<CommonTags>(directory, "CommonTags");
                return;
            }
            if (GUILayout.Button("Add new tag"))
            {
                var assetInFolder = AssetFinder.FindAssetsWithType<TagSO>("", directory);
                AddNewSO<TagSO>(directory, $"NewTag{assetInFolder.Count()}");
            }
        }

        private T AddNewSO<T>(string directory, string name) where T : ScriptableObject
        {
            var newAsset = ScriptableObject.CreateInstance<T>();
            newAsset.name = name;
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            
            AssetDatabase.CreateAsset(newAsset, $"{directory}/{newAsset.name}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return newAsset;
        }

        private void FetchTags()
        {
            _allTags = AssetDatabase.FindAssets("t:TagSO")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<TagSO>)
                .ToList();
            _commonTags = AssetFinder.FindAssetsWithType<CommonTags>().FirstOrDefault();
            _tagTreeView = null;

            if (_commonTags == null || _allTags.Count <= 0) return;

            _treeViewState ??= new TreeViewState ();
            _tagTreeView = new TagsTreeView(_treeViewState, _allTags);
        }

        private void OnFocus()
        {
            FetchTags();
        }

        private void OnProjectChange()
        {
            FetchTags();
        }
    }
}