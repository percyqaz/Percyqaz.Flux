﻿namespace Percyqaz.Flux.UI

open Percyqaz.Common
open Percyqaz.Flux.Input

type Button(text: unit -> string, on_click: unit -> unit) as this =
    inherit
        StaticContainer(
            NodeType.Button(fun () ->
                if not (this.Disabled()) then
                    Style.click.Play()
                    on_click ()
            )
        )

    member val Hotkey: Hotkey = "none" with get, set
    member val Disabled: unit -> bool = K false with get, set
    member val Floating = false with get, set

    new(text: string, on_click: unit -> unit) = Button(K text, on_click)

    override this.OnFocus() =
        Style.hover.Play()
        base.OnFocus()

    override this.Init(parent: Widget) =
        this
        |+ Text(
            text,
            Align = Alignment.CENTER,
            Color =
                fun () ->
                    if this.Disabled() then Colors.text_greyout
                    elif this.Focused then Colors.text_yellow_2
                    else Colors.text
        )
        |+ Clickable(
            (fun () ->
                if not (this.Disabled()) then
                    this.Select()
            ),
            Floating = this.Floating,
            OnHover =
                fun b ->
                    if b && not (this.Disabled()) then
                        this.Focus()
        )
        |* HotkeyAction(
            this.Hotkey,
            fun () ->
                Style.click.Play()
                on_click ()
        )

        base.Init parent

// todo: look into getting rid of this in favour of normal buttons with icons as part of the label
type IconButton(text: unit -> string, icon: string, icon_size: float32, on_click: unit -> unit) as this =
    inherit
        StaticContainer(
            NodeType.Button(fun () ->
                if not (this.Disabled()) then
                    Style.click.Play()
                    on_click ()
            )
        )

    member val Hotkey: Hotkey = "none" with get, set
    member val Disabled: unit -> bool = K false with get, set
    member val HoverIcon = icon with get, set

    new(text: string, icon, icon_size, on_click: unit -> unit) = IconButton(K text, icon, icon_size, on_click)

    override this.OnFocus() =
        Style.hover.Play()
        base.OnFocus()

    override this.Init(parent: Widget) =
        this
        |+ Text(
            (fun () -> if this.Focused then this.HoverIcon else icon),
            Align = Alignment.CENTER,
            Color =
                (fun () ->
                    if this.Disabled() then Colors.text_greyout
                    elif this.Focused then Colors.text_yellow_2
                    else Colors.text
                ),
            Position = Position.SliceLeft icon_size
        )
        |+ Text(
            text,
            Align = Alignment.CENTER,
            Color =
                (fun () ->
                    if this.Disabled() then Colors.text_greyout
                    elif this.Focused then Colors.text_yellow_2
                    else Colors.text
                ),
            Position = Position.TrimLeft icon_size
        )
        |+ Clickable(
            (fun () ->
                if not (this.Disabled()) then
                    this.Select()
            ),
            OnHover =
                fun b ->
                    if b && not (this.Disabled()) then
                        this.Focus()
        )
        |* HotkeyAction(
            this.Hotkey,
            fun () ->
                Style.click.Play()
                on_click ()
        )

        base.Init parent
