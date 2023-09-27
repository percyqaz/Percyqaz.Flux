﻿namespace Percyqaz.Flux.Windowing

open Percyqaz.Common

type WindowType =
    | Windowed = 0
    | Borderless = 1
    | Fullscreen = 2
    | ``Borderless Fullscreen`` = 3

module WindowResolution =
    let presets : (int * int) array =
        [|
            800, 600
            1024, 768
            1280, 800
            1280, 1024
            1366, 768
            1600, 900
            1600, 1024
            1680, 1050
            1920, 1080
            2715, 1527
        |]


type FrameLimit =
    | Unlimited = 0
    | Smart = 1

type Config = 
    {
        WorkingDirectory: string
        Locale: string
        WindowMode: Setting<WindowType>
        WindowResolution: Setting<int * int>
        RenderMode: Setting<FrameLimit>
        FullscreenRefreshRateOverride: Setting<int>
        Display: Setting<int>
        AudioDevice: Setting<int>
    }
    static member Default = 
        {
            WorkingDirectory = ""
            Locale = "en_GB.txt"
            WindowMode = Setting.simple WindowType.Borderless
            WindowResolution = Setting.simple (1024, 768)
            RenderMode = Setting.simple FrameLimit.Smart
            FullscreenRefreshRateOverride = Setting.simple 0
            Display = Setting.simple 0
            AudioDevice = Setting.simple -1
        }
