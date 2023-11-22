using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using Codice.Client.Commands;
using Codice.CM.Common;
using PlasticGui;
using PlasticGui.WorkspaceWindow.Items;
using PlasticGui.WorkspaceWindow.Open;
using PlasticGui.WorkspaceWindow.PendingChanges;
using PlasticGui.WorkspaceWindow.PendingChanges.Changelists;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.Views.PendingChanges.Changelists;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges
{
    internal class PendingChangesViewPendingChangeMenu
    {
        internal interface IMetaMenuOperations
        {
            void DiffMeta();
            void OpenMeta();
            void OpenMetaWith();
            void OpenMetaInExplorer();
            void HistoryMeta();
            bool SelectionHasMeta();
        }

        internal PendingChangesViewPendingChangeMenu(
            WorkspaceInfo wkInfo,
            IPendingChangesMenuOperations pendingChangesMenuOperations,
            IChangelistMenuOperations changelistMenuOperations,
            IOpenMenuOperations openMenuOperations,
            IMetaMenuOperations metaMenuOperations,
            IFilesFilterPatternsMenuOperations filterMenuOperations)
        {
            mPendingChangesMenuOperations = pendingChangesMenuOperations;
            mChangelistMenuOperations = changelistMenuOperations;
            mOpenMenuOperations = openMenuOperations;
            mMetaMenuOperations = metaMenuOperations;

            mFilterMenuBuilder = new FilesFilterPatternsMenuBuilder(filterMenuOperations);
            mMoveToChangelistMenuBuilder = new MoveToChangelistMenuBuilder(
                wkInfo,
                changelistMenuOperations);

            BuildComponents();
        }

        internal void Popup()
        {
            GenericMenu menu = new GenericMenu();

            UpdateMenuItems(menu);

            menu.ShowAsContext();
        }

        internal bool ProcessKeyActionIfNeeded(Event e)
        {
            PendingChangesMenuOperations operationToExecute =
                GetPendingChangesMenuOperation(e);

            OpenMenuOperations openOperationToExecute =
                GetOpenMenuOperation(e);

            if (operationToExecute == PendingChangesMenuOperations.None &&
                openOperationToExecute == OpenMenuOperations.None)
                return false;

            SelectedChangesGroupInfo info =
                mPendingChangesMenuOperations.GetSelectedChangesGroupInfo();

            if (operationToExecute != PendingChangesMenuOperations.None)
                return ProcessKeyActionForPendingChangesMenu(
                    operationToExecute, mPendingChangesMenuOperations, info);

            return ProcessKeyActionForOpenMenu(
                openOperationToExecute, mOpenMenuOperations, info);
        }

        void OpenMenuItem_Click()
        {
            mOpenMenuOperations.Open();
        }

        void OpenWithMenuItem_Click()
        {
            mOpenMenuOperations.OpenWith();
        }

        void OpenInExplorerMenuItem_Click()
        {
            mOpenMenuOperations.OpenInExplorer();
        }

        void OpenMetaMenuItem_Click()
        {
            mMetaMenuOperations.OpenMeta();
        }

        void OpenMetaWithMenuItem_Click()
        {
            mMetaMenuOperations.OpenMetaWith();
        }

        void OpenMetaInExplorerMenuItem_Click()
        {
            mMetaMenuOperations.OpenMetaInExplorer();
        }

        void DiffMenuItem_Click()
        {
            mPendingChangesMenuOperations.Diff();
        }

        void DiffMetaMenuItem_Click()
        {
            mMetaMenuOperations.DiffMeta();
        }

        void UndoChangesMenuItem_Click()
        {
            mPendingChangesMenuOperations.UndoChanges();
        }

        void CheckoutMenuItem_Click()
        {
            mPendingChangesMenuOperations.ApplyLocalChanges();
        }

        void DeleteMenuItem_Click()
        {
            mPendingChangesMenuOperations.Delete();
        }

        void HistoryMenuItem_Click()
        {
            mPendingChangesMenuOperations.History();
        }

        void HistoryMetaMenuItem_Click()
        {
            mMetaMenuOperations.HistoryMeta();
        }

        void UpdateMenuItems(GenericMenu menu)
        {
            SelectedChangesGroupInfo info =
                mPendingChangesMenuOperations.GetSelectedChangesGroupInfo();

            PendingChangesMenuOperations operations =
                PendingChangesMenuUpdater.GetAvailableMenuOperations(info);

            ChangelistMenuOperations changelistOperations =
                ChangelistMenuOperations.None;

            OpenMenuOperations openOperations =
                GetOpenMenuOperations.ForPendingChangesView(info);

            bool useChangelists = PlasticGuiConfig.Get().
                Configuration.CommitUseChangeLists;

            if (useChangelists)
            {
                List<ChangeListInfo> selectedChangelists =
                    mChangelistMenuOperations.GetSelectedChangelistInfos();

                changelistOperations = ChangelistMenuUpdater.
                    GetAvailableMenuOperations(info, selectedChangelists);
            }

            if (operations == PendingChangesMenuOperations.None &&
                changelistOperations == ChangelistMenuOperations.None &&
                openOperations == OpenMenuOperations.None)
            {
                menu.AddDisabledItem(GetNoActionMenuItemContent());
                return;
            }

            UpdateOpenMenuItems(menu, openOperations);

            menu.AddSeparator(string.Empty);

            if (operations.HasFlag(PendingChangesMenuOperations.DiffWorkspaceContent))
                menu.AddItem(mDiffMenuItemContent, false, DiffMenuItem_Click);
            else
                menu.AddDisabledItem(mDiffMenuItemContent);

            if (mMetaMenuOperations.SelectionHasMeta())
            {
                if (operations.HasFlag(PendingChangesMenuOperations.DiffWorkspaceContent))
                    menu.AddItem(mDiffMetaMenuItemContent, false, DiffMetaMenuItem_Click);
                else
                    menu.AddDisabledItem(mDiffMetaMenuItemContent);
            }

            menu.AddSeparator(string.Empty);

            if (operations.HasFlag(PendingChangesMenuOperations.UndoChanges))
                menu.AddItem(mUndoChangesMenuItemContent, false, UndoChangesMenuItem_Click);
            else
                menu.AddDisabledItem(mUndoChangesMenuItemContent);

            menu.AddSeparator(string.Empty);

            if (operations.HasFlag(PendingChangesMenuOperations.ApplyLocalChanges))
                menu.AddItem(mCheckoutMenuItemContent, false, CheckoutMenuItem_Click);
            else
                menu.AddDisabledItem(mCheckoutMenuItemContent);

            if (operations.HasFlag(PendingChangesMenuOperations.Delete))
                menu.AddItem(mDeleteMenuItemContent, false, DeleteMenuItem_Click);
            else
                menu.AddDisabledItem(mDeleteMenuItemContent);

            if (useChangelists)
            {
                menu.AddSeparator(string.Empty);
                
                mMoveToChangelistMenuBuilder.UpdateMenuItems(
                    menu,
                    changelistOperations,
                    info.SelectedChanges,
                    info.ChangelistsWithSelectedChanges);
            }

            menu.AddSeparator(string.Empty);

            mFilterMenuBuilder.UpdateMenuItems(
                menu, FilterMenuUpdater.GetMenuActions(info));

            menu.AddSeparator(string.Empty);

            if (operations.HasFlag(PendingChangesMenuOperations.History))
                menu.AddItem(mViewHistoryMenuItemContent, false, HistoryMenuItem_Click);
            else
                menu.AddDisabledItem(mViewHistoryMenuItemContent, false);

            if (mMetaMenuOperations.SelectionHasMeta())
            {
                if (operations.HasFlag(PendingChangesMenuOperations.History))
                    menu.AddItem(mViewHistoryMetaMenuItemContent, false, HistoryMetaMenuItem_Click);
                else                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       