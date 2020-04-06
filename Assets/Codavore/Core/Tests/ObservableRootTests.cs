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
        public void GetNode_WhenCalledForNewName_CreatesNewNode()
        {
            // Arrange
            var root = new ObservableRoot();

            // Act
            var node = root.GetNode("NewName");

            // Assert
            Assert.IsNotNull(node, "Requested a node that did not exist, and it should have been created. Instead we were given a null object.");
        }

        [Test]
        public void GetNode_WhenGivenAnExistingNode_ReturnsSameNode()
        {
            // Arrange
            var root = new ObservableRoot();
            var name = "AskedTwice";
            var expected = root.GetNode(name);

            // Act
            var actual = root.GetNode(name);

            // Assert
            Assert.AreSame(
                expected,
                actual,
                "The same name was used for two GetNode requests, but two different objects were returned.");
        }

        [Test]
        public void GetNode_WithPath_ReturnsNodeWithParent()
        {
            // Arrange
            var root = new ObservableRoot();
            var name = "Parent/Child";

            // Act
            var child = root.GetNode(name);

            // Assert
            Assert.IsNotNull(
                child.GetParent(),
                "A path was given to GetNode, but the parent was not set");
        }

        [Test]
        public void GetNode_WithPath_ReturnsNodeWithParentWithChildThatMatchesNode()
        {
            // Arrange
            var root = new ObservableRoot();
            var name = "Parent/Child";
            var child = root.GetNode(name);
            var parent = child.GetParent();

            // Act
            var repeatChildCall = parent.GetChild("Child");

            // Assert
            Assert.AreSame(
                child,
                repeatChildCall,
                "Asking a parent node for the same child returned a null.");
        }

        [Test]
        public void GetNode_WithNameOnly_ReturnsNodeWithNullParent()
        {
            // Arrange
            var root = new ObservableRoot();

            // Act
            var node = root.GetNode("NewName");

            // Assert
            Assert.IsNull(node.GetParent(), "Requested a node that did not exist, and it should have been created. Instead we were given a null object.");
        }

        [Test]
        public void GetRootNodes_WhenRootHasNoNodes_ReturnsNothing()
        {
            // Arrange
            var root = new ObservableRoot();

            // Act
            var nodes = root.GetRootNodes().ToArray();

            // Assert
            Assert.AreEqual(
                0,
                nodes.Count(), 
                "There should have been 0 items returned, since it was not set, but "
                + nodes.Count() 
                + " were returned.");
        }

        [Test]
        public void GetRootNodes_WhenRootHasOneNode_ReturnsThatNode()
        {
            // Arrange
            var root = new ObservableRoot();
            var expected = root.GetNode("asdf");

            // Act
            var actuals = root.GetRootNodes().ToArray();

            // Assert
            Assert.AreEqual(
                1,
                actuals.Count(),
                "There should be only one node, but this returned " + actuals.Count());

            Assert.AreSame(
                expected,
                actuals[0],
                "The first and only node returned should have been the one we added, but it was not.");
        }

        [Test]
        public void GetRootNodes_WithTwoNodes_ReturnsBothNodes()
        {
            // Arrange
            var root = new ObservableRoot();
            var expected = root.GetNode("asdf");
            var expected2 = root.GetNode("asdf2");

            // Act
            var actuals = root.GetRootNodes().ToArray();

            // Assert
            Assert.AreEqual(
                2,
                actuals.Count(),
                "There should be 2 nodes, but this returned " + actuals.Count());
        }

        [Test]
        public void GetRootNodes_WithNodeWithChild_ReturnsOneNode()
        {
            // Arrange
            var root = new ObservableRoot();
            var expected = root.GetNode("asdf/asdf");

            // Act
            var actuals = root.GetRootNodes().ToArray();

            // Assert
            Assert.AreEqual(
                1,
                actuals.Count(),
                "There should be only one node, but this returned " + actuals.Count());
        }

        [Test]
        public void StartListeningToLoadRequest_TriggersMethod_WhenAttemptingToGetANewRootNode()
        {
            // Arrange
            var root = new ObservableRoot();
            var hasTriggered = false;
            root.StartListeningToLoadRequest((s) => { hasTriggered = true; });

            // Act
            root.GetNode("newNode");

            // Assert
            Assert.IsTrue(
                hasTriggered,
                "After requesting to get a new node, the listener assigned was not notified.");
        }

        [Test]
        public void StartListeningToLoadRequest_IncludesNameRequestedToLoad_WhenAttemptingToGetANewRootNode()
        {
            // Arrange
            var root = new ObservableRoot();
            var nodeName = "Correct Name";
            var hasCorrectName = false;
            root.StartListeningToLoadRequest((s) => { hasCorrectName = nodeName == s; });

            // Act
            root.GetNode(nodeName);

            // Assert
            Assert.IsTrue(
                hasCorrectName,
                "After requesting to get a new node, the listener assigned was not given the same name as requested.");
        }

        [Test]
        public void StopListeningToLoadRequest_DoesNotTriggerMethod_WhenAttemptingToGetANewRootNode()
        {
            // Arrange
            var root = new ObservableRoot();
            var hasTriggered = false;
            var listener = new System.Action<string>((s) => { hasTriggered = true; });
            root.StartListeningToLoadRequest(listener);
            root.StopListeningToLoadRequest(listener);

            // Act
            root.GetNode("newNode");

            // Assert
            Assert.IsFalse(
                hasTriggered,
                "When requesting to get a new node, the listener should not have been notified.");
        }

        [Test]
        public void RootNodeExists_WithoutMatchingRootNode_ReturnsFalse()
        {
            // Arrange
            var root = new ObservableRoot();

            // Act
            var exists = root.RootNodeExists("no matching result");

            // Assert
            Assert.IsFalse(
                exists,
                "It was told the node exists.");
        }

        [Test]
        public void RootNodeExists_WithMatchingRootNode_ReturnsTrue()
        {
            // Arrange
            var root = new ObservableRoot();
            var nodeName = "match";
            root.GetNode(nodeName);

            // Act
            var exists = root.RootNodeExists(nodeName);

            // Assert
            Assert.IsTrue(
                exists,
                "The root node exists, but when asked, said it did not.");
        }

        [Test]
        public void DeleteRootNode_WhenNodeExists_DeletesIt()
        {
            // Arrange
            var root = new ObservableRoot();
            var name = "delete me";
            root.GetNode(name);

            // Act
            root.DeleteRootNode(name);

            // Assert
            Assert.IsFalse(
                root.RootNodeExists(name),
                "The given root node was not deleted on request.");
        }

        [Test]
        public void DeleteRootNode_WhenNodeDoesNotExist_DoesNotError()
        {
            // Arrange
            var root = new ObservableRoot();


            // Act
            root.DeleteRootNode("Non-existant node");

            // Assert
        }
        [Serializable]
        public class C1
        {
            public string value;
        }

        [Test]
        public void SaveRootNode_WithNodesWithNoValues_ReturnsAJsonForANodeOfTheSameNameWithNoValueOrChildren()
        {
            // Arrange
            var root = new ObservableRoot();
            var asdf = root.GetNode("Asdf");
            //asdf.GetChild("int");//.SetValue(5);
            //var c = new C1() { value = "asdf1 4 C1" };
            //var c = new string[] { "asdf1", "asdf2" };
            //var c = Vector3.one;
            //var c = (int)5;
            //asdf.GetChild("obj");//.SetValue(c);

            // Act
            var content = root.SaveRootNode("Asdf");
            Debug.Log(content);

            // Assert
            var newRoot = new ObservableRoot();
            newRoot.LoadRootNode(content);
        }

        [Test]
        public void SaveRootNode_WithNodesWithIntValue_ReturnsAJsonForANodeOfTheSameNameWithTheSameIntValue()
        {
            // Arrange
            var root = new ObservableRoot();
            var asdf = root.GetNode("Asdf");
            asdf.GetChild("int").SetValue(5);
            //var c = new C1() { value = "asdf1 4 C1" };
            //var c = new string[] { "asdf1", "asdf2" };
            //var c = Vector3.one;
            //var c = (int)5;
            //asdf.GetChild("obj");//.SetValue(c);

            // Act
            var content = root.SaveRootNode("Asdf");
            Debug.Log(content);

            // Assert
            var newRoot = new ObservableRoot();
            newRoot.LoadRootNode(content);
            Assert.AreEqual(
                5, 
                newRoot.GetNode("Asdf").GetValue<int>(),
                "Was expecting node:Asdf to return int of 5.");
        }

        [Test]
        public void SaveRootNode_WithNodes_ValidJson()        
        {
            // Arrange
            var root = new ObservableRoot();

            // Act
            var content = root.SaveRootNode("Asdf");

            // Assert
            var newRoot = new ObservableRoot();
            newRoot.LoadRootNode(content);
        }

        [Test]
        public void LoadRootNode_WithEmptyJson_AffectsNothing()
        {
            // Arrange
            var root = new ObservableRoot();


            // Act


            // Assert
            throw new System.NotImplementedException();
        }

        [Test]
        public void LoadRootNode_WithValidJson_LoadsRootNode()
        {
            // Arrange
            var root = new ObservableRoot();


            // Act


            // Assert
            throw new System.NotImplementedException();
        }

        [Test]
        public void LoadRootNode_WithJsonForRootWithChild_LoadsRootNodeWithChild()
        {
            // Arrange
            var root = new ObservableRoot();


            // Act


            // Assert
            throw new System.NotImplementedException();
        }

        [Test]
        public void LoadRootNode_WithRootNodeWithDataValue_ReturnsRootNodeWithCorrectValue()
        {
            // Arrange
            var root = new ObservableRoot();


            // Act


            // Assert
            throw new System.NotImplementedException();
        }

        [Test]
        public void LoadRootNode_WithRootNodeWithReferenceValue_ReturnsRootNodeWithCorrectValue()
        {
            // Arrange
            var root = new ObservableRoot();


            // Act


            // Assert
            throw new System.NotImplementedException();
        }

        [Test]
        public void LoadRootNode_WhenRootNodeExists_DoesNotChangeExistingData()
        {
            // Arrange
            var root = new ObservableRoot();


            // Act


            // Assert
            throw new System.NotImplementedException();
        }
    }
}