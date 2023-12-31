                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     m_LastPlayableTime = kUnsetTime;
                return;
            }

            var time = (float)playable.GetTime();
            var particleTime = particleSystem.time;

            // if particle system time has changed externally, a re-sync is needed
            if (m_LastPlayableTime > time || !Mathf.Approximately(particleTime, m_LastParticleTime))
                Simulate(time, true);
            else if (m_LastPlayableTime < time)
                Simulate(time - m_LastPlayableTime, false);

            m_LastPlayableTime = time;
            m_LastParticleTime = particleSystem.time;
        }

        /// <summary>
        /// This function is called when the Playable play state is changed to Playables.PlayState.Playing.
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            m_LastPlayableTime = kUnsetTime;
        }

        /// <summary>
        /// This function is called when the Playable play state is changed to PlayState.Paused.
        /// </summary>
        /// <param name="playable">The playable this behaviour is attached to.</param>
        /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            m_LastPlayableTime = kUnsetTime;
        }

        private void Simulate(float time, bool restart)
        {
            const bool withChildren = false;
            const bool fixedTimeStep = false;
            float maxTime = Time.maximumDeltaTime;

            if (restart)
                particleSystem.Simulate(0, withChildren, true, fixedTimeStep);

            // simulating by too large a time-step causes sub-emitters not to work, and loops not to
            // simulate correctly
            while (time > maxTime)
            {
                particleSystem.Simulate(maxTime, withChildren, false, fixedTimeStep);
                time -= maxTime;
            }

            if (time > 0)
                particleSystem.Simulate(time, withChildren, false, fixedTimeStep);
        }
    }
}
