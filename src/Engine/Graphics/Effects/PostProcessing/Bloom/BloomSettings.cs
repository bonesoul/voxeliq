/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System.Collections.Generic;

namespace Engine.Graphics.Effects.PostProcessing.Bloom
{
    public class BloomSettings
    {
        // Name of a preset bloom setting, for display to the user.
        public readonly BloomState Name;

        // Controls how bright a pixel needs to be before it will bloom.
        // Zero makes everything bloom equally, while higher values select
        // only brighter colors. Somewhere between 0.25 and 0.5 is good.
        public readonly float BloomThreshold;

        // Controls how much blurring is applied to the bloom image.
        // The typical range is from 1 up to 10 or so.
        public readonly float BlurAmount;

        // Controls the amount of the bloom and base images that
        // will be mixed into the final scene. Range 0 to 1.
        public readonly float BloomIntensity;
        public readonly float BaseIntensity;

        // Independently control the color saturation of the bloom and
        // base images. Zero is totally desaturated, 1.0 leaves saturation
        // unchanged, while higher values increase the saturation level.
        public readonly float BloomSaturation;
        public readonly float BaseSaturation;

        /// <summary>
        /// Constructs a new bloom settings descriptor.
        /// </summary>
        public BloomSettings(BloomState name, float bloomThreshold, float blurAmount,
                             float bloomIntensity, float baseIntensity,
                             float bloomSaturation, float baseSaturation)
        {
            Name = name;
            BloomThreshold = bloomThreshold;
            BlurAmount = blurAmount;
            BloomIntensity = bloomIntensity;
            BaseIntensity = baseIntensity;
            BloomSaturation = bloomSaturation;
            BaseSaturation = baseSaturation;
        }


        /// <summary>
        /// Table of preset bloom settings, used by the sample program.
        /// </summary>
        public static Dictionary<BloomState, BloomSettings> PresetSettings = new Dictionary<BloomState, BloomSettings>()
        {
            {BloomState.Default, new BloomSettings(BloomState.Default, 0.25f, 4, 1.25f, 1, 1, 1)},
            {BloomState.Soft, new BloomSettings(BloomState.Soft, 0, 3, 1, 1, 1, 1)},
            {BloomState.Desaturated, new BloomSettings(BloomState.Desaturated, 0.5f, 8, 2, 1, 0, 1)},
            {BloomState.Saturated, new BloomSettings(BloomState.Saturated, 0.25f, 4, 2, 1, 2, 0)},
            {BloomState.Blurry, new BloomSettings(BloomState.Blurry, 0, 2, 1, 0.1f, 1, 1)},
            {BloomState.Subtle, new BloomSettings(BloomState.Subtle, 0.5f, 2, 1, 1, 1, 1)},
        };
    }

    public enum BloomState
    {
        Default,
        Soft,
        Desaturated,
        Saturated,
        Blurry,
        Subtle
    }
}
