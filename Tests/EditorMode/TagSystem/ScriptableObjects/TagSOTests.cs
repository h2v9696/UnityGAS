using NUnit.Framework;
using UnityEngine;
using H2V.ExtensionsCore.Editor.Helpers;
using H2V.GameplayAbilitySystem.TagSystem.ScriptableObjects;

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
            Assert.IsTrue(_childTag.IsChildTag(_tag));
        }

        [Test]
        public void IsChildTag_SameTag_False()
        {
            Assert.IsFalse(_tag.IsChildTag(_tag));
        }

        [Test]
        public void IsGrandChildTag_Depth_1_False()
        {
            Assert.IsFalse(_grandChildTag.IsChildTag(_tag, 1));
        }

        [Test]
        public void IsGrandChildTag_Depth_2_True()
        {
            Assert.IsTrue(_grandChildTag.IsChildTag(_tag, 2));
        }
    }
}
