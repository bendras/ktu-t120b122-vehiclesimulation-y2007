#region File Description
//-----------------------------------------------------------------------------
// AudioManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace Audio3D
{
    /// <summary>
    /// Audio manager keeps track of what 3D sounds are playing, updating
    /// their settings as the camera and entities move around the world,
    /// and automatically disposing cue instances after they finish playing.
    /// </summary>
    public class AudioManager : Microsoft.Xna.Framework.GameComponent
    {
        #region Fields


        // XACT objects.
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;


        // The listener describes the ear which is hearing 3D sounds.
        // This is usually set to match the camera.
        public AudioListener Listener
        {
            get { return listener; }
        }

        AudioListener listener = new AudioListener();


        // The emitter describes an entity which is making a 3D sound.
        AudioEmitter emitter = new AudioEmitter();

        #endregion


        public AudioManager(Game game) : base(game)
        { }


        /// <summary>
        /// Loads the XACT data.
        /// </summary>
        public override void Initialize()
        {
            audioEngine = new AudioEngine("Content/Audio/audio.xgs");
            waveBank = new WaveBank(audioEngine, "Content/Audio/Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content/Audio/Sound Bank.xsb");

            base.Initialize();
        }


        /// <summary>
        /// Unloads the XACT data.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    soundBank.Dispose();
                    waveBank.Dispose();
                    audioEngine.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        
        /// <summary>
        /// Updates the state of the 3D audio system.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Loop over all the currently playing 3D sounds.
            int index = 0;

            
            // Update the XACT engine.
            audioEngine.Update();

            base.Update(gameTime);
        }
    }
}
