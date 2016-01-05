using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using PdfRpt.Core.Contracts;

namespace PdfRpt.Core.Helper
{
    /// <summary>
    /// Compiler type
    /// </summary>
    public enum CompilerType
    {
        /// <summary>
        /// C# language
        /// </summary>
        CSharp,

        /// <summary>
        /// VB.NET language
        /// </summary>
        VB
    }

    /// <summary>
    /// Using C#/VB.NET compiler as a service to compile string data (SourceCode) as IPdfReportData
    /// </summary>
    public class CompileAsIPdfReportData : IDisposable
    {
        #region Fields (2)

        CompilerParameters _compilerParams;
        CodeDomProvider _provider;

        #endregion Fields

        #region Properties (5)

        /// <summary>
        /// Compiler type, C# or VB.NET
        /// </summary>
        public CompilerType CompilerType { set; get; }

        /// <summary>
        /// Compiler version, v3.5, v4.0 etc.
        /// </summary>
        public string CompilerVersion { set; get; }

        /// <summary>
        /// A case-sensitive name to locate the specified type from this assembly and create an instance of it.
        /// </summary>
        public string FullyQualifiedClassName { set; get; }

        /// <summary>
        /// Sets the assembly names that are referenced by the source to compile.
        /// System.dll, System.Data.dll, System.XML.dll, System.Windows.Forms.dll, System.Drawing.dll, System.Core.dll are pre referenced assemblies here.
        /// </summary>
        public IList<string> ReferencedAssemblies { set; get; }

        /// <summary>
        /// Text content/source code which should be compiled dynamically
        /// </summary>
        public string SourceCode { set; get; }

        #endregion Properties

        #region Methods (5)

        // Public Methods (2) 

        /// <summary>
        /// Compiles SourceCode dynamically by using C#/VB.NET compiler as a service
        /// </summary>
        /// <typeparam name="T">Type of the returned instance</typeparam>
        /// <returns>An instance of T</returns>
        public T DynamicCompileAs<T>()
        {
            initCompiler();
            setReferencedAssemblies();
            var assembly = compile();
            return (T)assembly.CompiledAssembly.CreateInstance(FullyQualifiedClassName);
        }

        /// <summary>
        /// Compiles SourceCode dynamically by using C#/VB.NET compiler as a service
        /// </summary>
        /// <returns>An instance of the class name</returns>
        public object DynamicCompile()
        {
            initCompiler();
            setReferencedAssemblies();
            var assembly = compile();
            return assembly.CompiledAssembly.CreateInstance(FullyQualifiedClassName);
        }

        /// <summary>
        /// Compiles SourceCode dynamically by using C#/VB.NET compiler as a service
        /// </summary>
        /// <returns>An instance of IPdfReportData</returns>
        public IPdfReportData DynamicCompileAsIPdfReportData()
        {
            return DynamicCompileAs<IPdfReportData>();
        }
        // Private Methods (3) 

        private CompilerResults compile()
        {
            var dynamicAssembly = _provider.CompileAssemblyFromSource(_compilerParams, SourceCode);
            if (dynamicAssembly.Errors.HasErrors)
            {
                var sb = new StringBuilder();
                foreach (CompilerError error in dynamicAssembly.Errors)
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, "---Compiler Error---{0}, Line {1}, Column {2}: Error Number: {3} {4} \r\n",
                                                            DateTime.Now.ToString(CultureInfo.InvariantCulture),
                                                            error.Line,
                                                            error.Column,
                                                            error.ErrorNumber,
                                                            error.ErrorText);
                }

                throw new InvalidOperationException(sb.ToString());
            }
            return dynamicAssembly;
        }

        private void initCompiler()
        {
            var providerOptions = new Dictionary<string, string>  
                {  
                    {"CompilerVersion", CompilerVersion}  
                };

            if (CompilerType == CompilerType.VB)
                _provider = new VBCodeProvider(providerOptions);
            else
                _provider = new CSharpCodeProvider(providerOptions);

            _compilerParams = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true
            };
        }

        private void setReferencedAssemblies()
        {
            _compilerParams.ReferencedAssemblies.Add("System.dll");
            _compilerParams.ReferencedAssemblies.Add("System.Data.dll");
            _compilerParams.ReferencedAssemblies.Add("System.XML.dll");
            _compilerParams.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            _compilerParams.ReferencedAssemblies.Add("System.Drawing.dll");
            _compilerParams.ReferencedAssemblies.Add("System.Core.dll");
            _compilerParams.ReferencedAssemblies.Add("System.ComponentModel.DataAnnotations.dll");

            if (ReferencedAssemblies == null || !ReferencedAssemblies.Any()) return;
            foreach (var asm in ReferencedAssemblies)
            {
                _compilerParams.ReferencedAssemblies.Add(asm);
            }
        }

        #endregion Methods

        #region IDisposable Members
        private bool _disposed;

        /// <summary>
        /// Destructor
        /// </summary>
        ~CompileAsIPdfReportData()
        {
            Dispose(false);
        }

        /// <summary>
        /// Free compiler provider
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // tell the GC that the Finalize process no longer needs to be run for this object.  
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Free compiler provider
        /// </summary>
        /// <param name="disposeManagedResources">Indicates disposing managed resources</param>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (_disposed) return;
            if (!disposeManagedResources) return;            

            if (_provider != null) _provider.Dispose();

            _disposed = true;
        }

        #endregion
    }
}