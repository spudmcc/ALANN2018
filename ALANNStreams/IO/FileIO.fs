﻿ (*
 * The MIT License
 *
 * Copyright 2018 The ALANN2018 authors.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 *)

module FileIO

open MBrace.FsPickler
open System.IO
open SystemState

let ExportGraph filename = 
    let binarySerializer = FsPickler.CreateBinarySerializer()
    let pickle = binarySerializer.Pickle systemState
    
    use streamWriter = new BinaryWriter(File.Open(filename, FileMode.OpenOrCreate))
    streamWriter.Write pickle

let LoadGraph filename = 
    let binarySerializer = FsPickler.CreateBinarySerializer()

    use streamReader = new BinaryReader(File.Open(filename, FileMode.Open))
    systemState <- streamReader.ReadBytes(int(FileInfo(filename).Length)) |> binarySerializer.UnPickle 

let createDirectoryIfNotExists path =
    match Directory.Exists path with
    | false -> Directory.CreateDirectory(path) |> ignore
    | _ -> ()