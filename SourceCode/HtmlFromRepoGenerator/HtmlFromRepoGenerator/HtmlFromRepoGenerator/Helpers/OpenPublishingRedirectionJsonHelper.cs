using System;
using System.Collections.Generic;
using System.IO;
using HtmlFromRepoGenerator.Exceptions;
using Newtonsoft.Json.Linq;

namespace HtmlFromRepoGenerator.Helpers
{
    public class OpenPublishingRedirectionJsonHelper
    {
        #region Public Methods

        /// <summary>
        /// Determines whether .openpublishing.redirection.json is correct.
        /// </summary>
        /// <param name="openPublishingRedirectionJsonPath">The .openpublishing.redirection.json path.</param>
        /// <returns>
        ///   <c>true</c> if .openpublishing.redirection.json is correct; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsOpenPublishingRedirectionJsonCorrect(string openPublishingRedirectionJsonPath)
        {
            string openPublishingRedirectionJsonContent = File.ReadAllText(openPublishingRedirectionJsonPath);
            JObject openPublishingRedirection = JObject.Parse(openPublishingRedirectionJsonContent);
            
            return openPublishingRedirection["redirections"] != null;
        }

        public static Dictionary<string, string> GetPathRedirectPairs(string openPublishingRedirectionJsonPath)
        {
            Dictionary<string, string> pathRedirectPairs = new Dictionary<string, string>();

            string openPublishingRedirectionJsonContent = File.ReadAllText(openPublishingRedirectionJsonPath);
            JObject openPublishingRedirection = JObject.Parse(openPublishingRedirectionJsonContent);
            foreach (JObject redirection in openPublishingRedirection["redirections"])
            {
                if (redirection["source_path"].ToString().StartsWith("business-central/"))
                {
                    pathRedirectPairs.Add(redirection["source_path"].ToString().Substring("business-central/".Length), redirection["redirect_url"].ToString());
                }
            }
            return pathRedirectPairs;
        }
        #endregion Public Methods
    }
}