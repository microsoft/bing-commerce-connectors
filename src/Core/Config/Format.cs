// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core.Config
{
    /// <summary>
    /// The format to use when pushing to the Bing for Commerce endpoint.
    /// </summary>
    public enum Format
    {
        /// <summary>
        /// A Json array of objects.
        /// </summary>
        JsonArray,

        /// <summary>
        /// New-line Delimited Json objects.
        /// </summary>
        NDJson,

        /// <summary>
        /// Comma-Separated Values.
        /// </summary>
        Csv,

        /// <summary>
        /// Tab-Separated Values.
        /// </summary>
        Tsv
    }
}
