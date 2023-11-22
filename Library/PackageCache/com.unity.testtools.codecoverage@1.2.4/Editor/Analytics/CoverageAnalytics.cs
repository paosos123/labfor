// #define COVERAGE_ANALYTICS_LOGGING

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.TestTools.CodeCoverage.Utils;
using UnityEngine;
using UnityEngine.Analytics;

namespace UnityEditor.TestTools.CodeCoverage.Analytics
{
    [Serializable]
    internal class Timer
    {
        public void Start()
        {
            startTime = DateTime.Now;
        }

        public long elapsedTimeMs => (long)(DateTime.Now - startTime).TotalMilliseconds;

        [SerializeField]
        private DateTime startTime = DateTime.Now;
    }

    [Serializable]
    internal class CoverageAnalytics : ScriptableSingleton<CoverageAnalytics>
    {
        [SerializeField]
        private bool s_Registered;
        [SerializeField]
        private List<int> s_ResultsIdsList;

        public CoverageAnalyticsEvent CurrentCoverageEvent;
        public Timer CoverageTimer;

        protected CoverageAnalytics() : base()
        {
            ResetEvents();
        }

        public void ResetEvents()
        {
            CurrentCoverageEvent = new CoverageAnalyticsEvent();
            CoverageTimer = new Timer();
            s_ResultsIdsList = new List<int>();

            CurrentCoverageEvent.actionID = ActionID.Other;
            CurrentCoverageEvent.coverageModeId = CoverageModeID.None;
            CurrentCoverageEvent.numOfIncludedPaths = 0;
            CurrentCoverageEvent.numOfExcludedPaths = 0;
        }

        public void StartTimer()
        {
            CoverageTimer.Start();
        }

        // See ResultsLogger.cs for details
        public void AddResult(ResultID resultId)
        {
            s_ResultsIdsList.Add((int)resultId);
        }

        public void SendCoverageEvent(bool success)
        {
            CurrentCoverageEvent.success = success;
            CurrentCoverageEvent.duration = CoverageTimer.elapsedTimeMs;
            CurrentCoverageEvent.resultIds = s_ResultsIdsList.ToArray();

            bool runFromCommandLine = CommandLineManager.instance.runFromCommandLine;
            bool batchmode = CommandLineManager.instance.batchmode;
            bool useProjectSettings = CommandLineManager.instance.useProjectSettings;

            CurrentCoverageEvent.runFromCommandLine = runFromCommandLine;
            CurrentCoverageEvent.batchmode = batchmode;
            CurrentCoverageEvent.useProjectSettings = useProjectSettings;

            if (batchmode && !useProjectSettings)
            {
                CurrentCoverageEvent.autogenerate = CommandLineManager.instance.generateBadgeReport || CommandLineManager.instance.generateHTMLReport || CommandLineManager.instance.generateAdditionalReports;
                CurrentCoverageEvent.createBadges = CommandLineManager.instance.generateBadgeReport;
                CurrentCoverageEvent.generateHistory = CommandLineManager.instance.generateHTMLReportHistory;
                CurrentCoverageEvent.generateHTMLReport = CommandLineManager.instance.generateHTMLReport;
                CurrentCoverageEvent.generateMetrics = CommandLineManager.instance.generateAdditionalMetrics;
                CurrentCoverageEvent.generateTestReferences = CommandLineManager.instance.generateTestReferences;
                CurrentCoverageEvent.generateAdditionalReports = CommandLineManager.instance.generateAdditionalReports;
                CurrentCoverageEvent.dontClear = CommandLineManager.instance.dontClear;
                CurrentCoverageEvent.useDefaultAssemblyFilters = !CommandLineManager.instance.assemblyFiltersSpecified;
                CurrentCoverageEvent.useDefaultPathFilters = !CommandLineManager.instance.pathFiltersSpecified;
                CurrentCoverageEvent.useDefaultResultsLoc = CommandLineManager.instance.coverageResultsPath.Length == 0;
                CurrentCoverageEvent.useDefaultHistoryLoc = CommandLineManager.instance.coverageHistoryPath.Length == 0;
                CurrentCoverageEvent.usePathReplacePatterns = CommandLineManager.instance.pathReplacingSpecified;
                CurrentCoverageEvent.useSourcePaths = CommandLineManager.instance.sourcePathsSpecified;
                CurrentCoverageEvent.usePathFiltersFromFile = CommandLineManager.instance.pathFiltersFromFileSpecified;
                CurrentCoverageEvent.useAssemblyFiltersFromFile = CommandLineManager.instance.assemblyFiltersFromFileSpecified;
            }
            else
            {
                CurrentCoverageEvent.autogenerate = CommandLineManager.instance.generateBadgeReport || CommandLineManager.instance.generateHTMLReport || CommandLineManager.instance.generateAdditionalReports || CoveragePreferences.instance.GetBool("AutoGenerateReport", true);
                CurrentCoverageEvent.autoOpenReport = CoveragePreferences.instance.GetBool("OpenReportWhenGenerated", true);
                CurrentCoverageEvent.createBadges = CommandLineManager.instance.generateBadgeReport || CoveragePreferences.instance.GetBool("GenerateBadge", true);
                CurrentCoverageEvent.generateHistory = CommandLineManager.instance.generateHTMLReportHistory || CoveragePreferences.instance.GetBool("IncludeHistoryInReport", true);
                CurrentCoverageEvent.generateHTMLReport = CommandLineManager.instance.generateHTMLReport || CoveragePreferences.instance.GetBool("GenerateHTMLReport", true);
                CurrentCoverageEvent.generateMetrics = CommandLineManager.instance.generateAdditionalMetrics || CoveragePreferences.instance.GetBool("GenerateAdditionalMetrics", false);
                CurrentCoverageEvent.generateTestReferences = CommandLineManager.instance.generateTestReferences || CoveragePreferences.instance.GetBool("GenerateTestReferences", false);
                CurrentCoverageEvent.generateAdditionalReports = CommandLineManager.instance.generateAdditionalReports || CoveragePreferences.instance.GetBool("GenerateAdditionalReports", false);
                CurrentCoverageEvent.dontClear = CommandLineManager.instance.dontClear;
                CurrentCoverageEvent.usePathReplacePatterns = CommandLineManager.instance.pathReplacingSpecified;
                CurrentCoverageEvent.useSourcePaths = CommandLineManager.instance.sourcePathsSpecified;
                CurrentCoverageEvent.usePathFiltersFromFile = CommandLineManager.instance.pathFiltersFromFileSpecified;
                CurrentCoverageEvent.useAssemblyFiltersFromFile = CommandLineManager.instance.assemblyFiltersFromFileSpecified;


                CurrentCoverageEvent.useDefaultAssemblyFilters = !CommandLineManager.instance.assemblyFiltersSpecified;
                if (!CommandLineManager.instance.assemblyFiltersSpecified)
                    CurrentCoverageEvent.useDefaultAssemblyFilters = string.Equals(CoveragePreferences.instance.GetString("IncludeAssemblies", AssemblyFiltering.GetUserOnlyAssembliesString()), AssemblyFiltering.GetUserOnlyAssembliesString(), StringComparison.InvariantCultureIgnoreCase);

                CurrentCoverageEvent.useDefaultPathFilters = !CommandLineManager.instance.pathFiltersSpecified;
                if (!CommandLineManager.instance.pathFiltersSpecified)
                    CurrentCoverageEvent.useDefaultPathFilters = string.Equals(CoveragePreferences.instance.GetString("PathsToInclude", string.Empty), string.Empty) && string.Equals(CoveragePreferences.instance.GetString("PathsToExclude", string.Empty), string.Empty);

                CurrentCoverageEvent.useDefaultResultsLoc = CommandLineManager.instance.coverageResultsPath.Length == 0;
                if (CommandLineManager.instance.coverageResultsPath.Length == 0)
                    CurrentCoverageEvent.useDefaultResultsLoc = string.Equals(CoveragePreferences.instance.GetStringForPaths("Path", string.Empty), CoverageUtils.GetProjectPath(), StringComparison.InvariantCultureIgnoreCase);

                CurrentCoverageEvent.useDefaultHistoryLoc = CommandLineManager.instance.coverageHistoryPath.Length == 0;
                if (CommandLineManager.instance.coverageHistoryPath.Length == 0)
                    CurrentCoverageEvent.useDefaultHistoryLoc = string.Equals(CoveragePreferences.instance.GetStringForPaths("HistoryPath", string.Empty), Cover                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       