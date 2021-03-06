// 
// ITextTemplatingEngineHost.cs
//  
// Author:
//       Mikayla Hutchinson <m.j.hutchinson@gmail.com>
//
// Modified:
//		Kenneth Carter <kccarter32@gmail.com>
//
// Copyright (c) 2009-2010 Novell, Inc. (http://www.novell.com)
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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
#if NET45 || NETSTANDARD
using System.Threading.Tasks;
#endif
using System.Runtime.Serialization;
using System.Text;
using Mono.TextTemplating;

namespace Mono.VisualStudio.TextTemplating
{
	using Mono.VisualStudio.TextTemplating.VSHost;

	public interface IRecognizeHostSpecific
	{
		void SetProcessingRunIsHostSpecific (bool hostSpecific);
		bool RequiresProcessingRunIsHostSpecific { get; }
	}

	public interface ITextTemplatingService
		: ITextTemplatingEngineHost
		, ITextTemplatingSessionHost
		, ITextTemplatingComponents
		, IProcessTextTemplating
		, ITextTemplating
	{
		new IProcessTextTemplatingEngine Engine { get; }
	}

	public interface IProcessTextTemplating
		: ITextTemplating
	{
		event EventHandler<ProcessTemplateEventArgs> TransformProcessCompleted;
#if NETSTANDARD || NET45
		Task<string> ProcessTemplateAsync (string inputFilename, string content, ITextTemplatingCallback callback, object hierarchy, bool debugging = false);
#elif NET35
		void ProcessTemplate (string inputFilename, string content, ITextTemplatingCallback callback, object hierarchy, bool debugging = false);
#endif
	}

	public interface ITextTemplating
	{
		void BeginErrorSession ();
		bool EndErrorSession ();
		string PreprocessTemplate (string inputFile, string content, ITextTemplatingCallback callback, string className, string classNamespace, out string[] references);
		string ProcessTemplate (string inputFile, string content, ITextTemplatingCallback callback = null, object hierarchy = null);
	}

	public interface IProcessTransformationRunner
	{
		/// <summary>
		/// Get the errors, if anything prevented a successful run.
		/// </summary>
		TemplateErrorCollection Errors { get; }
		/// <summary>
		/// Get the runner identifier
		/// </summary>
		Guid RunnerId { get; }
	}

	public interface IProcessTransformationRunFactory
	{
		/// <summary>
		/// Get the factory id
		/// </summary>
		/// <returns>returns the run factory identifier</returns>
		Guid GetFactoryId ();
		/// <summary>
		/// Check to determine that the run factory is up and running
		/// </summary>
		/// <returns>true, if factory is ready to process.</returns>
		bool IsRunFactoryAlive ();
		/// <summary>
		/// instanciate process runner
		/// </summary>
		/// <returns></returns>
		IProcessTransformationRunner CreateTransformationRunner ();
		/// <summary>
		/// We have no further need for the runner.
		/// </summary>
		/// <param name="runnerId">the runner id</param>
		/// <returns>true, if successful</returns>
		bool DisposeOfRunner (Guid runnerId);
		/// <summary>
		/// Prepare the runner that the factory created to perform the transformation
		/// </summary>
		/// <param name="runnerId"></param>
		/// <param name="pt"></param>
		/// <param name="content"></param>
		/// <param name="host"></param>
		/// <param name="settings"></param>
		/// <returns></returns>
		bool PrepareTransformation (Guid runnerId, ParsedTemplate pt, string content, ITextTemplatingEngineHost host, TemplateSettings settings);
		/// <summary>
		/// Start the transformation from a template to a code file
		/// </summary>
		/// <param name="runnerId">the runner id</param>
		/// <returns>result of transformation run.</returns>
		ITextTemplatingCallback StartTransformation (Guid runnerId);
		/// <summary>
		/// Get all the errors that were logged by the transformation
		/// </summary>
		/// <returns>returns the callback object</returns>
		TemplateErrorCollection GetErrors (Guid runnerId);
	}

	public interface IProcessTextTemplatingEngine
		: ITextTemplatingEngine
	{
		IProcessTransformationRunner PrepareTransformationRunner (string content, ITextTemplatingEngineHost host, IProcessTransformationRunFactory runFactory, bool debugging = false);

		CompiledTemplate CompileTemplate (string content, ITextTemplatingEngineHost host);

		CompiledTemplate CompileTemplate (ParsedTemplate pt, string content, ITextTemplatingEngineHost host, TemplateSettings settings = null);
	}

	public interface ITextTemplatingEngine
	{
		string ProcessTemplate (string content, ITextTemplatingEngineHost host);
		string PreprocessTemplate (string content, ITextTemplatingEngineHost host, string className,
			string classNamespace, out string language, out string [] references);
	}

	public interface ITextTemplatingComponents
	{
		ITextTemplatingEngineHost Host { get; }

		ITextTemplatingEngine Engine { get; }

		string TemplateFile { get; set; }

		ITextTemplatingCallback Callback { get; set; }

		object Hierarchy { get; set; }
	}

	public interface ITextTemplatingCallback
	{
		TemplateErrorCollection Errors { get; }
		string Extension { get; }
		void ErrorCallback (bool warning, string message, int line, int column);
		void SetFileExtension (string extension);
		void SetOutputEncoding (Encoding encoding, bool fromOutputDirective);
		ITextTemplatingCallback DeepCopy ();

		int CodePage { get; }
		string TemplateOutput { get; }

		/// <summary>
		/// Get the output encoding
		/// </summary>
		/// <returns></returns>
		Encoding GetOutputEncoding ();
		void SetTemplateOutput (string output);
	}

	public interface ITextTemplatingEngineHost
	{
		object GetHostOption (string optionName);
		bool LoadIncludeText (string requestFileName, out string content, out string location);
		void LogErrors (TemplateErrorCollection errors);

#if FEATURE_APPDOMAINS
		AppDomain ProvideTemplatingAppDomain (string content);
#endif
		string ResolveAssemblyReference (string assemblyReference);
		Type ResolveDirectiveProcessor (string processorName);
		string ResolveParameterValue (string directiveId, string processorName, string parameterName);
		string ResolvePath (string path);
		void SetFileExtension (string extension);
		void SetOutputEncoding (Encoding encoding, bool fromOutputDirective);
		IList<string> StandardAssemblyReferences { get; }
		IList<string> StandardImports { get; }
		string TemplateFile { get; }
	}

#pragma warning disable CA1710 // Identifiers should have correct suffix
	public interface ITextTemplatingSession :
#pragma warning restore CA1710 // Identifiers should have correct suffix
		IEquatable<ITextTemplatingSession>, IEquatable<Guid>, IDictionary<string, object>,
		ICollection<KeyValuePair<string, object>>,
		IEnumerable<KeyValuePair<string, object>>,
		IEnumerable, ISerializable
	{
		Guid Id { get; }
	}
	
	public interface ITextTemplatingSessionHost
	{
		ITextTemplatingSession CreateSession ();
#pragma warning disable CA2227 // Collection properties should be read only
		ITextTemplatingSession Session { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
	}

	public interface IDirectiveProcessor
	{
		TemplateErrorCollection Errors { get; }
		bool RequiresProcessingRunIsHostSpecific { get; }

		void FinishProcessingRun ();
		string GetClassCodeForProcessingRun ();
		string[] GetImportsForProcessingRun ();
		string GetPostInitializationCodeForProcessingRun ();
		string GetPreInitializationCodeForProcessingRun ();
		string[] GetReferencesForProcessingRun ();
		CodeAttributeDeclarationCollection GetTemplateClassCustomAttributes ();  //TODO
		void Initialize (ITextTemplatingEngineHost host, TemplateSettings settings);
		bool IsDirectiveSupported (string directiveName);
		void ProcessDirective (string directiveName, IDictionary<string, string> arguments);
		void SetProcessingRunIsHostSpecific (bool hostSpecific);
		void StartProcessingRun (string templateContents, TemplateErrorCollection errors);
	}
}
