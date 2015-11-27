//
// PanFlag.cs
//
// Author:
//       Tomona Nanase <nanase@users.noreply.github.com>
//
// The MIT License (MIT)
//
// Copyright (c) 2015 Tomona Nanase
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

namespace Sharp98
{
    [Flags]
    public enum PanFlag : uint
    {
        Stereo = 0,

        CH1Left = 1,
        CH1Right = 2,

        CH2Left = 4,

        CH2Right = 8,
        CH3Left = 16,
        CH4Right = 32,
        FMLeft = 64,
        FMRight = 128,
        Left = 1,
        Right = 2,
    }
}
