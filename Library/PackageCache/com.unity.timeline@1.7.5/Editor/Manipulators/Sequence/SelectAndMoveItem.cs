                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                tion should only keep this item selected.
                if (SelectionManager.Count() > 1 && ItemSelection.CanClearSelection(evt))
                {
                    SelectionManager.Clear();
                    item.Select();
                    return true;
                }

                if (m_CycleMarkersPending)
                {
                    m_CycleMarkersPending = false;
                    TimelineMarkerClusterGUI.CycleMarkers();
                    return true;
                }

                return false;
            }

            m_TimeAreaAutoPanner = null;

            DropItems();

            m_SnapEngine = null;
            m_MoveItemHandler = null;

            state.Evaluate();
            state.RemoveCaptured(this);
            m_Dragged = false;
            TimelineCursors.ClearCursor();

            return true;
        }

        protected override bool DoubleClick(Event evt, WindowState state)
        {
            return MouseDown(evt, state) && MouseUp(evt, state);
        }

        protected override bool MouseDrag(Event evt, WindowState state)
        {
            if (state.editSequence.isReadOnly)
                return false;

            // case 1099285 - ctrl-click can cause no clips to be selected
            var selectedItemsGUI = SelectionManager.SelectedItems();
            if (!selectedItemsGUI.Any())
            {
                m_Dragged = false;
                return false;
            }

            const float hDeadZone = 5.0f;
            const float vDeadZone = 5.0f;

            bool vDone = m_VerticalMovementDone || Math.Abs(evt.mousePosition.y - m_MouseDownPosition.y) > vDeadZone;
            bool hDone = m_HorizontalMovementDone || Math.Abs(evt.mousePosition.x - m_MouseDownPosition.x) > hDeadZone;

            m_CycleMarkersPending = false;

            if (!m_Dragged)
            {
                var canStartMove = vDone || hDone;

                if (canStartMove)
                {
                    state.AddCaptured(this);
                    m_Dragged = true;

                    var referenceTrack = GetTrackDropTargetAt(state, m_MouseDownPosition);

                    foreach (var item in selectedItemsGUI)
                        item.gui.StartDrag();

                    m_MoveItemHandler = new MoveItemHandler(state);

                    m_MoveItemHandler.Grab(selectedItemsGUI, referenceTrack, m_MouseDownPosition);

                    m_SnapEngine = new SnapEngine(m_MoveItemHandler, m_MoveItemHandler, ManipulateEdges.Both,
                        state, m_MouseDownPosition);

                    m_TimeAreaAutoPanner = new TimeAreaAutoPanner(state);
                }
            }

            if (!m_VerticalMovementDone)
            {
                m_VerticalMovementDone = vDone;

                if (m_VerticalMovementDone)
                    m_MoveItemHandler.OnTrackDetach();
            }

            if (!m_HorizontalMovementDone)
            {
                m_HorizontalMovementDone = hDone;
            }

            if (m_Dragged)
            {
                if (m_HorizontalMovementDone)
                    m_SnapEngine.Snap(evt.mousePosition, evt.modifiers);

                if (m_VerticalMovementDone)
                {
                    var track = GetTrackDropTargetAt(state, evt.mousePosition);
                    m_MoveItemHandler.UpdateTrackTarget(track);
                }

                state.Evaluate();
            }

            return true;
        }

        public override void Overlay(Event evt, WindowState state)
        {
            if (!m_Dragged)
                return;

            if (m_TimeAreaAutoPanner != null)
                m_TimeAreaAutoPanner.OnGUI(evt);

            m_MoveItemHandler.OnGUI(evt);

            if (!m_MoveItemHandler.allowTrackSwitch || m_MoveItemHandler.targetTrack != null)
            {
                TimeIndicator.Draw(state, m_MoveItemHandler.start, m_MoveItemHandler.end);
                m_SnapEngine.OnGUI();
            }
        }

        bool HandleMarkerCycle()
        {
            m_CycleMarkersPending = TimelineMarkerClusterGUI.CanCycleMarkers();
            return m_CycleMarkersPending;
        }

        static bool HandleSingleSelection(Event evt)
        {
            return ItemSelection.HandleSingleSelection(evt) != null;
        }

        void DropItems()
        {
            // Order matters here: m_MoveItemHandler.movingItems is destroyed during call to Drop()
            foreach (var movingItem in m_MoveItemHandler.movingItems)
            {
                foreach (var item in movingItem.items)
                    item.gui.StopDrag();
            }

            m_MoveItemHandler.Drop();
        }

        static TrackAsset GetTrackDropTargetAt(WindowState state, Vector2 point)
        {
            var track = state.spacePartitioner.GetItemsAtPosition<IRowGUI>(point).FirstOrDefault();
            return track != null ? track.asset : null;
        }
    }
}
