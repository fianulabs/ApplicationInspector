﻿// Copyright (C) Microsoft. All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.ApplicationInspector.Commands;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ApplicationInspector.CLI
{
    /// <summary>
    /// Writes in json format
    /// Users can select arguments to filter output to 1. only simple tags 2. only matchlist without rollup metadata etc. 3. everything
    /// Lists of tagreportgroups are written as well as match list details so users have chose to present the same
    /// UI as shown in the HTML report to the level of detail desired...
    /// </summary>
    public class AnalyzeJsonWriter : CommandResultsWriter
    {
        /// <summary>
        /// simple wrapper for serializing results for simple tags only during processing
        /// </summary>
        private class TagsFile
        {
            [JsonProperty(PropertyName = "tags")]
            public string[] Tags { get; set; }
        }

        public override void WriteResults(Result result, CLICommandOptions commandOptions, bool autoClose = true)
        {
            CLIAnalyzeCmdOptions cLIAnalyzeCmdOptions = (CLIAnalyzeCmdOptions)commandOptions;
            AnalyzeResult analyzeResult = (AnalyzeResult)result;

            WriteOnce.Result("Result");

            if (cLIAnalyzeCmdOptions.SimpleTagsOnly)
            {
                List<string> keys = new List<string>(analyzeResult.Metadata.UniqueTags);
                keys.Sort();
                TagsFile tags = new TagsFile();
                tags.Tags = keys.ToArray();
                TextWriter.Write(JsonConvert.SerializeObject(tags, Formatting.Indented));
            }
            else
            {
                JsonSerializer jsonSerializer = new JsonSerializer();
                jsonSerializer.Formatting = Formatting.Indented;
                jsonSerializer.Serialize(TextWriter, analyzeResult);
            }

            WriteOnce.NewLine();

            if (autoClose)
            {
                FlushAndClose();
            }
        }

        public override void FlushAndClose()
        {
            TextWriter.Flush();
            TextWriter.Close();
            WriteOnce.TextWriter = null;
        }
    }
}