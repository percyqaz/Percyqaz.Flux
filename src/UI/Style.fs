﻿namespace Percyqaz.Flux.UI

open Percyqaz.Common
open Percyqaz.Flux.Utils
open Percyqaz.Flux.Graphics.Fonts

open System.Drawing

module Style =

    let padding = 5.0f
    let mutable baseFont = Unchecked.defaultof<SpriteFont>
    let accentColor = Animation.Color Color.Blue
    let changePrimaryColor col = accentColor.Target <- col

    let color (alpha, brightness, white) =
        let accentColor = accentColor.Value
        let r = float32 accentColor.R
        let g = float32 accentColor.G
        let b = float32 accentColor.B
        let rd = (255.0f - r * brightness) * white
        let gd = (255.0f - g * brightness) * white
        let bd = (255.0f - b * brightness) * white
        Color.FromArgb(alpha,
            int ((r + rd) * brightness) |> min 255,
            int ((g + gd) * brightness) |> min 255,
            int ((b + bd) * brightness) |> min 255)

module Colors =

    let black = Color.Black
    let shadow_1 = Color.FromArgb 0xFF_050308
    let shadow_2 = Color.FromArgb 0xFF_0a0911

    let white = Color.White
    let grey_1 = Color.FromArgb 0xFF_cecfd9
    let grey_2 = Color.FromArgb 0xFF_b4b3bc
    let text = white, shadow_1
    let text_subheading = grey_1, shadow_2
    let text_greyout = grey_2, shadow_2

    let green_accent = Color.FromArgb 0xFF_43ef70
    let green = Color.FromArgb 0xFF_1d9d4b
    let green_shadow = Color.FromArgb 0xFF_0c4b24
    let text_green = green_accent, green_shadow
    let text_green_2 = green_accent, shadow_1

    let cyan_accent = Color.FromArgb 0xFF_43e0ef
    let cyan = Color.FromArgb 0xFF_1d869d
    let cyan_shadow = Color.FromArgb 0xFF_084251
    let text_cyan = cyan_accent, cyan_shadow
    let text_cyan_2 = cyan_accent, shadow_1
    
    let red_accent = Color.FromArgb 0xFF_ef5d57
    let red = Color.FromArgb 0xFF_9c3736
    let red_shadow = Color.FromArgb 0xFF_6e190d
    let text_red = red_accent, red_shadow
    let text_red_2 = red_accent, shadow_1

    let pink_accent = Color.FromArgb 0xFF_ff85c0
    let pink = Color.FromArgb 0xFF_bc4980
    let pink_shadow = Color.FromArgb 0xFF_6c2d4d
    let text_pink = pink_accent, pink_shadow
    let text_pink_2 = pink_accent, shadow_1
    
    let blue_accent = Color.FromArgb 0xFF_0032ff
    let blue = Color.FromArgb 0xFF_001696
    let blue_shadow = Color.FromArgb 0xFF_000450
    let text_blue = blue_accent, blue_shadow

    let yellow_accent = Color.FromArgb 0xFF_ffe670
    let text_yellow_2 = yellow_accent, shadow_1

[<AutoOpen>]
module ColorExtensions =

    type Color with
        member this.O1a a = Color.FromArgb(a / 4, this)
        member this.O2a a = Color.FromArgb(a / 2, this)
        member this.O3a a = Color.FromArgb(a * 7 / 8, this)
        member this.O4a a = Color.FromArgb(a, this)

        member this.O1 = Color.FromArgb(63, this)
        member this.O2 = Color.FromArgb(127, this)
        member this.O3 = Color.FromArgb(223, this)

type PaletteColor =
    {
        Alpha: int
        Brightness: float32
        White: float32
    }
    member this.Lerp (other: PaletteColor) (amount: float32) =
        { 
            Alpha = lerp amount (float32 this.Alpha) (float32 other.Alpha) |> int
            Brightness = lerp amount this.Brightness other.Brightness
            White = lerp amount this.White other.White
        }

module Palette =
    
    let BLACK = { Alpha = 255; Brightness = 0f; White = 0f }
    let WHITE = { Alpha = 255; Brightness = 1f; White = 1f }

    let DARKER = { Alpha = 255; Brightness = 0.25f; White = 0.15f }
    let DARK = { Alpha = 255; Brightness = 0.5f; White = 0.2f }
    let BASE = { Alpha = 255; Brightness = 0.8f; White = 0.25f }
    let LIGHT = { Alpha = 255; Brightness = 1.0f; White = 0.45f }
    let LIGHTER = { Alpha = 255; Brightness = 1.0f; White = 0.7f }

    let HOVER = { LIGHT with Alpha = 127 }
    let SELECTED = { LIGHTER with Alpha = 127 }

    let transition (f: Animation.Fade) (a: PaletteColor) (b: PaletteColor) =
        fun () -> 
            let p = a.Lerp b f.Value
            Style.color (p.Alpha, p.Brightness, p.White)

    let text (a: unit -> Color) (b: unit -> Color) = fun () -> (a(), b())

[<AutoOpen>]
module PaletteOperators =

    let (!%) (p: PaletteColor) =
        if p = Palette.BLACK then K Color.Black
        elif p = Palette.WHITE then K Color.White
        else fun () -> Style.color (p.Alpha, p.Brightness, p.White)
    
    let (!*) (p: PaletteColor) =
        if p = Palette.BLACK then Color.Black
        elif p = Palette.WHITE then Color.White
        else Style.color (p.Alpha, p.Brightness, p.White)
    
    let (!^) (p: PaletteColor) =
        fun () -> Style.color (p.Alpha, p.Brightness, p.White), Color.Black