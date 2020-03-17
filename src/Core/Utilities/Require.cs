// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core.Utilities
{
    using System;

    /// <summary>
    /// A utility to validate arguments, state and others that uses the fluent pattern.
    /// </summary>
    public class Require
    {
        /// <summary>
        /// Returns an instance of the Require utility class.
        /// </summary>
        public static readonly Require Instance = new Require();

        private Require()
        {
        }

        /// <summary>
        /// Validates the given argument is not null, otherwise throws an ArgumentNullException.
        /// </summary>
        /// <param name="argument">The argument to validate.</param>
        /// <param name="name">The name of the argument.</param>
        /// <returns>this</returns>
        public Require IsNotNull(object argument, string name)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(name);
            }

            return this;
        }

        /// <summary>
        /// Validates the given condition is true, otherwise throws an ArgumentException.
        /// </summary>
        /// <param name="condition">The condition to validate</param>
        /// <param name="message">The message to give to the exception in case the condition is false.</param>
        /// <returns>this</returns>
        public Require IsTrue(bool condition, string message)
        {
            if (!condition)
            {
                throw new ArgumentException(message);
            }

            return this;
        }

        /// <summary>
        /// Validates the given condition is true, otherwise throws an InvalidOperationException.
        /// </summary>
        /// <param name="condition">The condition to validate</param>
        /// <param name="message">The message to give to the exception in case the condition is false.</param>
        /// <returns>thiss</returns>
        public Require State(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }

            return this;
        }

        /// <summary>
        /// Assigns a default to a given object in case it's a certain value.
        /// </summary>
        /// <typeparam name="T">The type of the given object</typeparam>
        /// <param name="value">The object to validate and assign to default if a certain condition is met.</param>
        /// <param name="inCase">The value to compare the object to.</param>
        /// <param name="defaultVal">The default value to set the object to</param>
        /// <returns>this</returns>
        public Require OrDefault<T>(ref T value, T inCase, T defaultVal)
        {
            return this.OrDefault(ref value, object.Equals(value, inCase), defaultVal);
        }

        /// <summary>
        /// Assigns a default to a given object in case a certain condition is met.
        /// </summary>
        /// <typeparam name="T">The type of the given object</typeparam>
        /// <param name="value">The object to validate and assign to default if it's a certain value.</param>
        /// <param name="condition">The condition that the value would be set to default in case it's met.</param>
        /// <param name="defaultVal">The default value to set the object to</param>
        /// <returns>this</returns>
        public Require OrDefault<T>(ref T value, bool condition, T defaultVal)
        {
            if (condition)
            {
                value = defaultVal;
            }

            return this;
        }
    }
}
