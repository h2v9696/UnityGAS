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
    public class TagTreeViewItem : TreeViewItem
    {
        public TagSO Tag { get; set; }
        public bool IsCommon { get; set; }
    }

    public class TagsTreeView : TreeView
    {
        private List<TagSO> _tags;
        private static CommonTags _commonTags;
        public static List<TagSO> CommonTags => _commonTags.Tags;


        public TagsTreeView(TreeViewState treeViewState, List<TagSO> tags)
            : base(treeViewState)
        {
            _commonTags = AssetFinder.FindAssetsWithType<CommonTags>().FirstOrDefault();
            _tags = tags;
            if (_tags.Count <= 0 || _commonTags == null) return;
            
            showAlternatingRowBackgrounds = true;
            useScrollView = true;
            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            var tagInfos = GenerateTagItems();
            
            var root = new TreeViewItem {id = 0, depth = -1, displayName = "Root"};
            
            // Sort by depth
            tagInfos.Sort((x, y) => x.Item2.CompareTo(y.Item2));

            var allItems = new List<TreeViewItem>();

            foreach (var tagInfo in tagInfos)
            {
                var tag = tagInfo.Item1;
                var depth = tagInfo.Item2;
                if (tag == null) continue;
                var item = CreateTagTreeItem(tag);
                allItems.Add(item);

                if (depth == 0)
                {
                    root.AddChild(item);
                    continue;
                }

                allItems.First(c => c.id == tag.Parent.GetInstanceID())?.AddChild(item);
            }
                
            // Utility method that initializes the TreeViewItem.children and .parent for all items.
            SetupDepthsFromParentsAndChildren(root);
            // Return root of the tree
            return root;
        }

        // Return list of tag and its depth
        private List<(TagSO, int)> GenerateTagItems()
        {
            var tagItems = new List<(TagSO, int)>();
            foreach (var tag in _tags)
            {
                var tagItem = (tag, 0);
                var parent = tag.Parent;
                while (parent != null)
                {
                    tagItem.Item2++;
                    parent = parent.Parent;
                }
                tagItems.Add(tagItem);
            }
            return tagItems;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = args.item as TagTreeViewItem;
            var rect = args.rowRect;
            rect.xMin += GetContentIndent(item);
            EditorGUI.LabelField(rect, args.item.displayName);
            
            CommonToggle(rect, item);
            AddNewTagButton(rect, item);
        }

        private void CommonToggle(Rect rect, TagTreeViewItem item)
        {
            var toggleRect = new Rect(rect.xMax - 32, rect.y, 16, rect.height);
            item.IsCommon = EditorGUI.Toggle(toggleRect, item.IsCommon);
            if (item.IsCommon) 
            {
                if (CommonTags.Contains(item.Tag)) return;
                CommonTags.Add(item.Tag);
                return;
            }
            if (!CommonTags.Remove(item.Tag)) return;
        }

        private void AddNewTagButton(Rect rect, TagTreeViewItem item)
        {
            var buttonRect = new Rect(rect.xMax - 16, rect.y, 16, rect.height);
            if (GUI.Button(buttonRect, "+"))
            {
                var newTag = ScriptableObject.CreateInstance<TagSO>();
                var newItem = CreateTagTreeItem(newTag);
                newTag.name = $"New Tag {newItem.id}";
                item.AddChild(newItem);
                SetExpanded(item.id, true);
                SetSelection(new List<int>() {newItem.id});

                var itemPath = AssetDatabase.GetAssetPath(item.id);
                var parentPath = Path.GetDirectoryName(itemPath);
                var newPath = Path.Combine(parentPath, newTag.name + ".asset");
                newTag.SetPrivateProperty("_parent", item.Tag);
                AssetDatabase.CreateAsset(newTag, newPath);
                AssetDatabase.SaveAssets();
            }
        }

        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);
            var item = FindItem(id, rootItem) as TagTreeViewItem;
            Selection.activeObject = item.Tag;
        }

        // rename on double click
        protected override bool CanRename(TreeViewItem item) => true;
        protected override bool CanStartDrag(CanStartDragArgs args) => true;
        protected override bool CanBeParent(TreeViewItem item) => true;

        private const string _dragId = "ParentingDragId";
        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            if (hasSearch)
                return;
 
            DragAndDrop.PrepareStartDrag();
            var draggedRows = GetRows().Where(item => args.draggedItemIDs.Contains(item.id)).ToList();
            DragAndDrop.SetGenericData(_dragId, draggedRows);
            // DragAndDrop.objectReferences = new UnityEngine.Object[] { }; // this IS required for dragging to work
            string title = draggedRows.Count == 1 ? draggedRows[0].displayName : "< Multiple >";
            DragAndDrop.StartDrag(title);
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            // Check if we can handle the current drag data (could be dragged in from other areas/windows
            if (!DragAndDrop.GetGenericData(_dragId).GetType().IsAssignableFrom(typeof(List<TreeViewItem>)))
                return DragAndDropVisualMode.None;

            if (!args.performDrop) return DragAndDropVisualMode.Move;
            // get the dragged rows
            var draggedRows = DragAndDrop.GetGenericData(_dragId) as List<TreeViewItem>;

            if (args.parentItem == null)
            {
                SetTagParent(draggedRows, null);
                return DragAndDropVisualMode.None;
            }
            // get the parent item
            var parentItem = FindItem(args.parentItem.id, rootItem) as TagTreeViewItem;
            if (parentItem.Tag == null)
                return DragAndDropVisualMode.None;

            SetTagParent(draggedRows, parentItem.Tag);
            
            return DragAndDropVisualMode.Move;
        }

        private void SetTagParent(List<TreeViewItem> draggedRows, TagSO parent)
        {
            bool assetChanged = false;
            foreach (var row in draggedRows)
            {
                var id = row.id;
                var tagSO = ((TagTreeViewItem) row).Tag;
                if (row == null || tagSO == parent) continue;
                tagSO.SetPrivateProperty("_parent", parent);
                assetChanged = true;
            }
            if (!assetChanged) return;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Reload();
        }

        protected override void RenameEnded(RenameEndedArgs args)
        {
            base.RenameEnded(args);
            var assetPath = AssetDatabase.GetAssetPath(args.itemID);
            // rename asset
            AssetDatabase.RenameAsset(assetPath, args.newName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private TagTreeViewItem CreateTagTreeItem(TagSO tag)
        {
            var item = new TagTreeViewItem
            {
                id = tag.GetInstanceID(),
                displayName = tag.name,
                IsCommon = CommonTags.Contains(tag),
                Tag = tag
            };
            return item;
        }
    }
}