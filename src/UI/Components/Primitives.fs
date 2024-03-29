﻿namespace Percyqaz.Flux.UI

open System.Drawing
open Percyqaz.Common
open Percyqaz.Flux.Graphics
open Percyqaz.Flux.Input

module Alignment =
    let LEFT = 0.0f
    let CENTER = 0.5f
    let RIGHT = 1.0f

[<Sealed>]
type Dummy() =
    inherit StaticWidget(NodeType.None)
    override this.Draw() = ()

[<Sealed>]
type Text(text_func) =
    inherit StaticWidget(NodeType.None)

    new(text: string) = Text(K text)

    member val Align = Alignment.CENTER with get, set
    member val Color = K Colors.text with get, set

    override this.Draw() =
        Text.fill_b (Style.font, text_func (), this.Bounds, this.Color(), this.Align)

    override this.Init(parent) = base.Init parent

[<Sealed>]
type Clickable(on_left_click) =
    inherit StaticWidget(NodeType.None)

    let mutable hover = false

    member val OnLeftClick = on_left_click with get, set
    member val OnRightClick = ignore with get, set
    member val OnHover = ignore with get, set
    member val Floating = false with get, set

    override this.Update(elapsed_ms, moved) =
        base.Update(elapsed_ms, moved)
        let was_hovering = hover
        hover <- Mouse.hover (if this.Floating then this.Bounds else this.VisibleBounds)

        if was_hovering && not hover then
            this.OnHover false
        elif not was_hovering && hover && Mouse.moved_recently () then
            this.OnHover true
        elif hover then
            if Mouse.left_click () then
                this.OnLeftClick()

            if Mouse.right_click () then
                this.OnRightClick()

    override this.Draw() = ()

    static member Focus(w: Widget) =
        Clickable(
            w.Select,
            OnHover =
                fun b ->
                    if b then
                        w.Focus()
        )

[<Sealed>]
type HotkeyAction(hotkey: Hotkey, action) =
    inherit StaticWidget(NodeType.None)

    override this.Update(elapsed_ms, moved) =
        base.Update(elapsed_ms, moved)

        if (%%hotkey).Tapped() then
            action ()

    override this.Draw() = ()

[<Sealed>]
type Image(sprite: Sprite) =
    inherit StaticWidget(NodeType.None)

    member val Sprite = sprite with get, set

    override this.Draw() =
        Draw.sprite this.Bounds Color.White this.Sprite

[<Sealed>]
type Conditional(condition: unit -> bool, child: Widget) =
    inherit StaticWidget(NodeType.Switch(fun () -> child))

    override this.Init(parent: Widget) =
        base.Init parent
        child.Init this

    override this.Draw() =
        if condition () then
            child.Draw()

    override this.Update(elapsed_ms, moved) =
        base.Update(elapsed_ms, moved)

        if moved || condition () then
            child.Update(elapsed_ms, moved)

    override this.Focusable = child.Focusable && condition ()

type Frame(node_type) =
    inherit StaticContainer(node_type)

    member val Fill = !%Palette.DARK with get, set
    member val Border = !%Palette.LIGHT with get, set

    override this.Draw() =
        let border = this.Border()

        if border.A > 0uy then

            let r = this.Bounds.Expand Style.PADDING
            Draw.rect (r.SliceLeft Style.PADDING) border
            Draw.rect (r.SliceRight Style.PADDING) border

            let r = this.Bounds.Expand(0.0f, Style.PADDING)
            Draw.rect (r.SliceTop Style.PADDING) border
            Draw.rect (r.SliceBottom Style.PADDING) border

        let fill = this.Fill()

        if fill.A > 0uy then

            Draw.rect base.Bounds fill

        base.Draw()
