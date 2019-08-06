﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using Selenium.Axe.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Selenium.Axe
{
    /// <summary>
    /// Fluent style builder for invoking aXe. Instantiate a new Builder and configure testing with the include(),
    /// exclude(), and options() methods before calling analyze() to run.
    /// </summary>
    public class AxeBuilder
    {
        private readonly IWebDriver _webDriver;

        private readonly AxeRunContext runContext = new AxeRunContext();
        private readonly AxeRunOptions runOptions = new AxeRunOptions();
        private static readonly AxeBuilderOptions DefaultOptions = new AxeBuilderOptions { ScriptProvider = new EmbeddedResourceAxeProvider() };

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore
        };

        [Obsolete("Options is deprecated. This will be removed in future versions. Please use WithTags, WithRules & DisableRules apis instead")]
        public string Options { get; set; } = "{}";

        /// <summary>
        /// Initialize an instance of <see cref="AxeBuilder"/>
        /// </summary>
        /// <param name="webDriver">Selenium driver to use</param>
        public AxeBuilder(IWebDriver webDriver) : this(webDriver, DefaultOptions)
        {
        }

        /// <summary>
        /// Limit analysis to only the specified tags. Cannot be used with <see cref="WithRules(string[])"/>
        /// </summary>
        /// <param name="tags">tags to be used for scanning</param>
        public AxeBuilder WithTags(params string[] tags)
        {
            Options = null;
            runOptions.RunOnly = new RunOnlyOptions
            {
                Type = "tag",
                Values = tags.ToList()
            };
            return this;
        }

        /// <summary>
        /// Limit analysis to only the specified rules. Cannot be used with <see cref="WithTags(string[])"/>
        /// </summary>
        /// <param name="rules">rules to be used for scanning</param>
        public AxeBuilder WithRules(params string[] rules)
        {
            Options = null;
            runOptions.RunOnly = new RunOnlyOptions
            {
                Type = "rule",
                Values = rules.ToList()
            };

            return this;
        }

        /// <summary>
        ///  Set the list of rules to skip when running an analysis
        /// </summary>
        /// <param name="rules">rules to be skipped from analysis</param>
        public AxeBuilder DisableRules(params string[] rules)
        {
            Options = null;
            var rulesMap = new Dictionary<string, RuleOptions>();
            foreach (var rule in rules)
            {
                rulesMap[rule] = new RuleOptions
                {
                    Enabled = false
                };
            }
            runOptions.Rules = rulesMap;
            return this;
        }

        /// <summary>
        /// Initialize an instance of <see cref="AxeBuilder"/>
        /// </summary>
        /// <param name="webDriver">Selenium driver to use</param>
        /// <param name="options">Builder options</param>
        public AxeBuilder(IWebDriver webDriver, AxeBuilderOptions options)
        {
            if (webDriver == null)
                throw new ArgumentNullException(nameof(webDriver));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _webDriver = webDriver;
            _webDriver.Inject(options.ScriptProvider);
        }

        /// <summary>
        /// Execute the script into the target.
        /// </summary>
        /// <param name="args">args to be passed to scan function (context, options)</param>
        private AxeResult Execute(params object[] args)
        {
            string stringifiedResult = (string) ((IJavaScriptExecutor)_webDriver).ExecuteAsyncScript(Resources.scan, args);
            var jObject = JObject.Parse(stringifiedResult);
            return new AxeResult(jObject);   
        }

        /// <summary>
        /// Selectors to include in the validation.
        /// </summary>
        /// <param name="selectors">Any valid CSS selectors</param>
        /// <returns></returns>
        public AxeBuilder Include(params string[] selectors)
        {
            runContext.Include = runContext.Include ?? new List<string[]>();
            runContext.Include.Add(selectors);
            return this;
        }

        /// <summary>
        /// Selectors to exclude in the validation.
        /// </summary>
        /// <param name="selectors">Any valid CSS selectors</param>
        /// <returns></returns>
        public AxeBuilder Exclude(params string[] selectors)
        {
            runContext.Exclude = runContext.Exclude ?? new List<string[]>();
            runContext.Exclude.Add(selectors);
            return this;
        }

        /// <summary>
        /// Run aXe against a specific WebElement.
        /// </summary>
        /// <param name="context"> A WebElement to test</param>
        /// <returns>An aXe results document</returns>
        public AxeResult Analyze(IWebElement context)
        {
            //string command = string.Format("axe.a11yCheck(arguments[0], {0}, arguments[arguments.length - 1]);", Options);
            return Execute(context, JsonConvert.SerializeObject(runOptions, JsonSerializerSettings));
        }

        /// <summary>
        /// Run aXe against the page.
        /// </summary>
        /// <returns>An aXe results document</returns>
        public AxeResult Analyze()
        {
            bool runContextHasData = runContext.Include?.Any() == true || runContext.Exclude?.Any() == true;

            string contextToBeSent = runContextHasData ? JsonConvert.SerializeObject(runContext, JsonSerializerSettings) : null;
            string runOptionsToBeSent = string.IsNullOrWhiteSpace(Options) ? JsonConvert.SerializeObject(runOptions, JsonSerializerSettings) : Options;

            return Execute(contextToBeSent, runOptionsToBeSent);
        }
    }
}