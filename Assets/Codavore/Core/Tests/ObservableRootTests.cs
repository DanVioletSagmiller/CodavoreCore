using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Codavore.Core;
using System.Linq;
using System;

namespace Tests
{   
    public class ObservableRootTests
    {
        [Test]
        public void GetNode_WhenCalledForNewGuid_CreatesNewNode()
        {
            // Arrange
            var root = new ObservableRoot();
            var guid = Guid.NewGuid();

            // Act
            var node = root.GetNode(guid);

            // Assert
            Assert.IsNotNull(node, "Requested a node that did not exist, and it should have been created. Instead we were given a null object.");
        }

        [Test]
        public void GetNode_WhenGivenAnExistingNode_ReturnsSameNode()
        {
            // Arrange
            var root = new ObservableRoot();
            var guid = Guid.NewGuid();
            var expected = root.GetNode(guid);

            // Act
            var actual = root.GetNode(guid);

            // Assert
            Assert.AreSame(
                expected,
                actual,
                "The same name was used for two GetNode requests, but two different objects were returned.");
        }

        [Test]
        public void GetRootPaths_WithNoNodes_ReturnsNothing()
        {
            // Arrange
            var root = new ObservableRoot();

            // Act
            var nodes = root.GetRootPaths();

            // Assert
            Assert.AreEqual(
                0,
                nodes.Count(),
                "There should have been 0 items returned, since it was not set, but "
                + nodes.Count()
                + " were returned.");
        }

        [Test]
        public void GetRootPaths_WhenRootHasOneNode_ReturnsThatPathRoot()
        {
            // Arrange
            var root = new ObservableRoot();
            var guid = Guid.NewGuid();
            var path = "rootp1/childc2";
            var node = root.GetNode(guid);
            node.Reset("", path, node);

            // Act
            var actuals = root.GetRootPaths();

            // Assert
            Assert.AreEqual(
                1,
                actuals.Count(),
                "There should be only one node, but this returned " + actuals.Count());

            Assert.AreEqual(
                "rootp1",
                actuals[0],
                "The first and only node returned should have been the one we added, but it was not.");
        }

        [Test]
        public void GetRootPaths_WithDifferentPathsWithSameRoot_ReturnsOneRoot()
        {
            // Arrange
            var root = new ObservableRoot();
            var guid = Guid.NewGuid();
            var path = "rootp1/childc2";
            var node = root.GetNode(guid);
            node.Reset("", path, null);

            guid = Guid.NewGuid();
            path = "rootp1/childc3";
            node = root.GetNode(guid);
            node.Reset("", path, null);

            // Act
            var actuals = root.GetRootPaths();

            // Assert
            Assert.AreEqual(
                1,
                actuals.Count(),
                "There should be only one root, but this returned " + actuals.Count());

            Assert.AreEqual(
                "rootp1",
                actuals[0],
                "The first and only node returned should have been the one we added, but it was not.");
        }

        [Test]
        public void GetRootPaths_WithTwoRootPaths_ReturnsTwoRoots()
        {
            // Arrange
            var root = new ObservableRoot();
            var guid = Guid.NewGuid();
            var path = "rootp1/childc1";
            var node = root.GetNode(guid);
            node.Reset("", path, null);

            guid = Guid.NewGuid();
            path = "rootp2/childc2";
            node = root.GetNode(guid);
            node.Reset("", path, null);

            // Act
            var actuals = root.GetRootPaths();

            // Assert
            Assert.AreEqual(
                2,
                actuals.Count(),
                "There should be only one root, but this returned " + actuals.Count());

            Assert.AreEqual(
                "rootp1",
                actuals[0],
                "The first root returned should have been the one we added, but it was not.");
        }

        [Serializable]
        public class C1
        {
            public string value;
        }

        [Test]
        public void SaveRootNode_WithNodesWithIntValue_ReturnsAJsonForANodeOfTheSameNameWithTheSameIntValue()
        {
            // Arrange
            var root = new ObservableRoot();
            var guid = Guid.NewGuid();
            var node = root.GetNode(guid);
            node.Reset("int", "level1Data", (int)5);

            // Act
            var content = root.SaveLineage("level1Data");

            // Assert
            var newRoot = new ObservableRoot();
            newRoot.LoadLineage(content);
            Assert.AreEqual(
                5,
                newRoot.GetNode(guid).GetValue<int>(),
                "Was expecting node:Asdf to return int of 5.");
        }
    }
}