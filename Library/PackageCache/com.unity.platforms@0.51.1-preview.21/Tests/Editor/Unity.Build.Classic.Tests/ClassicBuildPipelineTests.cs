using NUnit.Framework;
using System;
using Unity.Build.Classic.Private;
using Unity.Build.MockPlatform.Classic;
using UnityEngine;

namespace Unity.Build.MockPlatform
{
    [HideInInspector]
    class MockPlatformWrongNamespace : Platform
    {
        public static MockPlatformWrongNamespace Instance = new MockPlatformWrongNamespace();
        public MockPlatformWrongNamespace() : base(new PlatformInfo("mockwrong", "Mock Platform Wrong Namespace", "com.unity.platforms", null)) { }
    }

    class TestPlatformClassicNonIncrementalPipelineWrongNamespace : ClassicNonIncrementalPipelineBase
    {
        protected override RunResult OnRun(RunContext context) => throw new NotImplementedException();
        public override Platform Platform { get; } = Classic.MockPlatform.Instance;
    }
}

namespace Unity.Build.MockPlatform.Classic
{
    [HideInInspector]
    class MockPlatform : Platform
    {
        public static MockPlatform Instance = new MockPlatform();
        public MockPlatform() : base(new PlatformInfo("mock", "Mock Platform", "com.unity.platforms", null)) { }
    }

    class TestPlatformClassicNonIncrementalPipeline : ClassicNonIncrementalPipelineBase
    {
        protected override RunResult OnRun(RunContext context) => throw new NotImplementedException();
        public override Platform Platform { get; } = MockPlatform.Instance;
    }
}

namespace Unity.Build.Classic.Tests
{
    /// <summary>
    /// BuildPipelineSelector should only pick pipelines from namespace Unity.Build.*Platform*.Classic
    /// If pipeline class namespace contains "Test" word, ignore it
    /// </summary>
    [TestFixture]
    public class ClassicBuildPipelineTests
    {
        [Test]
        public void BuildPipelineSelectorTests()
        {
            var selector = new BuildPipelineSelector();
            Assert.That(selector.SelectFor(MockPlatform.MockPlatformWrongNamespace.Instance).GetType(), Is.EqualTo(typeof(MissingClassicNonIncrementalPipeline)));
            Assert.That(selector.SelectFor(MockPlatform.Classic.MockPlatform.Instance).GetType(), Is.EqualTo(typeof(TestPlatformClassicNonIncrementalPipeline)));
        }
    }
}
