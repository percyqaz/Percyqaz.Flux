﻿namespace Percyqaz.Flux.Graphics

open OpenTK.Graphics.OpenGL

type private Buffer = BufferTarget * int

module private Buffer =

    let create (btype: BufferTarget) (data: 'Vertex array) : Buffer =
        let handle = GL.GenBuffer()
        GL.BindBuffer(btype, handle)
        GL.BufferData(btype, data.Length * sizeof<'Vertex>, data, BufferUsageHint.DynamicDraw)
        (btype, handle)

    let destroy (buf: Buffer) = GL.DeleteBuffer(snd buf)

    let bind ((btype, handle): Buffer) = GL.BindBuffer(btype, handle)

    /// Needs to be bound first
    let data (data: 'Vertex array) (count: int) ((btype, handle): Buffer) =
        GL.BufferSubData(btype, 0, nativeint (count * sizeof<'Vertex>), data)

type private VertexArrayObject = int

module private VertexArrayObject =

    let create<'Vertex> (vbo: Buffer, ebo: Buffer) : VertexArrayObject =
        let handle = GL.GenVertexArray()
        GL.BindVertexArray handle
        Buffer.bind vbo
        Buffer.bind ebo
        handle

    let destroy (vao: VertexArrayObject) = GL.DeleteVertexArray vao

    let vertex_attrib_pointer
        (
            index: int,
            count: int,
            vtype: VertexAttribPointerType,
            normalise: bool,
            vertex_size: int,
            offset: int
        ) =
        //Logging.Debug (sprintf "Attribute %i: %i %Os at offset %i; Total stride %i" index count vtype offset vertexSize)
        GL.VertexAttribPointer(index, count, vtype, normalise, vertex_size, offset)
        GL.EnableVertexAttribArray index

    let bind (vao: VertexArrayObject) = GL.BindVertexArray vao
