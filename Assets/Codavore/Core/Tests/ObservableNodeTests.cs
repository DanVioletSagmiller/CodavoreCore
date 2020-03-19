using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Codavore.Core;

namespace Tests
{
    public class ObservableNodeTests
    {
        [Test]
        public void Constructor_GivenNulls_StillCreatesNode()
        {
            // Arrange
            string name = null;
            IObservableNode parent = null;

            // Act
            // Assert
            new ObservableNode(name, parent);
        }

        [Test]
        public void GetParent_ReturnsParentSetByConstructor()
        {
            // Arrange
            var expected = new ObservableNode(null, null);

            // Act
            var current = new ObservableNode(null, expected);

            // Assert
            Assert.AreSame(
                expected,
                current.GetParent(),
                "GetParent did not return the same parent that was set via the constructor.");
        }

        [Test]
        public void GetName_ReturnsTheNameSetByConstructor()
        {
            // Arrange
            var expected = "NameOfNode";

            // Act
            var current = new ObservableNode(expected, null);

            // Assert
            Assert.AreSame(
                expected,
                current.GetName(),
                "GetName did not return the same name that was set via the constructor. It was expecting '"
                + expected + "', but received '" + current.GetName() + "'");
        }

        [Test]
        public void GetName_WithCunstructorSetNull_ReturnsNull()
        {
            // Arrange
            string expected = null;

            // Act
            var current = new ObservableNode(expected, null);

            // Assert
            Assert.AreEqual(
                expected,
                current.GetName(),
                "GetName did not return null, even though the constructor set it so.");
        }

        [Test]
        public void GetValue_GivenTOfValue_ReturnsValue()
        {
            // Arrange
            var node = new ObservableNode(null, null);
            int expected = 5;
            node.SetValue(expected);

            // Act
            var actual = node.GetValue<int>();

            // Assert
            Assert.AreEqual(
                expected,
                actual,
                "GetValue<int> did not return value, even though the value was set to an int of " + expected);
        }

        [Test]
        public void GetValue_GivenTNotOfValue_ThrowsInvalidCastException()
        {
            // Arrange
            var node = new ObservableNode(null, null);
            node.SetValue((int)5);

            // Act
            Assert.Throws<System.InvalidCastException>(
                () => { node.GetValue<bool>(); },
            // Assert
                "Value was set with an Integer, yet did not error when trying to extract a boolean.");

        }

        [Test]
        public void SetValue_GivenNewType_WillNotError()
        {
            // Arrange
            var node = new ObservableNode(null, null);
            node.SetValue((int)5);

            // Act
            node.SetValue(new Dictionary<string, bool>());

            // Assert
            // Act will throw an exception itself if there is an error.
        }

        [Test]
        public void SetValue_WhetherNullDataOrReferenceType_WillNotError()
        {
            // Arrange
            var node = new ObservableNode(null, null);

            // Act
            node.SetValue((int)5);
            node.SetValue(new Dictionary<string, bool>());
            node.SetValue(null);

            // Assert
            // Act will throw an exception itself if there is an error.
        }

        [Test]
        public void StartListening_WhenValueChanges_WillTriggerListener()
        {
            // Arrange
            var node = new ObservableNode(null, null);
            var hasListenerTriggered = false;
            System.Action listener = new System.Action(() =>
            {
                hasListenerTriggered = true;
            });

            // Act
            node.StartListening(listener);
            node.SetValue((int)5);

            // Assert
            Assert.IsTrue(
                hasListenerTriggered,
                "The trigger should have fired when we changed the data value, but it has not.");
        }

        [Test]
        public void StartListening_WhenSetWithSameValue_WillNotTriggerListener()
        {
            // Arrange
            var node = new ObservableNode(null, null);
            var hasListenerTriggered = false;
            System.Action listener = new System.Action(() =>
            {
                hasListenerTriggered = true;
            });

            // Act
            node.SetValue((int)5);
            node.StartListening(listener);
            node.SetValue((int)5);

            // Assert
            Assert.IsFalse(
                hasListenerTriggered,
                "The trigger should not have fired because the new value was the same");
        }

        [Test]
        public void StartListening_WhenTypeChanges_WillTriggerListener()
        {
            // Arrange
            var node = new ObservableNode(null, null);
            var hasListenerTriggered = false;
            System.Action listener = new System.Action(() =>
            {
                hasListenerTriggered = true;
            });

            // Act
            node.SetValue((int)5);
            node.StartListening(listener);
            node.SetValue((byte)5);

            // Assert
            Assert.IsTrue(
                hasListenerTriggered,
                "The trigger should have fired because the data type changed.");
        }

        [Test]
        public void StopListening_WhenListenerExisted_WillStopListenerBeingCalledWhenValueChanges()
        {
            // Arrange
            var node = new ObservableNode(null, null);
            var hasListenerTriggered = false;
            System.Action listener = new System.Action(() =>
            {
                hasListenerTriggered = true;
            });

            // Act
            node.SetValue((int)0);
            node.StartListening(listener);
            node.StopListening(listener);
            node.SetValue((int)1);

            // Assert
            Assert.IsFalse(
                hasListenerTriggered,
                "The trigger should not have fired when the data type changed because StopListener was called.");
        }

        [Test]
        public void StopListening_WhenListenerDidNotExist_WillNotError()
        {
            // Arrange
            var node = new ObservableNode(null, null);
            var hasListenerTriggered = false;
            System.Action listener = new System.Action(() =>
            {
                hasListenerTriggered = true;
            });

            // Act
            // Assert
            node.StopListening(listener);
        }

        [Test]
        public void GetChild_WhenNameNotUsedAsChildYet_WillCreateANewChildWithoutError()
        {
            // Arrange
            var node = new ObservableNode(null, null);

            // Act
            var child = node.GetChild("AnyName");

            // Assert
            Assert.IsNotNull(
                child,
                "It returned a null child. That should never happen.");
        }

        [Test]
        public void GetChild_WhenTheNameAlreadyExists_ShouldReturnTheSameChild()
        {
            // Arrange
            var node = new ObservableNode(null, null);
            var name = "#1";
            var expected = node.GetChild(name);

            // Act
            var actual = node.GetChild(name);

            // Assert
            Assert.AreSame(
                expected,
                actual,
                "GetChild returned two different objects when given the same name.");
        }

        [Test]
        public void GetChild_GivenName_ChildNameWillBeName()
        {
            // Arrange
            var node = new ObservableNode(null, null);
            var expected = "#1";


            // Act
            var child = node.GetChild(expected);
            var actual = child.GetName();

            // Assert
            Assert.AreEqual(
                expected,
                actual,
                "GetChild returned a child with the name '"
                + actual
                + "' when it was given the name '"
                + expected
                + "'");
        }

        [Test]
        public void GetChild_WillHaveParentSetToGetChildCallingNode()
        {
            // Arrange
            var expected = new ObservableNode(null, null);

            // Act
            var child = expected.GetChild("anyName");
            var actual = child.GetParent();

            // Assert
            Assert.AreSame(
                expected,
                actual,
                "GetChild should have returned an object with the parent set to the node calling GetChild.");
        }

        [Test]
        public void GetChildFromPath_GivenNullPath_WillThrowArgumentNullException()
        {
            // Arrange
            var expected = new ObservableNode(null, null);

            // Act
            // Assert
            Assert.Throws<System.ArgumentNullException>(() =>
            {
                expected.GetChildFromPath(null);
            },
            "This should have thrown a ArgumentNullException, but did not.");
        }

        [Test]
        public void GetChildFromPath_GivenEmptyPath_ReturnsNodeWithEmptyName()
        {
            // Arrange
            var expected = new ObservableNode(null, null);

            // Act
            var child = expected.GetChildFromPath(string.Empty);

            // Assert
            Assert.IsEmpty(
                child.GetName(),
                "This should have returned a child with '' for its name, but it had the name '"
                + child.GetName()
                + "'");
        }

        [Test]
        public void GetChildFromPath_GivenPathWithOutSeparator_ReturnsTheSameAsGetChild()
        {
            // Arrange
            var node = new ObservableNode(null, null);
            var path = "asdf";
            var expected = node.GetChild(path);

            // Act
            var actual = node.GetChildFromPath(path);

            // Assert
            Assert.AreSame(
                expected,
                actual,
                "Since the path passed to GetChildFromPath contained no separaters, "
                + "it should have returned a child object with the same name, but it did not. :(");
        }

        [Test]
        public void GetChildFromPath_GivenPathWithSeparator_ReturnsTheSameAsGetChildGetChild()
        {
            // Arrange
            var node = new ObservableNode(null, null);
            var name1 = "asdf";
            var name2 = "1234";
            var expected = node.GetChild(name1).GetChild(name2);
            var path = name1 + "/" + name2;

            // Act
            var actual = node.GetChildFromPath(path);

            // Assert
            Assert.AreSame(
                expected,
                actual,
                "Calling GetChild(\"asdf\").GetChild(\"1234\") should have returned the same as GetChildFromPath("
                + "\"asdf/1234\"), but it did not.");
        }
    }
}