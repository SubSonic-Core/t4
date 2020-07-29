// 
// TextTemplatingSession.cs
//  
// Author:
//       Mikayla Hutchinson <m.j.hutchinson@gmail.com>
// 
// Copyright (c) 2010 Novell, Inc.
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
using System.Collections.Generic;
using System.Collections;
using System.Runtime.Serialization;

namespace Mono.VisualStudio.TextTemplating
{
	[Serializable]
	public sealed class TextTemplatingSession : Dictionary<string, Object>, ITextTemplatingSession, ISerializable
	{
		public TextTemplatingSession () : this (Guid.NewGuid ())
		{
		}

		TextTemplatingSession (SerializationInfo info, StreamingContext context)
			: base (info, context)
		{
		}

		public TextTemplatingSession (Guid id)
		{
			this.Id = id;
			this.IncludeStack = new Stack<string> ();
		}
		
		public Guid Id {
			get => (Guid)this[nameof (Id)];
			set => this[nameof (Id)] = value;
		}

		public bool Debug {
			get => (bool)this[nameof (Debug)];
			set => this[nameof (Debug)] = value;
		}

		public string TemplateFile {
			get => (string)this[nameof (TemplateFile)];
			set => this[nameof (TemplateFile)] = value;
		}
		public ITextTemplatingSessionHost UserTransformationSession {
			get => (ITextTemplatingSessionHost)this[nameof (UserTransformationSession)];
			set => this[nameof (UserTransformationSession)] = value;
		}

		public Stack<string> IncludeStack {
			get => (Stack<string>)this[nameof (IncludeStack)];
			set => this[nameof (IncludeStack)] = value;
		}

		public override int GetHashCode ()
		{
			return Id.GetHashCode ();
		}
		
		public override bool Equals (object obj)
		{
			var o = obj as TextTemplatingSession;
			return o != null && o.Equals (this);
		}
		
		public bool Equals (Guid other)
		{
			return other.Equals (Id);
		}
		
		public bool Equals (ITextTemplatingSession other)
		{
			return other != null && other.Id == this.Id;
		}

		void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
	}
}

