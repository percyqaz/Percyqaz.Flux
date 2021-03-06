namespace Percyqaz.Flux.Input

open System.Collections.Generic
open OpenTK.Windowing.GraphicsLibraryFramework
open Bind

type Hotkey = string

module Hotkeys =

    let private defaults = Dictionary<string, Bind>()
    let hotkeys = Dictionary<string, Bind>()

    let register (id: string) (value: Bind) =
        defaults.Add(id, value)
        hotkeys.Add(id, value)

    let inline get (id: string) = hotkeys.[id]

    let reset (id: string) =
        hotkeys.[id] <- defaults.[id]

    let init() =
        register "none" Dummy
        register "exit" (mk Keys.Escape)
        register "select" (mk Keys.Enter)
        register "up" (mk Keys.Up)
        register "down" (mk Keys.Down)
        register "left" (mk Keys.Left)
        register "right" (mk Keys.Right)

    let import(d: Dictionary<string, Bind>) =
        for k in d.Keys do
            ignore (hotkeys.Remove k)
            hotkeys.Add (k, d.[k])

    let export(d: Dictionary<string, Bind>) =
        d.Clear()
        for k in hotkeys.Keys do
            d.Add (k, hotkeys.[k])

[<AutoOpen>]
module Helpers =
    
    // The hotkey lookup operator
    let inline (!|) (id: string) = Hotkeys.get id