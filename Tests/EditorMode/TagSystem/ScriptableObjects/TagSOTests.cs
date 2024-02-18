using NUnit.Framework;
using UnityEngine;
using H2V.ExtensionsCore.Editor.Helpers;
using H2V.GameplayAbilitySystem.TagSystem.ScriptableObjects;
using UnityEngine.TestTools;

namespace H2V.GameplayAbilitySystem.Tests.TagSystem.ScriptableObjects
{
    public class TagSOTests
    {
        private TagSO _tag;
        private TagSO _childTag;
        private TagSO _grandChildTag;

        [SetUp]
        public void Setup()
        {
            _tag = ScriptableObject.CreateInstance<TagSO>();
            _childTag = ScriptableObject.CreateInstance<TagSO>();
            _grandChildTag = ScriptableObject.CreateInstance<TagSO>();

            _childTag.SetPrivateProperty("_parent", _tag);
            _grandChildTag.SetPrivateProperty("_parent", _childTag);
        }

        [Test]
        public void IsChildTag_True()
        {
            Assert.IsTrue(_childTag.IsChildOf(_tag));
        }

        [Test]
        public void IsChildTag_SameTag_False()
        {
            Assert.IsFalse(_tag.IsChildOf(_tag));
        }

        [Test]
        public void IsGrandChildTag_Depth_1_False()
        {
            Assert.IsFalse(_grandChildTag.IsChildOf(_tag, 1));
        }

        [Test]
        public void IsGrandChildTag_Depth_2_True()
        {
            Assert.IsTrue(_grandChildTag.IsChildOf(_tag, 2));
        }

        [Test]
        public void SetSelfParent_Valdated_ParentNull()
        {
            LogAssert.ignoreFailingMessages = true;
            _childTag.SetPrivateProperty("_parent", _childTag);
            Assert.IsNull(_childTag.Parent);
        }

        [Test]
        public void SetLoopParent_Valdated_ParentNull()
        {
            LogAssert.ignoreFailingMessages = true;
            _tag.SetPrivateProperty("_parent", _grandChildTag);
            Assert.IsNull(_tag.Parent);
        }

        [Test]
        public void SetMaxDepthParent_Valdated_ParentNull()
        {
            LogAssert.ignoreFailingMessages = true;
            var rootTag = ScriptableObject.CreateInstance<TagSO>();
            for (int i = 0; i < TagSystemConfig.MaxDepth + 1; i++)
            {
                var childTag = ScriptableObject.CreateInstance<TagSO>();
                childTag.SetPrivateProperty("_parent", rootTag);
                rootTag = childTag;
            }
            LogAssert.ignoreFailingMessages = true;
            Assert.IsNull(rootTag.Parent);
        }
    }
}
