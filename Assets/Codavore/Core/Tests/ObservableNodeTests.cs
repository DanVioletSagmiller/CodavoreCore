using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Codavore.Core;
using System;

namespace Tests
{
    public class ObservableNodeTests
    {
        [Test]
        public void GetValue_GivenTOfValue_ReturnsValue()
        {
            // Arrange
            var node = new ObservableNode(Guid.NewGuid());
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
            var node = new ObservableNode(Guid.NewGuid());
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
            var node = new ObservableNode(Guid.NewGuid());
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
            var node = new ObservableNode(Guid.NewGuid());

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
            var node = new ObservableNode(Guid.NewGuid());
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
            var node = new ObservableNode(Guid.NewGuid());
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
            var node = new ObservableNode(Guid.NewGuid());
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
            var node = new ObservableNode(Guid.NewGuid());
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
            var node = new ObservableNode(Guid.NewGuid());
            var hasListenerTriggered = false;
            System.Action listener = new System.Action(() =>
            {
                hasListenerTriggered = true;
            });

            // Act
            // Assert
            node.StopListening(listener);
        }
    }
}